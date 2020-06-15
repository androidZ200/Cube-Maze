using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Drawing;
using System.Diagnostics;

namespace cube_maze
{
    public class Game
    {
        public enum Mode
        {
            Normal, Cyclical, Dualex, Abstract, Oriented
        }

        private Maze maze;
        private Point3 Position;
        private Stopwatch StartTime;
        private bool isPlaying = false;
        private bool firstGame = true;
        public event Action Win;
        public TimeSpan Time { get; protected set; }
        public Color line { get; set; } = Color.Gray;
        public Color sfPoint { get; set; } = Color.Green;
        public Mode mode { get; private set; }

        public void Click(int pictureBoxWidth, int pictureBoxHeight, Point Mouse)
        {
            Point position = GetPosition(pictureBoxWidth, pictureBoxHeight, Mouse);
            Click(position);
        }
        public void Move(int pictureBoxWidth, int pictureBoxHeight, Point Mouse)
        {
            Point position = GetPosition(pictureBoxWidth, pictureBoxHeight, Mouse);
            Move(position);
        }
        private void Click(Point coord)
        {
            if (!isPlaying && coord == maze.Start.toPoint())
            {
                Position = maze.Start;
                ClickStart();
            }
            else if (isPlaying && Position == maze.Finish)
                ClickFinish();
            else if (mode == Mode.Dualex && (((MazeDuplex)maze).GetCell(Position) & (1 << 4)) != 0)
                Position.Z = Position.Z ^ 1;
        }
        private void Move(Point coord)
        {
            if (isPlaying)
            {
                if (coord != Position.toPoint())
                {
                    Point3[] neighbors = maze.GetNeighbors(Position);
                    for (int i = 0; i < neighbors.Length; i++)
                        if (neighbors[i].toPoint() == coord)
                        {
                            Position = neighbors[i];
                            return;
                        }
                    if (mode != Mode.Abstract) isPlaying = false;
                }
            }
        }
        public virtual Bitmap GetImage()
        {
            if (isPlaying)
                return maze.GetImage(Position, line, sfPoint);
            Bitmap t = new Bitmap(maze.Width * 64 - 2, maze.Height * 64 - 2);
            Graphics g = Graphics.FromImage(t);
            g.FillRectangle(new SolidBrush(sfPoint), maze.Start.X * 64, maze.Start.Y * 64, 62, 62);
            return t;
        }
        public Bitmap GetFullImage()
        {
            return maze.GetFullImage(line, sfPoint);
        }
        public void NewGenerate(Mode mode)
        {
            this.mode = mode;
            switch(mode)
            {
                case Mode.Abstract:
                    maze = new MazeAbstract();
                    break;
                case Mode.Cyclical:
                    maze = new MazeCyclical();
                    break;
                case Mode.Dualex:
                    maze = new MazeDuplex();
                    break;
                case Mode.Normal:
                    maze = new MazeNormal();
                    break;
                case Mode.Oriented:
                    maze = new MazeOriented();
                    break;
            }
        }

        private Point GetPosition(int pictureBoxWidth, int pictureBoxHeight, Point Mouse)
        {
            Size size = sizeImage(pictureBoxWidth, pictureBoxHeight);
            Point padding = new Point((pictureBoxWidth - size.Width) / 2, (pictureBoxHeight - size.Height) / 2);
            double Step = size.Height * 1.0 / maze.Height;
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
        private Size sizeImage(int currentWidth, int currentHeight)
        {
            double XYmaze = maze.Width * 1.0 / maze.Height;
            double XYpicture = currentWidth * 1.0 / currentHeight;
            if (XYpicture > XYmaze)
                return new Size((int)(currentHeight * XYmaze), currentHeight);
            return new Size(currentWidth, (int)(currentWidth / XYmaze));
        }
        private void ClickStart()
        {
            isPlaying = true;
            if (firstGame)
            {
                firstGame = false;
                StartTime = Stopwatch.StartNew();
            }
        }
        private void ClickFinish()
        {
            Time = StartTime.Elapsed;
            isPlaying = false;
            firstGame = true;
            Win();
        }
    }
}