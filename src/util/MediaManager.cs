using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework;

using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Chaotx.Minesweeper {
    public enum RepeatMode {NoReapeat, RepeatCurrent, RepeatAll}

    public class MediaManager {
        private static Dictionary<Song, int> runningSongs = new Dictionary<Song, int>();
        private static Song runningSong;
        private static int instances;

        private List<Song> songs = new List<Song>();
        public ReadOnlyCollection<Song> Songs => songs.AsReadOnly();

        private List<SoundEffect> sounds = new List<SoundEffect>();
        public ReadOnlyCollection<SoundEffect> Sounds => sounds.AsReadOnly();

        public bool IsRunning => running;
        public RepeatMode Repeat {get; set;}
        public float SoundVolume {get; set;}
        public float SongVolume {get; set;}

        private Game game;
        private int activeSong;
        private bool running;

        /// Creates a new media manager
        /// associated to the passed game
        public MediaManager(Game game) {
            this.game = game;
            SoundVolume = 1;
            SongVolume = 1;
        }

        /// Adds a song to the manager.
        /// Returns the song index
        public int AddSong(string song) {
            songs.Add(game.Content.Load<Song>(song));
            return songs.Count-1;
        }

        /// Adds a sound to the manager.
        /// Returns the sound index
        public int AddSound(string sound) {
            sounds.Add(game.Content.Load<SoundEffect>(sound));
            return sounds.Count-1;
        }

        /// Plays the sound at the specified index
        public void PlaySong(int songIndex) {
            if(songIndex >= songs.Count || songIndex < 0)
                throw new Exception("no song at index " + songIndex);
                
            MediaPlayer.Volume = SongVolume;
            MediaPlayer.Play(songs[activeSong = songIndex]);
            running = true;
        }

        /// Starts playing the song at the specified index
        public void PlaySound(int soundIndex) {
            if(soundIndex >= sounds.Count  || soundIndex < 0)
                throw new Exception("no sound at index " + soundIndex);

            sounds[soundIndex].Play(SoundVolume, 0, 0);
        }

        /// Stops currently running song
        public void StopSong() {
            MediaPlayer.Stop();
            running = false;
        }

        /// Toggles the next song
        public void NextSong() {
            if(songs.Count > 0) {
                if(Repeat == RepeatMode.RepeatAll)
                    PlaySong((activeSong+1)%songs.Count);
                else PlaySong(activeSong);
            }
        }

        /// Updates this media manager
        public void Update(GameTime gameTime) {
            if(IsRunning) {
                if(MediaPlayer.State == MediaState.Stopped
                && Repeat != RepeatMode.NoReapeat)
                    NextSong();

                MediaPlayer.Volume = SongVolume;
            }
        }
    }
}