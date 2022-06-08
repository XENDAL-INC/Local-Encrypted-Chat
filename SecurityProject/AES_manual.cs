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
    public partial class AES_manual : Form
    {
        public AES_manual()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AES aes = new AES();
            richTextBox3.Text = aes.AES_ENC(richTextBox2.Text, richTextBox1.Text, false);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            AES aes = new AES();
            richTextBox5.Text = aes.AES_DEC(richTextBox4.Text, richTextBox1.Text, false);
        }
    }
}
