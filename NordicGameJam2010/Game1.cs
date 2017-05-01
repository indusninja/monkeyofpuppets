using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using NordicGameJam2010.Components;

namespace NordicGameJam2010
{
    public enum GameState
    {
        Progress,
        SplashScreen,
        Pause,
        LevelWin,
        LevelLoose,
        Play,
        Stop,
        GameWin
    }

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        public static int ScreenWidth = 1280;
        public static int ScreenHeight = 720;

        private GameState gameState = GameState.SplashScreen;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D splashscreen, endscreen, loseLevel, winLevel;

        SpriteFont hud;

        List<string> lvlList = new List<string>();
        int currentLevelIndex = 0;
        Level lvl;
        TileLayer tileLayer;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            lvl = new Level();
            lvlList.Add("Content/Levels/Level1.layer");
            lvlList.Add("Content/Levels/Level2.layer");
            lvlList.Add("Content/Levels/Level3.layer");
            lvlList.Add("Content/Levels/Level5.layer");
            lvlList.Add("Content/Levels/Level4.layer");
            lvlList.Add("Content/Levels/Level6.layer");
            lvlList.Add("Content/Levels/Level7.layer");
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = ScreenWidth;
            graphics.PreferredBackBufferHeight = ScreenHeight;
            graphics.IsFullScreen = true;
            graphics.ApplyChanges();

            base.Initialize();

            lvl.Layers.Add(tileLayer);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            hud = Content.Load<SpriteFont>("Hud");

            Audio.LoadAudio(Content, "Content/Audio/game.audio");

            splashscreen = Content.Load<Texture2D>("SplashScreens/main");
            endscreen = Content.Load<Texture2D>("SplashScreens/end");
            loseLevel = Content.Load<Texture2D>("SplashScreens/lost_level");
            winLevel = Content.Load<Texture2D>("SplashScreens/won_level");

            ResetGame();
        }

        protected void ResetGame()
        {
            Settings.ReadSettings("Content/Settings.ini");

            tileLayer = TileLayer.FromFile(Content, lvlList[currentLevelIndex]);

            Components.Clear();

            lvl.ResetLevel(Content, lvlList[currentLevelIndex]);

            for (int i = 0; i < lvl.pickupPositions.Count; i++)
                Components.Add(new Pickup(this, tileLayer, lvl.pickupPositions[i] + new Vector2(0.5f, 0.5f), lvl.pickupTextures[i]));

            for (int i = 0; i < lvl.enemyTextures.Count; i++)
                Components.Add(new Enemy(this, tileLayer, lvl.enemyPositions[i] + new Vector2(0.5f, 0.5f), lvl.enemyTextures[i]));

            Player player = new Player(this, tileLayer, lvl.playerStartPosition + new Vector2(0.5f, 0.5f), lvl.playerTexture, lvl.DecoyCount);
            Components.Add(player);
            TileLayer.Player = player;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if ((GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed) || (Keyboard.GetState().IsKeyDown(Keys.Escape)))
                this.Exit();

            if (((gameState == GameState.SplashScreen)) &&
                (Keyboard.GetState().IsKeyDown(Keys.Enter) || GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.Start)))
                UpdateGameState(GameState.Progress);

            if ((gameState == GameState.LevelWin) &&
                (Keyboard.GetState().IsKeyDown(Keys.Enter) || GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.Start)))
                UpdateGameState(GameState.Progress);

            if ((gameState == GameState.LevelLoose) &&
                (Keyboard.GetState().IsKeyDown(Keys.Enter) || GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.Start)))
                UpdateGameState(GameState.Progress);

            if ((gameState == GameState.GameWin) &&
                (Keyboard.GetState().IsKeyDown(Keys.Enter) || GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.Start)))
                Exit();

            // Disabled in final build
            //if (Keyboard.GetState().IsKeyDown(Keys.R))
            //{
            //    ResetGame();
            //    Audio.State = AudioState.Playing;
            //}

            switch (Audio.State)
            {
                case AudioState.Kill:
                    Audio.BackgroundAudio.Stop(true);
                    Audio.KillAudio.Play();
                    Audio.State = AudioState.None;
                    break;
                case AudioState.Win:
                    Audio.BackgroundAudio.Stop(true);
                    Audio.WinAudio.Play();
                    Audio.State = AudioState.None;
                    break;
                case AudioState.Angry:
                    Audio.AngryAudio.Play();
                    Audio.State = AudioState.Playing;
                    break;
                case AudioState.Playing:
                    if (Audio.BackgroundAudio.State != SoundState.Playing)
                        Audio.BackgroundAudio.Play();
                    break;
                case AudioState.None:
                    Audio.BackgroundAudio.Stop(true);
                    break;
                default:
                    break;
            }

            if (gameState == GameState.Play)
                base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteBlendMode.AlphaBlend);

            if (gameState == GameState.SplashScreen)
            {
                Vector2 pos = new Vector2(ScreenWidth / 2, ScreenHeight / 2);
                spriteBatch.Draw(splashscreen,
                    pos,
                    null,
                    Color.White,
                    0.0f,
                    new Vector2(splashscreen.Width / 2, splashscreen.Height / 2),
                    1.0f,
                    SpriteEffects.None,
                    1.0f);
            }
            else if (gameState == GameState.GameWin)
            {
                Vector2 pos = new Vector2(ScreenWidth / 2, ScreenHeight / 2);
                spriteBatch.Draw(endscreen,
                    pos,
                    null,
                    Color.White,
                    0.0f,
                    new Vector2(endscreen.Width / 2, endscreen.Height / 2),
                    1.0f,
                    SpriteEffects.None,
                    1.0f);
            }
            else
            {
                lvl.Draw(spriteBatch);

                // Get sorted list so UpdateOrder also defines render order
                IEnumerable<GameComponent> sortedComponents = Components.Cast<GameComponent>().OrderBy(gameComponent => gameComponent.UpdateOrder);
                foreach (GameObject gameObject in sortedComponents)
                {
                    if (gameObject != null)
                    {
                        gameObject.Draw(spriteBatch);
                    }
                }

                if (gameState == GameState.LevelWin)
                {
                    Vector2 pos = new Vector2(ScreenWidth / 2, ScreenHeight / 2);
                    spriteBatch.Draw(winLevel,
                        pos,
                        null,
                        Color.White,
                        0.0f,
                        new Vector2(winLevel.Width / 2, winLevel.Height / 2),
                        1.0f,
                        SpriteEffects.None,
                        1.0f);
                }
                else if (gameState == GameState.LevelLoose)
                {
                    Vector2 pos = new Vector2(ScreenWidth / 2, ScreenHeight / 2);
                    spriteBatch.Draw(loseLevel,
                        pos,
                        null,
                        Color.White,
                        0.0f,
                        new Vector2(loseLevel.Width / 2, loseLevel.Height / 2),
                        1.0f,
                        SpriteEffects.None,
                        1.0f);
                }

            }
            spriteBatch.End();

            base.Draw(gameTime);
        }

        public void UpdateGameState(GameState state)
        {
            switch (state)
            {
                case GameState.Progress:
                    switch (gameState)
                    {
                        case GameState.SplashScreen:
                            Play();
                            break;
                        case GameState.LevelWin:
                            Play();
                            break;
                        case GameState.LevelLoose:
                            if (currentLevelIndex == lvlList.Count - 1)
                            {
                                gameState = GameState.GameWin;
                                break;
                            }
                            else
                                Play();
                            break;
                        case GameState.Pause:
                            Play();
                            break;
                        default:
                            break;
                    }
                    break;
                case GameState.SplashScreen:
                    this.Components.Clear();
                    gameState = GameState.SplashScreen;
                    break;
                case GameState.Play:
                    Play();
                    break;
                case GameState.LevelWin:
                    currentLevelIndex++;
                    if (currentLevelIndex > lvlList.Count)
                    {
                        currentLevelIndex = lvlList.Count - 1;
                        gameState = GameState.GameWin;
                    }
                    else
                        gameState = GameState.LevelWin;
                    Audio.State = AudioState.Win;
                    break;
                case GameState.LevelLoose:
                    if (currentLevelIndex == lvlList.Count - 1)
                        gameState = GameState.GameWin;
                    else
                        gameState = GameState.LevelLoose;
                    Audio.State = AudioState.Kill;
                    break;
                case GameState.GameWin:
                    this.Components.Clear();
                    gameState = GameState.GameWin;
                    Audio.State = AudioState.None;
                    break;
                case GameState.Pause:
                    gameState = GameState.Pause;
                    Audio.State = AudioState.None;
                    break;
                case GameState.Stop:
                    gameState = GameState.Stop;
                    Audio.State = AudioState.None;
                    break;
                default:
                    break;
            }
        }

        protected void Play()
        {
            ResetGame();
            gameState = GameState.Play;
            Audio.State = AudioState.Playing;
        }
    }
}
