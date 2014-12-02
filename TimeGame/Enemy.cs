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
        public Enemy(Texture2D loadedTex) : base(loadedTex)
        {
            visible = false;
            color = Color.Black;
        }

        public void Update(Player player, GameTimeWrapper gameTime, GraphicsDeviceManager graphics)
        {
            float distance = Vector2.Distance(player.pos, pos);
            if (distance < 300)
            {
                color = Color.Red;

                if (distance < player.tex.Width / 2 + tex.Width / 2)
                {
                    float distanceToMove = Math.Abs(player.tex.Width / 2 + tex.Width / 2 - distance);
                    distanceToMove /= 2;
                    Vector2 intersectVector = new Vector2(player.pos.X - pos.X, player.pos.Y - pos.Y);
                    intersectVector.Normalize();
                    intersectVector *= distanceToMove;
                    pos -= intersectVector;
                    player.pos += intersectVector;
                }
            }
            else
            {
                color = Color.Black;
            }
            base.Update(gameTime, graphics);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}
