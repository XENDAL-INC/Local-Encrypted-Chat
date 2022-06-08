using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;

namespace SecurityProject
{
    [Serializable()]
    public class imgClass
    {
        [Serializable()]
        public class gamalBlock
        {
            public List<BigInteger> gblock = new List<BigInteger>();
            public BigInteger c1;

            public byte[] Serialize(gamalBlock obj)
            {
                BinaryFormatter bf = new BinaryFormatter();
                using (var ms = new MemoryStream())
                {
                    bf.Serialize(ms, obj);
                    return ms.ToArray();
                }
            }
            public gamalBlock Deserialize(byte[] arrBytes)
            {
                MemoryStream memStream = new MemoryStream();
                BinaryFormatter binForm = new BinaryFormatter();
                memStream.Write(arrBytes, 0, arrBytes.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                gamalBlock obj = (gamalBlock)binForm.Deserialize(memStream);

                return obj;
            }
        }
        
        public List<byte[]> blocks = new List<byte[]>();
        public string encryptionType = "";

        public void addBlock(byte[] block)
        {
            blocks.Add(block);
        }

        public string imgToBin(Image img)
        {
            byte[] arr;
            using (MemoryStream ms = new MemoryStream())
            {
                img.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                arr = ms.ToArray();
            }

            var hexString = BitConverter.ToString(arr);
            hexString = hexString.Replace("-", "");
            string binaryString = String.Join(String.Empty,
            hexString.Select(c => Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0')));

            return binaryString;
        }

        public void ImgBlockingGamal(Image img, int blockSize, string encryptType)
        {
            gamalBlock gamal = new gamalBlock();
            string temp = "";

            string binaryString = imgToBin(img);

            encryptionType = encryptType;

            while (binaryString != "")
            {
                if (temp == "")
                {
                    try
                    {
                        temp = binaryString.Substring(binaryString.Length - blockSize, blockSize);
                    }
                    catch
                    {
                        temp = binaryString;

                        EncryptGamal(gamal, temp);
                        gamal.gblock.Reverse();
                        byte[] block = gamal.Serialize(gamal);

                        addBlock(block);
                        temp = "";
                        binaryString = "";
                        break;
                    }
                }
                else if (temp[0] == '0')
                {
                    while (temp[0] != '1')
                    {
                        temp = temp.Remove(0, 1);
                    }
                }
                else
                {
                    binaryString = binaryString.Substring(0, binaryString.Length - temp.Length);
                    EncryptGamal(gamal, temp);
                    temp = "";
                }
            }
        }

        public void EncryptGamal(gamalBlock gamal, string plainText)
        {
            BigInteger q = BigInteger.Parse("263870583091513976470265453625581739161047917466186865871406525323320802806187252865545085006417466380812152088704661167660962900072203444590934444646524287016361683158718994906997986116863144603022819774542139001172209730315504820569783667470476337725096331034396362280082992910359926464615087952107");
            BigInteger a = BigInteger.Parse("972786759073880157450128634013863227859718995243670813749754531661639796562745939091684670527649807468135246320228161197083645642474430573260100103054695904036965578089405756866828153890077040758719543785791050848076048536002887539831769648262681681513996402657001694358435296773625016921888521706471");
            BigInteger Xa = BigInteger.Parse("17820880237161200882434641825747158061340044178561");/* rnd.Next((int)q - 1); //privateKey(Alex)*/
            BigInteger Ya = BigInteger.ModPow(a, Xa, q); //publicKey(Alex)

            BigInteger Ksmall = BigInteger.Parse("74751729786219471113700411646762466406536193945869"); //k small
            BigInteger Kbig = BigInteger.ModPow(Ya, Ksmall, q);
            gamal.c1 = BigInteger.ModPow(a, Ksmall, q);
            BigInteger M = BigInteger.Parse(plainText);

            BigInteger KM = BigInteger.Multiply(Kbig, M);
            gamal.gblock.Add(BigInteger.ModPow(KM, 1, q));
        }

        public void DecryptGamal(byte[] msg)
        {
            BigInteger q = BigInteger.Parse("263870583091513976470265453625581739161047917466186865871406525323320802806187252865545085006417466380812152088704661167660962900072203444590934444646524287016361683158718994906997986116863144603022819774542139001172209730315504820569783667470476337725096331034396362280082992910359926464615087952107");
            BigInteger a = BigInteger.Parse("972786759073880157450128634013863227859718995243670813749754531661639796562745939091684670527649807468135246320228161197083645642474430573260100103054695904036965578089405756866828153890077040758719543785791050848076048536002887539831769648262681681513996402657001694358435296773625016921888521706471");
            BigInteger Xa = BigInteger.Parse("17820880237161200882434641825747158061340044178561");/* rnd.Next((int)q - 1); //privateKey(Alex)*/
            BigInteger Ya = BigInteger.ModPow(a, Xa, q); //publicKey(Alex)

            gamalBlock gamal = new gamalBlock();
            gamal = gamal.Deserialize(msg);
            
            //Alice(Decipher)
            BigInteger AK = BigInteger.ModPow(gamal.c1, Xa, q);
            BigInteger Kinverse = BigInteger.ModPow(AK, q - 2, q);
            string test = "";
            for (int i = 0; i < gamal.gblock.Count; i++)
            {
                gamal.gblock[i] = BigInteger.Multiply(gamal.gblock[i], Kinverse);
                gamal.gblock[i] = BigInteger.ModPow(gamal.gblock[i], 1, q);
                test += gamal.gblock[i].ToString();
            }

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

            using (Image image = Image.FromStream(new MemoryStream(bytes)))
            {
                image.Save("outputIMG.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);  // Or Png
            }
        }

        public byte[] Encrypt(string temp, string encryptType)
        {
            byte[] block = new byte[1024];
            /*if (encryptType == "El-Gamal")
            {
                Gamal gamal = new Gamal();
                block = gamal.Encrypt(gamal, temp, true);
            }*/
            if (encryptType == "RSA")
            {
                RSA rsa = new RSA();
                block = rsa.Encrypt(rsa, temp);
            }
            else if (encryptType == "DES")
            {
                DES des = new DES();
                block = des.EncryptionDES(temp, true);
            }
            else if (encryptType == "AES")
            {
                AES aes = new AES();
                block = aes.Encrypt(temp, true);
            }

            return block;
        }

        public string Decrypt(byte[] block)
        {
            string decryptedMsg = "";
            /*if (encryptionType == "El-Gamal")
            {
                Gamal gamal = new Gamal();
                decryptedMsg = gamal.Decrypt(gamal, block);
            }*/
            if (encryptionType == "RSA")
            {
                RSA rsa = new RSA();
                decryptedMsg = rsa.Decrypt(rsa, block);
            }
            else if (encryptionType == "DES")
            {
                DES des = new DES();
                decryptedMsg = des.DecryptionDES(block, true);
            }
            else if (encryptionType == "AES")
            {
                AES aes = new AES();
                decryptedMsg = aes.Decrypt(block, true);
            }

            return decryptedMsg;
        }

        public void DecryptImage()
        {
            string test = "";
            for (int i = 0; i < blocks.Count; i++)
            {

                string blockDecrypt = Decrypt(blocks[i]);
                test += blockDecrypt;
            }

            int mod4Len = test.Length % 8;
            if (mod4Len != 0)
            {
                // pad to length multiple of 8
                test = test.PadLeft(((test.Length / 8) + 1) * 8, '0');
            }

            int numOfBytes = (test.Length / 8) - 12;
            byte[] bytes = new byte[numOfBytes];
            for (int i = 0; i < numOfBytes; ++i)
            {
                bytes[i] = Convert.ToByte(test.Substring(8 * i, 8), 2);
            }

            using (Image image = Image.FromStream(new MemoryStream(bytes)))
            {
                image.Save("outputIMG.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);  // Or Png
            }
        }

        public byte[] Serialize(imgClass obj)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }
        public imgClass Deserialize(byte[] arrBytes)
        {
            MemoryStream memStream = new MemoryStream();
            BinaryFormatter binForm = new BinaryFormatter();
            memStream.Write(arrBytes, 0, arrBytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            imgClass obj = (imgClass)binForm.Deserialize(memStream);

            return obj;

        }

        public List<byte[]> packageBlocking(byte[] arr)
        {
            List<byte[]> package = new List<byte[]>();
            int arrLength = arr.Length;
            int i = 0, j = 0, index = 0;
            int size = (arrLength / 1000) + 1;
            byte[] pack = new byte[1000];
            while(j<arrLength)
            {
                if(i%1000 == 0 && i!=0)
                {
                    packageBlock block = new packageBlock();
                    block.id = index; block.package = pack;
                    block.size = size; block.type = "image";
                    byte[] serialBlock = block.Serialize(block);
                    package.Add(serialBlock);

                    pack = new byte[1000];
                    index++;
                    i = 0;
                }
                else if(j>=arrLength-1)
                {
                    packageBlock block = new packageBlock();
                    byte[] lastPack = new byte[i+1];
                    for (int h = 0; h <= i; h++)
                    {
                        lastPack[h] = pack[h];
                    }
                    lastPack[i] = arr[arrLength-1];
                    block.id = index; block.package = lastPack;
                    block.type = "image";
                    byte[] serialBlock = block.Serialize(block);
                    package.Add(serialBlock);
                    index++;
                    i = 0;
                    j++;
                }
                else
                {
                    pack[i] = arr[j];
                    i++; j++;
                }
                
            }
            return package;
            
        }
    }
}
