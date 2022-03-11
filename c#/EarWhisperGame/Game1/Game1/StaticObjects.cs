using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace Game1
{
    abstract class StaticObjects : MapElements  
    {
        public StaticObjects(Game1 game) : base(game)
        { }
        protected abstract void Interact(Creatures interactWith);
        public override void Update(GameTime gameTime)
        {
            if (Map.ElementsOnScreen.Contains(this))
                foreach (var element in Map.ElementsOnScreen)
                    if (!(element is StaticObjects) && CheckCollision(element.ElementRect))
                        Interact((Creatures)element);
            _transparency = _transparency - 1 > 0 ? _transparency - 1 : 0;
        }
    }
    abstract class SoundingInteractable : StaticObjects // contains all sound methods from Creature class
    {
        protected float _startInteractionTime = -10;
        protected int _soundWaveTransparency = 0;
        protected float _soundWaveLength = 0;
        protected float _soundWaveMaxLength = 0;
        protected bool _isTriggered = false;
        protected bool _isSomethingInside = false;
        protected Creatures _creatureTriggered;
        public SoundingInteractable(Game1 game) : base(game) { }

        public Rectangle SoundWaveRect
        {
            get
            {
                return new Rectangle((int)(Position.X - Game1.step.Width / 2 * _soundWaveLength + _texture.Width / 2),
              (int)(Position.Y - Game1.step.Height / 2 * _soundWaveLength + _texture.Height / 2),
              (int)(Game1.step.Width * _soundWaveLength), (int)(Game1.step.Height * _soundWaveLength));
            }
        }
        public bool SoundWaveCollision(Rectangle collideWith)
        {
            return SoundWaveRect.Intersects(collideWith);
        }
        protected void CalculateFlashEnvironment(float volume, float dominantVol) // volume 0 - 1
        {
            bool isWaveFading;
            if (volume >= 0.2f || dominantVol >= 0.2f)
            {
                isWaveFading = false;
                if (dominantVol > volume)
                    _soundWaveMaxLength = dominantVol;
                else
                    _soundWaveMaxLength = volume;
            }
            else
                isWaveFading = true;

            if (_soundWaveLength > _soundWaveMaxLength)
                _soundWaveLength = (_soundWaveLength - 0.05f > 0) ? _soundWaveLength - 0.05f : 0;
            else
                _soundWaveLength = (_soundWaveLength + 0.1f < 1) ? _soundWaveLength + 0.1f : 1;

            if (isWaveFading)
                _soundWaveTransparency = (_soundWaveTransparency - 3 > 0) ? _soundWaveTransparency - 3 : 0;
            else
                _soundWaveTransparency = (_soundWaveTransparency + 4 < 200) ? _soundWaveTransparency + 4 : 200;


            if (_soundWaveTransparency <= 0)
            {
                _soundWaveMaxLength = 0;
                _soundWaveLength = 0;
            }
        }
        protected void DrawFlash()
        {
            _game.Sprite.Draw(Game1.step,
                new Vector2(SoundWaveRect.X - Map.ScreenPos.X, SoundWaveRect.Y - Map.ScreenPos.Y),
                null,
                Color.FromNonPremultiplied(255, 255, 255, _soundWaveTransparency),
                0,
                new Vector2(0, 0),
                _soundWaveLength,
                SpriteEffects.None,
                0);

        }
        public override void Draw(GameTime gameTime)
        {
            DrawFlash();
            base.Draw(gameTime);
        }
        protected abstract void MakeNoise(Creatures creatureWhichInteracts);
        protected override void Interact(Creatures interactWith) { }
        protected void Interact(Creatures interactWith, GameTime gameTime)
        {
            if (!_isSomethingInside)
            {
                _isSomethingInside = true;
                _startInteractionTime = (float)gameTime.TotalGameTime.TotalSeconds;
                _creatureTriggered = interactWith;
                PlaySound();
            }
        }
        protected abstract void PlaySound();
        public override void Update(GameTime gameTime)
        {
            _isTriggered = false;
            if (gameTime.TotalGameTime.TotalSeconds - _startInteractionTime < 0.4f)
                CalculateFlashEnvironment(0.9f, 0);
            else if (gameTime.TotalGameTime.TotalSeconds - _startInteractionTime < 1)
                CalculateFlashEnvironment(0, 0);
            if (Map.ElementsOnScreen.Contains(this))
            {
                foreach (var element in Map.ElementsOnScreen)
                    if (!(element is StaticObjects) && CheckCollision(element.ElementRect))
                    {
                        if (!_isTriggered)
                            _isTriggered = true;
                        Interact((Creatures)element, gameTime);
                    }
                if (!_isTriggered)
                {
                    _isSomethingInside = false;
                    _creatureTriggered = null;
                }
                if (_creatureTriggered != null)
                    MakeNoise(_creatureTriggered);
            }
            _transparency = _transparency - 1 > 0 ? _transparency - 1 : 0;
        }
    }

    class Bush : SoundingInteractable
    {
        public Bush(Game1 game) : base(game)
        { }
        public override void Load()
        {
            _texture = _game.Content.Load<Texture2D>("bush");
        }
        protected override void MakeNoise(Creatures creatureWhichInteracts)
        {
            foreach (var element in Map.ElementsOnScreen)
            {
                if (SoundWaveCollision(element.ElementRect) && element is Creatures && element != creatureWhichInteracts)
                {
                    Creatures creature = (Creatures)element;
                    creature.ReactToSoundOrSee(this);
                }
            }
        }

        protected override void PlaySound()
        {
            Sound.PlaySound(PositionOnScreen, Sound.bush);
        }
    }

    class Puddle : SoundingInteractable
    {
        public Puddle(Game1 game) : base(game)
        {

        }
        protected override void MakeNoise(Creatures creatureWhichInteracts)
        {
            foreach (var element in Map.ElementsOnScreen)
            {
                if (SoundWaveCollision(element.ElementRect) && element is Creatures && element != creatureWhichInteracts)
                {
                    Creatures creature = (Creatures)element;
                    creature.ReactToSoundOrSee(creatureWhichInteracts);
                }
            }
        }
        protected override void PlaySound()
        {
            Sound.PlaySound(PositionOnScreen, Sound.puddle);
        }
        public override void Load()
        {
            _texture = _game.Content.Load<Texture2D>("puddle");
        }
    }

    class Apple : StaticObjects
    {
        public Apple(Game1 game) : base(game)
        {

        }
        protected override void Interact(Creatures interactWith)
        {
            if (interactWith is Monster)
            {
                Monster monster = (Monster)interactWith;
                monster.Health = monster.Health + 10 < 100 ? monster.Health + 10 : 100;
                monster.Hunger = monster.Hunger + 10 < 100 ? monster.Hunger + 10 : 100;
                Sound.PlaySound(Sound.eatApple);
                Map.ElementsToDelete.Add(this);
            }
        }

        public override void Load()
        {
            _texture = _game.Content.Load<Texture2D>("apple");
        }
    }

    class Uninteractable : StaticObjects
    {
        string _type; 
        public Uninteractable(Game1 game) : base(game)
        {
            if (rnd.Next(0, 2) == 0)
                _type = "boulder";
            else
                _type = "tree";
        }
        protected override void Interact(Creatures interactWith)
        {
            PullOut(interactWith);
        }

        public override void Load()
        {
            if (_type == "boulder")
                _texture = _game.Content.Load<Texture2D>("boulder");
            else if (_type == "tree")
                _texture = _game.Content.Load<Texture2D>("tree");
        } 

        void PullOut(Creatures creature)
        {
            if (this.Position.X + this.ElementRect.Width / 2 < creature.Position.X + creature.ElementRect.Width / 2)
                creature.Position = new Vector2(this.ElementRect.Right + 1, creature.Position.Y);
            else
                creature.Position = new Vector2(this.Position.X - creature.ElementRect.Width - 1, creature.Position.Y);

            if (this.Position.Y + this.ElementRect.Height / 2 > creature.Position.Y + creature.ElementRect.Height / 2)
                creature.Position = new Vector2(creature.Position.X, this.Position.Y - creature.ElementRect.Height - 1);
            else
                creature.Position = new Vector2(creature.Position.X, this.ElementRect.Bottom + 1);

            if (creature is Monster)
            {
                Monster monster = (Monster)creature;
                monster.CentreCam();
            }
        }
    }

}
