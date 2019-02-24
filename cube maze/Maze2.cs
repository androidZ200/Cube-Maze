using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cube_maze
{
    class Maze2 : IMaze
    {
        public byte[,,] field { get; private set; }
        public Point3 Finish { get; private set; }
        public Point3 Start { get; private set; }
        public int Height { get; private set; }
        public int Width { get; private set; }
        private Bitmap[] blocks = new Bitmap[16];
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
            return new Bitmap(1, 1);
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
            while(!isOneGroup(group))
            {
                int X = rnd.Next(Width);
                int Y = rnd.Next(Height);
                int Z = rnd.Next(2);
                byte neighbors = checkNeighbors(group, X, Y, Z);
                neighbors &= withWhomYouCanConnect(field[X, Y, Z], field[X, Y, Z ^ 1]);
                while(neighbors != 0)
                {
                    byte rotate = (byte)rnd.Next(8);
                    if((neighbors & (1 << rotate)) != 0)
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
    }
}
