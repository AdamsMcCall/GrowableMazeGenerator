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
    public enum Bloc
    {
        empty,
        floor,
        wall,
        door
    }

    class Door
    {
        public Vector2 pos;
        public int nb;

        public Door(Vector2 pos, int nb)
        {
            this.pos = pos;
            this.nb = nb;
        }
    }

    public class Maze
    {
        public UInt32 size_x;
        public UInt32 size_y;
        UInt32 size;
        public Bloc[] map;
        Func<Vector2, UInt32, Vector2>[] dirmap = new Func<Vector2, UInt32, Vector2>[4];
        List<Door> doors = new List<Door>();
        List<Vector2> walls = new List<Vector2>();
        Random rng;
        bool SpaceIsPressed = false;
        bool AIsPressed = false;
        int iteration = 0;
        Vector2 posMin;
        Vector2 posMax;

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
            //clear all containers and reset every relevant attributes
            doors.Clear();
            iteration = 0;
            posMin = new Vector2(size_x / 2, size_y / 2);
            //then regenerate stuff
            GenerateBorders();
            GenerateFirstPath();
        }
        
        void UpdateDoors()
        {
            int nb_floor;
            bool isBlocked = true;
            List<Door> Obsolete = new List<Door>();

            foreach (Door door in doors)
            {
                nb_floor = 0;
                isBlocked = true;
                for (int i = 0; i < 4; ++i)
                {
                    if (map[GetCoord(dirmap[i](door.pos, 1))] == Bloc.floor)
                        ++nb_floor;
                    if (map[GetCoord(dirmap[i](door.pos, 1))] == Bloc.empty)
                        isBlocked = false;
                }
                if (nb_floor > 1)
                {
                    map[GetCoord(door.pos)] = Bloc.floor;
                    Obsolete.Add(door);
                }
                else if (isBlocked)
                {
                    map[GetCoord(door.pos)] = Bloc.wall;
                    Obsolete.Add(door);
                }
            }
            foreach (Door door in Obsolete)
                doors.Remove(door);
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
                            map[GetCoord(pos)] = Bloc.door;
                            doors.Add(new Door(pos, iteration));
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
                    map[j * (size_y - 1) * i] = Bloc.wall;
                }
                for (UInt32 i = 1; i < size_y - 1; ++i)
                {
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
            pos = dirmap[(dir + 2) % 4](pos, 1);
            addWalls(revDir, revDir, pos);
            GenerateDoors((length / 3) * 2);
            ++iteration;
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
                        map[GetCoord(dirmap[i](pos, 1))] = Bloc.wall;
                        walls.Add(dirmap[i](pos, 1));
                    }
                    //corners
                    if (prevDir != dir && prevDir != revDir)
                    {
                        tmp = dirmap[prevDir](dirmap[i](pos, 1), 1);
                        if (map[GetCoord(tmp)] == Bloc.empty)
                        {
                            map[GetCoord(tmp)] = Bloc.wall;
                            if (tmp.X < posMin.X)
                                posMin.X = tmp.X;
                            UpdateDrawingDimensions(tmp);
                        }
                    }
                }
                prevDir = i;
            }
        }

        void UpdateDrawingDimensions(Vector2 pos)
        {
            if (pos.X < posMin.X)
                posMin.X = pos.X;
            if (pos.Y < posMin.Y)
                posMin.Y = pos.Y;
            if (pos.X > posMax.X)
                posMax.X = pos.X;
            if (pos.Y > posMax.Y)
                posMax.Y = pos.Y;
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

        Door GetLowestDoor()
        {
            List<Door> list = new List<Door>();

            for (int i = 0; i < 1000; ++i)
            {
                list = doors.FindAll(s => s.nb == i);
                if (list.Count() != 0)
                    break;
            }
            if (list.Count() != 0)
                return (list[rng.Next(list.Count())]);
            else
                return (null);
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
                    Door door = GetLowestDoor();
                    if (door != null)
                    {
                        int nb = doors.FindIndex(s => s == door);
                        GeneratePath(door.pos, rng.Next(8, 15));
                        doors.RemoveAt(nb);
                        UpdateDoors();
                    }
                    SpaceIsPressed = false;
                }
        }

        public void Draw(View view)
        {
            /*foreach (KeyValuePair<Vector2, Texture2D> key in drawable_elements)
            {
                view.Draw(Convert.ToInt32(key.Key.X * 32), Convert.ToInt32(key.Key.Y * 32), key.Value);
            }*/
            Bloc content;

            for (int j = Convert.ToInt32(posMin.Y); j <= posMax.Y; ++j)
            {
                for (int i = Convert.ToInt32(posMin.X); i <= posMax.X; ++i)
                {
                    content = map[j * size_y + i];
                    switch (content)
                    {
                        case Bloc.door:
                            view.Draw(i * 32, j * 32, Gfx.door);
                            break;
                        case Bloc.floor:
                            view.Draw(i * 32, j * 32, Gfx.floor);
                            break;
                        case Bloc.wall:
                            view.Draw(i * 32, j * 32, Gfx.wall);
                            break;
                    }
                }
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
