using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GLX;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace TimeGame
{
    public class PlayerAI : OtherPlayer
    {
        float leftBound = 100.0f;
        float rightBound = 700.0f;
        float distanceCovered = 0;
        int animationsDone = 0;
        int previousFrame = 500;
        TimeSpan waitTime;
        bool waiting = true;

        public PlayerAI(SpriteSheetInfo spriteSheetInfo, GameTimeWrapper gameTime) : base(spriteSheetInfo, gameTime)
        {
        }

        public override void Update(GameTimeWrapper gameTime, GraphicsDeviceManager graphics)
        {
            if (pos.X < leftBound)
            {
                facing = Facing.Right;
                animations.currentAnimation = "idle";
                distanceCovered = 0;
                animationsDone = 0;
                waiting = true;
                waitTime = TimeSpan.FromSeconds(1);
                vel.X = 0;
                pos.X = leftBound;
                //Debug.WriteLine("");
            }

            if (pos.X > rightBound)
            {
                facing = Facing.Left;
                animations.currentAnimation = "idle";
                distanceCovered = 0;
                animationsDone = 0;
                waiting = true;
                waitTime = TimeSpan.FromSeconds(1);
                vel.X = 0;
                pos.X = rightBound;
                //Debug.WriteLine("");
            }

            if (waiting)
            {
                waitTime -= TimeSpan.FromTicks(Math.Abs(gameTime.ElapsedGameTime.Ticks));
                if (waitTime <= TimeSpan.Zero)
                {
                    waiting = false;
                    if (facing == Facing.Right)
                    {
                        if (gameTime.GameSpeed > 0)
                        {
                            vel.X = 4.0f * (float)gameTime.GameSpeed;
                        }
                        else if (gameTime.GameSpeed < 0)
                        {
                            vel.X = -(4.0f * (float)gameTime.GameSpeed);
                        }
                    }
                    else if (facing == Facing.Left)
                    {
                        if (gameTime.GameSpeed > 0)
                        {
                            vel.X = -(4.0f * (float)gameTime.GameSpeed);
                        }
                        else if (gameTime.GameSpeed < 0)
                        {
                            vel.X = 4.0f * (float)gameTime.GameSpeed;
                        }
                    }
                    distanceCovered += Math.Abs(vel.X);
                    animations.currentAnimation = "run";
                }
            }
            else
            {
                if (facing == Facing.Right)
                {
                    if (gameTime.GameSpeed > 0)
                    {
                        vel.X = 4.0f * (float)gameTime.GameSpeed;
                    }
                    else if (gameTime.GameSpeed < 0)
                    {
                        vel.X = -(4.0f * (float)gameTime.GameSpeed);
                    }
                }
                else if (facing == Facing.Left)
                {
                    if (gameTime.GameSpeed > 0)
                    {
                        vel.X = -(4.0f * (float)gameTime.GameSpeed);
                    }
                    else if (gameTime.GameSpeed < 0)
                    {
                        vel.X = 4.0f * (float)gameTime.GameSpeed;
                    }
                }
                distanceCovered += Math.Abs(vel.X);
            }

            if (animations.currentAnimation == "run")
            {
                if (animations.currentFrame == 0 && previousFrame != 0)
                {
                    animationsDone++;
                    //Debug.WriteLine(animationsDone + " animations done in " + distanceCovered + " distance");
                }
                previousFrame = animations.currentFrame;
            }
            base.Update(gameTime, graphics);
        }
    }
}
