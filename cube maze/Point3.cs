using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cube_maze
{
    class Point3
    {
        public int X, Y, Z;

        public Point3()
        {
            X = Y = Z = 0;
        }
        public Point3(int X, int Y, int Z)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
        }
        public Point3(Point other)
        {
            X = other.X;
            Y = other.Y;
            Z = 0;
        }
        public Point3(Point other, int Z)
        {
            X = other.X;
            Y = other.Y;
            this.Z = Z;
        }
        public Point3(Point3 other)
        {
            X = other.X;
            Y = other.Y;
            Z = other.Z;
        }

        public static bool operator ==(Point3 left, Point3 right)
        {
            return left.X == right.X && left.Y == right.Y && left.Z == right.Z;
        }
        public static bool operator !=(Point3 left, Point3 right)
        {
            return !(left == right);
        }

        public Point toPoint()
        {
            return new Point(X, Y);
        }
    }
}