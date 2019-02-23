using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace cube_maze
{
    class GameMaze
    {
        private IMaze maze;
        private Point player = new Point();
        private bool isPlaying = false;
        private List<Point> neighbors = new List<Point>();
        public delegate void WinHandler();
        public event WinHandler Win;

        public GameMaze()
        {
            maze = new Labyrinth();
        }

        public void Click(Point coordinate)
        {
            if (!isPlaying && coordinate == maze.Start)
            {
                isPlaying = true;
                UpdateNeighbors(maze.Start);
                player = maze.Start;
            }
            else if (isPlaying && player == maze.Finish)
            {
                Win();
                isPlaying = false;
                maze = new Labyrinth();
            }
        }
        public void Move(Point coordinate)
        {
            if (isPlaying)
            {
                if (coordinate != player)
                {
                    for (int i = 0; i < neighbors.Count; i++)
                        if (neighbors[i] == coordinate)
                        {
                            UpdateNeighbors(coordinate);
                            player = coordinate;
                            return;
                        }
                    isPlaying = false;
                }
            }
        }
        public Bitmap GetImage(int width, int height)
        {
            Point imageSize = sizeImage(width, height);
            Bitmap bmp = new Bitmap(imageSize.X, imageSize.Y);
            Graphics g = Graphics.FromImage(bmp);
            double Step = imageSize.X * 1.0 / maze.Width;
            if (!isPlaying)
                g.FillRectangle(new SolidBrush(Color.Green), (float)(maze.Start.X * Step), (float)(maze.Start.Y * Step), (float)Step - 1, (float)Step - 1);
            else
            {
                Brush b = new SolidBrush(Color.Gray);
                if (player == maze.Finish)
                    g.FillRectangle(new SolidBrush(Color.Green), (float)(maze.Finish.X * Step), (float)(maze.Finish.Y * Step), (float)Step - 1, (float)Step - 1);
                else g.FillRectangle(b, (float)(player.X * Step), (float)(player.Y * Step), (float)Step - 1, (float)Step - 1);
                for (int i = 0; i < neighbors.Count; i++)
                    if (neighbors[i] == maze.Finish)
                        g.FillRectangle(new SolidBrush(Color.Green), (float)(maze.Finish.X * Step), (float)(maze.Finish.Y * Step), (float)Step - 1, (float)Step - 1);
                    else g.FillRectangle(b, (float)(neighbors[i].X * Step), (float)(neighbors[i].Y * Step), (float)Step - 1, (float)Step - 1);
            }
            return bmp;
        }
        public Bitmap GetFullImage(int width, int height)
        {
            Point size = sizeImage(width, height);
            return new Bitmap(maze.GetImage(Color.Transparent, Color.Gray, Color.Green), size.X, size.Y);
        }
        public Point GetPosition(int pictureBoxWidth, int pictureBoxHeight, Point Mouse)
        {
            Point size = sizeImage(pictureBoxWidth, pictureBoxHeight);
            Point padding = new Point((pictureBoxWidth - size.X) / 2, (pictureBoxHeight - size.Y) / 2);
            double Step = size.Y * 1.0 / maze.Height;
            Point position = new Point(-1, -1);
            for (int i = 1; i <= maze.Width; i++)
                if ((i - 1) * Step + padding.X < Mouse.X && Mouse.X <= i * Step + padding.X)
                {
                    position.X = i - 1;
                    break;
                }
            for (int i = 1; i <= maze.Height; i++)
                if ((i - 1) * Step + padding.Y < Mouse.Y && Mouse.Y <= i * Step + padding.Y)
                {
                    position.Y = i - 1;
                    break;
                }
            return position;
        }

        private void UpdateNeighbors(Point coordinate)
        {
            byte cell = maze.GetCell(coordinate.X, coordinate.Y);
            neighbors.Clear();
            if ((cell & 1) != 0) neighbors.Add(new Point(coordinate.X, coordinate.Y - 1));
            if ((cell & 2) != 0) neighbors.Add(new Point(coordinate.X + 1, coordinate.Y));
            if ((cell & 4) != 0) neighbors.Add(new Point(coordinate.X, coordinate.Y + 1));
            if ((cell & 8) != 0) neighbors.Add(new Point(coordinate.X - 1, coordinate.Y));
        }
        private Point sizeImage(int currentWidth, int currentHeight)
        {
            double XYmaze = maze.Width * 1.0 / maze.Height;
            double XYpicture = currentWidth * 1.0 / currentHeight;
            if (XYpicture > XYmaze)
                return new Point((int)(currentHeight * XYmaze), currentHeight);
            return new Point(currentWidth, (int)(currentWidth / XYmaze));
        }
    }
}
