using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GLX;

namespace TimeGame
{
    public class Emitter
    {
        public Line line;
        bool ready;
        public Vector2 pos;
        float lineLength;

        public Emitter(GraphicsDeviceManager graphics)
        {
            line = new Line(graphics);
            ready = true;
            lineLength = 1;
        }

        public void Fire()
        {
            if (ready)
            {
                ready = false;
                line.point1 = pos;
                line.point2 = new Vector2(pos.X - lineLength, pos.Y);
            }
        }

        public void Update(GameTimeWrapper gameTime)
        {
            if (!ready)
            {
                decimal distance = 100m * gameTime.ActualGameSpeed;
                line.point2.X = line.point1.X;
                line.point1.X += (float)distance;
                if (line.point2.X > 800)
                {
                    ready = true;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            line.Draw(spriteBatch);
        }
    }
}
