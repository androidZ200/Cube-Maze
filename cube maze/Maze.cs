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
        public Bitmap GetImage(Color BackGround, Color Line, Color SFPoibt)
        {
            Bitmap bmp = new Bitmap(Width * 40, Height * 40);
            Graphics g = Graphics.FromImage(bmp);
            g.Clear(BackGround);
            for(int j = 0; j < Height; j++)
                for(int i = 0; i < Width; i++)
                    g.DrawImage(GetBlockImage(new Point(i, j), Line), i * 40, j * 40);
            g.FillEllipse(new SolidBrush(SFPoibt), Start.X * 40 + 8, Start.Y * 40 + 8, 24, 24);
            g.FillEllipse(new SolidBrush(SFPoibt), Finish.X * 40 + 8, Finish.Y * 40 + 8, 24, 24);
            return bmp;
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
        private Bitmap GetBlockImage(Point block, Color line)
        {
            Bitmap bmp = new Bitmap(40, 40);
            Graphics g = Graphics.FromImage(bmp);
            g.FillEllipse(new SolidBrush(line), 4, 4, 32, 32);
            for(int i = 0; i < 4; i++)
                if((Maze[block.X, block.Y] & (1 << i)) != 0)
                    switch (i)
                    {
                        case 0:
                            g.FillRectangle(new SolidBrush(line), 4, 0, 32, 20);
                            break;
                        case 1:
                            g.FillRectangle(new SolidBrush(line), 20, 4, 20, 32);
                            break;
                        case 2:
                            g.FillRectangle(new SolidBrush(line), 4, 20, 32, 20);
                            break;
                        case 3:
                            g.FillRectangle(new SolidBrush(line), 0, 4, 20, 32);
                            break;
                    }
            return bmp;
        }
    }
}
