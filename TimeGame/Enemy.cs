using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GLX;

namespace TimeGame
{
    public class Enemy : Sprite
    {
        ColorTweener anger;
        bool charging;
        TimeSpan chargingTimer;

        public Enemy(Texture2D loadedTex) : base(loadedTex)
        {
            visible = false;
            anger = new ColorTweener();
            anger.Value = Color.Black;
            anger.smoothingActive = true;
            anger.smoothingType = TweenerBase.SmoothingType.Linear;
            anger.smoothingRate = 0.05f;
            color = Color.Black;
            charging = false;
            chargingTimer = TimeSpan.Zero;
        }

        public void Update(Player player, GameTimeWrapper gameTime, GraphicsDeviceManager graphics)
        {
            foreach (Bullet bullet in player.bullets)
            {
                if (Vector2.Distance(bullet.point1, pos) < 200)
                {
                    if (rect.ContainsLine(bullet))
                    {
                        bullet.visible = false;
                        visible = false;
                        break;
                    }
                }
            }
            float distance = Vector2.Distance(player.pos, pos);
            if (distance < 300)
            {
                if (!charging)
                {
                    anger.Value = Color.Red;
                    chargingTimer += gameTime.ElapsedGameTime;
                    if (chargingTimer > TimeSpan.FromMilliseconds(2000))
                    {
                        charging = true;
                        vel = new Vector2(player.pos.X - pos.X, player.pos.Y - pos.Y);
                        vel.Normalize();
                        vel *= 50;
                    }
                }

                if (distance < player.tex.Width / 2 + tex.Width / 2)
                {
                    float distanceToMove = Math.Abs(player.tex.Width / 2 + tex.Width / 2 - distance);
                    distanceToMove /= 2;
                    Vector2 intersectVector = new Vector2(player.pos.X - pos.X, player.pos.Y - pos.Y);
                    intersectVector.Normalize();
                    intersectVector *= distanceToMove;
                    pos -= intersectVector;
                    player.pos += intersectVector + vel / 2;
                }
            }
            else
            {
                if (chargingTimer > TimeSpan.Zero)
                {
                    chargingTimer -= gameTime.ElapsedGameTime;
                }
                anger.Value = Color.Black;
            }

            if (charging)
            {
                anger.Value = Color.Black;
                if (vel.X < 0.001f && vel.Y < 0.001f)
                {
                    vel = Vector2.Zero;
                    charging = false;
                    chargingTimer = TimeSpan.Zero;
                }
            }
            anger.Update(gameTime);
            color = anger.Value;
            vel *= 0.90f;
            base.Update(gameTime, graphics);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}
