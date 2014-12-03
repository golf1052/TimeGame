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
        int health;
        int damage;

        Particle[] particles;

        public Enemy(Texture2D loadedTex, GraphicsDeviceManager graphics) : base(loadedTex)
        {
            visible = false;
            anger = new ColorTweener();
            anger.Value = Color.Black;
            anger.smoothingActive = true;
            anger.smoothingType = TweenerBase.SmoothingType.Linear;
            anger.smoothingRate = 0.05f;
            color = Color.DarkGreen;
            charging = false;
            chargingTimer = TimeSpan.Zero;
            health = 100;
            damage = 0;
            particles = new Particle[500];
            for (int i = 0; i < particles.Length; i++)
            {
                particles[i] = new Particle(graphics);
            }
        }

        public void Update(Player player, GameTimeWrapper gameTime, GraphicsDeviceManager graphics)
        {
            foreach (Bullet bullet in player.bullets)
            {
                if (bullet.visible)
                {
                    if (Vector2.Distance(bullet.point1, pos) < 200)
                    {
                        if (rect.ContainsLine(bullet))
                        {
                            bullet.shouldBeDead = true;
                            damage++;
                            Splatter(player);
                            Vector2 bulletForce = Vector2.Normalize(new Vector2(player.pos.X - pos.X, player.pos.Y - pos.Y));
                            bulletForce *= -1;
                            Vector2 secondaryForce = rect.PointOnLine(bullet);
                            secondaryForce = Vector2.Normalize(new Vector2(secondaryForce.X - pos.X, secondaryForce.Y - pos.Y));
                            secondaryForce *= -1;
                            bulletForce *= (bullet.speed / 100);
                            bulletForce += secondaryForce;
                            vel += bulletForce;
                            break;
                        }
                    }
                }
            }
            if (damage >= health)
            {
                visible = false;
                damage = 0;
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
                anger.Value = Color.DarkGreen;
            }

            if (charging)
            {
                anger.Value = Color.DarkGreen;
                if (vel.X < 0.001f && vel.Y < 0.001f)
                {
                    vel = Vector2.Zero;
                    charging = false;
                    chargingTimer = TimeSpan.Zero;
                }
            }
            anger.Update(gameTime);
            color = anger.Value;
            vel *= 0.80f;
            foreach (Particle particle in particles)
            {
                particle.Update(gameTime, graphics);
            }
            base.Update(gameTime, graphics);
        }

        public void Splatter(Player player)
        {
            int particlesSpawned = 0;
            foreach (Particle particle in particles)
            {
                if (!particle.visible)
                {
                    particle.SpawnParticle(pos, Color.Red, new Tuple<int, int>(250, 500), new Tuple<int, int>(4, 4), new Tuple<float, float>(4, 7),
                        new Tuple<float, float>(1, 1), new Tuple<float, float>(0, 0), new Tuple<float, float>(0, 0), player.rotation, 10, Color.Red, false, 0);
                    particlesSpawned++;
                }
                if (particlesSpawned == damage)
                {
                    break;
                }
            }
            
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            foreach (Particle particle in particles)
            {
                particle.Draw(spriteBatch);
            }
            base.Draw(spriteBatch);
        }
    }
}
