using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;

namespace cube_maze
{
    public abstract class Maze
    {
        public int Height { get; protected set; }
        public int Width { get; protected set; }
        public Point3 Start { get; protected set; }
        public Point3 Finish { get; protected set; }
        protected Random rnd = new Random();
        public abstract Bitmap GetFullImage(Color Line, Color SFPoibt);
        public abstract Point3[] GetNeighbors(Point3 I);
        public virtual Bitmap GetImage(Point3 coordinate, Color Line, Color SFPoibt)
        {
            Bitmap image = new Bitmap(Width * 64 - 2, Height * 64 - 2);
            Graphics g = Graphics.FromImage(image);
            Point3[] neighbors = GetNeighbors(coordinate);
            g.DrawImage(GetCell(coordinate, Line, SFPoibt), coordinate.X * 64, coordinate.Y * 64);
            for(int i = 0; i < neighbors.Length; i++)
                g.DrawImage(GetCell(neighbors[i], Line, SFPoibt), neighbors[i].X * 64, neighbors[i].Y * 64);
            return image;
        }
        protected virtual Bitmap GetCell(Point3 cell, Color Line, Color SFPoibt)
        {
            Bitmap bmp = new Bitmap(62, 62);
            Graphics g = Graphics.FromImage(bmp);
            if (cell == Finish) g.Clear(SFPoibt);
            else g.Clear(Line);
            return bmp;
        }

        protected bool isOneGroup(IEnumerator groups)
        {
            groups.MoveNext();
            int s = (int)groups.Current;
            while (groups.MoveNext())
                if (s != (int)groups.Current) return false;
            return true;
        }
    }

    class MazeNormal : Maze
    {
        public byte[,] maze { get; private set; }
        private Bitmap[] blocks = new Bitmap[16];

        public MazeNormal()
        {
            Height = rnd.Next(15, 21);
            Width = rnd.Next(9, 14);
            if (Width % 2 == 0) Width++;

            GenerateMaze();
        }
        public MazeNormal(int width, int height)
        {
            Width = width;
            Height = height;
            GenerateMaze();
        }
        public override Point3[] GetNeighbors(Point3 Position)
        {
            Point3[] neighbors;
            int s = 0;
            int t = maze[Position.X, Position.Y];

            if ((t & 8) != 0) s++;
            if ((t & 4) != 0) s++;
            if ((t & 2) != 0) s++;
            if ((t & 1) != 0) s++;

            neighbors = new Point3[s];
            int i = 0;

            if ((t & 8) != 0) { neighbors[i] = new Point3(Position.X - 1, Position.Y, 0); i++; }
            if ((t & 4) != 0) { neighbors[i] = new Point3(Position.X, Position.Y + 1, 0); i++; }
            if ((t & 2) != 0) { neighbors[i] = new Point3(Position.X + 1, Position.Y, 0); i++; }
            if ((t & 1) != 0) { neighbors[i] = new Point3(Position.X, Position.Y - 1, 0); i++; }

            return neighbors;
        }
        public override Bitmap GetFullImage(Color Line, Color SFPoibt)
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
            } while (!isOneGroup(group.GetEnumerator()));
            Finish = new Point3(Width / 2, 0, 0);
            Start = new Point3(Width / 2, Height - 1, 0);
            maze = field;
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
        private void fillGroup(int[,] groups, int A, int B)
        {
            for (int j = 0; j < groups.GetLength(1); j++)
                for (int i = 0; i < groups.GetLength(0); i++)
                    if (groups[i, j] == A) groups[i, j] = B;
        }
        private byte checkNeighbors(int[,] groups, int x, int y)
        {
            int myGroup = groups[x, y];
            byte neighbors = 0;
            for (int i = 0; i < 4; i++)
                switch (i)
                {
                    case 0:
                        try { if (myGroup != groups[x, y - 1]) neighbors += 1 << 0; }
                        catch { }
                        break;
                    case 1:
                        try { if (myGroup != groups[x + 1, y]) neighbors += 1 << 1; }
                        catch { }
                        break;
                    case 2:
                        try { if (myGroup != groups[x, y + 1]) neighbors += 1 << 2; }
                        catch { }
                        break;
                    case 3:
                        try { if (myGroup != groups[x - 1, y]) neighbors += 1 << 3; }
                        catch { }
                        break;
                }
            return neighbors;
        }
    }
    class MazeCyclical : Maze
    {
        public byte[,,] field { get; private set; }

        public MazeCyclical()
        {
            Height = rnd.Next(15, 21);
            Width = rnd.Next(9, 14);
            if (Width % 2 == 0) Width++;

            Generate();
        }
        public MazeCyclical(int width, int height)
        {
            Width = width;
            Height = height;
            Generate();

        }
        public byte GetCell(int x, int y, int z)
        {
            return field[x, y, z];
        }
        public override Bitmap GetFullImage(Color Line, Color SFPoibt)
        {
            Bitmap bmp = new Bitmap(Width * 160, Height * 160);
            Graphics g = Graphics.FromImage(bmp);
            for (int j = 0; j < Height; j++)
                for (int i = 0; i < Width; i++)
                    g.DrawImage(GetBlockImage(field[i, j, 0], field[i, j, 1], Line), i * 160, j * 160);
            g.FillEllipse(new SolidBrush(SFPoibt), Start.X * 160 + 60, Start.Y * 160 + 60, 40, 40);
            g.FillEllipse(new SolidBrush(SFPoibt), Finish.X * 160 + 60, Finish.Y * 160 + 60, 40, 40);
            return bmp;
        }
        public override Point3[] GetNeighbors(Point3 Position)
        {
            byte I = field[Position.X, Position.Y, Position.Z];
            List<Point3> neighbors = new List<Point3>();
            for (int i = 0; i < 8; i++)
                if ((I & (1 << i)) != 0)
                    switch (i)
                    {
                        case 0:
                            neighbors.Add(new Point3(Position.X, Position.Y - 1, Position.Z));
                            break;
                        case 1:
                            neighbors.Add(new Point3(Position.X + 1, Position.Y, Position.Z));
                            break;
                        case 2:
                            neighbors.Add(new Point3(Position.X, Position.Y + 1, Position.Z));
                            break;
                        case 3:
                            neighbors.Add(new Point3(Position.X - 1, Position.Y, Position.Z));
                            break;
                        case 4:
                            neighbors.Add(new Point3(Position.X, Position.Y - 1, Position.Z ^ 1));
                            break;
                        case 5:
                            neighbors.Add(new Point3(Position.X + 1, Position.Y, Position.Z ^ 1));
                            break;
                        case 6:
                            neighbors.Add(new Point3(Position.X, Position.Y + 1, Position.Z ^ 1));
                            break;
                        case 7:
                            neighbors.Add(new Point3(Position.X - 1, Position.Y, Position.Z ^ 1));
                            break;
                    }
            return neighbors.ToArray();
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
            while (!isOneGroup(group.GetEnumerator()))
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
    class MazeDuplex : Maze
    {
        public byte[,,] field { get; private set; }

        public MazeDuplex()
        {
            Height = rnd.Next(15, 21);
            Width = rnd.Next(9, 14);
            if (Width % 2 == 0) Width++;

            Generate();
        }
        public MazeDuplex(int width, int height)
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
        public override Bitmap GetFullImage(Color Line, Color SFPoibt)
        {
            Bitmap bmp = new Bitmap(Width * 160, Height * 160);
            Graphics g = Graphics.FromImage(bmp);
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
        public override Point3[] GetNeighbors(Point3 Position)
        {
            Point3[] neighbors;
            int s = 0;
            int t = field[Position.X, Position.Y, Position.Z];

            if ((t & 8) != 0) s++;
            if ((t & 4) != 0) s++;
            if ((t & 2) != 0) s++;
            if ((t & 1) != 0) s++;

            neighbors = new Point3[s];
            int i = 0;

            if ((t & 8) != 0) { neighbors[i] = new Point3(Position.X - 1, Position.Y, Position.Z); i++; }
            if ((t & 4) != 0) { neighbors[i] = new Point3(Position.X, Position.Y + 1, Position.Z); i++; }
            if ((t & 2) != 0) { neighbors[i] = new Point3(Position.X + 1, Position.Y, Position.Z); i++; }
            if ((t & 1) != 0) { neighbors[i] = new Point3(Position.X, Position.Y - 1, Position.Z); i++; }

            return neighbors;
        }
        protected override Bitmap GetCell(Point3 cell, Color Line, Color SFPoibt)
        {
            if((GetCell(cell) & (1 << 4)) == 0)
                return base.GetCell(cell, Line, SFPoibt);
            Bitmap t = new Bitmap(62, 62);
            Graphics g = Graphics.FromImage(t);

            Color line = Line;
            if (cell == Finish) line = SFPoibt;

            g.FillRectangle(new SolidBrush(line), 0, 0, 62, 16);
            g.FillRectangle(new SolidBrush(line), 0, 46, 62, 16);
            g.FillRectangle(new SolidBrush(line), 0, 0, 16, 62);
            g.FillRectangle(new SolidBrush(line), 46, 0, 16, 62);

            return t;
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
            if ((blockUp & (1 << 4)) != 0)
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
            while (!isOneGroup(group.GetEnumerator()))
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
    class MazeAbstract : Maze
    {
        public class Cell
        {
            public Point coord;
            public Cell[] Neighbors;
            public Cell(int x, int y)
            {
                Neighbors = new Cell[4];
                coord = new Point(x, y);
            }
        }

        public Cell[,] field { get; private set; }

        public MazeAbstract()
        {
            Height = rnd.Next(15, 21);
            Width = rnd.Next(9, 14);
            if (Width % 2 == 0) Width++;

            GenerateMaze();
        }
        public MazeAbstract(int width, int height)
        {
            Width = width;
            Height = height;
            GenerateMaze();
        }
        public List<Point> GetNeighbors(Point Position)
        {
            List<Point> neighbors = new List<Point>();
            foreach (var x in field[Position.X, Position.Y].Neighbors)
                if (x != null)
                    neighbors.Add(x.coord);
            return neighbors;
        }
        public override Bitmap GetFullImage(Color Line, Color SFPoint)
        {
            Bitmap bmp = new Bitmap(Width * 160, Height * 160);
            Graphics g = Graphics.FromImage(bmp);
            DrawLines(field[Start.X, Start.Y], null, g, new Pen(Line, 5));
            for (int j = 0; j < Height; j++)
                for (int i = 0; i < Width; i++)
                    g.FillEllipse(new SolidBrush(Line), i * 160 + 70, j * 160 + 70, 20, 20);
            g.FillEllipse(new SolidBrush(SFPoint), Start.X * 160 + 60, Start.Y * 160 + 60, 40, 40);
            g.FillEllipse(new SolidBrush(SFPoint), Finish.X * 160 + 60, Finish.Y * 160 + 60, 40, 40);
            return bmp;
        }
        public override Point3[] GetNeighbors(Point3 Position)
        {
            int t = 0;
            for (int i = 0; i < 4; i++)
                if (field[Position.X, Position.Y].Neighbors[i] != null) t = i + 1;
            Point3[] neighbors = new Point3[t];

            for (int i = 0; i < neighbors.Length; i++)
                neighbors[i] = new Point3(field[Position.X, Position.Y].Neighbors[i].coord, 0);

            return neighbors;
        }
        private void DrawLines(Cell x, Cell prev, Graphics g, Pen line)
        {
            foreach (var i in x.Neighbors)
                if (i != null && i != prev)
                {
                    g.DrawLine(line, GetPoint(x), GetPoint(i));
                    DrawLines(i, x, g, line);
                }
        }
        private Point GetPoint(Cell c)
        {
            return new Point(c.coord.X * 160 + 80, c.coord.Y * 160 + 80);
        }

        private void GenerateMaze()
        {
            field = new Cell[Width, Height];
            Start = new Point3(Width / 2, Height - 1, 0);
            Finish = new Point3(Width / 2, 0, 0);
            int[,] groups = new int[Width, Height];
            for (int j = 0; j < Height; j++)
                for (int i = 0; i < Width; i++)
                {
                    groups[i, j] = j * Width + i;
                    field[i, j] = new Cell(i, j);
                }
            while (!isOneGroup(groups.GetEnumerator()))
            {
                int x = rnd.Next(Width);
                int y = rnd.Next(Height);
                if (field[x, y].Neighbors[3] != null) continue;
                int i = 0;
                for (i = 0; i < 4; i++) if (field[x, y].Neighbors[i] == null) break;

                int ox = rnd.Next(Width);
                int oy = rnd.Next(Height);
                if (field[ox, oy].Neighbors[3] != null) continue;
                int oi = 0;
                for (oi = 0; oi < 4; oi++) if (field[ox, oy].Neighbors[oi] == null) break;

                if (groups[x, y] != groups[ox, oy])
                {
                    field[x, y].Neighbors[i] = field[ox, oy];
                    field[ox, oy].Neighbors[oi] = field[x, y];
                    fillGroup(groups, groups[x, y], groups[ox, oy]);
                }
            }
        }
        private void fillGroup(int[,] groups, int A, int B)
        {
            for (int j = 0; j < groups.GetLength(1); j++)
                for (int i = 0; i < groups.GetLength(0); i++)
                    if (groups[i, j] == A) groups[i, j] = B;
        }
    }
}