using System;
using System.Diagnostics;
using GLX;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace TimeGame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;

        KeyboardState previousKeyboardState = Keyboard.GetState();
        MouseState previousMouseState = Mouse.GetState();
        GamePadState previousGamePadState = GamePad.GetState(PlayerIndex.One);

        TextItem gameSpeedText;

        Player player;
        GameTimeWrapper mainGameTime;
        decimal oldGameSpeed;
        World world;

        EnemyHandler enemyHandler;

        public Game1()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            this.IsMouseVisible = true;
            System.Windows.Forms.Screen screen = System.Windows.Forms.Screen.AllScreens[0];
            Window.IsBorderless = true;
            Window.Position = new Point(screen.Bounds.X, screen.Bounds.Y);
            graphics.PreferredBackBufferWidth = screen.Bounds.Width;
            graphics.PreferredBackBufferHeight = screen.Bounds.Height;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            DebugText.Initialize(Vector2.Zero, DebugText.Corner.TopLeft, 0);
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            world = new World(graphics);
            mainGameTime = new GameTimeWrapper(MainUpdate, this, 1.0m);
            world.AddGameState("game1", graphics);
            world.gameStates["game1"].AddTime(mainGameTime);
            world.gameStates["game1"].AddDraw(GameDraw);
            world.gameStates["game1"].camera1.focus = Camera.Focus.Center;
            world.gameStates["game1"].camera1.pan.smoothingActive = true;
            world.gameStates["game1"].camera1.pan.smoothingType = TweenerBase.SmoothingType.RecursiveLinear;
            world.gameStates["game1"].camera1.pan.smoothingRate = 0.1f;

            world.AddMenuState("mainMenu", graphics, this);
            world.menuStates["mainMenu"].menuFont = Content.Load<SpriteFont>("DisplayFont");
            world.menuStates["mainMenu"].menuDirection = MenuState.Direction.TopToBottom;
            world.menuStates["mainMenu"].unselectedColor = Color.Black;
            world.menuStates["mainMenu"].selectedColor = Color.Yellow;
            world.menuStates["mainMenu"].initialPosition = new Vector2(graphics.GraphicsDevice.Viewport.Width / 2, graphics.GraphicsDevice.Viewport.Height / 2);
            world.menuStates["mainMenu"].spacing = 100;
            world.menuStates["mainMenu"].AddMenuItem("Play");
            world.menuStates["mainMenu"].SetMenuAction("Play", () =>
                {
                    world.ClearStates();
                    world.ActivateGameState("game1");
                });
            world.menuStates["mainMenu"].AddMenuItem("Help");
            world.menuStates["mainMenu"].AddMenuItem("Exit");
            world.menuStates["mainMenu"].SetMenuAction("Exit", () =>
                {
                    this.Exit();
                });
            world.ActivateMenuState("mainMenu");

            world.AddMenuState("pauseMenu", graphics, this);
            world.menuStates["pauseMenu"].menuFont = Content.Load<SpriteFont>("DisplayFont");
            world.menuStates["pauseMenu"].menuDirection = MenuState.Direction.TopToBottom;
            world.menuStates["pauseMenu"].unselectedColor = Color.Black;
            world.menuStates["pauseMenu"].selectedColor = Color.Yellow;
            world.menuStates["pauseMenu"].initialPosition = new Vector2(graphics.GraphicsDevice.Viewport.Width / 2, graphics.GraphicsDevice.Viewport.Height / 2);
            world.menuStates["pauseMenu"].spacing = 10;
            world.menuStates["pauseMenu"].AddMenuItem("Resume");
            world.menuStates["pauseMenu"].SetMenuAction("Resume", () =>
                {
                    world.ClearStates();
                    world.ActivateGameState("game1");
                });
            world.menuStates["pauseMenu"].AddMenuItem("To Main Menu");
            world.menuStates["pauseMenu"].SetMenuAction("To Main Menu", () =>
                {
                    world.ClearStates();
                    world.ActivateMenuState("mainMenu");
                });
            world.menuStates["pauseMenu"].AddMenuItem("Exit");
            world.menuStates["pauseMenu"].SetMenuAction("Exit", () =>
            {
                this.Exit();
            });

            player = new Player(Content.Load<Texture2D>("testguy"), graphics);
            player.pos = new Vector2(graphics.GraphicsDevice.Viewport.Width / 2, graphics.GraphicsDevice.Viewport.Height / 2);
            player.gunShotSound = new Sound(Content.Load<SoundEffect>("gunshot"));

            enemyHandler = new EnemyHandler(Content.Load<Texture2D>("testenemy"),
                Content.Load<SoundEffect>("bulletimpact"), graphics);

            gameSpeedText = new TextItem(Content.Load<SpriteFont>("DebugFont"), "Game speed: " + (float)mainGameTime.GameSpeed);
            DebugText.debugTexts.Add(gameSpeedText);
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
            KeyboardState keyboardState = Keyboard.GetState();
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
            //if (keyboardState.IsKeyDown(Keys.Escape) || gamePadState.Buttons.Back == ButtonState.Pressed)
            //{
            //    this.Exit();
            //}

            if (keyboardState.IsKeyDown(Keys.OemMinus) || gamePadState.Buttons.LeftShoulder == ButtonState.Pressed)
            {
                mainGameTime.GameSpeed -= 0.01m;
            }
            else if (keyboardState.IsKeyDown(Keys.OemPlus) || gamePadState.Buttons.RightShoulder == ButtonState.Pressed)
            {
                mainGameTime.GameSpeed += 0.01m;
            }

            if (keyboardState.IsKeyDown(Keys.OemTilde) || gamePadState.Buttons.LeftStick == ButtonState.Pressed)
            {
                mainGameTime.GameSpeed = 1.0m;
            }

            if (mainGameTime.GameSpeed < 0)
            {
                mainGameTime.GameSpeed = 0;
            }

            gameSpeedText.text = "Game speed: " + (float)mainGameTime.GameSpeed;
            world.Update(gameTime);
        }

        public void MainUpdate(GameTimeWrapper gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            MouseState mouseState = Mouse.GetState();
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);

            player.Control(gameTime, world.gameStates["game1"].camera1);

            enemyHandler.Update(player, gameTime, graphics);
            player.Update(gameTime, graphics, world.gameStates["game1"].camera1);

            world.gameStates["game1"].camera1.pan.Value = player.pos;
            world.gameStates["game1"].UpdateCurrentCamera(gameTime);
            DebugText.pos = new Vector2(world.gameStates["game1"].camera1.pan.Value.X - graphics.GraphicsDevice.Viewport.Width / 2, world.gameStates["game1"].camera1.pan.Value.Y - graphics.GraphicsDevice.Viewport.Height / 2);

            if (keyboardState.IsKeyDown(Keys.Escape) && previousKeyboardState.IsKeyDown(Keys.Escape) ||
                keyboardState.IsKeyDown(Keys.Back) && previousKeyboardState.IsKeyDown(Keys.Back))
            {
                World.thingsToDo.Add(() =>
                    {
                        world.ClearStates();
                        world.ActivateMenuState("pauseMenu");
                    });
            }
            previousKeyboardState = keyboardState;
            previousMouseState = mouseState;
            previousGamePadState = gamePadState;
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            world.DrawWorld();

            base.Draw(gameTime);
        }

        void MainMenuDraw()
        {
            world.BeginDraw();
            world.EndDraw();
        }

        void GameDraw()
        {
            world.BeginDraw();
            world.Draw(enemyHandler.Draw);
            world.Draw(player.Draw);
            world.Draw(DebugText.Draw);
            world.EndDraw();
        }
    }
}
