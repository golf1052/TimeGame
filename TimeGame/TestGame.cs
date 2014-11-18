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
    public class TestGame : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        World world;
        KeyboardState previousKeyboardState = Keyboard.GetState();
        GamePadState previousGamePadState = GamePad.GetState(PlayerIndex.One);
        OtherPlayer ida;
        PlayerAI ai;
        Sprite pixel;
        Sprite square;
        Line line;
        Polygon poly;
        GameTimeWrapper mainGameTime;

        Emitter emitter;

        TextItem gameSpeedText;

        enum CameraTarget
        {
            Player,
            AI,
            None
        }
        CameraTarget cameraTarget = CameraTarget.None;

        public TestGame()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
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
            mainGameTime = new GameTimeWrapper(SecondUpdate, this, 1.0m);
            world.AddTime(mainGameTime);
            world.camera1.smoothPan = true;
            world.camera1.panSmoothingType = Camera.SmoothingType.RecursiveLinear;
            world.camera1.panSmoothingRate = 0.05f;

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
            world.LoadSpriteBatch();

            ida = new OtherPlayer(new SpriteSheetInfo(160, 120), mainGameTime);
            ida.animations["idle"] = ida.animations.AddSpriteSheet(Content.Load<Texture2D>("stand"), 8, 100, true);
            ida.animations["run"] = ida.animations.AddSpriteSheet(Content.Load<Texture2D>("run"), 10, 100, true);
            ida.animations["jump"] = ida.animations.AddSpriteSheet(Content.Load<Texture2D>("jump"), 20, 100, false);
            ida.animations.SetFrameAction("jump", 6, ida.StartJump);
            ida.animations.SetFrameAction("jump", 19, ida.SetGround);
            ida.animations["slash1"] = ida.animations.AddSpriteSheet(Content.Load<Texture2D>("slash1"), 7, 50, false);
            ida.animations.SetFrameAction("slash1", 6, ida.SetGround);
            ida.animations.SetReverseFrameAction("slash1", 0, ida.SetGround);
            ida.animations.SetFrameAction("slash1", 6, ida.SetSlashMeter);
            ida.animations["slash2"] = ida.animations.AddSpriteSheet(Content.Load<Texture2D>("slash2"), 8, 50, false);
            ida.animations.SetFrameAction("slash2", 7, ida.SetGround);
            ida.animations.SetReverseFrameAction("slash2", 0, ida.SetGround);
            ida.animations.SetFrameAction("slash2", 7, ida.SetSlashMeter);
            ida.animations["slash3"] = ida.animations.AddSpriteSheet(Content.Load<Texture2D>("slash3"), 15, 35, false);
            ida.animations.SetFrameAction("slash3", 14, ida.SetGround);
            ida.animations.SetReverseFrameAction("slash3", 0, ida.SetGround);
            ida.animations.SetFrameAction("slash3", 14, ida.SetSlashMeter);
            ida.Ready(graphics.GraphicsDevice);
            ida.animations.currentAnimation = "idle";
            ida.pos = new Vector2(300, 350);

            ai = new PlayerAI(new SpriteSheetInfo(160, 120), mainGameTime);
            ai.animations["idle"] = ida.animations.AddSpriteSheet(Content.Load<Texture2D>("stand"), 8, 100, true);
            ai.animations["run"] = ida.animations.AddSpriteSheet(Content.Load<Texture2D>("run"), 10, 100, true);
            ai.Ready(graphics.GraphicsDevice);
            ai.animations.currentAnimation = "idle";
            ai.pos = new Vector2(100, 350);

            square = new Sprite(Content.Load<Texture2D>("square"));
            square.pos = new Vector2(400, 100);

            pixel = new Sprite(Content.Load<Texture2D>("pixel"));
            pixel.color = Color.LawnGreen;
            pixel.drawRect = new Rectangle(0, 350 + (ida.tex.Height / 2) - 9, graphics.PreferredBackBufferWidth, 0);
            pixel.drawRect.Height = graphics.PreferredBackBufferHeight - pixel.drawRect.Y;

            line = new Line(graphics, Line.Type.Point, new Vector2(100, 100), new Vector2(500, 300), 1);

            poly = new Polygon(graphics);
            poly.AddSide(new Vector2(150, 150), new Vector2(150, 350));
            poly.AddSide(new Vector2(350, 350));
            poly.AddSide(new Vector2(350, 150));
            poly.AddSide();

            gameSpeedText = new TextItem(Content.Load<SpriteFont>("DisplayFont"), "Current Game Speed: " + mainGameTime.GameSpeed.ToString());
            gameSpeedText.pos = new Vector2(graphics.GraphicsDevice.Viewport.Width / 2, 50);

            emitter = new Emitter(graphics);
            emitter.pos = new Vector2(0, graphics.GraphicsDevice.Viewport.Height / 2);
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
            if (keyboardState.IsKeyDown(Keys.OemMinus))
            {
                mainGameTime.GameSpeed -= 0.01m;
                Debug.WriteLine(mainGameTime.GameSpeed);
            }
            else if (keyboardState.IsKeyDown(Keys.OemPlus))
            {
                mainGameTime.GameSpeed += 0.01m;
                Debug.WriteLine(mainGameTime.GameSpeed);
            }
            else if (keyboardState.IsKeyDown(Keys.OemTilde) && previousKeyboardState.IsKeyUp(Keys.OemTilde))
            {
                mainGameTime.GameSpeed *= -1.0m;
            }
            else if (keyboardState.IsKeyDown(Keys.D1))
            {
                mainGameTime.GameSpeed = 1.0m;
            }
            else if (keyboardState.IsKeyDown(Keys.D2))
            {
                mainGameTime.GameSpeed = 2.0m;
            }
            else if (keyboardState.IsKeyDown(Keys.D3))
            {
                mainGameTime.GameSpeed = 3.0m;
            }
            gameSpeedText.text = "Current Game Speed: " + mainGameTime.GameSpeed.ToString("#.0000");
            gameSpeedText.pos = new Vector2(graphics.GraphicsDevice.Viewport.Width / 2, 50);
            if (gamePadState.Buttons.LeftStick == ButtonState.Pressed &&
                previousGamePadState.Buttons.LeftStick == ButtonState.Released)
            {
                world.camera1.focus = Camera.Focus.TopLeft;
                world.camera1.focalPoint = Vector2.Zero;
                cameraTarget = CameraTarget.None;
            }
            else if (gamePadState.Buttons.LeftShoulder == ButtonState.Pressed &&
                previousGamePadState.Buttons.LeftShoulder == ButtonState.Released)
            {
                world.camera1.focus = Camera.Focus.Center;
                world.camera1.focalPoint = ida.pos;
                cameraTarget = CameraTarget.Player;
            }
            else if (gamePadState.Buttons.RightShoulder == ButtonState.Pressed &&
                previousGamePadState.Buttons.RightShoulder == ButtonState.Released)
            {
                world.camera1.focus = Camera.Focus.Center;
                world.camera1.focalPoint = ai.pos;
                cameraTarget = CameraTarget.AI;
            }
            else if (gamePadState.Buttons.X == ButtonState.Pressed &&
                previousGamePadState.Buttons.X == ButtonState.Released)
            {
                world.camera1.focus = Camera.Focus.Center;
                world.camera1.focalPoint = Vector2.Zero;
                cameraTarget = CameraTarget.None;
            }
            else if (gamePadState.Buttons.Y == ButtonState.Pressed &&
                previousGamePadState.Buttons.Y == ButtonState.Released)
            {
                world.camera1.focus = Camera.Focus.Center;
                world.camera1.focalPoint = new Vector2(graphics.GraphicsDevice.Viewport.Width, 0);
                cameraTarget = CameraTarget.None;
            }
            else if (gamePadState.Buttons.B == ButtonState.Pressed &&
                previousGamePadState.Buttons.B == ButtonState.Released)
            {
                world.camera1.focus = Camera.Focus.Center;
                world.camera1.focalPoint = new Vector2(graphics.GraphicsDevice.Viewport.Width,
                    graphics.GraphicsDevice.Viewport.Height);
                cameraTarget = CameraTarget.None;
            }
            else if (gamePadState.Buttons.A == ButtonState.Pressed &&
                previousGamePadState.Buttons.A == ButtonState.Released)
            {
                world.camera1.focus = Camera.Focus.Center;
                world.camera1.focalPoint = new Vector2(0, graphics.GraphicsDevice.Viewport.Height);
                cameraTarget = CameraTarget.None;
            }

            if (cameraTarget == CameraTarget.AI)
            {
                world.camera1.focalPoint = ai.pos;
            }
            else if (cameraTarget == CameraTarget.Player)
            {
                world.camera1.focalPoint = ida.pos;
            }

            world.Update(gameTime);
            previousGamePadState = gamePadState;
        }

        void SecondUpdate(GameTimeWrapper gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (keyboardState.IsKeyDown(Keys.Space) && previousKeyboardState.IsKeyUp(Keys.Space))
            {
                if (ida.state == OtherPlayer.State.Ground)
                {
                    ida.state = OtherPlayer.State.Attacking;
                    if (ida.slashMeter == 0)
                    {
                        ida.animations.currentAnimation = "slash1";
                    }
                    else if (ida.slashMeter == 1)
                    {
                        ida.animations.currentAnimation = "slash2";
                    }
                    else if (ida.slashMeter == 2)
                    {
                        ida.animations.currentAnimation = "slash3";
                    }
                }
            }

            if (keyboardState.IsKeyDown(Keys.Up) && previousKeyboardState.IsKeyUp(Keys.Up))
            {
                if (ida.animations.currentAnimation != "jump" && ida.state == OtherPlayer.State.Ground)
                {
                    ida.animations.currentAnimation = "jump";
                }
            }

            float idaMoveSpeed = 4.0f * (float)gameTime.GameSpeed;
            if (keyboardState.IsKeyDown(Keys.Left))
            {
                poly.pos = new Vector2(poly.pos.X - 1, poly.pos.Y);
                if (ida.state == OtherPlayer.State.Ground)
                {
                    if (ida.animations.currentAnimation != "jump")
                    {
                        if (ida.animations.currentAnimation != "run")
                        {
                            ida.animations.currentAnimation = "run";
                        }
                    }
                    ida.facing = OtherPlayer.Facing.Left;
                    ida.pos.X -= idaMoveSpeed;
                }
            }
            else if (keyboardState.IsKeyDown(Keys.Right))
            {
                poly.pos = new Vector2(poly.pos.X + 1, poly.pos.Y);
                if (ida.state == OtherPlayer.State.Ground)
                {
                    if (ida.animations.currentAnimation != "jump")
                    {
                        if (ida.animations.currentAnimation != "run")
                        {
                            ida.animations.currentAnimation = "run";
                        }
                    }
                    ida.facing = OtherPlayer.Facing.Right;
                    ida.pos.X += idaMoveSpeed;
                }
            }

            if (keyboardState.IsKeyUp(Keys.Left) && previousKeyboardState.IsKeyDown(Keys.Left))
            {
                if (ida.state == OtherPlayer.State.Ground)
                {
                    ida.animations.currentAnimation = "idle";
                }
            }
            else if (previousKeyboardState.IsKeyDown(Keys.Right) && keyboardState.IsKeyUp(Keys.Right))
            {
                if (ida.state == OtherPlayer.State.Ground)
                {
                    ida.animations.currentAnimation = "idle";
                }
            }

            emitter.Fire();
            emitter.Update(gameTime);
            ida.Update(gameTime, graphics);
            ai.Update(gameTime, graphics);
            line.Aim(gamePadState, SpriteBase.ThumbStick.Right);
            //gameSpeedText.Update(gameTime, graphics.GraphicsDevice);

            previousKeyboardState = keyboardState;
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            world.BeginDraw();
            world.Draw(pixel.DrawRect);
            world.Draw(ai.Draw);
            world.Draw(ida.Draw);
            world.Draw(gameSpeedText.Draw);
            world.Draw(emitter.Draw);
            world.EndDraw();

            //spriteBatch.Begin();
            //world.Draw(pixel.DrawRect);
            //pixel.DrawRect(spriteBatch);
            //ai.Draw(spriteBatch);
            //ida.Draw(spriteBatch);
            //gameSpeedText.Draw(spriteBatch);
            ////line.Draw(spriteBatch);
            ////square.Draw(spriteBatch);
            ////poly.Draw(spriteBatch);
            //emitter.Draw(spriteBatch);
            //spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
