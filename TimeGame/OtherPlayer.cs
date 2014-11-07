using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using GLX;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TimeGame
{
    public class OtherPlayer : Sprite
    {
        public enum Facing
        {
            Right,
            Left
        }

        public enum State
        {
            Ground,
            Air,
            Attacking
        }

        public bool alive;
        public Facing facing;
        public State state;
        float startingHeight;
        float maxHeight = 50.0f;
        bool hitApex;
        public int slashMeter;
        public TimeSpan slashTime;

        public OtherPlayer(Texture2D loadedTex) : base(loadedTex)
        {
            PlayerBase();
        }

        public OtherPlayer(SpriteSheetInfo spriteSheetInfo, GameTimeWrapper gameTime) : base(spriteSheetInfo, gameTime)
        {
            PlayerBase();
        }

        void PlayerBase()
        {
            this.facing = Facing.Right;
            this.state = State.Ground;
            this.alive = true;
            this.slashMeter = 0;
            SetSlashTime();
        }

        public void StartJump()
        {
            state = State.Air;
            hitApex = false;
            startingHeight = pos.Y;
        }

        public void SetGround()
        {
            state = State.Ground;
            animations.currentAnimation = "idle";
        }

        public void SetSlashTime()
        {
            slashTime = TimeSpan.FromMilliseconds(1000);
        }

        public void SetSlashMeter()
        {
            slashMeter++;
            if (slashMeter >= 3)
            {
                slashMeter = 0;
            }
            SetSlashTime();
        }

        public override void Update(GameTimeWrapper gameTime, GraphicsDevice graphicsDevice)
        {
            if (slashMeter != 0)
            {
                slashTime -= gameTime.ElapsedGameTime;
                //Debug.WriteLine("Time left: " + slashTime.TotalMilliseconds);
                if (slashTime <= TimeSpan.Zero)
                {
                    //Debug.WriteLine("Time's up");
                    slashMeter = 0;
                    SetSlashTime();
                }
            }

            if (state == State.Air)
            {
                if (pos.Y <= startingHeight - maxHeight)
                {
                    hitApex = true;
                    Debug.WriteLine("Jump Height: " + (startingHeight - pos.Y));
                }

                if (!hitApex)
                {
                    pos.Y -= 2.0f * (float)gameTime.GameSpeed;
                }
                else
                {
                    pos.Y += 2.0f * (float)gameTime.GameSpeed;
                }

                if (pos.Y >= startingHeight)
                {
                    state = State.Ground;
                    //animations.currentAnimation = "idle";
                }
            }

            base.Update(gameTime, graphicsDevice);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (facing == Facing.Right)
            {
                spriteBatch.Draw(tex, pos, null, color, MathHelper.ToRadians(rotation), origin, scale, SpriteEffects.None, 0);
            }
            else
            {
                spriteBatch.Draw(tex, pos, null, color, MathHelper.ToRadians(rotation), origin, scale, SpriteEffects.FlipHorizontally, 0);
            }
        }
    }
}
