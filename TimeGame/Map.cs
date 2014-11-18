using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GLX;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;

namespace TimeGame
{
    public class Map
    {
        GraphicsDeviceManager graphics;
        Texture2D pixel;
        public List<Block> blocks;

        public Map(GraphicsDeviceManager graphics)
        {
            this.graphics = graphics;
            pixel = new Texture2D(graphics.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            pixel.SetData(new[] { Color.Black });
            blocks = new List<Block>();
        }

        public void LoadMap(string filePath)
        {
            StreamReader streamReader = new StreamReader(File.Open(filePath, FileMode.Open));
            JArray blocksData = JArray.Parse(streamReader.ReadToEnd());
            foreach (JObject blockData in blocksData)
            {
                Block tmp = new Block(pixel);
                tmp.pos = new Vector2((float)blockData["x"], (float)blockData["y"]);
                tmp.drawRect = new Rectangle((int)tmp.pos.X, (int)tmp.pos.Y, (int)blockData["width"], (int)blockData["height"]);
                blocks.Add(tmp);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Block block in blocks)
            {
                block.DrawRect(spriteBatch);
            }
        }
    }
}
