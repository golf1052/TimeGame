using System;
using System.Diagnostics;
using GLX;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TimeGame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        KeyboardState previousKeyboardState = Keyboard.GetState();
        MouseState previousMouseState = Mouse.GetState();
        GamePadState previousGamePadState = GamePad.GetState(PlayerIndex.One);

        TextItem gameSpeedText;

        Map map;

        Player player;
        GameTimeWrapper mainGameTime;
        World world;

        public Game1()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            this.IsMouseVisible = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            world = new World(graphics);
            mainGameTime = new GameTimeWrapper(MainUpdate, this, 1.0m);
            world.AddTime(mainGameTime);

            DebugText.Initialize(Vector2.Zero, DebugText.Corner.TopLeft, 0);
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            player = new Player(Content.Load<Texture2D>("testguy"), graphics);
            player.pos = new Vector2(graphics.GraphicsDevice.Viewport.Width / 2, graphics.GraphicsDevice.Viewport.Height / 2);

            gameSpeedText = new TextItem(Content.Load<SpriteFont>("DebugFont"), "Game speed: " + (float)mainGameTime.GameSpeed);
            DebugText.debugTexts.Add(gameSpeedText);

            map = new Map(graphics);
            map.LoadMap(Content.RootDirectory + "\\testmap.json");
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
            if (keyboardState.IsKeyDown(Keys.Escape) || gamePadState.Buttons.Back == ButtonState.Pressed)
            {
                this.Exit();
            }

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

            //// Keyboard + mouse control
            //player.Move(keyboardState, 5.0f * (float)gameTime.GameSpeed, SpriteBase.MovementDirection.Up, Keys.W);
            //player.Move(keyboardState, 5.0f * (float)gameTime.GameSpeed, SpriteBase.MovementDirection.Down, Keys.S);
            //player.Move(keyboardState, 5.0f * (float)gameTime.GameSpeed, SpriteBase.MovementDirection.Left, Keys.A);
            //player.Move(keyboardState, 5.0f * (float)gameTime.GameSpeed, SpriteBase.MovementDirection.Right, Keys.D);
            ////testGuy.Aim(mouseState);
            //if (mouseState.LeftButton == ButtonState.Pressed)
            //{
            //    player.Fire();
            //}

            //// Gamepad control
            //player.pos.X += gamePadState.ThumbSticks.Left.X * (5.0f * (float)gameTime.GameSpeed);
            //player.pos.Y -= gamePadState.ThumbSticks.Left.Y * (5.0f * (float)gameTime.GameSpeed);
            //player.Aim(gamePadState, SpriteBase.ThumbStick.Right);
            //if (gamePadState.Triggers.Right >= 0.5)
            //{
            //    player.Fire();
            //}

            player.Control(gameTime, map);

            if (gamePadState.Buttons.Y == ButtonState.Pressed)
            {
                DebugText.corner = DebugText.Corner.TopLeft;
                DebugText.pos = Vector2.Zero;
            }
            else if (gamePadState.Buttons.X == ButtonState.Pressed)
            {
                DebugText.corner = DebugText.Corner.BottomLeft;
                DebugText.pos = new Vector2(0, graphics.GraphicsDevice.Viewport.Height);
            }
            else if (gamePadState.Buttons.B == ButtonState.Pressed)
            {
                DebugText.corner = DebugText.Corner.TopRight;
                DebugText.pos = new Vector2(graphics.GraphicsDevice.Viewport.Width, 0);
            }
            else if (gamePadState.Buttons.A == ButtonState.Pressed)
            {
                DebugText.corner = DebugText.Corner.BottomRight;
                DebugText.pos = new Vector2(graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height);
            }

            //player.CheckCollision(map);
            player.Update(gameTime, graphics);

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

            spriteBatch.Begin();
            map.Draw(spriteBatch);
            player.Draw(spriteBatch);

            DebugText.Draw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
