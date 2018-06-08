using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pathTest
{
    public class View
    {
        public int x;
        public int y;
        public double zoom = 1;
        public int speed = 2;
        int width;
        int height;

        public View(int start_x, int start_y, int start_speed, int width, int height)
        {
            x = start_x;
            y = start_y;
            speed = start_speed;
            this.width = width;
            this.height = height;
        }

        public void Draw(SpriteBatch spriteBatch, int x, int y, Texture2D image)
        {
            Console.WriteLine((x - this.x).ToString() + " " + (y - this.y).ToString());
            spriteBatch.Draw(image, new Rectangle(
                Convert.ToInt32((x - this.x) * zoom + width / 2),
                Convert.ToInt32((y - this.y) * zoom + height / 2),
                Convert.ToInt32(image.Width * zoom),
                Convert.ToInt32(image.Height * zoom)), Color.White);
        }
    }
}
