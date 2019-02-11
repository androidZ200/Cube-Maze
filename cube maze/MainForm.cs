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
    public partial class MainForm : Form
    {
        Labyrinth currentMaze;
        bool isPlaying = false;
        Point position = new Point();
        Point imageSize = new Point();
        List<Point> neighbors = new List<Point>();

        public MainForm()
        {
            InitializeComponent();
            currentMaze = new Labyrinth();
            imageSize = sizeImage();
            Draw();
        }
        void Draw()
        {
            if (pictureBox1.Height > 0)
            {
                Bitmap bmp = new Bitmap(imageSize.X, imageSize.Y);
                Graphics g = Graphics.FromImage(bmp);
                double Step = imageSize.X * 1.0 / currentMaze.Width;
                if (!isPlaying)
                    g.FillRectangle(new SolidBrush(Color.Green), (float)(currentMaze.Start.X * Step), (float)(currentMaze.Start.Y * Step), (float)Step - 1, (float)Step - 1);
                else
                {
                    Brush b = new SolidBrush(Color.Gray);
                    if (position == currentMaze.Finish)
                        g.FillRectangle(new SolidBrush(Color.Green), (float)(currentMaze.Finish.X * Step), (float)(currentMaze.Finish.Y * Step), (float)Step - 1, (float)Step - 1);
                    else g.FillRectangle(b, (float)(position.X * Step), (float)(position.Y * Step), (float)Step - 1, (float)Step - 1);
                    for (int i = 0; i < neighbors.Count; i++)
                        if (neighbors[i] == currentMaze.Finish)
                            g.FillRectangle(new SolidBrush(Color.Green), (float)(currentMaze.Finish.X * Step), (float)(currentMaze.Finish.Y * Step), (float)Step - 1, (float)Step - 1);
                        else g.FillRectangle(b, (float)(neighbors[i].X * Step), (float)(neighbors[i].Y * Step), (float)Step - 1, (float)Step - 1);
                }
                pictureBox1.Image = bmp;
            }
        }
        Point isPosition(Point Mouse)
        {
            Point padding = new Point((pictureBox1.Width - imageSize.X) / 2, (pictureBox1.Height - imageSize.Y) / 2);
            double Step = imageSize.Y * 1.0 / currentMaze.Height;
            Point position = new Point(-1, -1);
            for(int i = 1; i <= currentMaze.Width; i++)
                if((i-1) * Step + padding.X < Mouse.X && Mouse.X <= i * Step + padding.X)
                {
                    position.X = i - 1;
                    break;
                }
            for (int i = 1; i <= currentMaze.Height; i++)
                if ((i - 1) * Step + padding.Y < Mouse.Y && Mouse.Y <= i * Step + padding.Y)
                {
                    position.Y = i - 1;
                    break;
                }
            return position;
        }
        Point sizeImage()
        {
            double XYmaze = currentMaze.Width * 1.0 / currentMaze.Height;
            double XYpicture = pictureBox1.Width * 1.0 / pictureBox1.Height;
            if(XYpicture > XYmaze)
                return new Point((int)(pictureBox1.Height * XYmaze), pictureBox1.Height);
            return new Point(pictureBox1.Width, (int)(pictureBox1.Width / XYmaze));
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
                            neighbors = currentMaze.GetNeighbors(neighbors[i]);
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
            if(!isPlaying && currentPosition == currentMaze.Start)
            {
                isPlaying = true;
                neighbors = currentMaze.GetNeighbors(currentMaze.Start);
                position = currentMaze.Start;
                Draw();
            }
            else if(isPlaying && currentPosition == currentMaze.Finish)
            {
                pictureBox1.Image = new Bitmap(currentMaze.GetImage(Color.Transparent, Color.Gray, Color.Green), imageSize.X, imageSize.Y);
                currentMaze = new Labyrinth();
                MessageBox.Show("Победа!");
                imageSize = sizeImage();
                isPlaying = false;
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