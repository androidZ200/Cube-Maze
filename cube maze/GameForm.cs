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
    public partial class GameForm : Form
    {
        Game GAME;

        public void Win()
        {
            pictureBox1.Image = GAME.GetFullImage();
            MessageBox.Show("Win!\nYour time = " + GAME.Time.ToString());
            Close();
        }

        public GameForm(Game game, Color Background)
        {
            InitializeComponent();
            GAME = game;
            BackColor = Background;
            GAME.Win += Win;
        }
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            GAME.Move(pictureBox1.Width, pictureBox1.Height, e.Location);
            pictureBox1.Image = GAME.GetImage();
            GC.Collect();
        }
        private void pictureBox1_SizeChanged(object sender, EventArgs e)
        {
            if (pictureBox1.Width != 0)
                pictureBox1.Image = GAME.GetImage();
        }
        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            GAME.Click(pictureBox1.Width, pictureBox1.Height, e.Location);
            pictureBox1.Image = GAME.GetImage();
        }
    }
}
