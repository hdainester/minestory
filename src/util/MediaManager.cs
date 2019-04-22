using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework;

using System.Collections.ObjectModel;
using System.Collections.Generic;
using System;

namespace Chaotx.Minesweeper {
    public enum RepeatMode {NoReapeat, RepeatCurrent, RepeatAll}

    public class MediaManager {
        private List<Song> songs = new List<Song>();
        public ReadOnlyCollection<Song> Songs => songs.AsReadOnly();

        private List<SoundEffect> sounds = new List<SoundEffect>();
        public ReadOnlyCollection<SoundEffect> Sounds => sounds.AsReadOnly();

        public RepeatMode Reapeat {get; set;}
        public float SoundVolume {get; set;}
        public float SongVolume {get; set;}
        public int SongFadeIn {get; set;}
        public int SongFadeOut {get; set;}
        public bool SongRunning {get; protected set;}

        private Game game;
        private int songFadeIn;
        private int songFadeOut;
        private int activeSong;
        private float volume;

        public MediaManager(Game game) {
            this.game = game;
            SoundVolume = 1;
            SongVolume = 1;
            SongFadeIn = 3000;
            SongFadeOut = 3000;
            songFadeIn = int.MaxValue;
            songFadeOut = int.MaxValue;
            volume = 1;
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

            StopSong();
            activeSong = songIndex;
            songFadeIn = 0;
        }

        /// Starts playing the song at the specified index
        public void PlaySound(int soundIndex) {
            if(soundIndex >= sounds.Count  || soundIndex < 0)
                throw new Exception("no sound at index " + soundIndex);

            sounds[soundIndex].Play(SoundVolume, 0, 0);
        }

        /// Stops currently running song
        public void StopSong() {
            if(SongRunning)
                songFadeOut = 0;
        }

        /// Updates this media manager
        public void Update(GameTime gameTime) {
            int ms = gameTime.ElapsedGameTime.Milliseconds;
            MediaPlayer.Volume = volume;

            if(songFadeOut < SongFadeOut) {
                volume = SongVolume*Math.Max(0, 1f - (songFadeOut += ms)/(float)SongFadeOut);

                if(songFadeOut >= SongFadeOut) {
                    SongRunning = false;
                    
                    if(Reapeat == RepeatMode.NoReapeat)
                        MediaPlayer.Stop();
                    else if(Reapeat == RepeatMode.RepeatAll)
                        PlaySong((activeSong+1)%songs.Count);
                    else PlaySong(activeSong);
                }
            } else if(songFadeIn < SongFadeIn) {
                if(songFadeIn == 0) {
                    MediaPlayer.Play(songs[activeSong]);
                    SongRunning = true;
                }

                volume = SongVolume*Math.Min(1, (songFadeIn += ms)/(float)SongFadeIn);
            }
        }
    }
}