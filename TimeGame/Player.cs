using System;
using System.Collections.Generic;
using GLX;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TimeGame
{
    public class Player : Sprite
    {
        public bool alive;
        List<Bullet> bullets;
        TimeSpan timeBetweenShots;
        bool canFire;

        GraphicsDeviceManager graphics;

        int shotDelay = 100;

        List<Line> debugLines;

        KeyboardState previousKeyboardState;
        GamePadState previousGamePadState;
        MouseState previousMouseState;

        public enum ControlScheme
        {
            KeyboardMouse,
            GamePad
        }

        public ControlScheme controlScheme;

        public Player(Texture2D loadedTex, GraphicsDeviceManager graphics) : base(loadedTex)
        {
            this.graphics = graphics;
            debugLines = new List<Line>();

            alive = true;
            bullets = new List<Bullet>();
            timeBetweenShots = TimeSpan.FromMilliseconds(shotDelay);
            canFire = true;

            controlScheme = ControlScheme.GamePad;

            previousKeyboardState = Keyboard.GetState();
            previousGamePadState = GamePad.GetState(PlayerIndex.One);
            previousMouseState = Mouse.GetState();
        }

        public void Fire()
        {
            if (canFire)
            {
                Bullet bullet = new Bullet(graphics);
                bullet.speed = 100;
                bullet.Fire(pos, rotation);
                bullets.Add(bullet);
                canFire = false;
            }
        }

        public void Control(GameTimeWrapper gameTime, Map map)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            MouseState mouseState = Mouse.GetState();
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);

            Vector2 currentPos = pos;
            Vector2 futurePos = pos;

            if (controlScheme == ControlScheme.KeyboardMouse)
            {
                if (keyboardState.IsKeyDown(Keys.W))
                {
                    futurePos.Y -= 5.0f * (float)gameTime.GameSpeed;
                }
                if (keyboardState.IsKeyDown(Keys.S))
                {
                    futurePos.Y += 5.0f * (float)gameTime.GameSpeed;
                }
                if (keyboardState.IsKeyDown(Keys.A))
                {
                    futurePos.X -= 5.0f * (float)gameTime.GameSpeed;
                }
                if (keyboardState.IsKeyDown(Keys.D))
                {
                    futurePos.X += 5.0f * (float)gameTime.GameSpeed;
                }
            }
            else if (controlScheme == ControlScheme.GamePad)
            {
                futurePos.X += gamePadState.ThumbSticks.Left.X * (5.0f * (float)gameTime.GameSpeed);
                futurePos.Y -= gamePadState.ThumbSticks.Left.Y * (5.0f * (float)gameTime.GameSpeed);
            }

            bool intersection = false;
            debugLines.Clear();
            foreach (Block block in map.blocks)
            {
                List<Tuple<Vector2, Vector2>> wallPoints = new List<Tuple<Vector2,Vector2>>();
                wallPoints.Add(new Tuple<Vector2,Vector2>(Vector2.Zero, Vector2.Zero));
                Vector2 point1 = Vector2.Zero;
                Vector2 point2 = Vector2.Zero;
                for (int i = 0; i < 4; i++)
                {
                    float closestWall = float.MaxValue;
                    if (i == 0)
                    {
                        // top edge
                        if (Math.Abs(currentPos.Y - block.drawRect.Top) < closestWall)
                        {
                            closestWall = Math.Abs(currentPos.Y - block.drawRect.Top);
                            point1 = new Vector2(block.drawRect.Left, block.drawRect.Top);
                            point2 = new Vector2(block.drawRect.Right, block.drawRect.Top);
                        }
                        //if (Math.Abs(currentPos.Y - block.drawRect.Top) < closestWall)
                        //{
                        //    closestWall = Math.Abs(currentPos.Y - block.drawRect.Top);
                        //    wallPoints[0] = new Tuple<Vector2,Vector2>(new Vector2(block.drawRect.Left, block.drawRect.Top),
                        //        new Vector2(block.drawRect.Right, block.drawRect.Top));
                        //}
                        //else if (Math.Abs(currentPos.Y - block.drawRect.Top) == closestWall)
                        //{
                        //    wallPoints.Add(new Tuple<Vector2,Vector2>(new Vector2(block.drawRect.Left, block.drawRect.Top),
                        //        new Vector2(block.drawRect.Right, block.drawRect.Top)));
                        //}
                    }
                    else if (i == 1)
                    {
                        // left edge
                        if (Math.Abs(currentPos.X - block.drawRect.Left) < closestWall)
                        {
                            closestWall = Math.Abs(currentPos.X - block.drawRect.Left);
                            point1 = new Vector2(block.drawRect.Left, block.drawRect.Top);
                            point2 = new Vector2(block.drawRect.Left, block.drawRect.Bottom);
                        }
                    }
                    else if (i == 2)
                    {
                        // bottom edge
                        if (Math.Abs(currentPos.Y - block.drawRect.Bottom) < closestWall)
                        {
                            closestWall = Math.Abs(currentPos.Y - block.drawRect.Bottom);
                            point1 = new Vector2(block.drawRect.Left, block.drawRect.Bottom);
                            point2 = new Vector2(block.drawRect.Right, block.drawRect.Bottom);
                        }
                    }
                    else if (i == 3)
                    {
                        // right edge
                        if (Math.Abs(currentPos.X - block.drawRect.Right) < closestWall)
                        {
                            closestWall = Math.Abs(currentPos.X - block.drawRect.Right);
                            point1 = new Vector2(block.drawRect.Right, block.drawRect.Top);
                            point2 = new Vector2(block.drawRect.Right, block.drawRect.Bottom);
                        }
                    }
                }
                Vector2 point3 = currentPos;
                Vector2 point4 = futurePos;
                Vector2 intersectionPoint = new Vector2();
                if (block.drawRect.Contains(futurePos))
                {
                    intersectionPoint = HelperMethods.Intersection(point1, point2, point3, point4);
                    pos = intersectionPoint;
                    intersection = true;
                }
                else
                {
                    Vector2 movementDirection = Vector2.Normalize(point4 - point3);
                }
            }
            if (!intersection)
            {
                pos = futurePos;
            }
        }

        public override void Update(GameTimeWrapper gameTime, GraphicsDeviceManager graphics)
        {
            if (!canFire)
            {
                timeBetweenShots -= gameTime.ElapsedGameTime;
                if (timeBetweenShots <= TimeSpan.Zero)
                {
                    canFire = true;
                    timeBetweenShots = TimeSpan.FromMilliseconds(shotDelay);
                }
            }
            foreach (Bullet bullet in bullets)
            {
                if (bullet.visible)
                {
                    if (!graphics.GraphicsDevice.Viewport.Bounds.Contains(bullet.point1) &&
                        !graphics.GraphicsDevice.Viewport.Bounds.Contains(bullet.point2))
                    {
                        bullet.visible = false;
                        break;
                    }

                    bullet.Update(gameTime);
                }
            }
            base.Update(gameTime, graphics);
        }

        public void CheckCollision(Map map)
        {
            debugLines.Clear();
            foreach (Block block in map.blocks)
            {
                
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            foreach (Bullet bullet in bullets)
            {
                if (bullet.visible)
                {
                    bullet.Draw(spriteBatch);
                }
            }
            foreach (Line line in debugLines)
            {
                line.Draw(spriteBatch);
            }
            base.Draw(spriteBatch);
        }
    }
}
