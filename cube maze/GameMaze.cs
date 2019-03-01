using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Drawing;
using System.Diagnostics;

namespace cube_maze
{
    abstract class Game
    {
        protected Stopwatch StartTime;
        protected bool isPlaying = false;
        protected bool firstGame = true;
        public event Action Win;
        public TimeSpan Time { get; protected set; }
        public Color line { get; set; } = Color.Gray;
        public Color sfPoint { get; set; } = Color.Green;

        public abstract void Click(int pictureBoxWidth, int pictureBoxHeight, Point Mouse);
        public abstract void Move(int pictureBoxWidth, int pictureBoxHeight, Point Mouse);
        public abstract Bitmap GetImage(int width, int height);
        public abstract Bitmap GetFullImage(int width, int height);
        public abstract void NewGenerate();

        protected Point GetPosition(int pictureBoxWidth, int pictureBoxHeight, Point Mouse, int mazeHeight, int mazeWidth)
        {
            Point size = sizeImage(pictureBoxWidth, pictureBoxHeight, mazeWidth, mazeHeight);
            Point padding = new Point((pictureBoxWidth - size.X) / 2, (pictureBoxHeight - size.Y) / 2);
            double Step = size.Y * 1.0 / mazeHeight;
            Point position = new Point(-1, -1);
            for (int i = 1; i <= mazeWidth; i++)
                if ((i - 1) * Step + padding.X < Mouse.X && Mouse.X <= i * Step + padding.X)
                {
                    position.X = i - 1;
                    break;
                }
            for (int i = 1; i <= mazeHeight; i++)
                if ((i - 1) * Step + padding.Y < Mouse.Y && Mouse.Y <= i * Step + padding.Y)
                {
                    position.Y = i - 1;
                    break;
                }
            return position;
        }
        protected Point sizeImage(int currentWidth, int currentHeight, int mazeWidth, int mazeHeight)
        {
            double XYmaze = mazeWidth * 1.0 / mazeHeight;
            double XYpicture = currentWidth * 1.0 / currentHeight;
            if (XYpicture > XYmaze)
                return new Point((int)(currentHeight * XYmaze), currentHeight);
            return new Point(currentWidth, (int)(currentWidth / XYmaze));
        }
        protected void ClickStart()
        {
            isPlaying = true;
            if (firstGame)
            {
                firstGame = false;
                StartTime = Stopwatch.StartNew();
            }
        }
        protected void ClickFinish()
        {
            Time = StartTime.Elapsed;
            isPlaying = false;
            firstGame = true;
            Win();
        }
        protected Bitmap DrawField(Point imageSize, int WidthMaze, List<Point> neighbors, Point player, Point Finish, Point Start)
        {
            Bitmap bmp = new Bitmap(imageSize.X, imageSize.Y);
            Graphics g = Graphics.FromImage(bmp);
            double Step = imageSize.X * 1.0 / WidthMaze;
            if (!isPlaying)
                g.FillRectangle(new SolidBrush(sfPoint), (float)(Start.X * Step), (float)(Start.Y * Step), (float)Step - 1, (float)Step - 1);
            else
            {
                Brush b = new SolidBrush(line);
                if (player == Finish)
                    g.FillRectangle(new SolidBrush(sfPoint), (float)(Finish.X * Step), (float)(Finish.Y * Step), (float)Step - 1, (float)Step - 1);
                else g.FillRectangle(b, (float)(player.X * Step), (float)(player.Y * Step), (float)Step - 1, (float)Step - 1);
                for (int i = 0; i < neighbors.Count; i++)
                    if (neighbors[i] == Finish)
                        g.FillRectangle(new SolidBrush(sfPoint), (float)(Finish.X * Step), (float)(Finish.Y * Step), (float)Step - 1, (float)Step - 1);
                    else g.FillRectangle(b, (float)(neighbors[i].X * Step), (float)(neighbors[i].Y * Step), (float)Step - 1, (float)Step - 1);
            }
            return bmp;
        }
    }

    class GameMazeNormal : Game
    {
        private Point player = new Point();
        private Maze maze;
        private List<Point> neighbors = new List<Point>();

        public GameMazeNormal()
        {
            maze = new Maze();
        }

        public override void Click(int pictureBoxWidth, int pictureBoxHeight, Point Mouse)
        {
            Point coordinate = GetPosition(pictureBoxWidth, pictureBoxHeight, Mouse, maze.Height, maze.Width);
            if (!isPlaying && coordinate == maze.Start)
            {
                UpdateNeighbors(maze.Start);
                player = maze.Start;
                ClickStart();

            }
            else if (isPlaying && player == maze.Finish)
                ClickFinish();
        }
        public override void Move(int pictureBoxWidth, int pictureBoxHeight, Point Mouse)
        {
            if (isPlaying)
            {
                Point coordinate = GetPosition(pictureBoxWidth, pictureBoxHeight, Mouse, maze.Height, maze.Width);
                if (coordinate != player)
                {
                    for (int i = 0; i < neighbors.Count; i++)
                        if (neighbors[i] == coordinate)
                        {
                            player = neighbors[i];
                            UpdateNeighbors(neighbors[i]);
                            return;
                        }
                    isPlaying = false;
                }
            }
        }
        public override Bitmap GetFullImage(int width, int height)
        {
            Point size = sizeImage(width, height, maze.Width, maze.Height);
            return new Bitmap(maze.GetImage(Color.Transparent, line, sfPoint), size.X, size.Y);
        }
        public override Bitmap GetImage(int width, int height)
        {
            Point imageSize = sizeImage(width, height, maze.Width, maze.Height);
            return DrawField(sizeImage(width, height, maze.Width, maze.Height), maze.Width, neighbors, player, maze.Finish, maze.Start);
        }
        public override void NewGenerate()
        {
            maze = new Maze();
        }

        private void UpdateNeighbors(Point cell)
        {
            neighbors = maze.GetNeighbors(cell);
        }
    }
    class GameMazeCyclical : Game
    {
        private Point3 player = new Point3();
        private Maze2 maze;
        private List<Point3> neighbors = new List<Point3>();

        public GameMazeCyclical()
        {
            maze = new Maze2();
        }

        public override void Click(int pictureBoxWidth, int pictureBoxHeight, Point Mouse)
        {
            Point coordinate = GetPosition(pictureBoxWidth, pictureBoxHeight, Mouse, maze.Height, maze.Width);
            if (!isPlaying && new Point3(coordinate, 0) == maze.Start)
            {
                UpdateNeighbors(maze.Start);
                player = maze.Start;
                ClickStart();
            }
            else if (isPlaying && player == maze.Finish)
                ClickFinish();
        }
        public override void Move(int pictureBoxWidth, int pictureBoxHeight, Point Mouse)
        {
            if (isPlaying)
            {
                Point coordinate = GetPosition(pictureBoxWidth, pictureBoxHeight, Mouse, maze.Height, maze.Width);
                if (coordinate != player.toPoint())
                {
                    for (int i = 0; i < neighbors.Count; i++)
                        if (neighbors[i].toPoint() == coordinate)
                        {
                            player = neighbors[i];
                            UpdateNeighbors(neighbors[i]);
                            return;
                        }
                    isPlaying = false;
                }
            }
        }
        public override Bitmap GetFullImage(int width, int height)
        {
            Point size = sizeImage(width, height, maze.Width, maze.Height);
            return new Bitmap(maze.GetImage(Color.Transparent, line, sfPoint), size.X, size.Y);
        }
        public override Bitmap GetImage(int width, int height)
        {
            Point imageSize = sizeImage(width, height, maze.Width, maze.Height);
            List<Point> neighbors = new List<Point>();
            Point Finish = new Point(-1, 0);
            for (int i = 0; i < this.neighbors.Count; i++)
            {
                if (this.neighbors[i] == maze.Finish) Finish = maze.Finish.toPoint();
                neighbors.Add(this.neighbors[i].toPoint());
            }
            if (player == maze.Finish) Finish = maze.Finish.toPoint();
            return DrawField(sizeImage(width, height, maze.Width, maze.Height),
                maze.Width, neighbors, player.toPoint(), Finish, maze.Start.toPoint());
        }
        public override void NewGenerate()
        {
            maze = new Maze2();
        }

        private void UpdateNeighbors(Point3 cell)
        {
            byte I = maze.GetCell(cell.X, cell.Y, cell.Z);
            neighbors.Clear();
            for (int i = 0; i < 8; i++)
                if ((I & (1 << i)) != 0)
                    switch (i)
                    {
                        case 0:
                            neighbors.Add(new Point3(cell.X, cell.Y - 1, cell.Z));
                            break;
                        case 1:
                            neighbors.Add(new Point3(cell.X + 1, cell.Y, cell.Z));
                            break;
                        case 2:
                            neighbors.Add(new Point3(cell.X, cell.Y + 1, cell.Z));
                            break;
                        case 3:
                            neighbors.Add(new Point3(cell.X - 1, cell.Y, cell.Z));
                            break;
                        case 4:
                            neighbors.Add(new Point3(cell.X, cell.Y - 1, cell.Z ^ 1));
                            break;
                        case 5:
                            neighbors.Add(new Point3(cell.X + 1, cell.Y, cell.Z ^ 1));
                            break;
                        case 6:
                            neighbors.Add(new Point3(cell.X, cell.Y + 1, cell.Z ^ 1));
                            break;
                        case 7:
                            neighbors.Add(new Point3(cell.X - 1, cell.Y, cell.Z ^ 1));
                            break;
                    }
        }
    }
    class GameMazeDuplex : Game
    {
        private Point3 player = new Point3();
        private Maze3 maze;
        private List<Point> neighbors = new List<Point>();

        public GameMazeDuplex()
        {
            maze = new Maze3();
        }

        public override void Click(int pictureBoxWidth, int pictureBoxHeight, Point Mouse)
        {
            Point coordinate = GetPosition(pictureBoxWidth, pictureBoxHeight, Mouse, maze.Height, maze.Width);
            if (!isPlaying && coordinate == maze.Start.toPoint())
            {
                UpdateNeighbors(maze.Start);
                player = maze.Start;
                ClickStart();
            }
            else if (isPlaying && player == maze.Finish)
                ClickFinish();
            else if (isPlaying && !isNormalCell(maze.GetCell(player)))
            {
                player.Z = player.Z ^ 1;
                UpdateNeighbors(player);
            }
        }
        public override void Move(int pictureBoxWidth, int pictureBoxHeight, Point Mouse)
        {
            if (isPlaying)
            {
                Point coordinate = GetPosition(pictureBoxWidth, pictureBoxHeight, Mouse, maze.Height, maze.Width);
                if (coordinate != player.toPoint())
                {
                    for (int i = 0; i < neighbors.Count; i++)
                        if (neighbors[i] == coordinate)
                        {
                            player = new Point3(neighbors[i], player.Z);
                            UpdateNeighbors(new Point3(neighbors[i], player.Z));
                            return;
                        }
                    isPlaying = false;
                }
            }
        }
        public override Bitmap GetFullImage(int width, int height)
        {
            Point size = sizeImage(width, height, maze.Width, maze.Height);
            return new Bitmap(maze.GetImage(Color.Transparent, line, sfPoint), size.X, size.Y);
        }
        public override Bitmap GetImage(int width, int height)
        {
            Point size = sizeImage(width, height, maze.Width, maze.Height);
            Bitmap bmp = new Bitmap(size.X, size.Y);
            Graphics g = Graphics.FromImage(bmp);
            float Step = (float)(size.X * 1.0 / maze.Width);
            if (!isPlaying)
                g.FillRectangle(new SolidBrush(sfPoint), maze.Start.X * Step, maze.Start.Y * Step, Step - 1, Step - 1);
            else
            {
                if (player == maze.Finish)
                    g.DrawImage(new Bitmap(GetBlock(sfPoint, isNormalCell(maze.GetCell(maze.Finish))), (int)Step - 1, (int)Step - 1),
                        maze.Finish.X * Step, maze.Finish.Y * Step);
                else g.DrawImage(new Bitmap(GetBlock(line, isNormalCell(maze.GetCell(player))), (int)Step - 1, (int)Step - 1),
                    player.X * Step, player.Y * Step);
                for (int i = 0; i < neighbors.Count; i++)
                    if (new Point3(neighbors[i], player.Z) == maze.Finish)
                        g.DrawImage(new Bitmap(GetBlock(sfPoint, isNormalCell(maze.GetCell(maze.Finish))), (int)Step - 1, (int)Step - 1),
                            maze.Finish.X * Step, maze.Finish.Y * Step);
                    else g.DrawImage(new Bitmap(GetBlock(line, isNormalCell(maze.GetCell(new Point3(neighbors[i], player.Z)))), (int)Step - 1, (int)Step - 1),
                        neighbors[i].X * Step, neighbors[i].Y * Step);
            }
            return bmp;
        }
        public override void NewGenerate()
        {
            maze = new Maze3();
        }

        private void UpdateNeighbors(Point3 cell)
        {
            neighbors.Clear();
            byte t = maze.field[cell.X, cell.Y, cell.Z];
            if ((t & 8) != 0) neighbors.Add(new Point(cell.X - 1, cell.Y));
            if ((t & 4) != 0) neighbors.Add(new Point(cell.X, cell.Y + 1));
            if ((t & 2) != 0) neighbors.Add(new Point(cell.X + 1, cell.Y));
            if ((t & 1) != 0) neighbors.Add(new Point(cell.X, cell.Y - 1));
        }
        private Bitmap GetBlock(Color line, bool Normal)
        {
            Bitmap bmp = new Bitmap(100, 100);
            Graphics g = Graphics.FromImage(bmp);
            if (Normal)
                g.FillRectangle(new SolidBrush(line), 0, 0, 100, 100);
            else
            {
                g.FillRectangle(new SolidBrush(line), 0, 0, 100, 20);
                g.FillRectangle(new SolidBrush(line), 0, 80, 100, 20);
                g.FillRectangle(new SolidBrush(line), 0, 0, 20, 100);
                g.FillRectangle(new SolidBrush(line), 80, 0, 20, 100);
            }
            return bmp;
        }
        private bool isNormalCell(byte cell)
        {
            return (cell & 1 << 4) == 0;
        }
    }
}