using Microsoft.Xna.Framework;
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
        List<Vector2> walls = new List<Vector2>();
        List<KeyValuePair<Vector2, Texture2D>> drawable_elements = new List<KeyValuePair<Vector2, Texture2D>>();
        Random rng;
        bool SpaceIsPressed = false;
        bool AIsPressed = false;

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

            InitMaze();
        }

        void InitMaze()
        {
            drawable_elements.Clear();
            doors.Clear();
            GenerateBorders();
            GenerateFirstPath();
        }
        
        void GenerateDoors(int length)
        {
            int dir;
            int revDir;
            Vector2 tmp;
            int occurrence = length;

            foreach (Vector2 pos in walls)
            {
                dir = FindFloor(pos);
                if (dir != -1)
                {
                    revDir = (dir + 2) % 4;
                    tmp = dirmap[revDir](pos, 1);
                    if (map[GetCoord(tmp)] == Bloc.empty && !IsNextToDoor(pos))
                    {
                        if (rng.Next(occurrence) == 0)
                        {
                            drawable_elements.Add(new KeyValuePair<Vector2, Texture2D>(pos, Gfx.door));
                            map[GetCoord(pos)] = Bloc.door;
                            doors.Add(pos);
                            occurrence = length;
                        }
                        if (occurrence > 1)
                            --occurrence;
                    }
                }
            }
            walls.Clear();
        }

        bool IsNextToDoor(Vector2 pos)
        {
            for (int i = 0; i < 4; ++i)
                if (map[GetCoord(dirmap[i](pos, 1))] == Bloc.door)
                    return true;
            return (false);
        }

        int FindFloor(Vector2 pos)
        {
            for (int i = 0; i < 4; ++i)
                if (map[GetCoord(dirmap[i](pos, 1))] == Bloc.floor)
                    return i;
            return (-1);
        }

        void GenerateBorders()
        {
            for (int j = 0; j < 2; ++j)
            {
                for (UInt32 i = 0; i < size_x; ++i)
                {
                    drawable_elements.Add(new KeyValuePair<Vector2, Texture2D>(new Vector2(i, (size_y - 1) * j), Gfx.wall));
                    map[j * (size_y - 1) * i] = Bloc.wall;
                }
                for (UInt32 i = 1; i < size_y - 1; ++i)
                {
                    drawable_elements.Add(new KeyValuePair<Vector2, Texture2D>(new Vector2((size_x - 1) * j, i), Gfx.wall));
                    map[i * size_x + (size_x - 1) * j] = Bloc.wall;
                }
            }
        }

        void GeneratePath(Vector2 pos, int length)
        {
            int dir = rng.Next(4);
            int revDir = -1;
            int way = 0;

            for (int i = 0; i < length; ++i)
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
            GenerateDoors((length / 3) * 2);
        }

        void GenerateFirstPath()
        {
            Vector2 pos = new Vector2(size_x / 2, size_y / 2);
            int dir = rng.Next(4);
            int revDir = -1;

            for (UInt32 i = 0; i < size; ++i)
                map[i] = Bloc.empty;
            addWalls(dir, revDir, pos);
            GeneratePath(pos, 15);
        }

        void addWalls(int dir, int revDir, Vector2 pos)
        {
            int prevDir = 3;
            Vector2 tmp;

            for (int i = 0; i < 4; ++i)
            {
                if (i != dir && i != revDir)
                {
                    //walls
                    if (map[GetCoord(dirmap[i](pos, 1))] == Bloc.empty)
                    {
                        drawable_elements.Add(new KeyValuePair<Vector2, Texture2D>(dirmap[i](pos, 1), Gfx.wall));
                        map[GetCoord(dirmap[i](pos, 1))] = Bloc.wall;
                        walls.Add(dirmap[i](pos, 1));
                    }
                    //corners
                    if (prevDir != dir && prevDir != revDir)
                    {
                        tmp = dirmap[prevDir](dirmap[i](pos, 1), 1);
                        if (map[GetCoord(tmp)] == Bloc.empty)
                        {
                            drawable_elements.Add(new KeyValuePair<Vector2, Texture2D>(tmp, Gfx.wall));
                            map[GetCoord(tmp)] = Bloc.wall;
                        }
                    }
                }
                prevDir = i;
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
                AIsPressed = true;
            if (Keyboard.GetState().IsKeyUp(Keys.A))
                if (AIsPressed)
                {
                    InitMaze();
                    AIsPressed = false;
                }
            if (Keyboard.GetState().IsKeyDown(Keys.Space))
                SpaceIsPressed = true;
            if (Keyboard.GetState().IsKeyUp(Keys.Space))
                if (SpaceIsPressed)
                {
                    int nb = rng.Next(doors.Count);
                    GeneratePath(doors[nb], rng.Next(8, 15));
                    doors.RemoveAt(nb);
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
