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
        Game GAME;
        Color[] colors = { Color.Green, Color.Red, Color.Blue, Color.Orange, Color.Purple };
        Random rand = new Random();

        public MainForm()
        {
            InitializeComponent();
            NewGenerate();
            pictureBox1.Image = GAME.GetImage(pictureBox1.Width, pictureBox1.Height);
        }

        private void GAME_Win()
        {
            pictureBox1.Image = GAME.GetFullImage(pictureBox1.Width, pictureBox1.Height);
            MessageBox.Show("Победа!\nВремя прохождения: " + GAME.Time);
            Text = "Идёт загрузка... Пожалуйста подождите";
            GAME.NewGenerate();
            pictureBox1.Image = GAME.GetImage(pictureBox1.Width, pictureBox1.Height);
            Text = "";
        }
        private void NewGenerate()
        {
            Text = "Идёт загрузка... Пожалуйста подождите";
            if (radioButton1.Checked) GAME = new GameMazeNormal();
            else if (radioButton2.Checked) GAME = new GameMazeCyclical();
            else if (radioButton3.Checked) GAME = new GameMazeDuplex();
            GAME.Win += GAME_Win;
            GAME.sfPoint = colors[rand.Next(colors.Length)];
            Text = "";
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            GAME.Move(pictureBox1.Width, pictureBox1.Height, e.Location);
            pictureBox1.Image = GAME.GetImage(pictureBox1.Width, pictureBox1.Height);
            GC.Collect();
        }
        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            GAME.Click(pictureBox1.Width, pictureBox1.Height, e.Location);
            pictureBox1.Image = GAME.GetImage(pictureBox1.Width, pictureBox1.Height);
        }
        private void pictureBox1_SizeChanged(object sender, EventArgs e)
        {
            if (pictureBox1.Width != 0)
                pictureBox1.Image = GAME.GetImage(pictureBox1.Width, pictureBox1.Height);
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            NewGenerate();
            pictureBox1.Image = GAME.GetImage(pictureBox1.Width, pictureBox1.Height);
        }
    }
}