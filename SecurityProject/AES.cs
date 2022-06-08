using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace SecurityProject
{
    public class AES
    {
        public byte[] binStringToByte(string test)
        {
            int numOfBytes = test.Length / 8;
            byte[] bytes = new byte[numOfBytes];
            for (int i = 0; i < numOfBytes; ++i)
            {
                bytes[i] = Convert.ToByte(test.Substring(8 * i, 8), 2);
            }
            
            return bytes;
        }
        
        public string bytesToBin(byte[] bytes)
        {
            var hexString = BitConverter.ToString(bytes);
            hexString = hexString.Replace("-", "");
            string binaryString = String.Join(String.Empty,
            hexString.Select(c => Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0')));

            return binaryString;
        }
        public string SessionKeyGeneration()
        {
            BigInteger q = BigInteger.Parse("811625542898199028798761685010661735539490805771693537579527331057313324571568448399462365181797487809911948886255338244497001176540203235940735497429168085634281811232131790596475519535461973753959592314363650044494749470886414557182282939272027164975882401017428707277564380765257706818702936363131");

            BigInteger Xa = BigInteger.Parse("8388089106629031386100099554200373068182096190836051003383095251861768840967721916418379465371310651");

            BigInteger Yb = BigInteger.Parse("230237959934522967741530097375752932445043483760693170154138464628182098627044086247153407788848046772813737159138267907696695119504782409347934163882990113885516559307762800544992449061531788269912661508070321123426623021721801346476665547451961138893687075307093080400871898442263928653910158948572");

            BigInteger sessionKey = BigInteger.ModPow(Yb, Xa, q);

            string key = sessionKey.ToString();

            return key;
        }

        public byte[] Encrypt(string str, bool isBinary)
        {
            string sessionKey = SessionKeyGeneration();
            string encryptedMsg = AES_ENC(str, sessionKey, isBinary);


            if (!isBinary)
            {
                byte[] bytes = Encoding.ASCII.GetBytes(encryptedMsg);

                return bytes;
            }
            else
            {
                byte[] bytes = binStringToByte(encryptedMsg);
                return bytes;
            }
        }

        public string Decrypt(byte[] msg, bool isBinary)
        {
            string cipher = "";
            if (!isBinary)
            {
                ASCIIEncoding eEncoding = new ASCIIEncoding();
                cipher = eEncoding.GetString(msg);
            }
            else
            {
                var hex = BitConverter.ToString(msg);
                hex = hex.Replace("-", "");
                cipher = String.Join(String.Empty,
                hex.Select(c => Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0')));
            }

            cipher = cipher.Replace("\0", "");

            string sessionKey = SessionKeyGeneration();

            string plainText = AES_DEC(cipher, sessionKey, isBinary);
            return plainText;
        }

        public class BitArray
        {
            public static int[] ToBits(int decimalnumber, int numberofbits)
            {
                int[] bitarray = new int[numberofbits];
                int k = numberofbits - 1;
                char[] bd = Convert.ToString(decimalnumber, 2).ToCharArray();

                for (int i = bd.Length - 1; i >= 0; --i, --k)
                {
                    if (bd[i] == '1')
                        bitarray[k] = 1;
                    else
                        bitarray[k] = 0;
                }

                while (k >= 0)
                {
                    bitarray[k] = 0;
                    --k;
                }

                return bitarray;
            }

            public static int ToDecimal(int[] bitsarray)
            {
                string stringvalue = "";
                for (int i = 0; i < bitsarray.Length; i++)
                {
                    stringvalue += bitsarray[i].ToString();
                }
                int DecimalValue = Convert.ToInt32(stringvalue, 2);

                return DecimalValue;
            }
        }

        public string AES_ENC(string input, string privateKey, bool isBinary)
        {
            int[,] sBox = new int[,]
            {{0x63, 0x7c, 0x77, 0x7b, 0xf2, 0x6b, 0x6f, 0xc5, 0x30, 0x01, 0x67, 0x2b, 0xfe, 0xd7, 0xab, 0x76 },
            { 0xca, 0x82, 0xc9, 0x7d, 0xfa, 0x59, 0x47, 0xf0, 0xad, 0xd4, 0xa2, 0xaf, 0x9c, 0xa4, 0x72, 0xc0},
            { 0xb7, 0xfd, 0x93, 0x26, 0x36, 0x3f, 0xf7, 0xcc, 0x34, 0xa5, 0xe5, 0xf1, 0x71, 0xd8, 0x31, 0x15},
            { 0x04, 0xc7, 0x23, 0xc3, 0x18, 0x96, 0x05, 0x9a, 0x07, 0x12, 0x80, 0xe2, 0xeb, 0x27, 0xb2, 0x75},
            { 0x09, 0x83, 0x2c, 0x1a, 0x1b, 0x6e, 0x5a, 0xa0, 0x52, 0x3b, 0xd6, 0xb3, 0x29, 0xe3, 0x2f, 0x84},
            { 0x53, 0xd1, 0x00, 0xed, 0x20, 0xfc, 0xb1, 0x5b, 0x6a, 0xcb, 0xbe, 0x39, 0x4a, 0x4c, 0x58, 0xcf},
            { 0xd0, 0xef, 0xaa, 0xfb, 0x43, 0x4d, 0x33, 0x85, 0x45, 0xf9, 0x02, 0x7f, 0x50, 0x3c, 0x9f, 0xa8},
            { 0x51, 0xa3, 0x40, 0x8f, 0x92, 0x9d, 0x38, 0xf5, 0xbc, 0xb6, 0xda, 0x21, 0x10, 0xff, 0xf3, 0xd2},
            { 0xcd, 0x0c, 0x13, 0xec, 0x5f, 0x97, 0x44, 0x17, 0xc4, 0xa7, 0x7e, 0x3d, 0x64, 0x5d, 0x19, 0x73},
            { 0x60, 0x81, 0x4f, 0xdc, 0x22, 0x2a, 0x90, 0x88, 0x46, 0xee, 0xb8, 0x14, 0xde, 0x5e, 0x0b, 0xdb},
            { 0xe0, 0x32, 0x3a, 0x0a, 0x49, 0x06, 0x24, 0x5c, 0xc2, 0xd3, 0xac, 0x62, 0x91, 0x95, 0xe4, 0x79},
            { 0xe7, 0xc8, 0x37, 0x6d, 0x8d, 0xd5, 0x4e, 0xa9, 0x6c, 0x56, 0xf4, 0xea, 0x65, 0x7a, 0xae, 0x08},
            { 0xba, 0x78, 0x25, 0x2e, 0x1c, 0xa6, 0xb4, 0xc6, 0xe8, 0xdd, 0x74, 0x1f, 0x4b, 0xbd, 0x8b, 0x8a},
            { 0x70, 0x3e, 0xb5, 0x66, 0x48, 0x03, 0xf6, 0x0e, 0x61, 0x35, 0x57, 0xb9, 0x86, 0xc1, 0x1d, 0x9e},
            { 0xe1, 0xf8, 0x98, 0x11, 0x69, 0xd9, 0x8e, 0x94, 0x9b, 0x1e, 0x87, 0xe9, 0xce, 0x55, 0x28, 0xdf},
            { 0x8c, 0xa1, 0x89, 0x0d, 0xbf, 0xe6, 0x42, 0x68, 0x41, 0x99, 0x2d, 0x0f, 0xb0, 0x54, 0xbb, 0x16}};

            List<int> inputAscii = new List<int>();
            string output = "";


            if (!isBinary)
            {
                for (int i = 0; i < input.Length; i++)
                {
                    inputAscii.Add(input[i]);
                }
            }
            else
            {
                byte[] bytes = binStringToByte(input);
                for(int i=0; i<bytes.Length;i++)
                {
                    inputAscii.Add(bytes[i]);
                }
            }
            // Gen the Blocks
            int[,] block = new int[4, 4];
            int blockIndex = 0;
            while (blockIndex < inputAscii.Count)
            {
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 4; j++, blockIndex++)
                    {
                        if (blockIndex < inputAscii.Count)
                        {
                            block[i, j] = inputAscii[blockIndex];
                        }
                        else
                        {
                            block[i, j] = 0;
                        }
                    }
                }

                // Using the SBox
                int row, col;
                float reminder;
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        row = block[i, j] / 16;
                        reminder = (float)block[i, j];
                        reminder = ((reminder / 16) - row) * 16;
                        col = (int)reminder;
                        block[i, j] = sBox[row, col];
                    }
                }

                //Shift Rows
                int temp, t;
                for (int i = 1; i < 4; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        if (i <= 1)
                        {
                            t = j + 1;
                            if (t >= 4)
                            {
                                t = t - 4;
                            }
                            temp = block[1, j];
                            block[1, j] = block[1, t];
                            block[1, t] = temp;
                        }
                        if (i <= 2)
                        {
                            t = j + 1;
                            if (t >= 4)
                            {
                                t = t - 4;
                            }
                            temp = block[2, j];
                            block[2, j] = block[2, t];
                            block[2, t] = temp;
                        }
                        if (i <= 3)
                        {
                            t = j + 1;
                            if (t >= 4)
                            {
                                t = t - 4;
                            }
                            temp = block[3, j];
                            block[3, j] = block[3, t];
                            block[3, t] = temp;
                        }
                    }
                }

                //Key generation and XOR
                int[,] key = new int[4, 4];
                int keyindex = 0;
                BigInteger sessionKey = BigInteger.Parse(privateKey);
                byte[] byteKey = sessionKey.ToByteArray();
                for (int i = 0; i < 4; i++, keyindex++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if (byteKey.Length > keyindex)
                        {
                            key[i, j] = byteKey[keyindex];
                        }
                        else
                        {
                            key[i, j] = 32;
                        }

                    }
                }
                int XOR_Res;
                int[] XOR_Arr = new int[8];
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        int[] Bits_Array_Key = BitArray.ToBits(key[i, j], 8);
                        int[] Bits_Array_PT = BitArray.ToBits(block[i, j], 8);
                        for (int k = 0; k < 8; k++)
                        {
                            if (Bits_Array_Key[k] == 1 && Bits_Array_PT[k] == 0)
                            {
                                XOR_Res = 1;
                            }
                            else if (Bits_Array_Key[k] == 0 && Bits_Array_PT[k] == 1)
                            {
                                XOR_Res = 1;
                            }
                            else
                            {
                                XOR_Res = 0;
                            }
                            XOR_Arr[k] = XOR_Res;
                        }
                        block[i, j] = BitArray.ToDecimal(XOR_Arr);
                    }
                }

                byte[] binBlock = new byte[16];
                int count = 0;
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        binBlock[count] = (byte)block[i, j];
                        count++;
                    }
                }

                output += bytesToBin(binBlock);

            }
            return output;
        }
        public string AES_DEC(string input, string privateKey, bool isBinary)
        {
            int[,] sBoxInv = new int[,]
            {{0x52, 0x09, 0x6a, 0xd5, 0x30, 0x36, 0xa5, 0x38, 0xbf, 0x40, 0xa3, 0x9e, 0x81, 0xf3, 0xd7, 0xfb},
            { 0x7c, 0xe3, 0x39, 0x82, 0x9b, 0x2f, 0xff, 0x87, 0x34, 0x8e, 0x43, 0x44, 0xc4, 0xde, 0xe9, 0xcb},
            { 0x54, 0x7b, 0x94, 0x32, 0xa6, 0xc2, 0x23, 0x3d, 0xee, 0x4c, 0x95, 0x0b, 0x42, 0xfa, 0xc3, 0x4e},
            { 0x08, 0x2e, 0xa1, 0x66, 0x28, 0xd9, 0x24, 0xb2, 0x76, 0x5b, 0xa2, 0x49, 0x6d, 0x8b, 0xd1, 0x25},
            { 0x72, 0xf8, 0xf6, 0x64, 0x86, 0x68, 0x98, 0x16, 0xd4, 0xa4, 0x5c, 0xcc, 0x5d, 0x65, 0xb6, 0x92},
            { 0x6c, 0x70, 0x48, 0x50, 0xfd, 0xed, 0xb9, 0xda, 0x5e, 0x15, 0x46, 0x57, 0xa7, 0x8d, 0x9d, 0x84},
            { 0x90, 0xd8, 0xab, 0x00, 0x8c, 0xec, 0xd3, 0x0a, 0xf7, 0xe4, 0x58, 0x05, 0xb8, 0xb3, 0x45, 0x06},
            { 0xd0, 0x2c, 0x1e, 0x8f, 0xca, 0x3f, 0x0f, 0x02, 0xc1, 0xaf, 0xbd, 0x03, 0x01, 0x13, 0x8a, 0x6b},
            { 0x3a, 0x91, 0x11, 0x41, 0x4f, 0x67, 0xdc, 0xea, 0x97, 0xf2, 0xcf, 0xce, 0xf0, 0xb4, 0xe6, 0x73},
            { 0x96, 0xac, 0x74, 0x22, 0xe7, 0xad, 0x35, 0x85, 0xe2, 0xf9, 0x37, 0xe8, 0x1c, 0x75, 0xdf, 0x6e},
            { 0x47, 0xf1, 0x1a, 0x71, 0x1d, 0x29, 0xc5, 0x89, 0x6f, 0xb7, 0x62, 0x0e, 0xaa, 0x18, 0xbe, 0x1b},
            { 0xfc, 0x56, 0x3e, 0x4b, 0xc6, 0xd2, 0x79, 0x20, 0x9a, 0xdb, 0xc0, 0xfe, 0x78, 0xcd, 0x5a, 0xf4},
            { 0x1f, 0xdd, 0xa8, 0x33, 0x88, 0x07, 0xc7, 0x31, 0xb1, 0x12, 0x10, 0x59, 0x27, 0x80, 0xec, 0x5f},
            { 0x60, 0x51, 0x7f, 0xa9, 0x19, 0xb5, 0x4a, 0x0d, 0x2d, 0xe5, 0x7a, 0x9f, 0x93, 0xc9, 0x9c, 0xef},
            { 0xa0, 0xe0, 0x3b, 0x4d, 0xae, 0x28, 0xf5, 0xb0, 0xc8, 0xeb, 0xbb, 0x3c, 0x83, 0x53, 0x99, 0x61},
            { 0x17, 0x2b, 0x04, 0x7e, 0xba, 0x77, 0xd6, 0x26, 0xe1, 0x69, 0x14, 0x63, 0x55, 0x21, 0x0c, 0x7d}};
            List<int> inputAscii = new List<int>();
            string output = "";

            int numOfBytes = input.Length / 8;
            byte[] bytes = new byte[numOfBytes];
            for (int i = 0; i < numOfBytes; ++i)
            {
                bytes[i] = Convert.ToByte(input.Substring(8 * i, 8), 2);
            }

            for (int i = 0; i < bytes.Length; i++)
            {
                inputAscii.Add(bytes[i]);
            }
            // Gen the Blocks
            int[,] block = new int[4, 4];
            int blockIndex = 0;
            while (blockIndex < inputAscii.Count)
            {
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 4; j++, blockIndex++)
                    {
                        if (blockIndex < inputAscii.Count)
                        {
                            block[i, j] = inputAscii[blockIndex];
                        }
                        else
                        {
                            block[i, j] = 0;
                        }
                    }
                }
                //Reverse XOR0
                int[,] key = new int[4, 4];
                int XOR_Res;
                int[] XOR_Arr = new int[8];
                int keyindex = 0;
                BigInteger sessionKey = BigInteger.Parse(privateKey);
                byte[] byteKey = sessionKey.ToByteArray();
                for (int i = 0; i < 4; i++, keyindex++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if (byteKey.Length > keyindex)
                        {
                            key[i, j] = byteKey[keyindex];
                        }
                        else
                        {
                            key[i, j] = 32;
                        }

                    }
                }
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        int[] Bits_Array_Key = BitArray.ToBits(key[i, j], 8);
                        int[] Bits_Array_PT = BitArray.ToBits(block[i, j], 8);
                        for (int k = 0; k < 8; k++)
                        {
                            if (Bits_Array_Key[k] == 1 && Bits_Array_PT[k] == 0)
                            {
                                XOR_Res = 1;
                            }
                            else if (Bits_Array_Key[k] == 0 && Bits_Array_PT[k] == 1)
                            {
                                XOR_Res = 1;
                            }
                            else
                            {
                                XOR_Res = 0;
                            }
                            XOR_Arr[k] = XOR_Res;
                        }
                        block[i, j] = BitArray.ToDecimal(XOR_Arr);
                    }
                }
                //Shift Rows REVERSE
                int temp = 0, t = 0;
                for (int i = 1; i < 4; i++)
                {
                    for (int j = 3; j > 0; j--)
                    {
                        if (i <= 1)
                        {
                            t = j - 1;
                            if (t < 0)
                            {
                                t = t + 4;
                            }
                            temp = block[1, j];
                            block[1, j] = block[1, t];
                            block[1, t] = temp;
                        }
                        if (i <= 2)
                        {
                            t = j - 1;
                            if (t < 0)
                            {
                                t = t + 4;
                            }
                            temp = block[2, j];
                            block[2, j] = block[2, t];
                            block[2, t] = temp;
                        }
                        if (i <= 3)
                        {
                            t = j - 1;
                            if (t < 0)
                            {
                                t = t + 4;
                            }
                            temp = block[3, j];
                            block[3, j] = block[3, t];
                            block[3, t] = temp;
                        }
                    }
                }
                //SBox Reverse
                int row, col;
                float reminder;
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        row = block[i, j] / 16;
                        reminder = (float)block[i, j];
                        reminder = ((reminder / 16) - row) * 16;
                        col = (int)reminder;
                        block[i, j] = sBoxInv[row, col];
                    }
                }

                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if (!isBinary)
                            output += (char)block[i, j];
                        else
                        {
                            byte[] tmpByte = new byte[1];
                            tmpByte[0] = (byte)block[i, j];
                            output += bytesToBin(tmpByte);
                        }
                    }
                }
            }
            output = output.Replace("\0", "");
            return output;
        }
    }
}
