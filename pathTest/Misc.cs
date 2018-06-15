using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pathTest
{
    public static class Misc
    {
        public static Func<Vector2, UInt32, Vector2>[] dirmap = new Func<Vector2, UInt32, Vector2>[4]
            { GoUp, GoRight, GoDown, GoLeft };

        public static UInt32 VecToCoord(Vector2 pos, UInt32 size_x, UInt32 size_y)
        {
            UInt32 x, y;

            if (pos.X < 0)
                x = 0;
            else
                x = Convert.ToUInt32(pos.X);
            if (pos.Y < 0)
                y = 0;
            else
                y = Convert.ToUInt32(pos.Y);
            return (size_x * Convert.ToUInt32(y) + Convert.ToUInt32(x));
        }

        static Vector2 GoUp(Vector2 pos, UInt32 distance)
        {
            return (new Vector2(pos.X, pos.Y - distance));
        }

        static Vector2 GoDown(Vector2 pos, UInt32 distance)
        {
            return (new Vector2(pos.X, pos.Y + distance));
        }

        static Vector2 GoLeft(Vector2 pos, UInt32 distance)
        {
            return (new Vector2(pos.X - distance, pos.Y));
        }

        static Vector2 GoRight(Vector2 pos, UInt32 distance)
        {
            return (new Vector2(pos.X + distance, pos.Y));
        }
    }
}
