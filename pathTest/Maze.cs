﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pathTest
{
    enum Bloc
    {
        empty,
        floor,
        wall,
        door
    }

    public class Maze
    {
        UInt32 size_x;
        UInt32 size_y;
        UInt32 size;
        Bloc[] map;
        Func<Vector2, UInt32, Vector2>[] dirmap = new Func<Vector2, UInt32, Vector2>[4];
        List<Vector2> doors = new List<Vector2>();
        List<KeyValuePair<Vector2, Texture2D>> drawable_elements = new List<KeyValuePair<Vector2, Texture2D>>();
        Random rng;
        bool SpaceIsPressed = false;

        public Maze(UInt32 size_x, UInt32 size_y)
        {
            this.size_x = size_x;
            this.size_y = size_y;
            this.size = size_x * size_y;
            map = new Bloc[size];
            rng = new Random();

            dirmap[0] = GoUp;
            dirmap[1] = GoRight;
            dirmap[2] = GoDown;
            dirmap[3] = GoLeft;
            GenerateFirstPath();
        }

        void GenerateFirstPath()
        {
            Vector2 pos = new Vector2(size_x / 2, size_y / 2);
            int dir = rng.Next(4);
            int revDir = (dir + 2) % 4;
            int way = 0;

            for (UInt32 i = 0; i < size; ++i)
                map[i] = Bloc.empty;
            if (map[GetCoord(dirmap[revDir](pos, 1))] == Bloc.empty)
            {
                drawable_elements.Add(new KeyValuePair<Vector2, Texture2D>(dirmap[revDir](pos, 1), Gfx.wall));
                map[GetCoord(dirmap[revDir](pos, 1))] = Bloc.wall;
            }
            for (int i = 0; i < 10; ++i)
            {
                drawable_elements.Add(new KeyValuePair<Vector2, Texture2D>(pos, Gfx.floor));
                map[GetCoord(pos)] = Bloc.floor;
                revDir = (dir + 2) % 4;
                way = rng.Next(3) - 1;
                dir = ChangeDirection(dir, way);
                if (!DirectionIsValid(dir, pos))
                    if ((dir = CheckDirection(dir, revDir, pos)) == -1)
                        if ((dir = SeekWay(pos)) == -1)
                            return;
                addWalls(dir, revDir, pos);
                pos = dirmap[dir](pos, 1);
            }
            drawable_elements.Add(new KeyValuePair<Vector2, Texture2D>(pos, Gfx.door));
            pos = dirmap[(dir + 2) % 4](pos, 1);
            addWalls(revDir, revDir, pos);
        }

        void addWalls(int dir, int revDir, Vector2 pos)
        {
            for (int i = 0; i < 4; ++i)
                if (i != dir)
                {
                    if (map[GetCoord(dirmap[i](pos, 1))] == Bloc.empty)
                    {
                        drawable_elements.Add(new KeyValuePair<Vector2, Texture2D>(dirmap[i](pos, 1), Gfx.wall));
                        map[GetCoord(dirmap[i](pos, 1))] = Bloc.wall;
                    }
                }
        }

        int SeekWay(Vector2 pos)
        {
            for (int i = 0; i < 4; ++i)
            {
                if (DirectionIsValid(i, pos))
                    return (i);
            }
            return (-1);
        }

        int CheckDirection(int dir, int revDir, Vector2 pos)
        {
            int way = rng.Next(2);

            if (way == 0)
                way = 3;
            for (int i = 0; i < 5; i += way)
            {
                dir = (dir + way + i) % 4;
                if (dir == revDir)
                    continue;
                if (DirectionIsValid(dir, pos))
                    return (dir);
            }
            return (-1);
        }

        bool DirectionIsValid(int dir, Vector2 pos)
        {
            Vector2 tmp = dirmap[dir](pos, 1);

            if (map[GetCoord(tmp)] == Bloc.empty &&
                map[GetCoord(dirmap[dir](tmp, 1))] != Bloc.floor &&
                map[GetCoord(dirmap[ChangeDirection(dir, -1)](tmp, 1))] != Bloc.floor &&
                map[GetCoord(dirmap[ChangeDirection(dir, 1)](tmp, 1))] != Bloc.floor)
                return (true);
            else
                return (false);
        }

        bool IsDeadEnd(Vector2 pos)
        {
            for (int i = 0; i < 4; ++i)
            {
                if (map[GetCoord(dirmap[i](pos, 1))] == Bloc.empty)
                    return (false);
            }
            return (true);
        }

        UInt32 GetCoord(Vector2 pos)
        {
            return (size_x * Convert.ToUInt32(pos.Y) + Convert.ToUInt32(pos.X));
        }

        int ChangeDirection(int dir, int way)
        {
            dir += way;
            if (dir < 0)
                dir = dir - way + 3;
            dir = dir % 4;
            return (dir);
        }

        public void Update()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.A))
                SpaceIsPressed = true;
            if (Keyboard.GetState().IsKeyUp(Keys.A))
                if (SpaceIsPressed)
                {
                    drawable_elements.Clear();
                    GenerateFirstPath();
                    SpaceIsPressed = false;
                }
        }

        public void Draw(View view)
        {
            foreach (KeyValuePair<Vector2, Texture2D> key in drawable_elements)
            {
                view.Draw(Convert.ToInt32(key.Key.X * 32), Convert.ToInt32(key.Key.Y * 32), key.Value);
            }
        }

        Vector2 GoUp(Vector2 pos, UInt32 distance)
        {
            return (new Vector2(pos.X, pos.Y - distance));
        }

        Vector2 GoDown(Vector2 pos, UInt32 distance)
        {
            return (new Vector2(pos.X, pos.Y + distance));
        }

        Vector2 GoLeft(Vector2 pos, UInt32 distance)
        {
            return (new Vector2(pos.X - distance, pos.Y));
        }

        Vector2 GoRight(Vector2 pos, UInt32 distance)
        {
            return (new Vector2(pos.X + distance, pos.Y));
        }
    }
}