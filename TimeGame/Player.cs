using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GLX;

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

        public Player(Texture2D loadedTex, GraphicsDeviceManager graphics) : base(loadedTex)
        {
            this.graphics = graphics;

            alive = true;
            bullets = new List<Bullet>();
            timeBetweenShots = TimeSpan.FromMilliseconds(shotDelay);
            canFire = true;
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

        public override void Update(GameTimeWrapper gameTime, GraphicsDevice graphicsDevice)
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
                    if (!graphicsDevice.Viewport.Bounds.Contains(bullet.point1) &&
                        !graphicsDevice.Viewport.Bounds.Contains(bullet.point2))
                    {
                        bullet.visible = false;
                        break;
                    }

                    bullet.Update(gameTime);
                }
            }
            base.Update(gameTime, graphicsDevice);
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
            base.Draw(spriteBatch);
        }
    }
}
