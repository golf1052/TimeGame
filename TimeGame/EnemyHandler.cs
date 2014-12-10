using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GLX;
using Microsoft.Xna.Framework.Audio;

namespace TimeGame
{
    public class EnemyHandler
    {
        public List<Enemy> enemies;

        public EnemyHandler(Texture2D enemyTex, SoundEffect bulletImpactSound, GraphicsDeviceManager graphics)
        {
            enemies = new List<Enemy>();
            for (int i = 0; i < 25; i++)
            {
                enemies.Add(new Enemy(enemyTex, graphics));
                enemies[i].bulletImpactSound = new Sound(bulletImpactSound);
            }
        }

        public void Update(Player player, GameTimeWrapper gameTime, GraphicsDeviceManager graphics)
        {
            SpawnEnemy(player);
            foreach (Enemy enemy in enemies)
            {
                enemy.Update(player, gameTime, graphics);
            }
        }

        public void SpawnEnemy(Player player)
        {
            foreach (Enemy enemy in enemies)
            {
                if (!enemy.visible)
                {
                    enemy.visible = true;
                    enemy.pos = new Vector2(player.pos.X + World.random.Next(-1000, 1000),
                        player.pos.Y + World.random.Next(-1000, 1000));
                    break;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Enemy enemy in enemies)
            {
                enemy.Draw(spriteBatch);
            }
        }
    }
}
