using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GLX;
using Microsoft.Xna.Framework;

namespace TimeGame
{
    public class Weapon
    {
        public enum FireType
        {
            Semi,
            Auto
        }

        public FireType fireType;
        public int bulletsInShot;
        public int bulletsInClip;
        public int shotSpread;
        public int damageInBullet;
        public int reloadTime;
        public float shotSpeed;
        public Tuple<int, int> rateOfFire;
        public TimeSpan timeBetweenShots;
        public Sound gunShotSound;

        bool canFire;

        public Weapon()
        {
            canFire = true;
        }

        public void Fire(Player player)
        {
            if (canFire)
            {
                canFire = false;
                for (int i = 0; i < bulletsInShot; i++)
                {
                    Bullet bullet = new Bullet(player.graphics);
                    bullet.speed = shotSpeed;
                    bullet.Fire(player.pos,
                        World.random.Next((int)player.rotation - shotSpread,
                        (int)player.rotation + shotSpread + 1));
                    player.bullets.Add(bullet);
                }
                if (gunShotSound != null)
                {
                    gunShotSound.Play();
                }
                else
                {
                    player.gunShotSound.Play();
                }
                canFire = false;
            }
        }

        public void Update(GameTimeWrapper gameTime)
        {
            if (!canFire)
            {
                timeBetweenShots -= gameTime.ElapsedGameTime;
                if (timeBetweenShots <= TimeSpan.Zero)
                {
                    canFire = true;
                    ResetTimeBetweenShots();
                }
            }
        }

        public void ResetTimeBetweenShots()
        {
            timeBetweenShots = TimeSpan.FromMinutes(1.0 /
                (double)World.random.Next(rateOfFire.Item1, rateOfFire.Item2));
        }
    }
}
