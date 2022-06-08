using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Numerics;

namespace SecurityProject
{
    public partial class DiffieHellman : Form
    {
        public DiffieHellman()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)//generate publicKey
        {
            BigInteger q = BigInteger.Parse(richTextBox1.Text);
            BigInteger a = BigInteger.Parse(richTextBox2.Text);
            BigInteger Xa = BigInteger.Parse(richTextBox3.Text);

            BigInteger Ya = BigInteger.ModPow(a, Xa, q);
            richTextBox4.Text = Ya.ToString();
        }

        private void button2_Click(object sender, EventArgs e)//generate SessionKey
        {
            BigInteger q = BigInteger.Parse(richTextBox8.Text);

            BigInteger Xa = BigInteger.Parse(richTextBox6.Text);
            BigInteger Yb = BigInteger.Parse(richTextBox7.Text);

            BigInteger Kab = BigInteger.ModPow(Yb, Xa, q); //sessionKey(dephi-hellman)

            richTextBox5.Text = Kab.ToString();
        }
    }
}
