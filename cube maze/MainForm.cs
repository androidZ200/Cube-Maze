﻿using System;
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
        Color[] LightColor = { Color.Green, Color.Red, Color.Blue, Color.Orange, Color.Purple };
        Color Background = Color.FromArgb(0xed, 0xee, 0xf0);
        Random rand = new Random();

        public MainForm()
        {
            InitializeComponent();

            button1.FlatAppearance.BorderSize = 0;
            button2.FlatAppearance.BorderSize = 0;
            button3.FlatAppearance.BorderSize = 0;
            button1.FlatStyle = FlatStyle.Flat;
            button2.FlatStyle = FlatStyle.Flat;
            button3.FlatStyle = FlatStyle.Flat;
        }

        private void SetTheme(bool Light)
        {
            Action<Color, Color> action = (Color Background, Color Text) =>
            {

                button1.ForeColor = Text;
                button2.ForeColor = Text;
                button3.ForeColor = Text;
                checkBox1.ForeColor = Text;

                button1.BackColor = Background;
                button2.BackColor = Background;
                button3.BackColor = Background;
            };
            if (Light)
            {
                BackColor = Color.FromArgb(0xed, 0xee, 0xf0);
                Background = Color.FromArgb(0xed, 0xee, 0xf0);
                action(Color.FromArgb(0xdd, 0xde, 0xe0), Color.Black);
            }
            else
            {
                BackColor = Color.FromArgb(0x22, 0x22, 0x26);
                Background = Color.FromArgb(0x22, 0x22, 0x26);
                action(Color.FromArgb(0x33, 0x33, 0x36), Color.White);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Game t = new GameMazeNormal();
            t.sfPoint = LightColor[rand.Next(LightColor.Length)];
            GameForm form = new GameForm(t, Background);
            form.Show();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            Game t = new GameMazeCyclical();
            t.sfPoint = LightColor[rand.Next(LightColor.Length)];
            GameForm form = new GameForm(t, Background);
            form.Show();
        }
        private void button3_Click(object sender, EventArgs e)
        {
            Game t = new GameMazeDuplex();
            t.sfPoint = LightColor[rand.Next(LightColor.Length)];
            GameForm form = new GameForm(t, Background);
            form.Show();
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            SetTheme(!checkBox1.Checked);
        }
    }
}