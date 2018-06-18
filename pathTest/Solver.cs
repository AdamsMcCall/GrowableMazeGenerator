using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace pathTest
{
    class Path
    {
        public Vector2 pos;
        public Path prev;

        public Path(Vector2 pos, Path prev)
        {
            this.pos = pos;
            this.prev = prev;
        }
    }

    public class Solver
    {
        Maze maze;
        public Vector2 start { get; set; }
        public Vector2 finish { get; set; }
        List<Vector2> solution = new List<Vector2>();
        List<Path> diffusion = new List<Path>();
        List<Vector2> used_path = new List<Vector2>();

        public Solver(Maze maze)
        {
            this.maze = maze;
        }

        public bool Solve()
        {
            List<Path> tmp;
            Vector2 pos;
            Path way = null;

            solution.Clear();
            diffusion.Clear();
            used_path.Clear();
            solution.Add(start);
            diffusion.Add(new Path(start, null));
            while (diffusion.Exists(s => s.pos.X == finish.X && s.pos.Y == finish.Y) == false && diffusion.Count > 0)
            {
                tmp = diffusion.ToList();
                diffusion.Clear();
                foreach (Path path in tmp)
                    for (int i = 0; i < 4; ++i)
                    {
                        pos = Misc.dirmap[i](path.pos, 1);
                        if (pos.X == finish.X && pos.Y == finish.Y)
                        {
                            way = new Path(pos, path);
                            break;
                        }
                        if (maze.map[Misc.VecToCoord(pos, maze.size_x, maze.size_y)] == Bloc.floor &&
                            used_path.Exists(s => s.X == pos.X && s.Y == pos.Y) == false)
                        {
                            diffusion.Add(new Path(pos, path));
                            used_path.Add(pos);
                        }
                    }
            }
            /*if (diffusion.Exists(s => s.pos.X == finish.X && s.pos.Y == finish.Y) == false || diffusion.Count == 0)
            {
                if (diffusion.Count == 0)
                    MessageBox.Show("diffusion.Count == 0");
                else
                    MessageBox.Show("else");
                return (false);
            }*/
            if (way == null)
                return (false);
            //way = diffusion.Find(s => s.pos.X == finish.X && s.pos.Y == finish.Y);
            for (Path p = way; way.prev != null; way = way.prev)
                solution.Add(way.pos);
            solution.Add(finish);
            
            return (true);
        }

        public void Draw(View view)
        {
            foreach (Vector2 pos in solution)
                view.Draw(pos.X * 32, pos.Y * 32, Gfx.path);
        }
    }
}
