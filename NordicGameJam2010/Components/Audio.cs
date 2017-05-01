using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace NordicGameJam2010.Components
{
    public enum AudioState
    {
        Win,
        Kill,
        Angry,
        Playing,
        None
    }

    class Audio
    {
        public static AudioState State = AudioState.Playing;

        static SoundEffect _BackgroundAudio;
        static SoundEffectInstance _BackgroundAudioInstance;
        public static SoundEffectInstance BackgroundAudio
        {
            get { return _BackgroundAudioInstance; }
        }

        static SoundEffect _AngryAudio;
        static SoundEffectInstance _AngryAudioInstance;
        public static SoundEffectInstance AngryAudio
        {
            get { return _AngryAudioInstance; }
        }

        static SoundEffect _KillAudio;
        static SoundEffectInstance _KillAudioInstance;
        public static SoundEffectInstance KillAudio
        {
            get { return _KillAudioInstance; }
        }

        static SoundEffect _KillDummyAudio;
        static SoundEffectInstance _KillDummyAudioInstance;
        public static SoundEffectInstance KillDummyAudio
        {
            get { return _KillDummyAudioInstance; }
        }

        static SoundEffect _WinAudio;
        static SoundEffectInstance _WinAudioInstance;
        public static SoundEffectInstance WinAudio
        {
            get { return _WinAudioInstance; }
        }

        public static void LoadAudio(ContentManager content, String filename)
        {
            using (StreamReader reader = new StreamReader(filename))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine().Trim();

                    if (string.IsNullOrEmpty(line))
                        continue;

                    string[] split = line.Split(' ');

                    if (split[0].Equals("[Kill]"))
                    {
                        _KillAudio = content.Load<SoundEffect>(split[1]);
                        _KillAudioInstance = _KillAudio.CreateInstance();
                        _KillAudioInstance.IsLooped = false;
                        _KillAudioInstance.Volume = float.Parse(split[2])/100f;
                    }
                    if (split[0].Equals("[KillDummy]"))
                    {
                        _KillDummyAudio = content.Load<SoundEffect>(split[1]);
                        _KillDummyAudioInstance = _KillDummyAudio.CreateInstance();
                        _KillDummyAudioInstance.IsLooped = false;
                        _KillDummyAudioInstance.Volume = float.Parse(split[2]) / 100f;
                    }
                    else if (split[0].Equals("[Win]"))
                    {
                        _WinAudio = content.Load<SoundEffect>(split[1]);
                        _WinAudioInstance = _WinAudio.CreateInstance();
                        _WinAudioInstance.IsLooped = false;
                        _WinAudioInstance.Volume = float.Parse(split[2])/100f;
                    }
                    else if (split[0].Equals("[Angry]"))
                    {
                        _AngryAudio = content.Load<SoundEffect>(split[1]);
                        _AngryAudioInstance = _AngryAudio.CreateInstance();
                        _AngryAudioInstance.IsLooped = false;
                        _AngryAudioInstance.Volume = float.Parse(split[2])/100f;
                    }
                    else if (split[0].Equals("[Background]"))
                    {
                        _BackgroundAudio = content.Load<SoundEffect>(split[1]);
                        _BackgroundAudioInstance = _BackgroundAudio.CreateInstance();
                        _BackgroundAudioInstance.IsLooped = true;
                        _BackgroundAudioInstance.Volume = float.Parse(split[2])/100f;
                    }
                }
            }
        }
    }
}
