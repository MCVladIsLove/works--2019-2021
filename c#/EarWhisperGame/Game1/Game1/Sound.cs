using System;
using System.Collections.Generic;
using System.Text;
using NAudio.Wave;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework;

namespace Game1
{
    static class Sound
    {
        static WaveIn microphone;
        static public SoundEffect soldierAttacks;
        static public SoundEffect puddle;
        static public SoundEffect bush;
        static public SoundEffect dogAttacks;
        static public SoundEffect eatApple;
        static public SoundEffect dogDies;
        static public SoundEffect deerDies;
        static public SoundEffect deerRun;
        static public SoundEffect makeStep;
        static public SoundEffect soldierDies;
        static public SoundEffect monsterAttacks;
        static public SoundEffect monsterJumps;
        static public SoundEffect hungry;
        static public SoundEffect bigEarOpens;

        public static void SetMicrophone()
        {
            microphone = new WaveIn();
            microphone.DeviceNumber = 0;
            microphone.WaveFormat = new WaveFormat(80000, 1);
            microphone.StartRecording();

        }

        public static void Load()
        {
            soldierAttacks = Game1.ThisGame.Content.Load<SoundEffect>("shot");
            puddle = Game1.ThisGame.Content.Load<SoundEffect>("splash");
            bush = Game1.ThisGame.Content.Load<SoundEffect>("rustle");
            dogAttacks = Game1.ThisGame.Content.Load<SoundEffect>("bark");
            dogDies = Game1.ThisGame.Content.Load<SoundEffect>("whine");
            eatApple = Game1.ThisGame.Content.Load<SoundEffect>("crunch");
            deerDies = Game1.ThisGame.Content.Load<SoundEffect>("wheeze");
            deerRun = Game1.ThisGame.Content.Load<SoundEffect>("deerRunAway");
            makeStep = Game1.ThisGame.Content.Load<SoundEffect>("stepSound");
            soldierDies = Game1.ThisGame.Content.Load<SoundEffect>("soldierDie");
            monsterAttacks = Game1.ThisGame.Content.Load<SoundEffect>("monsterEat");
            monsterJumps = Game1.ThisGame.Content.Load<SoundEffect>("jump");
            hungry = Game1.ThisGame.Content.Load<SoundEffect>("stomack");
            bigEarOpens = Game1.ThisGame.Content.Load<SoundEffect>("bigEarOpen");
        }

        public static void PlaySound(Vector2 emitterPos, SoundEffect sound)
        {
            SoundEffectInstance snd = sound.CreateInstance();
            AudioEmitter emitter = new AudioEmitter();
            emitterPos.X = (int)(emitterPos.X - (emitterPos.X - Monster.Listener.Position.X) / 1.01);
            emitterPos.Y = (int)(emitterPos.Y - (emitterPos.Y - Monster.Listener.Position.Y) / 1.01);
            emitter.Position = new Vector3(emitterPos, 0);
            snd.Apply3D(Monster.Listener, emitter);
            snd.Play();
        }
        public static void PlaySound(SoundEffect sound)
        {
            SoundEffectInstance snd = sound.CreateInstance();
            snd.Play();
        }
    }
}
