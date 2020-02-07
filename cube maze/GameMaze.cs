using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Drawing;
using System.Diagnostics;

namespace cube_maze
{
    public abstract class Game
    {
        protected Maze maze;
        protected Stopwatch StartTime;
        protected bool isPlaying = false;
        protected bool firstGame = true;
        public event Action Win;
        public TimeSpan Time { get; protected set; }
        public Color line { get; set; } = Color.Gray;
        public Color sfPoint { get; set; } = Color.Green;

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
        protected abstract void Click(Point coord);
        protected abstract void Move(Point coord);
        public abstract Bitmap GetImage(int width, int height);
        public Bitmap GetFullImage(int width, int height)
        {
            Size size = sizeImage(width, height);
            return new Bitmap(maze.GetImage(line, sfPoint), size);
        }
        public abstract void NewGenerate();
        public abstract byte[] GetSave();

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
        protected Size sizeImage(int currentWidth, int currentHeight)
        {
            double XYmaze = maze.Width * 1.0 / maze.Height;
            double XYpicture = currentWidth * 1.0 / currentHeight;
            if (XYpicture > XYmaze)
                return new Size((int)(currentHeight * XYmaze), currentHeight);
            return new Size(currentWidth, (int)(currentWidth / XYmaze));
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
        protected Bitmap DrawField(Size imageSize, List<Point> neighbors, Point player, Point Finish, Point Start)
        {
            Bitmap bmp = new Bitmap(imageSize.Width, imageSize.Height);
            Graphics g = Graphics.FromImage(bmp);
            double Step = imageSize.Width * 1.0 / maze.Width;
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
        private MazeNormal maze;
        private List<Point> neighbors = new List<Point>();

        public GameMazeNormal()
        {
            base.maze = maze = new MazeNormal();
        }

        protected override void Click(Point coord)
        {
            if (!isPlaying && coord == maze.Start)
            {
                UpdateNeighbors(maze.Start);
                player = maze.Start;
                ClickStart();

            }
            else if (isPlaying && player == maze.Finish)
                ClickFinish();
        }
        protected override void Move(Point coord)
        {
            if (isPlaying)
            {
                if (coord != player)
                {
                    for (int i = 0; i < neighbors.Count; i++)
                        if (neighbors[i] == coord)
                        {
                            player = neighbors[i];
                            UpdateNeighbors(neighbors[i]);
                            return;
                        }
                    isPlaying = false;
                }
            }
        }
        public override Bitmap GetImage(int width, int height)
        {
            return DrawField(sizeImage(width, height), neighbors, player, maze.Finish, maze.Start);
        }
        public override void NewGenerate()
        {
            maze = new MazeNormal();
        }
        public override byte[] GetSave()
        {
            byte[] save = new byte[maze.Width * maze.Height + 1];
            int counter = 0;
            save[counter++] = (byte)maze.Width;
            save[0] |= (0 << 6);
            for (int j = 0; j < maze.Height; j++)
                for (int i = 0; i < maze.Width; i++)
                    save[counter++] = maze.maze[i, j];
            return save;
        }

        private void UpdateNeighbors(Point cell)
        {
            neighbors = maze.GetNeighbors(cell);
        }
    }
    class GameMazeCyclical : Game
    {
        private Point3 player = new Point3();
        private MazeCyclical maze;
        private List<Point3> neighbors = new List<Point3>();

        public GameMazeCyclical()
        {
            base.maze = maze = new MazeCyclical();
        }

        protected override void Click(Point coord)
        {
            if (!isPlaying && new Point3(coord, 0) == maze.Start)
            {
                UpdateNeighbors(maze.Start);
                player = maze.Start;
                ClickStart();
            }
            else if (isPlaying && player == maze.Finish)
                ClickFinish();
        }
        protected override void Move(Point coord)
        {
            if (isPlaying)
            {
                if (coord != player.toPoint())
                {
                    for (int i = 0; i < neighbors.Count; i++)
                        if (neighbors[i].toPoint() == coord)
                        {
                            player = neighbors[i];
                            UpdateNeighbors(neighbors[i]);
                            return;
                        }
                    isPlaying = false;
                }
            }
        }
        public override Bitmap GetImage(int width, int height)
        {
            List<Point> neighbors = new List<Point>();
            Point Finish = new Point(-1, 0);
            for (int i = 0; i < this.neighbors.Count; i++)
            {
                if (this.neighbors[i] == maze.Finish) Finish = maze.Finish.toPoint();
                neighbors.Add(this.neighbors[i].toPoint());
            }
            if (player == maze.Finish) Finish = maze.Finish.toPoint();
            return DrawField(sizeImage(width, height), neighbors, player.toPoint(), Finish, maze.Start.toPoint());
        }
        public override void NewGenerate()
        {
            maze = new MazeCyclical();
        }
        public override byte[] GetSave()
        {
            byte[] save = new byte[maze.Width * maze.Height * 2 + 1];
            int counter = 0;
            save[counter++] = (byte)maze.Width;
            save[0] |= (1 << 6); //метка лаюиринта
            for(int z = 0; z < 2; z++)
            for (int j = 0; j < maze.Height; j++)
                for (int i = 0; i < maze.Width; i++)
                    save[counter++] = maze.field[i, j, z];
            return save;
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
        private MazeDuplex maze;
        private List<Point> neighbors = new List<Point>();

        public GameMazeDuplex()
        {
            base.maze = maze = new MazeDuplex();
        }

        protected override void Click(Point coord)
        {
            if (!isPlaying && coord == maze.Start.toPoint())
            {
                UpdateNeighbors(maze.Start);
                player = new Point3(maze.Start);
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
        protected override void Move(Point coord)
        {
            if (isPlaying)
            {
                if (coord != player.toPoint())
                {
                    for (int i = 0; i < neighbors.Count; i++)
                        if (neighbors[i] == coord)
                        {
                            player = new Point3(neighbors[i], player.Z);
                            UpdateNeighbors(new Point3(neighbors[i], player.Z));
                            return;
                        }
                    isPlaying = false;
                }
            }
        }
        public override Bitmap GetImage(int width, int height)
        {
            Size size = sizeImage(width, height);
            Bitmap bmp = new Bitmap(size.Width, size.Height);
            Graphics g = Graphics.FromImage(bmp);
            float Step = (float)(size.Width * 1.0 / maze.Width);
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
            maze = new MazeDuplex();
        }
        public override byte[] GetSave()
        {
            byte[] save = new byte[maze.Width * maze.Height * 2 + 1];
            int counter = 0;
            save[counter++] = (byte)maze.Width;
            save[0] |= (2 << 6); //метка лаюиринта
            for (int z = 0; z < 2; z++)
                for (int j = 0; j < maze.Height; j++)
                    for (int i = 0; i < maze.Width; i++)
                        save[counter++] = maze.field[i, j, z];
            return save;
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
    class GameMazeAbstract : Game
    {
        private Point player = new Point();
        private MazeAbstract maze;
        private List<Point> neighbors = new List<Point>();

        public GameMazeAbstract()
        {
            base.maze = maze = new MazeAbstract();
        }

        protected override void Click(Point coord)
        {
            if (!isPlaying && coord == maze.Start)
            {
                UpdateNeighbors(maze.Start);
                player = maze.Start;
                ClickStart();
            }
            else if (isPlaying && player == maze.Finish)
                ClickFinish();
            else
                isPlaying = false;
        }
        protected override void Move(Point coord)
        {
            if (isPlaying)
            {
                if (coord != player)
                {
                    for (int i = 0; i < neighbors.Count; i++)
                        if (neighbors[i] == coord)
                        {
                            player = neighbors[i];
                            UpdateNeighbors(neighbors[i]);
                            return;
                        }
                }
            }
        }
        public override Bitmap GetImage(int width, int height)
        {
            return DrawField(sizeImage(width, height), neighbors, player, maze.Finish, maze.Start);
        }
        public override byte[] GetSave()
        {
            return new byte[0];
        }
        public override void NewGenerate()
        {
            maze = new MazeAbstract();
        }

        private void UpdateNeighbors(Point cell)
        {
            neighbors = maze.GetNeighbors(cell);
        }
    }
}