using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;


namespace cube_maze
{
    class Labyrinth
    {
        public byte[,] Maze { get; private set; }
        public Point Finish { get; private set; }
        public Point Start { get; private set; }
        public int Height { get; private set; }
        public int Width { get; private set; }

        public Labyrinth()
        {
            GenerateMaze();
        }
        public List<Point> GetNeighbors(Point Position)
        {
            List<Point> neighbors = new List<Point>();
            int t = Maze[Position.X, Position.Y];
            if ((t & 8) != 0) neighbors.Add(new Point(Position.X - 1, Position.Y));
            if ((t & 4) != 0) neighbors.Add(new Point(Position.X, Position.Y + 1));
            if ((t & 2) != 0) neighbors.Add(new Point(Position.X + 1, Position.Y));
            if ((t & 1) != 0) neighbors.Add(new Point(Position.X, Position.Y - 1));
            return neighbors;
        }

        private void GenerateMaze()
        {
            Random rnd = new Random();
            Width = rnd.Next(9, 13);
            if (Width % 2 == 0) Width++;
            Height = rnd.Next(15, 19);
            byte[,] field = new byte[Width, Height];
            int[,] group = new int[Width, Height];
            for (int i = 0, counter = 0; i < Width; i++)
                for (int j = 0; j < Height; j++)
                {
                    field[i, j] = 0;
                    group[i, j] = counter;
                    counter++;
                }
            while (true)
            {
                int X = rnd.Next(Width);
                int Y = rnd.Next(Height);
                int[] neighbors = new int[4];
                for (int i = 0; i < 4; i++)
                    if (i == 0)
                        try { neighbors[i] = group[X, Y - 1]; }
                        catch { neighbors[i] = -1; }
                    else if (i == 1)
                        try { neighbors[i] = group[X + 1, Y]; }
                        catch { neighbors[i] = -1; }
                    else if (i == 2)
                        try { neighbors[i] = group[X, Y + 1]; }
                        catch { neighbors[i] = -1; }
                    else if (i == 3)
                        try { neighbors[i] = group[X - 1, Y]; }
                        catch { neighbors[i] = -1; }
                bool link = false;
                for (int i = 0; i < 4; i++)
                    if (neighbors[i] != group[X, Y] && neighbors[i] != -1)
                        link = true;
                while (link)
                {
                    int rotate = rnd.Next(4);
                    if (neighbors[rotate] != group[X, Y] && neighbors[rotate] != -1)
                    {
                        int ReplacingAGroup = neighbors[rotate];
                        switch (rotate)
                        {
                            case 0:
                                field[X, Y] += 1;
                                field[X, Y - 1] += 4;
                                break;
                            case 1:
                                field[X, Y] += 2;
                                field[X + 1, Y] += 8;
                                break;
                            case 2:
                                field[X, Y] += 4;
                                field[X, Y + 1] += 1;
                                break;
                            case 3:
                                field[X, Y] += 8;
                                field[X - 1, Y] += 2;
                                break;
                        }
                        for (int i = 0; i < Width; i++)
                            for (int j = 0; j < Height; j++)
                                if (group[i, j] == ReplacingAGroup)
                                    group[i, j] = group[X, Y];
                        link = false;
                    }
                }
                link = true;
                for (int i = 0; i < Width; i++)
                    for (int j = 0; j < Height; j++)
                        if (group[i, j] != group[X, Y])
                            link = false;
                if (link)
                    break;
            }
            Finish = new Point((Width - 1) / 2, 0);
            Start = new Point((Width - 1) / 2, Height - 1);
            Maze = field;
        }
    }
}
