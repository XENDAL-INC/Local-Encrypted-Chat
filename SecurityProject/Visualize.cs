using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SecurityProject
{
    public partial class Visualize : Form
    {
        public Visualize()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)//GAMAL
        {
            El_Gamal gamal = new El_Gamal();
            gamal.Show();
        }

        private void button2_Click(object sender, EventArgs e)//RSA
        {
            RSA_manual rsa = new RSA_manual();
            rsa.Show();
        }

        private void button3_Click(object sender, EventArgs e)//DES
        {
            DES_manual des = new DES_manual();
            des.Show();
        }

        private void button4_Click(object sender, EventArgs e)//AES
        {
            AES_manual aes = new AES_manual();
            aes.Show();
        }

        private void button5_Click(object sender, EventArgs e)//Diffie-Hellman
        {
            DiffieHellman diffie = new DiffieHellman();
            diffie.Show();
        }
    }
}
