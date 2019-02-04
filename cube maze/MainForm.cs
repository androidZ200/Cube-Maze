using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace cube_maze
{
    public partial class Main : Form
    {
        byte[,] currentMaze;
        bool isPlaying = false;
        Point position = new Point();
        Point finish = new Point();
        Point start = new Point();
        Point imageSize = new Point();
        List<Point> neighbors = new List<Point>();

        public Main()
        {
            InitializeComponent();
            maze2D();
            imageSize = sizeImage();
            Draw();
        }
        void maze2D()
        {
            isPlaying = false;
            Random rnd = new Random();
            int x = rnd.Next(9, 13);
            if (x % 2 == 0) x++;
            int y = rnd.Next(15, 19);
            byte[,] field = new byte[x, y];
            int[,] group = new int[x, y];
            int counter = 0;
            for (int i = 0; i < x; i++)
                for (int j = 0; j < y; j++)
                {
                    field[i, j] = 0;
                    group[i, j] = counter;
                    counter++;
                }
            while (true)
            {
                int X = rnd.Next(x);
                int Y = rnd.Next(y);
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
                        if (rotate == 0)
                        {
                            field[X, Y] += 1;
                            field[X, Y - 1] += 4;
                        }
                        else if (rotate == 1)
                        {
                            field[X, Y] += 2;
                            field[X + 1, Y] += 8;
                        }
                        else if (rotate == 2)
                        {
                            field[X, Y] += 4;
                            field[X, Y + 1] += 1;
                        }
                        else if (rotate == 3)
                        {
                            field[X, Y] += 8;
                            field[X - 1, Y] += 2;
                        }
                        for (int i = 0; i < x; i++)
                            for (int j = 0; j < y; j++)
                                if (group[i, j] == ReplacingAGroup)
                                    group[i, j] = group[X, Y];
                        link = false;
                    }
                }
                link = true;
                for (int i = 0; i < x; i++)
                    for (int j = 0; j < y; j++)
                        if (group[i, j] != group[X, Y])
                            link = false;
                if (link)
                    break;
            }
            finish = new Point((x - 1) / 2, 0);
            start = new Point((x - 1) / 2, y - 1);
            currentMaze = field;
        }
        void Draw()
        {
            if (pictureBox1.Height > 0)
            {
                Bitmap bmp = new Bitmap(imageSize.X, imageSize.Y);
                Graphics g = Graphics.FromImage(bmp);
                double Step = imageSize.X * 1.0 / currentMaze.GetLength(0);
                if (!isPlaying)
                    g.FillRectangle(new SolidBrush(Color.Green), (float)(start.X * Step), (float)(start.Y * Step), (float)Step - 1, (float)Step - 1);
                else
                {
                    Brush b = new SolidBrush(Color.Gray);
                    if (position == finish)
                        g.FillRectangle(new SolidBrush(Color.Green), (float)(finish.X * Step), (float)(finish.Y * Step), (float)Step - 1, (float)Step - 1);
                    else g.FillRectangle(b, (float)(position.X * Step), (float)(position.Y * Step), (float)Step - 1, (float)Step - 1);
                    for (int i = 0; i < neighbors.Count; i++)
                        if (neighbors[i] == finish)
                            g.FillRectangle(new SolidBrush(Color.Green), (float)(finish.X * Step), (float)(finish.Y * Step), (float)Step - 1, (float)Step - 1);
                        else g.FillRectangle(b, (float)(neighbors[i].X * Step), (float)(neighbors[i].Y * Step), (float)Step - 1, (float)Step - 1);
                }
                pictureBox1.Image = bmp;
            }
        }
        Point isPosition(Point Mouse)
        {
            Point padding = new Point((pictureBox1.Width - imageSize.X) / 2, (pictureBox1.Height - imageSize.Y) / 2);
            double Step = imageSize.Y * 1.0 / currentMaze.GetLength(1);
            Point position = new Point(-1, -1);
            for(int i = 1; i <= currentMaze.GetLength(0); i++)
                if((i-1) * Step + padding.X < Mouse.X && Mouse.X <= i * Step + padding.X)
                {
                    position.X = i - 1;
                    break;
                }
            for (int i = 1; i <= currentMaze.GetLength(1); i++)
                if ((i - 1) * Step + padding.Y < Mouse.Y && Mouse.Y <= i * Step + padding.Y)
                {
                    position.Y = i - 1;
                    break;
                }
            return position;
        }
        Point sizeImage()
        {
            double XYmaze = currentMaze.GetLength(0) * 1.0 / currentMaze.GetLength(1);
            double XYpicture = pictureBox1.Width * 1.0 / pictureBox1.Height;
            if(XYpicture > XYmaze)
                return new Point((int)(pictureBox1.Height * XYmaze), pictureBox1.Height);
            return new Point(pictureBox1.Width, (int)(pictureBox1.Width / XYmaze));
        }
        void UpdateNeighbors(Point position)
        {
            neighbors.Clear();
            int t = currentMaze[position.X, position.Y];
            if ((t & 8) != 0) neighbors.Add(new Point(position.X - 1, position.Y));
            if ((t & 4) != 0) neighbors.Add(new Point(position.X, position.Y + 1));
            if ((t & 2) != 0) neighbors.Add(new Point(position.X + 1, position.Y));
            if ((t & 1) != 0) neighbors.Add(new Point(position.X, position.Y - 1));
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if(isPlaying)
            {
                Point currentPosition = isPosition(e.Location);
                if (currentPosition != position)
                {
                    for (int i = 0; i < neighbors.Count; i++)
                        if (neighbors[i] == currentPosition)
                        {
                            UpdateNeighbors(neighbors[i]);
                            position = currentPosition;
                            Draw();
                            return;
                        }
                    isPlaying = false;
                    Draw();
                }
            }
        }
        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            Point currentPosition = isPosition(e.Location);
            if(!isPlaying && currentPosition == start)
            {
                isPlaying = true;
                UpdateNeighbors(start);
                position = start;
                Draw();
            }
            else if(isPlaying && currentPosition == finish)
            {
                MessageBox.Show("Победа!");
                maze2D();
                imageSize = sizeImage();
                Draw();
            }
        }
        private void pictureBox1_SizeChanged(object sender, EventArgs e)
        {
            imageSize = sizeImage();
            Draw();
        }
    }
}