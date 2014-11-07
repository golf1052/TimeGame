using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GLX;

namespace TimeGame
{
    public class Bullet : Line
    {
        public float speed;
        Vector2 originalVel;

        public Bullet(GraphicsDeviceManager graphics) : base(graphics)
        {

        }

        public void Fire(Vector2 startingPosition, Vector2 targetPosition)
        {
            point2 = startingPosition;
            float xDistance = targetPosition.X - startingPosition.X;
            float yDistance = targetPosition.Y - startingPosition.Y;
            point1 = Vector2.Normalize(new Vector2(xDistance, yDistance));
        }

        public void Fire(Vector2 startingPosition, float angle)
        {
            point2 = startingPosition;
            point1 = startingPosition;
            angle = MathHelper.ToRadians(angle);
            vel = Vector2.Normalize(new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)));
            originalVel = vel;
            point1 += vel; 
        }

        public void Update(GameTimeWrapper gameTime)
        {
            if (visible)
            {
                decimal distanceTraveled = (decimal)speed * gameTime.ActualGameSpeed;
                vel = originalVel;
                vel *= (float)distanceTraveled;
                point2 = point1;
                point1 += vel;
            }
        }
    }
}
