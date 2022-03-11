using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;


namespace Game1
{
    abstract class Creatures : MapElements
    {
        protected float _speed;
        protected int _health;
        protected int _soundWaveTransparency = 0;
        protected float _soundWaveLength = 0;
        protected float _soundWaveMaxLength = 0;
        protected CreatureState _creatureState = CreatureState.Patrol;
        protected MovementDirection _movementDir = MovementDirection.Stay;
        protected double _timeStepStart = -10;
        protected List<RelationsManager> _elementsAround = new List<RelationsManager>();
        protected Random _rnd = new Random();
        protected bool _isAlive = true;
        protected float _visionRadius;

        public bool IsAlive { get { return _isAlive; } set { _isAlive = value; } }

        public MovementDirection MoveDir { get { return _movementDir; } }
        public int Health { get { return _health; } set { _health = value; } }
        public Rectangle SoundWaveRect
        {
            get
            {
                return new Rectangle((int)(Position.X - Game1.step.Width / 2 * _soundWaveLength + _texture.Width / 2),
              (int)(Position.Y - Game1.step.Height / 2 * _soundWaveLength + _texture.Height / 2),
              (int)(Game1.step.Width * _soundWaveLength), (int)(Game1.step.Height * _soundWaveLength));
            }
        }
        public Creatures(Game1 game) : base(game)
        {
            _health = 1;
        }
        public bool SoundWaveCollision(Rectangle collideWith)
        {
            return SoundWaveRect.Intersects(collideWith);
        }

        protected void LookAround()
        {
            foreach (var element in Map.ElementsOnScreen)
                if (!(element is StaticObjects) && CheckCollision(element.ElementRect, _visionRadius))
                    ReactToSoundOrSee(element);
        }

        protected RelationsManager GetNearestElement(Priority priority)
        {
            RelationsManager captured = null;
            float closestDistance = 100000;
            foreach (var element in _elementsAround)
                if (element.Priority == priority)
                    if (element.Distance < closestDistance)
                    {
                        closestDistance = element.Distance;
                        captured = element;
                    }
            return captured;
        }
        protected float CalculateRotationChangePos()
        {
            switch (_movementDir)
            {
                case MovementDirection.Bottom:
                    _position.Y += _speed;
                    if (_position.Y > _mapHeight)
                        _position.Y = _mapHeight;
                    return (float)Math.PI;
                case MovementDirection.BottomLeft:
                    _position.Y += _speed;
                    _position.X -= _speed;
                    if (_position.Y > _mapHeight)
                        _position.Y = _mapHeight;
                    if (_position.X < 0)
                        _position.X = 0;
                    return (float)Math.PI * 5 / 4;
                case MovementDirection.BottomRight:
                    _position.Y += _speed;
                    _position.X += _speed;
                    if (_position.Y > _mapHeight)
                        _position.Y = _mapHeight;
                    if (_position.X > _mapWidth)
                        _position.X = _mapWidth;
                    return (float)Math.PI * 3 / 4;
                case MovementDirection.Left:
                    _position.X -= _speed;
                    if (_position.X < 0)
                        _position.X = 0;
                    return (float)Math.PI * 3 / 2;
                case MovementDirection.Right:
                    _position.X += _speed;
                    if (_position.X > _mapWidth)
                        _position.X = _mapWidth;
                    return (float)Math.PI / 2;
                case MovementDirection.Top:
                    _position.Y -= _speed;
                    if (_position.Y < 0)
                        _position.Y = 0;
                    return 0;
                case MovementDirection.TopLeft:
                    _position.Y -= _speed;
                    _position.X -= _speed;
                    if (_position.Y < 0)
                        _position.Y = 0;
                    if (_position.X < 0)
                        _position.X = 0;
                    return (float)Math.PI * 7 / 4;
                case MovementDirection.TopRight:
                    _position.Y -= _speed;
                    _position.X += _speed;
                    if (_position.Y < 0)
                        _position.Y = 0;
                    if (_position.X > _mapWidth)
                        _position.X = _mapWidth;
                    return (float)Math.PI / 4;
                default:
                    return _rotation;
            }
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
        protected void MoveTo(Vector2 destination)
        {
            float angle = MathF.Atan2(destination.Y - _position.Y, destination.X - _position.X);
            if (angle >= 0)
            {
                if (angle < Math.PI / 16)
                    _movementDir = MovementDirection.Right;
                else if (angle < Math.PI / 16 * 3)
                    _movementDir = MovementDirection.BottomRight;
                else if (angle < Math.PI / 16 * 5)
                    _movementDir = MovementDirection.Bottom;
                else if (angle < Math.PI / 16 * 7)
                    _movementDir = MovementDirection.BottomLeft;
                else
                    _movementDir = MovementDirection.Left;
            }
            else
            {
                if (angle > -Math.PI / 16)
                    _movementDir = MovementDirection.Right;
                else if (angle > -Math.PI / 16 * 3)
                    _movementDir = MovementDirection.TopRight;
                else if (angle > -Math.PI / 16 * 5)
                    _movementDir = MovementDirection.Top;
                else if (angle > -Math.PI / 16 * 7)
                    _movementDir = MovementDirection.TopLeft;
                else
                    _movementDir = MovementDirection.Left;
            }
        }
        protected void RunAway(Vector2 runFrom)
        {
            float angle = MathF.Atan2(runFrom.Y - _position.Y, runFrom.X - _position.X);
            if (angle >= 0)
            {
                if (angle < Math.PI / 16)
                    _movementDir = MovementDirection.Left;
                else if (angle < Math.PI / 16 * 3)
                    _movementDir = MovementDirection.TopLeft;
                else if (angle < Math.PI / 16 * 5)
                    _movementDir = MovementDirection.Top;
                else if (angle < Math.PI / 16 * 7)
                    _movementDir = MovementDirection.TopRight;
                else
                    _movementDir = MovementDirection.Right;
            }
            else
            {
                if (angle > -Math.PI / 16)
                    _movementDir = MovementDirection.Left;
                else if (angle > -Math.PI / 16 * 3)
                    _movementDir = MovementDirection.BottomLeft;
                else if (angle > -Math.PI / 16 * 5)
                    _movementDir = MovementDirection.Bottom;
                else if (angle > -Math.PI / 16 * 7)
                    _movementDir = MovementDirection.BottomRight;
                else
                    _movementDir = MovementDirection.Right;
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

        public virtual void ReactToSoundOrSee(MapElements soundingElement)
        {
            if (soundingElement.GetType().Name != this.GetType().Name)
            {
                bool alreadyInList = false;
                if (_creatureState == CreatureState.Patrol)
                    _creatureState = CreatureState.HeardSomething;
                foreach (var heard in _elementsAround)
                    if (heard.SourceOfSound == soundingElement)
                    {
                        alreadyInList = true;
                        break;
                    }
                if (!alreadyInList)
                {
                    _elementsAround.Add(new RelationsManager(soundingElement));
                }
            }
        }

        protected virtual void WhileMakingMove(GameTime gameTime, float dominantSoundVol)
        {
            if (_movementDir == MovementDirection.Stay)
            {
                Move(gameTime);
                if (Map.ElementsOnScreen.Contains(this))
                    CalculateFlashEnvironment(0.2f, dominantSoundVol);
                if (_movementDir != MovementDirection.Stay)
                {
                    _rotation = CalculateRotationChangePos();
                    _timeStepStart = gameTime.TotalGameTime.TotalSeconds;
                    if (Map.ElementsOnScreen.Contains(this))
                        Sound.PlaySound(PositionOnScreen, Sound.makeStep);
                }
            }
            else if (gameTime.TotalGameTime.TotalSeconds - _timeStepStart < 0.7)
            {
                if (Map.ElementsOnScreen.Contains(this))
                    CalculateFlashEnvironment(0.2f, dominantSoundVol);
            }
            else if (gameTime.TotalGameTime.TotalSeconds - _timeStepStart < 1.65)
            {
                if (Map.ElementsOnScreen.Contains(this))
                    CalculateFlashEnvironment(0, dominantSoundVol);
            }
            else
                _movementDir = MovementDirection.Stay;
        }

        protected virtual void Hit(Creatures whoHit, int force, int chance)
        {
            if (_rnd.Next(0, chance + 1) == chance)
            {
                whoHit.Health -= force;
                if (whoHit.Health <= 0)
                {
                    whoHit.IsAlive = false;
                    if (whoHit is Soldier)
                    {
                        Sound.PlaySound(whoHit.PositionOnScreen, Sound.soldierDies);
                        Interface.EnemiesLeft--;
                    }
                    else if (whoHit is Wolf)
                        Sound.PlaySound(whoHit.PositionOnScreen, Sound.dogDies);
                    else if (whoHit is Deer)
                        Sound.PlaySound(whoHit.PositionOnScreen, Sound.deerDies);
                }
            }
        }
        protected virtual void MakeNoise()
        {
            foreach (var element in Map.ElementsOnScreen)
            {
                if (SoundWaveCollision(element.ElementRect) && element is Creatures)
                {
                    Creatures creature = (Creatures)element;
                    creature.ReactToSoundOrSee(this);
                }
            }
        }
        protected abstract void Attack(GameTime gameTime);
        protected abstract void Move(GameTime gameTime);
        protected abstract void WhileAttack(GameTime gameTime);
        protected abstract void SetCreatureState(GameTime gameTime);
        public override void Update(GameTime gameTime) { }
    }

    abstract class Enemy : Creatures
    {
        protected double _timeAttackStart = -10;
        protected float _attackSoundVolume = 0;
        public Enemy(Game1 game) : base(game)
        {
        }

        public override void Update(GameTime gameTime)
        {
            if (_isAlive)
            {

                if (Map.ElementsOnScreen.Contains(this))
                {
                    MakeNoise();
                    LookAround();
                }

                if (_creatureState != CreatureState.Patrol)
                    SetCreatureState(gameTime);

                WhileMakingMove(gameTime, _attackSoundVolume);

                WhileAttack(gameTime);

                _transparency = _transparency - 2 > 0 ? _transparency - 2 : 0;
            }
            else if (_transparency <= 0 && _soundWaveTransparency <= 0)
            {
                Map.ElementsToDelete.Add(this);
            }
            else
            {
                _transparency -= 7;
                _soundWaveTransparency -= 7;
            }
        }

    }

    class Soldier : Enemy
    {
        float _distanceToEnemy = 2.5f;
        public Soldier(Game1 game) : base(game)
        {
            _speed = 55;
            _visionRadius = 1.5f;
        }

        protected override void SetCreatureState(GameTime gameTime)
        {
            _creatureState = CreatureState.Patrol;
            bool hasChecks = false;
            foreach (var sound in _elementsAround.ToArray())
            {
                sound.CalculateDistance(this.Position);
                if (sound.Distance > _game.Graphics.PreferredBackBufferHeight / 2)
                {
                    _elementsAround.Remove(sound);
                    continue;
                }

                if (sound.Priority != Priority.Ignore)
                {
                    if (sound.SourceOfSound is Deer)
                        sound.Priority = Priority.Ignore;
                    else if (sound.SourceOfSound is Bush)
                    {
                        sound.Priority = Priority.Check;
                        hasChecks = true;
                        if (_creatureState != CreatureState.Fight) 
                            _creatureState = CreatureState.HeardSomething;
                    }
                    else
                        _creatureState = CreatureState.Fight; 
                }
            }
            if (hasChecks && _creatureState == CreatureState.Fight)
                foreach (var element in _elementsAround.ToArray())
                    if (element.Priority == Priority.Check)
                        _elementsAround.Remove(element);
        }
        public override void Load()
        {
            _texture = _game.Content.Load<Texture2D>("soldier");
        }
        protected override void Attack(GameTime gameTime)
        {
            RelationsManager creature = GetNearestElement(Priority.Track);
            Creatures attackedCreature = (Creatures)creature.SourceOfSound;
            if (attackedCreature.IsAlive)
            {
                _timeAttackStart = gameTime.TotalGameTime.TotalSeconds;
                _attackSoundVolume = 1;
                _transparency = 255;
                Sound.PlaySound(PositionOnScreen, Sound.soldierAttacks);
                if (attackedCreature is Monster)
                    Hit(attackedCreature, 35, 3);
                else if (attackedCreature is Wolf)
                    Hit(attackedCreature, 1, 4);
            }
            else
                creature.Priority = Priority.Ignore;
        }
        protected override void WhileAttack(GameTime gameTime)
        {
            if (_creatureState == CreatureState.Fight && gameTime.TotalGameTime.TotalSeconds - _timeAttackStart > 0.05)
                _attackSoundVolume = 0;
            if (_creatureState == CreatureState.Fight && gameTime.TotalGameTime.TotalSeconds - _timeAttackStart > 1.5f)
                Attack(gameTime);
        }

        protected override void Move(GameTime gameTime)
        {
            if (_creatureState == CreatureState.Patrol)
            {
                _movementDir = (MovementDirection)_rnd.Next(0, 9);
            }
            else if (_creatureState == CreatureState.Fight)
            {
                RelationsManager focused = GetNearestElement(Priority.Track);
                if (focused.Distance < _texture.Width * _distanceToEnemy)
                    RunAway(focused.SourceOfSound.Position);
                else
                    MoveTo(focused.SourceOfSound.Position);
            }
            else if (_creatureState == CreatureState.HeardSomething)
            {
                RelationsManager focused = GetNearestElement(Priority.Check);
                MoveTo(focused.Position);
                if (CheckCollision(focused.SourceOfSound.ElementRect))
                    _elementsAround.Remove(focused);
            }
        }
    }
    class Wolf : Enemy
    {
        public Wolf(Game1 game) : base(game)
        {
            _speed = 90;
            _visionRadius = 3;
        }
        public override void Load()
        {
            _texture = _game.Content.Load<Texture2D>("wolf");
        }
        protected override void SetCreatureState(GameTime gameTime)
        {
            _creatureState = CreatureState.Patrol;
            bool hasChecks = false;
            foreach (var sound in _elementsAround.ToArray())
            {
                sound.CalculateDistance(this.Position);
                if (sound.Distance > _game.Graphics.PreferredBackBufferHeight)
                {
                    _elementsAround.Remove(sound);
                    continue;
                }

                if (sound.Priority != Priority.Ignore)
                {
                    if (sound.SourceOfSound is Bush)
                    {
                        sound.Priority = Priority.Check;
                        hasChecks = true;
                        if (_creatureState != CreatureState.Fight)
                            _creatureState = CreatureState.HeardSomething;
                    }
                    else
                        _creatureState = CreatureState.Fight;
                }
            }
            if (hasChecks && _creatureState == CreatureState.Fight)
                foreach (var element in _elementsAround.ToArray())
                    if (element.Priority == Priority.Check)
                        _elementsAround.Remove(element);
        }
        protected override void Move(GameTime gameTime)
        {
            if (_creatureState == CreatureState.Patrol)
            {
                _movementDir = (MovementDirection)_rnd.Next(0, 9);
            }
            else if (_creatureState == CreatureState.Fight)
            {
                RelationsManager focused = GetNearestElement(Priority.Track);
                if (CheckCollision(focused.SourceOfSound.ElementRect))
                    _movementDir = MovementDirection.Stay;
                else
                    MoveTo(focused.SourceOfSound.Position);
            }
            else if (_creatureState == CreatureState.HeardSomething)
            {
                RelationsManager focused = GetNearestElement(Priority.Check);
                MoveTo(focused.Position);
                if (CheckCollision(focused.SourceOfSound.ElementRect))
                    _elementsAround.Remove(focused);
            }
        }

        protected override void WhileAttack(GameTime gameTime)
        {
            if (_creatureState == CreatureState.Fight && gameTime.TotalGameTime.TotalSeconds - _timeAttackStart > 0.1)
                _attackSoundVolume = 0;
            if (_creatureState == CreatureState.Fight && gameTime.TotalGameTime.TotalSeconds - _timeAttackStart > 1f)
                Attack(gameTime);
        }
        protected override void Attack(GameTime gameTime)
        {
            RelationsManager creature = GetNearestElement(Priority.Track);
            Creatures attackedCreature = (Creatures)creature.SourceOfSound;
            if (attackedCreature.IsAlive)
            {
                if (CheckCollision(attackedCreature.ElementRect))
                {
                    _timeAttackStart = gameTime.TotalGameTime.TotalSeconds;
                    _attackSoundVolume = 0.8f;
                    _transparency = 255;
                    Sound.PlaySound(PositionOnScreen, Sound.dogAttacks);
                    if (attackedCreature is Monster)
                        Hit(attackedCreature, 30, 1);
                    else
                        Hit(attackedCreature, 1, 0);
                }
            }
            else
                creature.Priority = Priority.Ignore;
        }
    }

    class Deer : Creatures
    {
        bool _runningAway = false;
        public Deer(Game1 game) : base(game)
        {
            _speed = 95;
            _visionRadius = 3.5f;
        }
        protected override void Move(GameTime gameTime)
        {
            if (_creatureState == CreatureState.Patrol)
            {
                _movementDir = (MovementDirection)_rnd.Next(0, 9);
            }
            else if (_creatureState == CreatureState.RunAway)
            {
                RelationsManager focused = GetNearestElement(Priority.Track);
                RunAway(focused.SourceOfSound.Position);
            }
        }

        protected override void SetCreatureState(GameTime gameTime)
        {
            _creatureState = CreatureState.Patrol;
            foreach (var sound in _elementsAround.ToArray())
            {
                sound.CalculateDistance(this.Position);
                if (sound.Distance > _game.Graphics.PreferredBackBufferHeight)
                {
                    _elementsAround.Remove(sound);
                    continue;
                }
                _creatureState = CreatureState.RunAway;
            }
        }

        public override void Load()
        {
            _texture = _game.Content.Load<Texture2D>("deer");
        }
        public override void Update(GameTime gameTime)
        {
            if (_isAlive)
            {
                if (Map.ElementsOnScreen.Contains(this))
                {
                    MakeNoise();
                    LookAround();
                }

                if (_creatureState != CreatureState.Patrol)
                {
                    if (!_runningAway)
                    {
                        _runningAway = true;
                        Sound.PlaySound(PositionOnScreen, Sound.deerRun);
                    }
                    SetCreatureState(gameTime);
                }
                else
                    _runningAway = false;

                WhileMakingMove(gameTime, 0);

                _transparency = _transparency - 2 > 0 ? _transparency - 2 : 0;
            }
            else if (_transparency <= 0 && _soundWaveTransparency <= 0)
            {
                Map.ElementsToDelete.Add(this);
            }
            else
            {
                _transparency -= 7;
                _soundWaveTransparency -= 7;
            }
        }


        protected override void Hit(Creatures whoHit, int force, int chance) { } // cant attack
        protected override void WhileAttack(GameTime gameTime) { } // cant attack
        protected override void Attack(GameTime gameTime) { } // cant attack
    }

    class Monster : Creatures
    {
        double _hunger;
        float _volume;
        float _volumeWhenHungry = 0;
        float _timeAttackFrameChange = 0;
        int _currentAttackFrame = 0;
        Vector2 _whereToJump;
        Texture2D _attackTextureSheet;
        Texture2D _redBlurredFrame;
        Texture2D _whereToJumpMark;
        AudioListener _listener = new AudioListener();
        int _blurredFrameTransparency;
        NAudio.CoreAudioApi.MMDevice _volMeter;

        public static AudioListener Listener { get; set; }
        public double Hunger { get { return _hunger; } set { _hunger = value; } }
        public Monster(Game1 game) : base(game) 
        {
            _visionRadius = 0;
            _speed = 80;
            _health = 100;
            _hunger = 100;
            _position = new Vector2(game.Graphics.PreferredBackBufferWidth / 2, game.Graphics.PreferredBackBufferHeight / 2);
            _position.X += Map.ScreenPos.X;
            _position.Y += Map.ScreenPos.Y;
            _volMeter = new NAudio.CoreAudioApi.MMDeviceEnumerator().GetDefaultAudioEndpoint(NAudio.CoreAudioApi.DataFlow.Capture, NAudio.CoreAudioApi.Role.Console);
        }
        public override void Load()
        {
            _texture = _game.Content.Load<Texture2D>("monster");
            _attackTextureSheet = _game.Content.Load<Texture2D>("bigEarSpriteSheet");
            _redBlurredFrame = _game.Content.Load<Texture2D>("blurredFrame");
            _whereToJumpMark = _game.Content.Load<Texture2D>("mark");
            _listener.Position = new Vector3(_game.Graphics.PreferredBackBufferWidth / 2 - _texture.Width / 2, _game.Graphics.PreferredBackBufferHeight / 2 - _texture.Height / 2, 0);
            Listener = _listener;
            _position.X -= _texture.Width / 2;
            _position.Y -= _texture.Height / 2;
        }
        protected override void WhileMakingMove(GameTime gameTime, float dominantSoundVol)
        {
            if (_movementDir == MovementDirection.Stay)
            {
                if (_creatureState != CreatureState.Fight)
                    Move(gameTime);
                CalculateFlashEnvironment(0, dominantSoundVol);
                if (_movementDir != MovementDirection.Stay)
                {
                    _rotation = CalculateRotationChangePos();
                    CalculateFlashEnvironment(0.2f, dominantSoundVol);
                    _transparency += 15;
                    _timeStepStart = gameTime.TotalGameTime.TotalSeconds;
                    Sound.PlaySound(Sound.makeStep);
                }
            }
            else if (gameTime.TotalGameTime.TotalSeconds - _timeStepStart < 0.7)
            {
                CalculateFlashEnvironment(0.2f, dominantSoundVol);
                _transparency = _transparency + 15 < 255 ? _transparency + 15 : 255;
            }
            else if (gameTime.TotalGameTime.TotalSeconds - _timeStepStart < 1.65)
            {
                CalculateFlashEnvironment(0, dominantSoundVol);
            }
            else
                _movementDir = MovementDirection.Stay;
        }

        void WhenHungry()
        {
            if (_hunger < 30)
            {
                if (rnd.Next(0, 300) == 0)
                {
                    _volumeWhenHungry = 0.7f;
                    Sound.PlaySound(Sound.hungry);
                }
                if (_volumeWhenHungry > _volume)
                    _volume = _volumeWhenHungry;
            }
        }
        public override void Update(GameTime gameTime)
        {
            if (_isAlive)
            {
                MakeNoise();

                _volume = _volMeter.AudioMeterInformation.MasterPeakValue * 50;

                WhenHungry();

                WhileMakingMove(gameTime, _volume);

                WhileAttack(gameTime);

                _transparency = _transparency - 5 > 0 ? _transparency - 5 : 0;
                _volumeWhenHungry = _volumeWhenHungry - 0.005f > 0 ? _volumeWhenHungry - 0.005f : 0;
                _hunger = _hunger - gameTime.ElapsedGameTime.TotalSeconds > 0 ? _hunger - gameTime.ElapsedGameTime.TotalSeconds : 0;
                CentreCam();
                _blurredFrameTransparency = 255 - (int)(255 / 100f * _health);
            }
            else if (_transparency > 0)
                _transparency -= 10;
            else
                Interface.Window = CurrentWindow.DeathMenu;
        }
        public void CentreCam()
        {
            Map.ScreenPos = new Vector2(this.Position.X - _game.Graphics.PreferredBackBufferWidth / 2 + this._texture.Width / 2,
                this.Position.Y - _game.Graphics.PreferredBackBufferHeight / 2 + this._texture.Height / 2);
        }
        protected override void Move(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                _movementDir = MovementDirection.Bottom;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                _movementDir = MovementDirection.Top;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                if (_movementDir == MovementDirection.Bottom)
                    _movementDir = MovementDirection.BottomLeft;
                else if (_movementDir == MovementDirection.Top)
                    _movementDir = MovementDirection.TopLeft;
                else
                    _movementDir = MovementDirection.Left;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                if (_movementDir == MovementDirection.Bottom)
                    _movementDir = MovementDirection.BottomRight;
                else if (_movementDir == MovementDirection.Top)
                    _movementDir = MovementDirection.TopRight;
                else
                    _movementDir = MovementDirection.Right;
            }
        }
        void DrawAttack()
        {
            _game.Sprite.Draw(_attackTextureSheet, new Vector2(PositionOnScreen.X + _texture.Width / 2 - _attackTextureSheet.Width / 12, PositionOnScreen.Y + _texture.Height / 2 - _attackTextureSheet.Height / 2),
                new Rectangle(_attackTextureSheet.Width / 6 * _currentAttackFrame, 0, _attackTextureSheet.Width / 6, _attackTextureSheet.Height),
                Color.FromNonPremultiplied(255, 255, 255, _transparency));
            _game.Sprite.Draw(_whereToJumpMark, new Vector2(_whereToJump.X - _whereToJumpMark.Width / 2 - Map.ScreenPos.X, _whereToJump.Y - _whereToJumpMark.Height / 2 - Map.ScreenPos.Y), Color.White);
        }
        public override void Draw(GameTime gameTime)
        {
            DrawRedFrame();
            base.Draw(gameTime);
            if (_creatureState == CreatureState.Fight)
                DrawAttack();
        }

        void DrawRedFrame()
        {
            _game.Sprite.Draw(_redBlurredFrame,
                    new Rectangle(0, 0, _game.Graphics.PreferredBackBufferWidth, _game.Graphics.PreferredBackBufferHeight),
                    null,
                    Color.FromNonPremultiplied(255, 255, 255, _blurredFrameTransparency));
        }
        protected override void MakeNoise()
        {
            foreach (var element in Map.ElementsOnScreen)
            {
                if (SoundWaveCollision(element.ElementRect) && element != this)
                {
                    element.Transparency = element.Transparency + 5 < 255 ? element.Transparency + 5 : 255;
                    if (element is Creatures)
                    {
                        Creatures creature = (Creatures)element;
                        creature.ReactToSoundOrSee(this);
                    }
                }
            }
        }
        public override void ReactToSoundOrSee(MapElements soundingElement) 
        {
            soundingElement.Transparency = soundingElement.Transparency + 5 < 255 ? soundingElement.Transparency + 5 : 255;
        }

        protected override void WhileAttack(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                _transparency = _transparency + 10 < 255 ? _transparency + 10 : 255;
                if (_creatureState == CreatureState.Fight)
                {
                    _timeAttackFrameChange += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (_timeAttackFrameChange > 0.1 && _currentAttackFrame < 4)
                    {
                        _currentAttackFrame++;
                        _timeAttackFrameChange = _timeAttackFrameChange - 0.1f;
                    }
                }
                else
                {
                    Sound.PlaySound(Sound.bigEarOpens);
                    _creatureState = CreatureState.Fight;
                    _currentAttackFrame = 0;
                    _transparency = 0;
                    _timeAttackFrameChange = 0;
                    _whereToJump = new Vector2(Position.X + _texture.Width / 2, Position.Y + _texture.Height / 2);
                }

                if (_currentAttackFrame == 4)
                {
                    ChooseAttackDirection();
                }
            }
            else if (_currentAttackFrame == 4)
                Attack(gameTime);
            else if (_transparency <= 0)
                _creatureState = CreatureState.Patrol;


        }
        protected override void Attack(GameTime gameTime) 
        {
            Position = new Vector2(_whereToJump.X - _texture.Width / 2, _whereToJump.Y - _texture.Height / 2);
            Sound.PlaySound(Sound.monsterJumps);
            foreach (var creature in Map.ElementsOnScreen)
                if (!(creature is StaticObjects) && CheckCollision(creature.ElementRect) && creature != this)
                {
                    Hit((Creatures)creature, 1, 0);
                    creature.Transparency = 0;
                    _health = _health + 30 < 100 ? _health + 30 : 100;
                    _hunger = _hunger + 35 < 100 ? _hunger + 35 : 100;
                    Sound.PlaySound(Sound.monsterAttacks);
                }
            _currentAttackFrame = 5;
        }

        void ChooseAttackDirection()
        {
            KeyboardState keyboard = Keyboard.GetState();
            float jumpDistance = MathF.Sqrt(MathF.Pow(Position.X + _texture.Width / 2 - _whereToJump.X, 2) + MathF.Pow(Position.Y + _texture.Height / 2 - _whereToJump.Y, 2));
            if (keyboard.IsKeyDown(Keys.Up))
            {
                _whereToJump.Y -= 1;
                jumpDistance = MathF.Sqrt(MathF.Pow(Position.X + _texture.Width / 2 - _whereToJump.X, 2) + MathF.Pow(Position.Y + _texture.Height / 2 - _whereToJump.Y, 2));
                if (jumpDistance > _game.Graphics.PreferredBackBufferHeight / 5)
                    _whereToJump.Y += 1;
            }
            if (keyboard.IsKeyDown(Keys.Down))
            {
                _whereToJump.Y += 1;
                jumpDistance = MathF.Sqrt(MathF.Pow(Position.X + _texture.Width / 2 - _whereToJump.X, 2) + MathF.Pow(Position.Y + _texture.Height / 2 - _whereToJump.Y, 2));
                if (jumpDistance > _game.Graphics.PreferredBackBufferHeight / 5)
                    _whereToJump.Y -= 1;
            }
            if (keyboard.IsKeyDown(Keys.Left))
            {
                _whereToJump.X -= 1;
                jumpDistance = MathF.Sqrt(MathF.Pow(Position.X + _texture.Width / 2 - _whereToJump.X, 2) + MathF.Pow(Position.Y + _texture.Height / 2 - _whereToJump.Y, 2));
                if (jumpDistance > _game.Graphics.PreferredBackBufferHeight / 5)
                    _whereToJump.X += 1;
            }
            if (keyboard.IsKeyDown(Keys.Right))
            {
                _whereToJump.X += 1;
                jumpDistance = MathF.Sqrt(MathF.Pow(Position.X + _texture.Width / 2 - _whereToJump.X, 2) + MathF.Pow(Position.Y + _texture.Height / 2 - _whereToJump.Y, 2));
                if (jumpDistance > _game.Graphics.PreferredBackBufferHeight / 5)
                    _whereToJump.X -= 1;
            }

        }
        protected override void SetCreatureState(GameTime gameTime) { } // empty
    }

}

