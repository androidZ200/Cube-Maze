using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cube_maze
{
    class Maze2
    {
        public byte[,,] field { get; private set; }
        public Point3 Finish { get; private set; }
        public Point3 Start { get; private set; }
        public int Height { get; private set; }
        public int Width { get; private set; }
        private Random rnd = new Random();

        public Maze2()
        {
            Height = rnd.Next(15, 21);
            Width = rnd.Next(9, 14);
            if (Width % 2 == 0) Width++;

            Generate();
        }
        public Maze2(int width, int height)
        {
            Width = width;
            Height = height;
            Generate();

        }
        public byte GetCell(int x, int y, int z)
        {
            return field[x, y, z];
        }
        public Bitmap GetImage(Color BackGround, Color Line, Color SFPoibt)
        {
            Bitmap bmp = new Bitmap(Width * 160, Height * 160);
            Graphics g = Graphics.FromImage(bmp);
            g.Clear(BackGround);
            for (int j = 0; j < Height; j++)
                for (int i = 0; i < Width; i++)
                    g.DrawImage(GetBlockImage(field[i, j, 0], field[i, j, 1], Line), i * 160, j * 160);
            g.FillEllipse(new SolidBrush(SFPoibt), Start.X * 160 + 60, Start.Y * 160 + 60, 40, 40);
            g.FillEllipse(new SolidBrush(SFPoibt), Finish.X * 160 + 60, Finish.Y * 160 + 60, 40, 40);
            return bmp;
        }

        private void Generate()
        {
            field = new byte[Width, Height, 2];
            int[,,] group = new int[Width, Height, 2];
            for (int z = 0; z < 2; z++)
                for (int j = 0; j < Height; j++)
                    for (int i = 0; i < Width; i++)
                    {
                        group[i, j, z] = z * Height * Width + j * Width + i;
                        field[i, j, z] = 0;
                    }
            while (!isOneGroup(group))
            {
                int X = rnd.Next(Width);
                int Y = rnd.Next(Height);
                int Z = rnd.Next(2);
                byte neighbors = checkNeighbors(group, X, Y, Z);
                neighbors &= withWhomYouCanConnect(field[X, Y, Z], field[X, Y, Z ^ 1]);
                while (neighbors != 0)
                {
                    byte rotate = (byte)rnd.Next(8);
                    if ((neighbors & (1 << rotate)) != 0)
                    {
                        int otherGroup = 0;
                        field[X, Y, Z] += (byte)(1 << rotate);
                        int z = Z ^ (rotate >= 4 ? 1 : 0);
                        switch (rotate % 4)
                        {
                            case 0:
                                field[X, Y - 1, z] += (byte)(1 << 2 + 4 * (rotate >= 4 ? 1 : 0));
                                otherGroup = group[X, Y - 1, z];
                                break;
                            case 1:
                                field[X + 1, Y, z] += (byte)(1 << 3 + 4 * (rotate >= 4 ? 1 : 0));
                                otherGroup = group[X + 1, Y, z];
                                break;
                            case 2:
                                field[X, Y + 1, z] += (byte)(1 << 0 + 4 * (rotate >= 4 ? 1 : 0));
                                otherGroup = group[X, Y + 1, z];
                                break;
                            case 3:
                                field[X - 1, Y, z] += (byte)(1 << 1 + 4 * (rotate >= 4 ? 1 : 0));
                                otherGroup = group[X - 1, Y, z];
                                break;
                        }
                        FillGriop(group, otherGroup, group[X, Y, Z]);
                        break;
                    }
                }
            }
            Finish = new Point3(Width / 2, 0, 0);
            Start = new Point3(Width / 2, Height - 1, 0);
        }
        private bool isOneGroup(int[,,] groups)
        {
            int s = groups[0, 0, 0];
            for (int z = 0; z < 2; z++)
                for (int j = 0; j < Height; j++)
                    for (int i = 0; i < Width; i++)
                        if (s != groups[i, j, z]) return false;
            return true;
        }
        private byte checkNeighbors(int[,,] groups, int x, int y, int z)
        {
            byte nei = 0;
            try { if (groups[x, y, z] != groups[x, y - 1, z]) nei |= (1 << 0); }
            catch { }
            try { if (groups[x, y, z] != groups[x + 1, y, z]) nei |= (1 << 1); }
            catch { }
            try { if (groups[x, y, z] != groups[x, y + 1, z]) nei |= (1 << 2); }
            catch { }
            try { if (groups[x, y, z] != groups[x - 1, y, z]) nei |= (1 << 3); }
            catch { }

            try { if (groups[x, y, z] != groups[x, y - 1, z ^ 1]) nei |= (1 << 4); }
            catch { }
            try { if (groups[x, y, z] != groups[x + 1, y, z ^ 1]) nei |= (1 << 5); }
            catch { }
            try { if (groups[x, y, z] != groups[x, y + 1, z ^ 1]) nei |= (1 << 6); }
            catch { }
            try { if (groups[x, y, z] != groups[x - 1, y, z ^ 1]) nei |= (1 << 7); }
            catch { }
            return nei;
        }
        private byte withWhomYouCanConnect(byte I, byte otherI)
        {
            I |= (byte)(I << 4);
            I |= (byte)(I >> 4);
            otherI = (byte)((otherI >> 4) + (otherI << 4));
            I |= otherI;
            return (byte)~I;
        }
        private void FillGriop(int[,,] groups, int A, int B)
        {
            for (int z = 0; z < groups.GetLength(2); z++)
                for (int j = 0; j < groups.GetLength(1); j++)
                    for (int i = 0; i < groups.GetLength(0); i++)
                        if (groups[i, j, z] == A) groups[i, j, z] = B;
        }

        private Bitmap GetBlockImage(byte blockDown, byte blockUp, Color line)
        {
            Bitmap bmp = new Bitmap(160, 160);
            Graphics g = Graphics.FromImage(bmp);
            g.FillEllipse(new SolidBrush(line), 16, 16, 128, 128);
            for (int i = 0; i < 8; i++)
                if ((blockUp & (1 << i)) != 0)
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
                        case 4:
                            g.DrawImage(GetTriangleDown(line), 0, -80);
                            break;
                        case 5:
                            g.DrawImage(GetTriangleLeft(line), 80, 0);
                            break;
                        case 6:
                            g.DrawImage(GetTriangleUp(line), 0, 80);
                            break;
                        case 7:
                            g.DrawImage(GetTriangleRight(line), -80, 0);
                            break;
                    }
            for (int i = 0; i < 4; i++)
                if ((blockDown & (1 << (i + 4))) != 0)
                    switch (i)
                    {
                        case 0:
                            g.DrawImage(GetTriangleUp(line), 0, -80);
                            break;
                        case 1:
                            g.DrawImage(GetTriangleRight(line), 80, 0);
                            break;
                        case 2:
                            g.DrawImage(GetTriangleDown(line), 0, 80);
                            break;
                        case 3:
                            g.DrawImage(GetTriangleLeft(line), -80, 0);
                            break;
                    }
            g.FillEllipse(new SolidBrush(Color.Black), 60, 60, 40, 40);
            for (int i = 0; i < 4; i++)
                if ((blockDown & (1 << i)) != 0)
                    switch (i)
                    {
                        case 0:
                            g.FillRectangle(new SolidBrush(Color.Black), 60, 0, 40, 80);
                            break;
                        case 1:
                            g.FillRectangle(new SolidBrush(Color.Black), 80, 60, 80, 40);
                            break;
                        case 2:
                            g.FillRectangle(new SolidBrush(Color.Black), 60, 80, 40, 80);
                            break;
                        case 3:
                            g.FillRectangle(new SolidBrush(Color.Black), 0, 60, 80, 40);
                            break;
                    }
            return bmp;
        }
        private Bitmap GetTriangleRight(Color line)
        {
            Bitmap bmp = new Bitmap(160, 160);
            Graphics g = Graphics.FromImage(bmp);
            LinearGradientBrush linGrBrush = new LinearGradientBrush(new Point(0, 80), new Point(135, 80), Color.Black, line);
            Point[] points = { new Point(0, 60), new Point(0, 100), new Point(135, 139), new Point(135, 21) };
            g.FillPolygon(linGrBrush, points);
            return bmp;
        }
        private Bitmap GetTriangleLeft(Color line)
        {
            Bitmap bmp = new Bitmap(160, 160);
            Graphics g = Graphics.FromImage(bmp);
            LinearGradientBrush linGrBrush = new LinearGradientBrush(new Point(160, 80), new Point(24, 80), Color.Black, line);
            Point[] points = { new Point(160, 60), new Point(160, 100), new Point(25, 139), new Point(25, 21) };
            g.FillPolygon(linGrBrush, points);
            return bmp;
        }
        private Bitmap GetTriangleUp(Color line)
        {
            Bitmap bmp = new Bitmap(160, 160);
            Graphics g = Graphics.FromImage(bmp);
            LinearGradientBrush linGrBrush = new LinearGradientBrush(new Point(80, 160), new Point(80, 24), Color.Black, line);
            Point[] points = { new Point(21, 25), new Point(139, 25), new Point(100, 160), new Point(60, 160) };
            g.FillPolygon(linGrBrush, points);
            return bmp;
        }
        private Bitmap GetTriangleDown(Color line)
        {
            Bitmap bmp = new Bitmap(160, 160);
            Graphics g = Graphics.FromImage(bmp);
            LinearGradientBrush linGrBrush = new LinearGradientBrush(new Point(80, 0), new Point(80, 135), Color.Black, line);
            Point[] points = { new Point(21, 135), new Point(139, 135), new Point(100, 0), new Point(60, 0) };
            g.FillPolygon(linGrBrush, points);
            return bmp;
        }
    }
}