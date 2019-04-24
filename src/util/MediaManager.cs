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
        private static List<Song> runningSongs = new List<Song>();

        private List<Song> songs = new List<Song>();
        public ReadOnlyCollection<Song> Songs => songs.AsReadOnly();

        private List<SoundEffect> sounds = new List<SoundEffect>();
        public ReadOnlyCollection<SoundEffect> Sounds => sounds.AsReadOnly();

        public RepeatMode Reapeat {get; set;}
        public float SoundVolume {get; set;}
        public float SongVolume {get; set;}
        public bool SongRunning => MediaPlayer.State == MediaState.Playing;

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
            runningSongs.Add(songs[activeSong]);
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
            if(SongRunning && songs.Count > 0) {
                runningSongs.Remove(songs[activeSong]);

                if(!runningSongs.Any(s => s == MediaPlayer.Queue.ActiveSong))
                    MediaPlayer.Stop();

                running = false;
            }
        }

        /// Toggle to the next song
        public void NextSong() {
            if(running)
                runningSongs.Remove(songs[activeSong]);

            if(songs.Count > 0) {
                if(Reapeat == RepeatMode.RepeatAll)
                    PlaySong((activeSong+1)%songs.Count);
                else PlaySong(activeSong);
            }
        }

        /// Updates this media manager
        public void Update(GameTime gameTime) {
            if(running) {
                if(MediaPlayer.State == MediaState.Stopped)
                    NextSong();

                MediaPlayer.Volume = SongVolume;
            }
        }
    }
}