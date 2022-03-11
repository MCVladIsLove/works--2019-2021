using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;

namespace Game1
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        static public Texture2D step;
        static public Game1 ThisGame { get; set; }
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 1980;
            _graphics.PreferredBackBufferHeight = 1024;
            _graphics.IsFullScreen = false;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            ThisGame = this;
            Sound.SetMicrophone();
        }
        public GraphicsDeviceManager Graphics { get { return _graphics; } }
        public SpriteBatch Sprite { get { return _spriteBatch; } }

        protected override void Initialize()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            Interface.Initialize();
            base.Initialize();
        }
        protected override void LoadContent()
        {
            step = Content.Load<Texture2D>("step");
            Sound.Load();
            Interface.Load();
        }
        protected override void Update(GameTime gameTime)
        {
            if (Interface.Window == CurrentWindow.Game)
                Map.Update(gameTime);
            Interface.Update(gameTime);
            base.Update(gameTime);
         }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.FromNonPremultiplied(0, 5, 30, 100));
            _spriteBatch.Begin();
            if (Interface.Window == CurrentWindow.Game)
                Map.Draw(gameTime);
            Interface.Show();
            _spriteBatch.End();
            base.Draw(gameTime);
        }

    }

}
