using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;


namespace cube_maze
{
    class Maze
    {
        public byte[,] maze { get; private set; }
        public Point Finish { get; private set; }
        public Point Start { get; private set; }
        public int Height { get; private set; }
        public int Width { get; private set; }
        private Bitmap[] blocks = new Bitmap[16];
        private Random rnd = new Random();

        public Maze()
        {
            Height = rnd.Next(15, 21);
            Width = rnd.Next(9, 14);
            if (Width % 2 == 0) Width++;

            GenerateMaze();
        }
        public Maze(int width, int height)
        {
            Width = width;
            Height = height;
            GenerateMaze();
        }
        public List<Point> GetNeighbors(Point Position)
        {
            List<Point> neighbors = new List<Point>();
            int t = maze[Position.X, Position.Y];
            if ((t & 8) != 0) neighbors.Add(new Point(Position.X - 1, Position.Y));
            if ((t & 4) != 0) neighbors.Add(new Point(Position.X, Position.Y + 1));
            if ((t & 2) != 0) neighbors.Add(new Point(Position.X + 1, Position.Y));
            if ((t & 1) != 0) neighbors.Add(new Point(Position.X, Position.Y - 1));
            return neighbors;
        }
        public Bitmap GetImage(Color Line, Color SFPoibt)
        {
            Bitmap bmp = new Bitmap(Width * 160, Height * 160);
            Graphics g = Graphics.FromImage(bmp);
            for (int j = 0; j < Height; j++)
                for (int i = 0; i < Width; i++)
                    g.DrawImage(GetBlockImage(new Point(i, j), Line), i * 160, j * 160);
            g.FillEllipse(new SolidBrush(SFPoibt), Start.X * 160 + 32, Start.Y * 160 + 32, 96, 96);
            g.FillEllipse(new SolidBrush(SFPoibt), Finish.X * 160 + 32, Finish.Y * 160 + 32, 96, 96);
            return bmp;
        }
        public byte GetCell(int x, int y)
        {
            return maze[x, y];
        }

        private void GenerateMaze()
        {
            byte[,] field = new byte[Width, Height];
            int[,] group = new int[Width, Height];
            for (int i = 0; i < Width; i++)
                for (int j = 0; j < Height; j++)
                {
                    field[i, j] = 0;
                    group[i, j] = i * Height + j;
                }
            do
            {
                byte X = (byte)rnd.Next(Width);
                byte Y = (byte)rnd.Next(Height);
                byte neighbors = checkNeighbors(group, X, Y);
                while (neighbors != 0)
                {
                    int rotate = rnd.Next(4);
                    if ((neighbors & (1 << rotate)) != 0)
                    {
                        int ReplacingAGroup = rotate <= 1 ?
                            (rotate == 0 ? group[X, Y - 1] : group[X + 1, Y]) :
                            (rotate == 2 ? group[X, Y + 1] : group[X - 1, Y]);
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
                        fillGroup(group, ReplacingAGroup, group[X, Y]);
                        break;
                    }
                }
            } while (!isOneGroup(group));
            Finish = new Point(Width / 2, 0);
            Start = new Point(Width / 2, Height - 1);
            maze = field;
        }
        private bool isOneGroup(int[,] groups)
        {
            int s = groups[0, 0];
            for (int j = 0; j < groups.GetLength(1); j++)
                for (int i = 0; i < groups.GetLength(0); i++)
                    if (s != groups[i, j]) return false;
            return true;
        }
        private byte checkNeighbors(int[,] groups, byte Ix, byte Iy)
        {
            byte nei = 0;
            try { if (groups[Ix, Iy] != groups[Ix, Iy - 1]) nei += 1; }
            catch { }
            try { if (groups[Ix, Iy] != groups[Ix + 1, Iy]) nei += 2; }
            catch { }
            try { if (groups[Ix, Iy] != groups[Ix, Iy + 1]) nei += 4; }
            catch { }
            try { if (groups[Ix, Iy] != groups[Ix - 1, Iy]) nei += 8; }
            catch { }
            return nei;
        }
        private void fillGroup(int[,] groups, int A, int B)
        {
            for (int j = 0; j < groups.GetLength(1); j++)
                for (int i = 0; i < groups.GetLength(0); i++)
                    if (groups[i, j] == A) groups[i, j] = B;
        }
        private Bitmap GetBlockImage(Point block, Color line)
        {
            if (blocks[maze[block.X, block.Y]] != null) return blocks[maze[block.X, block.Y]];
            Bitmap bmp = new Bitmap(160, 160);
            Graphics g = Graphics.FromImage(bmp);
            g.FillEllipse(new SolidBrush(line), 16, 16, 128, 128);
            for (int i = 0; i < 4; i++)
                if ((maze[block.X, block.Y] & (1 << i)) != 0)
                    switch (i)
                    {
                        case 0:
                            g.FillRectangle(new SolidBrush(line), 16, 0, 128, 80);
                            break;
                        case 1:
                            g.FillRectangle(new SolidBrush(line), 80, 16, 80, 128);
                            break;
                        case 2:
                            g.FillRectangle(new SolidBrush(line), 16, 80, 128, 80);
                            break;
                        case 3:
                            g.FillRectangle(new SolidBrush(line), 0, 16, 80, 128);
                            break;
                    }
            blocks[maze[block.X, block.Y]] = bmp;
            return bmp;
        }
    }
}