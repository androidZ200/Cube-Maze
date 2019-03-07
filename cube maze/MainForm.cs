using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace cube_maze
{
    public partial class MainForm : Form
    {
        Game GAME;
        Color[] LightColor = { Color.Green, Color.Red, Color.Blue, Color.Orange, Color.Purple };
        bool LightTheme = true;
        Random rand = new Random();

        public MainForm()
        {
            InitializeComponent();
            NewGenerate();
            SetTheme(LightTheme);
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
            GAME.sfPoint = LightColor[rand.Next(LightColor.Length)];
            Text = "";
            using (FileStream input = new FileStream("save.txt", FileMode.Create))
            {
                var t = GAME.GetSave();
                input.Write(t, 0, t.Length);
            }

        }
        private void SetTheme(bool Light)
        {
            Action<Color, Color> action = (Color Background, Color Text) =>
            {
                radioButton1.ForeColor = Text;
                radioButton2.ForeColor = Text;
                radioButton3.ForeColor = Text;

                radioButton1.BackColor = Background;
                radioButton2.BackColor = Background;
                radioButton3.BackColor = Background;

                radioButton1.FlatAppearance.BorderSize = 0;
                radioButton2.FlatAppearance.BorderSize = 0;
                radioButton3.FlatAppearance.BorderSize = 0;

                radioButton1.FlatStyle = FlatStyle.Flat;
                radioButton2.FlatStyle = FlatStyle.Flat;
                radioButton3.FlatStyle = FlatStyle.Flat;
            };
            if (Light)
            {
                this.BackColor = Color.FromArgb(0xed, 0xee, 0xf0);
                pictureBox1.BackColor = Color.FromArgb(0xed, 0xee, 0xf0);
                action(Color.FromArgb(0xdd, 0xde, 0xe0), Color.Black);
            }
            else
            {
                this.BackColor = Color.FromArgb(0x22, 0x22, 0x26);
                pictureBox1.BackColor = Color.FromArgb(0x22, 0x22, 0x26);
                action(Color.FromArgb(0x33, 0x33, 0x36), Color.White);
            }
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
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            LightTheme = !LightTheme;
            SetTheme(LightTheme);
        }
    }
}