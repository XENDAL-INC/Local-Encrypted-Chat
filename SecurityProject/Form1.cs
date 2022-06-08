using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using SecurityProject;
using System.IO;

namespace SecurityProject
{
    public partial class Form1 : Form
    {
        Socket sck;
        EndPoint epLocal, epRemote;
        int count = 0, packSize = 2;
        List<packageBlock> packs = new List<packageBlock>();
        public Form1()
        {
            InitializeComponent();
            sck = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            sck.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            textLocalIp.Text = GetLocalIP();
            textFriendsIp.Text = GetLocalIP();
        }

        private string GetLocalIP()
        {
            IPHostEntry host;
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            return "127.0.0.1";
        }
        private void MessageCallBack(IAsyncResult aResult)
        {
            try
            {
                int size = sck.EndReceiveFrom(aResult, ref epRemote);
                if (size > 0)
                {
                    byte[] receivedData = new byte[1464];
                    receivedData = (byte[])aResult.AsyncState;
                    string receivedMessage = "";
                    packageBlock package = new packageBlock();
                    package = package.Deserialize(receivedData);

                    //decrypt here (use recievedMessage)
                    if (package.type=="image")//Image
                    {
                        imgClass deserializedTest = new imgClass();

                        packageBlock pack = new packageBlock();
                        pack = pack.Deserialize(receivedData);
                        if (count == 0)
                        {
                            packs.Clear();
                            packs.Add(pack);
                            packSize = pack.size;
                            count++;
                        }
                        else if (count >= packSize - 1)
                        {
                            packs.Add(pack);
                            //sort
                            int limit = (packSize - 1) * 1000 + packs[packSize - 1].package.Length;
                            byte[] img = new byte[limit];
                            int j = 0;
                            for (int i = 0; i < packs.Count; i++)
                            {
                                for (int k = 0; k < packs[i].package.Length; k++)
                                {
                                    img[j] = packs[i].package[k];
                                    j++;
                                }
                            }
                            deserializedTest = deserializedTest.Deserialize(img);

                            if (deserializedTest.encryptionType == "El-Gamal")
                                deserializedTest.DecryptGamal(deserializedTest.blocks[0]);
                            else
                                deserializedTest.DecryptImage();

                            count = 0;
                            listMessage.Items.Add("(" + deserializedTest.encryptionType + ")" + "Friend: sent you an Image.");

                        }
                        else
                        {
                            packs.Add(pack);
                            count++;
                        }
                         
                    }

                    else if (package.encryption == "El-Gamal")//Gamal
                    {
                        Gamal gamal = new Gamal();
                        receivedMessage = gamal.Decrypt(gamal, package.package);
                    }

                    else if (package.encryption == "RSA")//RSA
                    {
                        RSA rsa = new RSA();
                        receivedMessage = rsa.Decrypt(rsa, package.package);
                    }

                    else if (package.encryption == "DES")//DES
                    {
                        DES des = new DES();
                        receivedMessage = des.DecryptionDES(package.package, false);
                    }

                    else if (package.encryption == "AES")//AES
                    {
                        AES aes = new AES();
                        receivedMessage = aes.Decrypt(package.package, false);
                    }

                    if (package.type != "file" && package.type != "image")
                        listMessage.Items.Add("(" + package.encryption + ")" + "Friend: " + receivedMessage);
                    else if (package.type == "file")//SAVE FILE
                    {

                        using (StreamWriter sw = File.CreateText("test.txt"))
                        {
                            sw.WriteLine(receivedMessage);
                        }
                        listMessage.Items.Add("(" + package.encryption + ")" + "Friend: sent you a file.");

                    }
                    if (count==0)//auto scroll down chat
                    {
                        listMessage.SelectedIndex = listMessage.Items.Count - 1;
                        listMessage.SelectedIndex = -1;
                    }
                    
                }
                byte[] buffer = new byte[1500];
                sck.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref epRemote, new AsyncCallback(MessageCallBack), buffer);
                
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.ToString());
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textLocalPort.Text != "" && textFriendsPort.Text != "")
            {
                try
                {
                    // binding socket
                    epLocal = new IPEndPoint(IPAddress.Parse(textLocalIp.Text),
                    Convert.ToInt32(textLocalPort.Text));
                    sck.Bind(epLocal);
                    // connect to remote IP and port
                    epRemote = new IPEndPoint(IPAddress.Parse(textFriendsIp.Text),
                    Convert.ToInt32(textFriendsPort.Text));
                    sck.Connect(epRemote);
                    // starts to listen to an specific port
                    byte[] buffer = new byte[1500];
                    sck.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref epRemote, new
                    AsyncCallback(MessageCallBack), buffer);
                    // release button to send message
                    button2.Enabled = true;//SEND
                    button5.Enabled = true;//IMAGE
                    button4.Enabled = true;//FILE
                    button1.Text = "Connected";
                    button1.Enabled = false;
                    textMessage.Focus();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
            else
            {
                MessageBox.Show("Please fill up the missing PORTS!!!");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Visualize visualize = new Visualize();
            visualize.Show();
        }

        //############################   SENDING IMAGE   ###############################
        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                string imgDirectory = "";


                OpenFileDialog openFileDialog1 = new OpenFileDialog();

                openFileDialog1.InitialDirectory = "c:\\";
                openFileDialog1.Filter = "Image files (*.jpg)|*.jpg";
                openFileDialog1.FilterIndex = 0;
                openFileDialog1.RestoreDirectory = true;

                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    imgDirectory = openFileDialog1.FileName;
                }

                Image img = Image.FromFile(imgDirectory);
                imgClass test = new imgClass();


                string encryptType = "";
                if (comboBox1.SelectedIndex == 0)
                {
                    encryptType = "El-Gamal";
                    test.ImgBlockingGamal(img, 300, encryptType);
                }
                else if (comboBox1.SelectedIndex == 1)
                {
                    encryptType = "RSA";
                    
                }
                else if (comboBox1.SelectedIndex == 2)
                {
                    encryptType = "DES";
                    string binImg = test.imgToBin(img);
                    test.blocks.Add(test.Encrypt(binImg, encryptType));
                    test.encryptionType = encryptType;
                }
                else if (comboBox1.SelectedIndex == 3)
                {
                    encryptType = "AES";
                    string binImg = test.imgToBin(img);
                    test.blocks.Add(test.Encrypt(binImg, encryptType));
                    test.encryptionType = encryptType;
                }

                byte[] msg = test.Serialize(test);
                List<byte[]> package = test.packageBlocking(msg);

                for(int i=0;i<package.Count;i++)
                    sck.Send(package[i]);// sending the message
                listMessage.Items.Add("(" + test.encryptionType + ")" + "You: sent an Image.");

                listMessage.SelectedIndex = listMessage.Items.Count - 1;
                listMessage.SelectedIndex = -1;
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 2;
        }

        private void listMessage_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        //############################   SENDING FILE   ###############################
        private void button4_Click(object sender, EventArgs e)//file
        {
            packageBlock package = new packageBlock();
            byte[] msg;
            string fileDirectory = "";


            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "Text File|*.txt";
            openFileDialog1.Title = "Open Text File";
            openFileDialog1.FilterIndex = 0;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                fileDirectory = openFileDialog1.FileName;
            }
            string plaintext = File.ReadAllText(fileDirectory);
            package.type = "file";

            if (comboBox1.SelectedIndex == 0)//Gamal
            {
                Gamal gamal = new Gamal();
                msg = gamal.Encrypt(gamal, plaintext);
                package.encryption = "El-Gamal";
                package.size = 1;
                package.id = 1;
                package.package = msg;
            }

            if (comboBox1.SelectedIndex == 1)//RSA
            {
                RSA rsa = new RSA();
                msg = rsa.Encrypt(rsa, plaintext);
                package.encryption = "RSA";
                package.size = 1;
                package.id = 1;
                package.package = msg;
            }

            if (comboBox1.SelectedIndex == 2)//DES
            {
                DES des = new DES();
                msg = des.EncryptionDES(plaintext, false);
                package.encryption = "DES";
                package.size = 1;
                package.id = 1;
                package.package = msg;
            }
            if (comboBox1.SelectedIndex == 3)//AES
            {
                AES aes = new AES();
                msg = aes.Encrypt(plaintext, false);
                package.encryption = "AES";
                package.size = 1;
                package.id = 1;
                package.package = msg;
            }

            byte[] msgPack = package.Serialize(package);//packing the msg
            sck.Send(msgPack);// sending the message
            listMessage.Items.Add("(" + package.encryption + ")" + "You: sent file to Friend.");

            listMessage.SelectedIndex = listMessage.Items.Count - 1;
            listMessage.SelectedIndex = -1;
        }

        //############################   SENDING PLAINTEXT   ###############################
        private void button2_Click(object sender, EventArgs e)
        {
            if(textMessage.Text!="")
            {
                try
                {
                    byte[] msg;
                    packageBlock package = new packageBlock();

                    if (comboBox1.SelectedIndex==0)//Gamal
                    {
                        Gamal gamal = new Gamal();
                        msg = gamal.Encrypt(gamal, textMessage.Text);
                        package.encryption = "El-Gamal";
                        package.size = 1;
                        package.id = 1;
                        package.package = msg;
                    }

                    if (comboBox1.SelectedIndex == 1)//RSA
                    {
                        RSA rsa = new RSA();
                        msg = rsa.Encrypt(rsa, textMessage.Text);
                        package.encryption = "RSA";
                        package.size = 1;
                        package.id = 1;
                        package.package = msg;
                    }

                    if (comboBox1.SelectedIndex == 2)//DES
                    {
                        DES des = new DES();
                        msg = des.EncryptionDES(textMessage.Text, false);
                        package.encryption = "DES";
                        package.size = 1;
                        package.id = 1;
                        package.package = msg;
                    }
                    if (comboBox1.SelectedIndex == 3)//AES
                    {
                        AES aes = new AES();
                        msg = aes.Encrypt(textMessage.Text, false);
                        package.encryption = "AES";
                        package.size = 1;
                        package.id = 1;
                        package.package = msg;
                    }

                    byte[] msgPack = package.Serialize(package);//packing the msg
                    sck.Send(msgPack);// sending the message


                    // add to listbox
                    listMessage.Items.Add("(" + package.encryption + ")" + "You: " + textMessage.Text);
                    // clear txtMessage
                    textMessage.Clear();
                    listMessage.SelectedIndex = listMessage.Items.Count - 1;
                    listMessage.SelectedIndex = -1;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
            
        }
    }
}
