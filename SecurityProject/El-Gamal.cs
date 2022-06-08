using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SecurityProject
{
    public partial class El_Gamal : Form
    {
        public El_Gamal()
        {
            InitializeComponent();
        }

        public (byte[], byte[]) CipherMessage()
        {
            BigInteger q = BigInteger.Parse(richTextBox1.Text);
            BigInteger a = BigInteger.Parse(richTextBox2.Text);
            BigInteger Ya = BigInteger.Parse(richTextBox3.Text);

            //Bob(Cipher)
            byte[] msg = Encoding.ASCII.GetBytes(richTextBox4.Text);

            var hexString = BitConverter.ToString(msg);
            hexString = hexString.Replace("-", "");
            string binaryString = String.Join(String.Empty,
            hexString.Select(c => Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0')));


            BigInteger M = BigInteger.Parse(binaryString);


            //BigInteger M = rnd.Next(1, (int)q); //plainText
            BigInteger Ksmall = BigInteger.Parse("74751729786219471113700411646762466406536193945869"); //k small
            BigInteger Kbig = BigInteger.ModPow(Ya, Ksmall, q);
            BigInteger C1 = BigInteger.ModPow(a, Ksmall, q);
            BigInteger KM = BigInteger.Multiply(Kbig, M);
            BigInteger C2 = BigInteger.ModPow(KM, 1, q);

            richTextBox5.Text = C1.ToString();
            richTextBox6.Text = C2.ToString();

            byte[] c1 = BigIntegerToByte(C1);
            byte[] c2 = BigIntegerToByte(C2);

            return (c1, c2);
        }

        public byte[] BigIntegerToByte(BigInteger number)
        {
            string text = number.ToString();
            byte[] bytes = Encoding.ASCII.GetBytes(text);
            return bytes;
        }

        public void DecipherGAMALMessage()
        {
            BigInteger q = BigInteger.Parse(richTextBox7.Text);
            BigInteger a = BigInteger.Parse(richTextBox8.Text);

            //Alex(generation)
            BigInteger Xa = BigInteger.Parse(richTextBox9.Text);/* rnd.Next((int)q - 1); //privateKey(Alex)*/

            BigInteger C1 = BigInteger.Parse(richTextBox10.Text);
            BigInteger C2 = BigInteger.Parse(richTextBox11.Text);

            //Alice(Decipher)
            BigInteger AK = BigInteger.ModPow(C1, Xa, q);
            BigInteger Kinverse = BigInteger.ModPow(AK, q - 2, q);
            BigInteger AM = BigInteger.Multiply(C2, Kinverse);
            AM = BigInteger.ModPow(AM, 1, q);
            string test = AM.ToString();

            int mod4Len = test.Length % 8;
            if (mod4Len != 0)
            {
                // pad to length multiple of 8
                test = test.PadLeft(((test.Length / 8) + 1) * 8, '0');
            }

            int numOfBytes = test.Length / 8;
            byte[] bytes = new byte[numOfBytes];
            for (int i = 0; i < numOfBytes; ++i)
            {
                bytes[i] = Convert.ToByte(test.Substring(8 * i, 8), 2);
            }
            richTextBox12.Text = System.Text.Encoding.ASCII.GetString(bytes);
        }
        private void richTextBox8_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            byte[] c1 = null, c2 = null;
            (c1, c2) = CipherMessage();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DecipherGAMALMessage();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            El_Gamal.ActiveForm.Close();
        }
    }
}
