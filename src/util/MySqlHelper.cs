using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System;
using System.Data;

namespace Chaotx.Minestory {
    public class MySqlHelper {
        private class SqlInstanceException : InvalidOperationException {
            public SqlInstanceException()
            : base("Connection not initialized. Call SqlConnect.Init first") {}
        }

        public static MySqlHelper Instance {
            get {
                if(instance == null)
                    throw new SqlInstanceException();

                return instance;
            }

            private set => instance = value;
        }

        public MySqlConnection Connection {get;}
        public string DBString {get;}
        public string Server {get;}
        public string DBName {get;}
        public string Table {get;}
        public string UID {get;}


        private static MySqlHelper instance;
        private MySqlHelper(string server, string dbname, string uid, string pw, string table) {
            DBString = string.Format(
                "Server={0}; database={1}; UID={2}; password={3}",
                server, dbname, uid, pw);

            Server = server;
            DBName = dbname;
            Table = table;
            UID = uid;

            Connection = new MySqlConnection(DBString);
        }

        public static void Init(string server, string dbname, string uid, string pw = "", string table = "Highscores") {
            instance = new MySqlHelper(server, dbname, uid, pw, table);
        }

        public void Sync(Minestory game) {
            if(Connection == null)
                throw new SqlInstanceException();

            try {
                Connection.Open();
                CreateTable();

                var scores = Retrieve();
                scores.ForEach(s => game.AddHighscore(s));
                var newScores = game.Scores.Except(scores);

                foreach(var score in newScores) {
                    var old = scores.FirstOrDefault(s => s.Name.Equals(score.Name)
                        && s.Difficulty == score.Difficulty
                        && s.Time >= score.Time);

                    if(old != null)
                        Update(old, score);
                    else
                        Insert(score);
                }

                Connection.Close();
            } catch(MySqlException e) {
                Console.WriteLine(e.Message);
            }
        }

        private void CreateTable() {
            var command = Connection.CreateCommand();
            command.CommandText = string.Format(
                "CREATE TABLE IF NOT EXISTS {0} ("
                + "PlayerID varchar(8),"
                + "MinesHit smallint,"
                + "TotalMines smallint,"
                + "TimeSpan bigint,"
                + "CreationDate bigint,"
                + "Difficulty tinyint"
                + ");", Table);

            command.ExecuteNonQuery();
        }

        private void Insert(Highscore score) {
            var command = Connection.CreateCommand();
            command.CommandText = string.Format("INSERT INTO {0} "
                + "VALUES('{1}', '{2}', '{3}', '{4}', '{5}', '{6}');",
                Table, score.Name, score.MinesHit, score.TotalMines,
                score.Time.Ticks, score.TimeStamp.Ticks, (int)score.Difficulty);

            command.ExecuteNonQuery();
        }

        private void Update(Highscore ol, Highscore nw) {
            var command = Connection.CreateCommand();
            command.CommandText = string.Format("UPDATE Highscores "
                + "SET MinesHit={0}, TotalMines={1}, "
                + "TimeSpan={2}, CreationDate={3} "
                + "WHERE PlayerID='{4}' and Difficulty={5};",
                nw.MinesHit, nw.TotalMines,
                nw.Time.Ticks, nw.TimeStamp.Ticks,
                ol.Name, (int)ol.Difficulty);

            command.ExecuteNonQuery();
        }

        private List<Highscore> Retrieve() {
            List<Highscore> scores = new List<Highscore>();
            var command = Connection.CreateCommand();
            command.CommandText = "SELECT * FROM Highscores;";

            using(var reader = command.ExecuteReader()) {
                if(reader.HasRows) while(reader.Read())
                    scores.Add(new Highscore(reader.GetString(0),
                        reader.GetInt32(1), reader.GetInt32(2),
                        TimeSpan.FromTicks(reader.GetInt64(3)),
                        new DateTime(reader.GetInt64(4)),
                        (MapDifficulty)reader.GetInt16(5)));
            }

            return scores;
        }
    }
}