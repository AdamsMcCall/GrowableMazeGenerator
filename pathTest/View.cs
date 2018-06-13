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
        SpriteBatch sb;

        public View(int start_x, int start_y, int start_speed, int width, int height, SpriteBatch sb)
        {
            x = start_x;
            y = start_y;
            speed = start_speed;
            this.width = width;
            this.height = height;
            this.sb = sb;
        }

        public void Draw(int x, int y, Texture2D image)
        {
            if (((x - this.x) * zoom + width / 2) <= width &&
                ((y - this.y) * zoom + height / 2) <= height &&
                ((x - this.x) * zoom + width / 2) + (image.Width * zoom + 1) >= 0 &&
                ((y - this.y) * zoom + height / 2) + (image.Height * zoom + 1) >= 0)
                sb.Draw(image, new Rectangle(
                    Convert.ToInt32((x - this.x) * zoom + width / 2),
                    Convert.ToInt32((y - this.y) * zoom + height / 2),
                    Convert.ToInt32(image.Width * zoom + 1),
                    Convert.ToInt32(image.Height * zoom + 1)), Color.White);
        }
    }
}
