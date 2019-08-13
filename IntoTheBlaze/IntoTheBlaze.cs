using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using IntoTheBlaze.Game;
using IntoTheBlaze.Menu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace IntoTheBlaze {

    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class IntoTheBlaze : Microsoft.Xna.Framework.Game {

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private IGameScreen screen;

        private SpriteFont font;

        private Queue<float> updateFps, drawFps;

        public readonly MenuScreen MenuScreen;
        public readonly LevelSelectScreen LevelSelectScreen;
        public readonly CreditsScreen CreditsScreen;
        public readonly LevelScreen LevelScreen;
        public readonly LevelEndScreen LevelEndScreen;

        public IntoTheBlaze() {
            graphics = new GraphicsDeviceManager(this) {
                PreferredBackBufferWidth = 1024,
                PreferredBackBufferHeight = 640
            };
            Window.Title = "Into The Blaze";
            graphics.ApplyChanges();
            Content.RootDirectory = "Content";
            MenuScreen = new MenuScreen();
            LevelSelectScreen = new LevelSelectScreen();
            CreditsScreen = new CreditsScreen();
            LevelScreen = new LevelScreen();
            LevelEndScreen = new LevelEndScreen();
            screen = null;
            updateFps = new Queue<float>();
            updateFps.Enqueue(0);
            drawFps = new Queue<float>();
            drawFps.Enqueue(0);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize() {
            MouseHelper.Initialize();
            KeyboardHelper.Initialize();
            MenuScreen.Initialize(this);
            LevelSelectScreen.Initialize(this);
            CreditsScreen.Initialize(this);
            LevelScreen.Initialize(this);
            LevelEndScreen.Initialize(this);
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent() {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("font");
            MenuScreen.LoadContent();
            LevelSelectScreen.LoadContent();
            CreditsScreen.LoadContent();
            LevelScreen.LoadContent();
            LevelEndScreen.LoadContent();
            ChangeScreen(MenuScreen);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent() => Content.Unload();

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="time">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime time) {
            if (GraphicsDevice.PresentationParameters.BackBufferWidth != 1024 ||
                GraphicsDevice.PresentationParameters.BackBufferHeight != 640) {
                graphics.PreferredBackBufferWidth = 1024;
                graphics.PreferredBackBufferHeight = 640;
                graphics.ApplyChanges();
            }
            updateFps.Enqueue((float)(1000.0 / time.ElapsedGameTime.TotalMilliseconds));
            if (updateFps.Count > 300) 
                updateFps.Dequeue();

            MouseHelper.Update();
            KeyboardHelper.Update();
            screen?.Update(time);
            base.Update(time);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="time">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime time) {
            drawFps.Enqueue((float)(1000.0 / time.ElapsedGameTime.TotalMilliseconds));
            if (drawFps.Count > 300) 
                drawFps.Dequeue();

            GraphicsDevice.Clear(Color.Black);
            screen?.Draw(spriteBatch, time);

            spriteBatch.Begin();
            spriteBatch.DrawString(font, ((int)Math.Round(updateFps.Average())).ToString(), new Vector2(8, 8), Color.Red);
            spriteBatch.DrawString(font, ((int)Math.Round(drawFps.Average())).ToString(), new Vector2(108, 8), Color.Red);
            spriteBatch.End();

            base.Draw(time);
        }

        public void ChangeScreen(IGameScreen screen) {
            this.screen?.Leave(screen);
            screen?.Enter(this.screen);
            this.screen = screen;
        }

    }
}
