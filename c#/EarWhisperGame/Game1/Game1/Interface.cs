using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace Game1
{
    static class Interface
    {
        static SpriteFont menuFont;
        static List<string> mainMenuEntries;
        static List<string> deathMenuEntries;
        static List<string> escapeMenuEntries;
        static List<string> winMenuEntries;
        static int[] transparency = new int[3] {0, 0, 0};
        static CurrentWindow currentWindow = CurrentWindow.MainMenu;
        static int selectedIndex = 0;
        static float timeBetweenButtonPush = -10;
        static int enemiesLeft = 70;
        static Texture2D pixel;
        static Texture2D author;
        static int titleTransparency = 0;
        static public CurrentWindow Window { get { return currentWindow; } set { currentWindow = value; } }
        static public int EnemiesLeft { get { return enemiesLeft; } set { enemiesLeft = value; } }
        public static void Initialize()
        {
            mainMenuEntries = new List<string>();
            mainMenuEntries.Add("Play");
            mainMenuEntries.Add("Author");
            mainMenuEntries.Add("Quit");

            deathMenuEntries = new List<string>();
            deathMenuEntries.Add("Try again");
            deathMenuEntries.Add("Quit");

            escapeMenuEntries = new List<string>();
            escapeMenuEntries.Add("Continue");
            escapeMenuEntries.Add("Retry");
            escapeMenuEntries.Add("Quit");

            winMenuEntries = new List<string>();
            winMenuEntries.Add("Play again");
            winMenuEntries.Add("Quit");
        }
        public static void Show()
        {
            int spaceBetweenStrings = 50;
            if (currentWindow == CurrentWindow.MainMenu)
            {
                float stringHeight = menuFont.MeasureString(mainMenuEntries[0]).Y;
                float topPos = Game1.ThisGame.Graphics.PreferredBackBufferHeight / 2 - (stringHeight * mainMenuEntries.Count + spaceBetweenStrings * (mainMenuEntries.Count - 1)) / 2;
                int i = 0;
                foreach (string str in mainMenuEntries)
                {
                    float stringWidth = menuFont.MeasureString(str).X;
                    Game1.ThisGame.Sprite.DrawString(menuFont, str,
                        new Vector2(Game1.ThisGame.Graphics.PreferredBackBufferWidth / 2 - stringWidth / 2, topPos + stringHeight * i + spaceBetweenStrings * i),
                        Color.FromNonPremultiplied(255, 255, 255, transparency[i]));
                    i++;
                }
                Game1.ThisGame.Sprite.DrawString(menuFont, "Ear Whisper",
                    new Vector2(Game1.ThisGame.Graphics.PreferredBackBufferWidth / 2 - menuFont.MeasureString("Ear Whisper").X / 2, 60),
                    Color.FromNonPremultiplied(255, 255, 255, titleTransparency));
            }
            else if (currentWindow == CurrentWindow.EscapeMenu)
            {
                float stringHeight = menuFont.MeasureString(mainMenuEntries[0]).Y;
                float topPos = Game1.ThisGame.Graphics.PreferredBackBufferHeight / 2 - (stringHeight * escapeMenuEntries.Count + spaceBetweenStrings * (escapeMenuEntries.Count - 1)) / 2;
                int i = 0;
                foreach (string str in escapeMenuEntries)
                {
                    float stringWidth = menuFont.MeasureString(str).X;
                    Game1.ThisGame.Sprite.DrawString(menuFont, str,
                        new Vector2(Game1.ThisGame.Graphics.PreferredBackBufferWidth / 2 - stringWidth / 2, topPos + stringHeight * i + spaceBetweenStrings * i),
                        Color.FromNonPremultiplied(255, 255, 255, transparency[i]));
                    i++;
                }
                Game1.ThisGame.Sprite.DrawString(menuFont, "Game is Paused",
                    new Vector2(Game1.ThisGame.Graphics.PreferredBackBufferWidth / 2 - menuFont.MeasureString("Game is Paused").X / 2, 60), 
                    Color.FromNonPremultiplied(255, 255, 255, titleTransparency));
            }
            else if (currentWindow == CurrentWindow.DeathMenu)
            {
                float stringHeight = menuFont.MeasureString(mainMenuEntries[0]).Y;
                float topPos = Game1.ThisGame.Graphics.PreferredBackBufferHeight / 2 - (stringHeight * deathMenuEntries.Count + spaceBetweenStrings * (deathMenuEntries.Count - 1)) / 2;
                int i = 0;
                foreach (string str in deathMenuEntries)
                {
                    float stringWidth = menuFont.MeasureString(str).X;
                    Game1.ThisGame.Sprite.DrawString(menuFont, str,
                        new Vector2(Game1.ThisGame.Graphics.PreferredBackBufferWidth / 2 - stringWidth / 2, topPos + stringHeight * i + spaceBetweenStrings * i),
                        Color.FromNonPremultiplied(255, 255, 255, transparency[i]));
                    i++;
                }
                Game1.ThisGame.Sprite.DrawString(menuFont, "You Lose",
                    new Vector2(Game1.ThisGame.Graphics.PreferredBackBufferWidth / 2 - menuFont.MeasureString("You Lose").X / 2, 60),
                    Color.FromNonPremultiplied(255, 255, 255, titleTransparency));

            }
            else if (currentWindow == CurrentWindow.WinMenu)
            {
                float stringHeight = menuFont.MeasureString(mainMenuEntries[0]).Y;
                float topPos = Game1.ThisGame.Graphics.PreferredBackBufferHeight / 2 - (stringHeight * winMenuEntries.Count + spaceBetweenStrings * (winMenuEntries.Count - 1)) / 2;
                int i = 0;
                foreach (string str in winMenuEntries)
                {
                    float stringWidth = menuFont.MeasureString(str).X;
                    Game1.ThisGame.Sprite.DrawString(menuFont, str,
                        new Vector2(Game1.ThisGame.Graphics.PreferredBackBufferWidth / 2 - stringWidth / 2, topPos + stringHeight * i + spaceBetweenStrings * i),
                        Color.FromNonPremultiplied(255, 255, 255, transparency[i]));
                    i++;
                }
                Game1.ThisGame.Sprite.DrawString(menuFont, "You Win!",
                    new Vector2(Game1.ThisGame.Graphics.PreferredBackBufferWidth / 2 - menuFont.MeasureString("You Win!").X / 2, 60),
                    Color.FromNonPremultiplied(255, 255, 255, titleTransparency));

            }
            else if (currentWindow == CurrentWindow.Game)
            {
                Game1.ThisGame.Sprite.DrawString(menuFont, Convert.ToString(enemiesLeft),
                    new Vector2(Game1.ThisGame.Graphics.PreferredBackBufferWidth - 60, 10),
                    Color.FromNonPremultiplied(255, 255, 255, 150), 0, Vector2.Zero, 0.3f, SpriteEffects.None, 0);
                Game1.ThisGame.Sprite.Draw(pixel, new Vector2(10, 10) + Map.ScreenPos / 10000 * 100, Color.White);
            }
            else if (currentWindow == CurrentWindow.AboutAuthorMenu)
            {
                Game1.ThisGame.Sprite.Draw(author, new Rectangle(0, 0, Game1.ThisGame.Graphics.PreferredBackBufferWidth, Game1.ThisGame.Graphics.PreferredBackBufferHeight), Color.White);
            }
        }

        static public void Update(GameTime gameTime)
        {
            if (currentWindow == CurrentWindow.Game)
            {
                transparency = new int[3] { 0, 0, 0 };
                selectedIndex = 0;
                titleTransparency = 0;
                if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                {
                    currentWindow = CurrentWindow.EscapeMenu;
                }
                if (enemiesLeft <= 0)
                {
                    currentWindow = CurrentWindow.WinMenu;
                }
            }
            else
            {
                titleTransparency++;
                if (gameTime.TotalGameTime.TotalSeconds - timeBetweenButtonPush > 0.25)
                {
                    if (Keyboard.GetState().IsKeyDown(Keys.Down))
                        selectedIndex++;
                    else if (Keyboard.GetState().IsKeyDown(Keys.Up))
                        selectedIndex--;
                    timeBetweenButtonPush = (float)gameTime.TotalGameTime.TotalSeconds;
                }

                if (currentWindow == CurrentWindow.MainMenu)
                {
                    if (selectedIndex == mainMenuEntries.Count)
                        selectedIndex = 0;
                    else if (selectedIndex < 0)
                        selectedIndex = mainMenuEntries.Count - 1;

                    if (Keyboard.GetState().IsKeyDown(Keys.Space))
                        switch (selectedIndex)
                        {
                            case 0:
                                currentWindow = CurrentWindow.Game;
                                Map.Initialize(Game1.ThisGame);
                                break;
                            case 1:
                                currentWindow = CurrentWindow.AboutAuthorMenu;
                                break;
                            case 2:
                                Game1.ThisGame.Exit();
                                break;
                        }
                }
                else if (currentWindow == CurrentWindow.DeathMenu)
                {
                    if (selectedIndex == deathMenuEntries.Count)
                        selectedIndex = 0;
                    else if (selectedIndex < 0)
                        selectedIndex = deathMenuEntries.Count - 1;

                    if (Keyboard.GetState().IsKeyDown(Keys.Space))
                        switch (selectedIndex)
                        {
                            case 0:
                                currentWindow = CurrentWindow.Game;
                                Map.Initialize(Game1.ThisGame);
                                break;
                            case 1:
                                Game1.ThisGame.Exit();
                                break;
                        }
                }
                else if (currentWindow == CurrentWindow.EscapeMenu)
                {
                    if (selectedIndex == escapeMenuEntries.Count)
                        selectedIndex = 0;
                    else if (selectedIndex < 0)
                        selectedIndex = escapeMenuEntries.Count - 1;

                    if (Keyboard.GetState().IsKeyDown(Keys.Space))
                        switch (selectedIndex)
                        {
                            case 0:
                                currentWindow = CurrentWindow.Game;
                                break;
                            case 1:
                                Map.Initialize(Game1.ThisGame);
                                currentWindow = CurrentWindow.Game;
                                break;
                            case 2:
                                Game1.ThisGame.Exit();
                                break;
                        }
                }
                else if (currentWindow == CurrentWindow.WinMenu)
                {
                    if (selectedIndex == winMenuEntries.Count)
                        selectedIndex = 0;
                    else if (selectedIndex < 0)
                        selectedIndex = winMenuEntries.Count - 1;

                    if (Keyboard.GetState().IsKeyDown(Keys.Space))
                        switch (selectedIndex)
                        {
                            case 0:
                                currentWindow = CurrentWindow.Game;
                                Map.Initialize(Game1.ThisGame);
                                break;
                            case 1:
                                Game1.ThisGame.Exit();
                                break;
                        }
                }
                else if (currentWindow == CurrentWindow.AboutAuthorMenu)
                {
                    if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                        currentWindow = CurrentWindow.MainMenu;
                    selectedIndex = 0;
                }

                transparency[selectedIndex] = transparency[selectedIndex] + 5 < 255 ? transparency[selectedIndex] + 5 : 255;

                for (int i = 0; i < transparency.Length; i++)
                    transparency[i] = transparency[i] - 3 > 0 ? transparency[i] - 3 : 0;
            }

        }

        public static void Load()
        {
            menuFont = Game1.ThisGame.Content.Load<SpriteFont>("MainMenuFont");
            pixel = Game1.ThisGame.Content.Load<Texture2D>("whitePixel");
            author = Game1.ThisGame.Content.Load<Texture2D>("author");
        }

    }

}