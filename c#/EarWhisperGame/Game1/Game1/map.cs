using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;

namespace Game1
{
    static class Map
    {
        static List<MapElements> _elements;
        static List<MapElements> _elementsOnScreen;
        static List<MapElements> _elementsToDelete;
        static Vector2 _screenPos;

        static public List<MapElements> ElementsOnScreen { get { return _elementsOnScreen; } }
        static public List<MapElements> ElementsToDelete { get { return _elementsToDelete; } }
        static public List<MapElements> Elements { get { return _elements; } }
        static public Vector2 ScreenPos { get { return _screenPos; } set { _screenPos = value; } }
        static Game1 _game;
        static public void Initialize(Game1 game)
        {
            _game = game;
            Interface.EnemiesLeft = 70;
            _elements = new List<MapElements>();
            _elementsOnScreen = new List<MapElements>();
            _elementsToDelete = new List<MapElements>();
            Random rnd = new Random();
            _screenPos = new Vector2(rnd.Next(0, 10000), rnd.Next(0, 10000));
            _elements.Add(new Monster(game));
            _elementsOnScreen.Add(_elements[0]);
            _elements[0].Load();
            for (int i = 0; i < 70; i++)
                _elements.Add(new Soldier(_game));
            for (int i = 0; i < 60; i++)
                _elements.Add(new Deer(_game));
            for (int i = 0; i < 300; i++)
                _elements.Add(new Uninteractable(_game));
            for (int i = 0; i < 60; i++)
                _elements.Add(new Bush(_game));
            for (int i = 0; i < 60; i++)
                _elements.Add(new Puddle(_game));
            for (int i = 0; i < 50; i++)
                _elements.Add(new Wolf(_game));
            for (int i = 0; i < 100; i++)
                _elements.Add(new Apple(_game));
        }

        static public void Update(GameTime gameTime)
        {

            foreach (var element in _elements.ToArray())
            {
                element.Update(gameTime);
                if (element.Position.X > _screenPos.X - 100 && element.Position.X < _screenPos.X + _game.Graphics.PreferredBackBufferWidth + 100
                && element.Position.Y > _screenPos.Y - 100 && element.Position.Y < _screenPos.Y + _game.Graphics.PreferredBackBufferHeight + 100
                && !_elementsOnScreen.Contains(element))
                {
                    _elementsOnScreen.Add(element);
                    element.Load();
                }
                else if ((element.Position.X < _screenPos.X - 100 || element.Position.X > _screenPos.X + _game.Graphics.PreferredBackBufferWidth + 100
                || element.Position.Y < _screenPos.Y - 100 || element.Position.Y > _screenPos.Y + _game.Graphics.PreferredBackBufferHeight + 100)
                && _elementsOnScreen.Contains(element))
                {
                    _elementsOnScreen.Remove(element);
                }

            }

            foreach (var element in _elementsToDelete)
            {
                _elements.Remove(element);
                _elementsOnScreen.Remove(element);
            }
            _elementsToDelete.Clear();
        }

        static public void Draw(GameTime gameTime)
        {
            foreach (var element in _elementsOnScreen)
                element.Draw(gameTime);
        }

    }

    abstract class MapElements : DrawableGameComponent
    {
        protected Random rnd = new Random();
        protected int _mapWidth = 10000;
        protected int _mapHeight = 10000;
        protected Vector2 _position;
        protected Texture2D _texture;
        protected Game1 _game;
        protected float _rotation = 0;
        protected int _transparency = 100;
        protected AudioEmitter _audioEmitter = new AudioEmitter();

        public int Transparency { get { return _transparency; } set { _transparency = value; } }

        public Rectangle ElementRect
        {
            get { return new Rectangle((int)Position.X, (int)Position.Y, _texture.Width, _texture.Height); } 
        }
        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public Vector2 PositionOnScreen
        {
            get { return new Vector2(_position.X - Map.ScreenPos.X, _position.Y - Map.ScreenPos.Y); }
        }
        public MapElements(Game1 game) : base(game)
        {
            _position = new Vector2(rnd.Next(0, _mapWidth), rnd.Next(0, _mapHeight));
            _game = game;
        }

        public override void Initialize()
        { }

        public abstract void Load();

        public override void Draw(GameTime gameTime)
        {
            _game.Sprite.Draw(_texture,
                new Vector2(PositionOnScreen.X + _texture.Width / 2, PositionOnScreen.Y + _texture.Height / 2),
                null, Color.FromNonPremultiplied(255, 255, 255, _transparency), _rotation, new Vector2(_texture.Width / 2, _texture.Height / 2), 
                1, SpriteEffects.None, 0);

        }

        public bool CheckCollision(Rectangle collideWith)
        {
            return ElementRect.Intersects(collideWith);
        }
        public bool CheckCollision(Rectangle collideWith, float rectangleScale)
        {
            Rectangle scaledRect = new Rectangle((int)(_position.X - (_texture.Width * rectangleScale / 2) + _texture.Width / 2),
                (int)(_position.Y - (_texture.Height * rectangleScale / 2) + _texture.Height / 2), 
                (int)(_texture.Width * rectangleScale), (int)(_texture.Height * rectangleScale));
            return scaledRect.Intersects(collideWith);
        }

    }

}