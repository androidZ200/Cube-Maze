using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace cube_maze
{
    interface IMaze
    {
        byte GetCell(int x, int y, int z);
        Bitmap GetImage(Color BackGround, Color Line, Color SFPoibt);
        int Height { get; }
        int Width { get; }
        Point3 Finish { get; }
        Point3 Start { get; }
    }
}
