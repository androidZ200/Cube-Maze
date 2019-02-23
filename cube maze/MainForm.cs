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
        GameMaze GAME = new GameMaze();

        public MainForm()
        {
            InitializeComponent();
            GAME.Win += GAME_Win;
            pictureBox1.Image = GAME.GetImage(pictureBox1.Width, pictureBox1.Height);
        }

        private void GAME_Win()
        {
            pictureBox1.Image = GAME.GetFullImage(pictureBox1.Width, pictureBox1.Height);
            MessageBox.Show("Победа!");
            pictureBox1.Image = GAME.GetImage(pictureBox1.Width, pictureBox1.Height);
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            GAME.Move(GAME.GetPosition(pictureBox1.Width, pictureBox1.Height, e.Location));
            pictureBox1.Image = GAME.GetImage(pictureBox1.Width, pictureBox1.Height);
        }
        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            GAME.Click(GAME.GetPosition(pictureBox1.Width, pictureBox1.Height, e.Location));
            pictureBox1.Image = GAME.GetImage(pictureBox1.Width, pictureBox1.Height);
        }
        private void pictureBox1_SizeChanged(object sender, EventArgs e)
        {
            pictureBox1.Image = GAME.GetImage(pictureBox1.Width, pictureBox1.Height);
        }
    }
}