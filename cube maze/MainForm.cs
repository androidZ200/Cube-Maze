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
        Color[] colors = { Color.Green, Color.Red, Color.Blue, Color.Orange, Color.Purple };
        Random rand = new Random();

        public MainForm()
        {
            InitializeComponent();
            NewGenerate();
            GAME.Win += GAME_Win;
            pictureBox1.Image = GAME.GetImage(pictureBox1.Width, pictureBox1.Height);
        }

        private void GAME_Win()
        {
            pictureBox1.Image = GAME.GetFullImage(pictureBox1.Width, pictureBox1.Height);
            MessageBox.Show("Победа!\nВремя прохождения: " + GAME.Time);
            pictureBox1.Image = GAME.GetImage(pictureBox1.Width, pictureBox1.Height);
            NewGenerate();
        }
        private void NewGenerate()
        {
            Text = "Идёт загрузка... Пожалуйста подождите";
            if (radioButton1.Checked) GAME.NewGenerate(new Labyrinth());
            else if (radioButton2.Checked) GAME.NewGenerate(new Maze2());
            GAME.sfPoint = colors[rand.Next(colors.Length)];
            Text = "";
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            GAME.Move(GAME.GetPosition(pictureBox1.Width, pictureBox1.Height, e.Location));
            pictureBox1.Image = GAME.GetImage(pictureBox1.Width, pictureBox1.Height);
            GC.Collect();
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

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            NewGenerate();
        }
    }
}