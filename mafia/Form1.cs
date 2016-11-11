using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace mafia
{
    public partial class Form1 : Form
    {
        List<Player> players = new List<Player>();
        public Form1()
        {
            InitializeComponent();
            players.Add(new Player() {Nick = "Dimon", Trump = Type.Citizen});
            players.Add(new Player() { Nick = "Denis", Trump = Type.Citizen });

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            var nick = textBox1.Text;

            if (players.FindLast(p => p.Nick == nick) != null)
            {
                if (!listBox1.Items.Contains(nick))
                {
                    listBox1.Items.Add(nick);
                }
                else
                {
                    MessageBox.Show("Already exist: " + nick);
                }
            }
            else
            {
                MessageBox.Show("Not found: " + nick);
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var nick = (string) listBox1.SelectedItem;
            var player = players.FindLast(p => p.Nick == nick);

            if (player != null)
            {
                currPlayer = player;
                label1.Text = "STATA: " + player.Stata;
                checkBox1.Checked = player.IsWinner;
            }
        }

        Player currPlayer;
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            currPlayer.IsWinner = checkBox1.Checked;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            foreach (var player in players)
            {
                if (player.IsWinner)
                ++player.Stata;
            }
            
        }
    }
}
