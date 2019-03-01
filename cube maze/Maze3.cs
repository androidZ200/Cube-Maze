using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cube_maze
{
    class Maze3
    {
        public byte[,,] field { get; private set; }
        public Point3 Finish { get; private set; }
        public Point3 Start { get; private set; }
        public int Height { get; private set; }
        public int Width { get; private set; }
        private Random rnd = new Random();

        public Maze3()
        {
            Height = rnd.Next(15, 21);
            Width = rnd.Next(9, 14);
            if (Width % 2 == 0) Width++;

            Generate();
        }
        public Maze3(int width, int height)
        {
            Width = width;
            Height = height;
            Generate();

        }
        public byte GetCell(int x, int y, int z)
        {
            return field[x, y, z];
        }
        public byte GetCell(Point3 p)
        {
            return GetCell(p.X, p.Y, p.Z);
        }
        public Bitmap GetImage(Color BackGround, Color Line, Color SFPoibt)
        {
            Bitmap bmp = new Bitmap(Width * 160, Height * 160);
            Graphics g = Graphics.FromImage(bmp);
            g.Clear(BackGround);
            for (int j = 0; j < Height; j++)
                for (int i = 0; i < Width; i++)
                    g.DrawImage(GetBlockImage(field[i, j, 0], field[i, j, 1], Line), i * 160, j * 160);
            if ((field[Start.X, Start.Y, Start.Z] & (1 << 4)) == 0)
                g.FillEllipse(new SolidBrush(SFPoibt), Start.X * 160 + 60, Start.Y * 160 + 60, 40, 40);
            else
            {
                g.FillEllipse(new SolidBrush(SFPoibt), Start.X * 160 + 50, Start.Y * 160 + 50, 60, 60);
                g.FillEllipse(new SolidBrush(Line), Start.X * 160 + 70, Start.Y * 160 + 70, 20, 20);
            }
            if ((field[Finish.X, Finish.Y, Finish.Z] & (1 << 4)) == 0)
                g.FillEllipse(new SolidBrush(SFPoibt), Finish.X * 160 + 60, Finish.Y * 160 + 60, 40, 40);
            else
            {
                g.FillEllipse(new SolidBrush(SFPoibt), Finish.X * 160 + 50, Finish.Y * 160 + 50, 60, 60);
                g.FillEllipse(new SolidBrush(Line), Finish.X * 160 + 70, Finish.Y * 160 + 70, 20, 20);
            }
            return bmp;
        }

        private Bitmap GetBlockImage(byte blockDown, byte blockUp, Color line)
        {
            Bitmap bmp = new Bitmap(160, 160);
            Graphics g = Graphics.FromImage(bmp);
            g.FillEllipse(new SolidBrush(line), 16, 16, 128, 128);
            for (int i = 0; i < 4; i++)
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
            if((blockUp & (1 << 4)) != 0)
            {
                g.FillEllipse(new SolidBrush(Color.Black), 50, 50, 60, 60);
                g.FillEllipse(new SolidBrush(line), 70, 70, 20, 20);
            }
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
                while (neighbors != 0)
                {
                    byte rotate = (byte)rnd.Next(5);
                    if ((neighbors & (1 << rotate)) != 0)
                    {
                        int otherGroup = 0;
                        switch (rotate)
                        {
                            case 0:
                                otherGroup = group[X, Y - 1, Z];
                                field[X, Y - 1, Z] += (1 << 2);
                                break;
                            case 1:
                                otherGroup = group[X + 1, Y, Z];
                                field[X + 1, Y, Z] += (1 << 3);
                                break;
                            case 2:
                                otherGroup = group[X, Y + 1, Z];
                                field[X, Y + 1, Z] += (1 << 0);
                                break;
                            case 3:
                                otherGroup = group[X - 1, Y, Z];
                                field[X - 1, Y, Z] += (1 << 1);
                                break;
                            case 4:
                                otherGroup = group[X, Y, Z ^ 1];
                                field[X, Y, Z ^ 1] += (1 << 4);
                                break;
                        }
                        field[X, Y, Z] += (byte)(1 << rotate);
                        FillGroup(group, otherGroup, group[X, Y, Z]);
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
        private void FillGroup(int[,,] groups, int A, int B)
        {
            for (int z = 0; z < groups.GetLength(2); z++)
                for (int j = 0; j < groups.GetLength(1); j++)
                    for (int i = 0; i < groups.GetLength(0); i++)
                        if (groups[i, j, z] == A) groups[i, j, z] = B;
        }
        private byte checkNeighbors(int[,,] groups, int x, int y, int z)
        {
            int myGroup = groups[x, y, z];
            byte neighbors = 0;
            for (int i = 0; i < 5; i++)
                switch (i)
                {
                    case 0:
                        try { if (myGroup != groups[x, y - 1, z]) neighbors += 1 << 0; }
                        catch { }
                        break;
                    case 1:
                        try { if (myGroup != groups[x + 1, y, z]) neighbors += 1 << 1; }
                        catch { }
                        break;
                    case 2:
                        try { if (myGroup != groups[x, y + 1, z]) neighbors += 1 << 2; }
                        catch { }
                        break;
                    case 3:
                        try { if (myGroup != groups[x - 1, y, z]) neighbors += 1 << 3; }
                        catch { }
                        break;
                    case 4:
                        try { if (myGroup != groups[x, y, z ^ 1]) neighbors += 1 << 4; }
                        catch { }
                        break;
                }
            return neighbors;
        }
    }
}
