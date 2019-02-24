using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Drawing;
using System.Diagnostics;

namespace cube_maze
{
    class GameMaze
    {
        private IMaze maze;
        private Point3 player = new Point3();
        private bool isPlaying = false;
        private Stopwatch StartTime;
        private List<Point3> neighbors = new List<Point3>();
        public bool firstGame { get; private set; }
        public delegate void WinHandler();
        public event WinHandler Win;
        public TimeSpan Time { get; private set; }
        public Color line { get; set; }
        public Color background { get; set; }
        public Color sfPoint { get; set; }

        public GameMaze(IMaze maze)
        {
            this.maze = maze;
            firstGame = true;
            line = Color.Gray;
            background = Color.Transparent;
            sfPoint = Color.Green;
        }
        public GameMaze()
        {
            firstGame = true;
            line = Color.Gray;
            background = Color.Transparent;
            sfPoint = Color.Green;
        }

        public void Click(Point coordinate)
        {
            if (!isPlaying && new Point3(coordinate) == maze.Start)
            {
                isPlaying = true;
                UpdateNeighbors(maze.Start);
                player = maze.Start;
                if(firstGame)
                {
                    firstGame = false;
                    StartTime = Stopwatch.StartNew();
                }
            }
            else if (isPlaying && player == maze.Finish)
            {
                Time = StartTime.Elapsed;
                Win();
                isPlaying = false;
                firstGame = true;
            }
        }
        public void Move(Point coordinate)
        {
            if (isPlaying)
            {
                if (coordinate != new Point(player.X, player.Y))
                {
                    for (int i = 0; i < neighbors.Count; i++)
                        if  (new Point(neighbors[i].X, neighbors[i].Y) == coordinate)
                        {
                            player = neighbors[i];
                            UpdateNeighbors(neighbors[i]);
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
                g.FillRectangle(new SolidBrush(sfPoint), (float)(maze.Start.X * Step), (float)(maze.Start.Y * Step), (float)Step - 1, (float)Step - 1);
            else
            {
                Brush b = new SolidBrush(line);
                if (player == maze.Finish)
                    g.FillRectangle(new SolidBrush(sfPoint), (float)(maze.Finish.X * Step), (float)(maze.Finish.Y * Step), (float)Step - 1, (float)Step - 1);
                else g.FillRectangle(b, (float)(player.X * Step), (float)(player.Y * Step), (float)Step - 1, (float)Step - 1);
                for (int i = 0; i < neighbors.Count; i++)
                    if (neighbors[i] == maze.Finish)
                        g.FillRectangle(new SolidBrush(sfPoint), (float)(maze.Finish.X * Step), (float)(maze.Finish.Y * Step), (float)Step - 1, (float)Step - 1);
                    else g.FillRectangle(b, (float)(neighbors[i].X * Step), (float)(neighbors[i].Y * Step), (float)Step - 1, (float)Step - 1);
            }
            return bmp;
        }
        public Bitmap GetFullImage(int width, int height)
        {
            Point size = sizeImage(width, height);
            return new Bitmap(maze.GetImage(background, line, sfPoint), size.X, size.Y);
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
        public void NewGenerate(IMaze maze)
        {
            isPlaying = false;
            firstGame = true;
            this.maze = maze;
        }

        private void UpdateNeighbors(Point3 coordinate)
        {
            byte cell = maze.GetCell(coordinate.X, coordinate.Y, coordinate.Z);
            neighbors.Clear();
            if ((cell & (1 << 0)) != 0) neighbors.Add(new Point3(coordinate.X, coordinate.Y - 1, coordinate.Z));
            if ((cell & (1 << 1)) != 0) neighbors.Add(new Point3(coordinate.X + 1, coordinate.Y, coordinate.Z));
            if ((cell & (1 << 2)) != 0) neighbors.Add(new Point3(coordinate.X, coordinate.Y + 1, coordinate.Z));
            if ((cell & (1 << 3)) != 0) neighbors.Add(new Point3(coordinate.X - 1, coordinate.Y, coordinate.Z));

            if ((cell & (1 << 4)) != 0) neighbors.Add(new Point3(coordinate.X, coordinate.Y - 1, coordinate.Z ^ 1));
            if ((cell & (1 << 5)) != 0) neighbors.Add(new Point3(coordinate.X + 1, coordinate.Y, coordinate.Z ^ 1));
            if ((cell & (1 << 6)) != 0) neighbors.Add(new Point3(coordinate.X, coordinate.Y + 1, coordinate.Z ^ 1));
            if ((cell & (1 << 7)) != 0) neighbors.Add(new Point3(coordinate.X - 1, coordinate.Y, coordinate.Z ^ 1));
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
