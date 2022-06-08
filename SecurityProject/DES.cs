using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace SecurityProject
{
    public class DES
    {
        public int[] initialPermuatuionTable = new int[] { 58,50,42,34,26,18,10,2,60,52,44,36,28,20,12,4,
                                       62,54,46,38,30,22,14,6,64,56,48,40,32,24,16,8,
                                       57,49,41,33,25,17,9,1,59,51,43,35,27,19,11,3,
                                       61,53,45,37,29,21,13,5,63,55,47,39,31,23,15,7 };

        public int[] circularLeftShiftTable = new int[] { 1, 1, 2, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 2, 1 };

        public int[] circularRightShiftTable = new int[] { 1, 2, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 2, 1, 1 };

        public int[] compressionPermutationTable = new int[] { 14,17,11,24,1,5,3,28,15,6,21,10,
                                        23,19,12,4,26,8,16,7,27,20,13,2,
                                        41,52,31,37,47,55,30,40,51,45,33,48,
                                        44,49,39,56,34,53,46,42,50,36,29,32 };

        public int[] expansionPermuationTable = new int[] { 32,1,2,3,4,5,4,5,6,7,8,9,
                                        8,9,10,11,12,13,12,13,14,15,16,17,
                                        16,17,18,19,20,21,20,21,22,23,24,25,
                                        24,25,26,27,28,29,28,29,30,31,32,1 };

        public int[,] sbox = new int[8, 64] { { 14,4,13,1,2,15,11,8,3,10,6,12,5,9,0,7,
                                                0,15,7,4,14,2,13,1,10,6,12,11,9,5,3,8,
                                                4,1,14,8,13,6,2,11,15,12,9,7,3,10,5,0,
                                                15,12,8,2,4,9,1,7,5,11,3,14,10,0,6,13 },
                                              { 15,1,8,14,6,11,3,4,9,7,2,13,12,0,5,10,
                                                3,13,4,7,15,2,8,14,12,0,1,10,6,9,11,5,
                                                0,14,7,11,10,4,13,1,5,8,12,6,9,3,2,15,
                                                13,8,10,1,3,15,4,2,11,6,7,12,0,5,14,9 },
                                              { 10,0,9,14,6,3,15,5,1,13,12,7,11,4,2,8,
                                                13,7,0,9,3,4,6,10,2,8,5,14,12,11,15,1,
                                                13,6,4,9,8,15,3,0,11,1,2,12,5,10,14,7,
                                                1,10,13,0,6,9,8,7,4,15,14,3,11,5,2,12 },
                                              { 7,13,14,3,0,6,9,10,1,2,8,5,11,12,4,15,
                                                13,8,11,5,6,15,0,3,4,7,2,12,1,10,14,9,
                                                10,6,9,0,12,11,7,13,15,1,3,14,5,2,8,4,
                                                3,15,0,6,10,1,13,8,9,4,5,11,12,7,2,14 },
                                              { 2,12,4,1,7,10,11,6,8,5,3,15,13,0,14,9,
                                                14,11,2,12,4,7,13,1,5,0,15,10,3,9,8,6,
                                                4,2,1,11,10,13,7,8,15,9,12,5,6,3,0,14,
                                                11,8,12,7,1,14,2,13,6,15,0,9,10,4,5,3 },
                                              { 12,1,10,15,9,2,6,8,0,13,3,4,14,7,5,11,
                                                10,15,4,2,7,12,9,5,6,1,13,14,0,11,3,8,
                                                9,14,15,5,2,8,12,3,7,0,4,10,1,13,11,6,
                                                4,3,2,12,9,5,15,10,11,14,1,7,6,0,8,13 },
                                              { 4,11,2,14,15,0,8,13,3,12,9,7,5,10,6,1,
                                                13,0,11,7,4,9,1,10,14,3,5,12,2,15,8,6,
                                                1,4,11,13,12,3,7,14,10,15,6,8,0,5,9,2,
                                                6,11,13,8,1,4,10,7,9,5,0,15,14,2,3,12 },
                                              { 13,2,8,4,6,15,11,1,10,9,3,14,5,0,12,7,
                                                1,15,13,8,10,3,7,4,12,5,6,11,0,14,9,2,
                                                7,11,4,1,9,12,14,2,0,6,10,13,15,3,5,8,
                                                2,1,14,7,4,10,8,13,15,12,9,0,3,5,6,11 } };

        public int[] pbox = new int[] { 16,7,20,21,29,12,28,17,1,15,23,26,5,18,31,10,
                                         2,8,24,14,32,27,3,9,19,13,30,6,22,11,4,25 };

        public int[] finalPermutationTable = new int[] { 40,8,48,16,56,24,64,32,39,7,47,15,55,23,63,31,
                                       38,6,46,14,54,22,62,30,37,5,45,13,53,21,61,29,
                                       36,4,44,12,52,20,60,28,35,3,43,11,51,19,59,27,
                                       34,2,42,10,50,18,58,26,33,1,41,9,49,17,57,25 };

        public int[] binaryPlainText = new int[500000];
        public int[] binaryCipherText = new int[500000];
        public int[] binaryKey = new int[64];
        public int[] ptextbitslice = new int[64];
        public int[] ctextbitslice = new int[64];
        public int[] initialPermuatationPlainText = new int[64];
        public int[] initialPermuatationCipherText = new int[64];
        public int[] plainTextLeft = new int[32];
        public int[] plainTextRight = new int[32];
        public int[] cipherTextLeft = new int[32];
        public int[] cipherTextRight = new int[32];
        public int[] changedkey = new int[56];
        public int[] shiftedkey = new int[56];
        public int[] tempRPT = new int[32];
        public int[] tempLPT = new int[32];
        public int[] CKey = new int[28];
        public int[] DKey = new int[28];
        public int[] compressedkey = new int[48];
        public int[] ctExpandedLPT = new int[48];
        public int[] ptExpandedRPT = new int[48];
        public int[] XoredRPT = new int[48];
        public int[] XoredLPT = new int[48];
        public int[] row = new int[2];
        public int rowindex;
        public int[] column = new int[4];
        public int columnindex;
        public int sboxvalue;
        public int[] tempsboxarray = new int[4];
        public int[] ptSBoxRPT = new int[32];
        public int[] ctSBoxLPT = new int[32];
        public int[] ctPBoxLPT = new int[32];
        public int[] ptPBoxRPT = new int[32];
        public int[] attachedPlainText = new int[64];
        public int[] attachedCipherText = new int[64];
        public int[] finalPermuatationPlainText = new int[64];
        public int[] finalPermuatationCipherText = new int[64];

        public string stringToBinString(string str)
        {
            byte[] msg = Encoding.ASCII.GetBytes(str);

            var hexString = BitConverter.ToString(msg);
            hexString = hexString.Replace("-", "");
            string binaryString = String.Join(String.Empty,
            hexString.Select(c => Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0')));

            return binaryString;
        }

        public string binStringToString(string test)
        {
            test = test.Replace("\0", "");

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
            string decryptedMessage = System.Text.Encoding.ASCII.GetString(bytes);

            return decryptedMessage;
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

        public int GetASCII(char ch)
        {
            int n = ch;
            return n;
        }
        public void AssignArray1ToArray2b(int[] array1, int[] array2, int fromIndex)
        {
            int x, y;
            for (x = 0, y = fromIndex; x < array1.Length; ++x, ++y)
                array2[y] = array1[x];
        }
        public int ConvertTextToBits(char[] chararray, int[] savedarray)
        {
            int j = 0;
            for (int i = 0; i < chararray.Length; ++i)
            {
                int[] ba = BitArray.ToBits(GetASCII(chararray[i]), 8);
                j = i * 8;
                AssignArray1ToArray2b(ba, savedarray, j);
            }
            return (j + 8);
        }
        public int AppendZeroes(int[] appendedarray, int len)
        {
            int zeroes;
            if (len % 64 != 0)
            {
                zeroes = (64 - (len % 64));

                for (int i = 0; i < zeroes; ++i)
                    appendedarray[len++] = 0;
            }
            return len;
        }
        public void InitialPermutation(int[] sentarray, int[] savedarray)
        {
            int tmp;
            for (int i = 0; i < 64; i++)
            {
                tmp = initialPermuatuionTable[i];
                savedarray[i] = sentarray[tmp - 1];
            }
        }
        public void DivideIntoLPTAndRPT(int[] sentarray, int[] savedLPT, int[] savedRPT)
        {
            for (int i = 0, k = 0; i < 32; i++, ++k)
            {
                savedLPT[k] = sentarray[i];
            }

            for (int i = 32, k = 0; i < 64; i++, ++k)
            {
                savedRPT[k] = sentarray[i];
            }
        }
        public void AssignChangedKeyToShiftedKey()
        {
            for (int i = 0; i < 56; i++)
            {
                shiftedkey[i] = changedkey[i];
            }
        }
        public void SaveTemporary(int[] text, int[] temp)
        {
            for (int i = 0; i < 32; i++)
            {
                temp[i] = text[i];
            }
        }
        public void DivideIntoCKeyAndDKey()
        {
            for (int i = 0, j = 0; i < 28; i++, ++j)
            {
                CKey[j] = shiftedkey[i];
            }

            for (int i = 28, j = 0; i < 56; i++, ++j)
            {
                DKey[j] = shiftedkey[i];
            }
        }

        public void CircularLeftShift(int[] HKey)
        {
            int i, FirstBit = HKey[0];
            for (i = 0; i < 27; i++)
            {
                HKey[i] = HKey[i + 1];
            }
            HKey[i] = FirstBit;
        }

        public void AttachCKeyAndDKey()
        {
            int j = 0;
            for (int i = 0; i < 28; i++)
            {
                shiftedkey[j++] = CKey[i];
            }

            for (int i = 0; i < 28; i++)
            {
                shiftedkey[j++] = DKey[i];
            }
        }

        public void CompressionPermutation()
        {
            int temp;
            for (int i = 0; i < 48; i++)
            {
                temp = compressionPermutationTable[i];
                compressedkey[i] = shiftedkey[temp - 1];
            }
        }

        public void ExpansionPermutation(int[] text, int[] ExpandedArray)
        {
            int temp;
            for (int i = 0; i < 48; i++)
            {
                temp = expansionPermuationTable[i];
                ExpandedArray[i] = text[temp - 1];
            }
        }

        public void XOROperation(int[] array1, int[] array2, int[] array3, int SizeOfTheArray)
        {
            for (int i = 0; i < SizeOfTheArray; i++)
            {
                array3[i] = array1[i] ^ array2[i];
            }
        }

        public void AssignSBox(int[] temparray, int[] SBoxArray, int fromIndex)
        {
            int j = fromIndex;
            for (int i = 0; i < 4; i++)
            {
                SBoxArray[j++] = tempsboxarray[i];
            }
        }

        public void SBoxSubstituion(int[] Xored, int[] SBox)
        {
            int r, t, j = 0, q = 0;
            for (int i = 0; i < 48; i += 6)
            {
                row[0] = Xored[i];
                row[1] = Xored[i + 5];
                rowindex = BitArray.ToDecimal(row);

                column[0] = Xored[i + 1];
                column[1] = Xored[i + 2];
                column[2] = Xored[i + 3];
                column[3] = Xored[i + 4];
                columnindex = BitArray.ToDecimal(column);

                t = ((16 * (rowindex)) + (columnindex));

                sboxvalue = sbox[j++, t];

                tempsboxarray = BitArray.ToBits(sboxvalue, 4);

                r = q * 4;

                AssignSBox(tempsboxarray, SBox, r);

                ++q;
            }
        }

        public void PBoxPermutation(int[] SBox, int[] PBox)
        {
            int temp;
            for (int i = 0; i < 32; i++)
            {
                temp = pbox[i];
                PBox[i] = SBox[temp - 1];
            }
        }

        public void Swap(int[] temp, int[] text)
        {
            for (int i = 0; i < 32; i++)
            {
                text[i] = temp[i];
            }
        }
        public void SixteenRounds()
        {
            int n;

            for (int i = 0; i < 16; i++)
            {
                SaveTemporary(plainTextRight, tempRPT);

                n = circularLeftShiftTable[i];

                DivideIntoCKeyAndDKey();

                for (int j = 0; j < n; j++)
                {
                    CircularLeftShift(CKey);
                    CircularLeftShift(DKey);
                }

                AttachCKeyAndDKey();

                CompressionPermutation();

                ExpansionPermutation(plainTextRight, ptExpandedRPT);

                XOROperation(compressedkey, ptExpandedRPT, XoredRPT, 48);

                SBoxSubstituion(XoredRPT, ptSBoxRPT); ///////////////

                PBoxPermutation(ptSBoxRPT, ptPBoxRPT);

                XOROperation(ptPBoxRPT, plainTextLeft, plainTextRight, 32);

                Swap(tempRPT, plainTextLeft);
            }
        }
        public void CircularRightShift(int[] Key)
        {
            int i, LastBit = Key[27];
            for (i = 27; i >= 1; --i)
            {
                Key[i] = Key[i - 1];
            }
            Key[i] = LastBit;
        }
        public void ReversedSixteenRounds()
        {
            int n;

            for (int i = 0; i < 16; i++)
            {
                SaveTemporary(cipherTextLeft, tempLPT);

                CompressionPermutation();

                ExpansionPermutation(cipherTextLeft, ctExpandedLPT);

                XOROperation(compressedkey, ctExpandedLPT, XoredLPT, 48);

                SBoxSubstituion(XoredLPT, ctSBoxLPT);

                PBoxPermutation(ctSBoxLPT, ctPBoxLPT);

                XOROperation(ctPBoxLPT, cipherTextRight, cipherTextLeft, 32);

                Swap(tempLPT, cipherTextRight);

                n = circularRightShiftTable[i];

                DivideIntoCKeyAndDKey();

                for (int j = 0; j < n; j++)
                {
                    CircularRightShift(CKey);
                    CircularRightShift(DKey);
                }

                AttachCKeyAndDKey();
            }
        }
        public void AttachLPTAndRPT(int[] savedLPT, int[] savedRPT, int[] AttachedPT)
        {
            int j = 0;
            for (int i = 0; i < 32; i++)
            {
                AttachedPT[j++] = savedLPT[i];
            }

            for (int i = 0; i < 32; i++)
            {
                AttachedPT[j++] = savedRPT[i];
            }
        }
        public void FinalPermutation(int[] fromPT, int[] toPT)
        {
            int temp;
            for (int i = 0; i < 64; i++)
            {
                temp = finalPermutationTable[i];
                toPT[i] = fromPT[temp - 1];
            }
        }
        public string ConvertBitsToText(int[] sentarray, int len)
        {
            string finaltext = "";
            int j, k, decimalvalue;
            int[] tempbitarray = new int[8];

            for (int i = 0; i < len; i += 8)
            {
                for (k = 0, j = i; j < (i + 8); ++k, ++j)
                {
                    tempbitarray[k] = sentarray[j];
                }

                decimalvalue = BitArray.ToDecimal(tempbitarray);

                if (decimalvalue == 0)
                    break;

                finaltext += (char)decimalvalue;
            }

            return finaltext;
        }
        public string Encrypt(string plaintext, string key)
        {
            string ciphertext = null;
            int j, k;
            int st = plaintext.Length;

            for (int i = 0; i < plaintext.Length; i++)
            {
                binaryPlainText[i] = plaintext[i] - '0';
            }

            int fst = AppendZeroes(binaryPlainText, st);

            for (int i = 0; i < key.Length; i++)
            {
                if (i >= 64)
                    break;
                binaryKey[i] = key[i] - '0';
            }

            // Basheel kol 8th bit
            for (int i = 0, h = 0; i < 64; i++)
            {
                if ((i + 1) % 8 == 0)
                    continue;
                changedkey[h++] = binaryKey[i];
            }
            //////b2sm el plain text l kaza plain text
            for (int i = 0; i < fst; i += 64)
            {
                for (k = 0, j = i; j < (i + 64); ++j, ++k)
                {
                    ptextbitslice[k] = binaryPlainText[j];
                }
                InitialPermutation(ptextbitslice, initialPermuatationPlainText);

                DivideIntoLPTAndRPT(initialPermuatationPlainText, plainTextLeft, plainTextRight);

                AssignChangedKeyToShiftedKey();

                SixteenRounds();

                AttachLPTAndRPT(plainTextLeft, plainTextRight, attachedPlainText);

                FinalPermutation(attachedPlainText, finalPermuatationPlainText);

                for (k = 0, j = i; j < (i + 64); ++j, ++k)
                {
                    binaryCipherText[j] = finalPermuatationPlainText[k];
                }
            }
            //ciphertext = ConvertBitsToText(ciphertextbin, fst);

            for (int i = 0; i < fst; i++)
                ciphertext += binaryCipherText[i];

            int cipherLength = ciphertext.Length;
            return ciphertext;
        }

        public string Decrypt(string ciphertext, string key, bool isBinary)
        {
            string plaintext = null;
            int j, k;

            int st = ciphertext.Length;

            for (int i = 0; i < ciphertext.Length; i++)
            {
                binaryCipherText[i] = ciphertext[i] - '0';
            }
            int fst = AppendZeroes(binaryCipherText, st);

            for (int i = 0; i < key.Length; i++)
            {
                if (i >= 64)
                    break;
                binaryKey[i] = key[i] - '0';
            }

            for (int i = 0, h = 0; i < 64; i++)
            {
                if ((i + 1) % 8 == 0)
                    continue;
                changedkey[h++] = binaryKey[i];
            }

            for (int i = 0; i < fst; i += 64)
            {
                for (k = 0, j = i; j < (i + 64); ++j, ++k)
                {
                    ctextbitslice[k] = binaryCipherText[j];
                }

                InitialPermutation(ctextbitslice, initialPermuatationCipherText);

                DivideIntoLPTAndRPT(initialPermuatationCipherText, cipherTextLeft, cipherTextRight);

                AssignChangedKeyToShiftedKey();

                ReversedSixteenRounds();

                AttachLPTAndRPT(cipherTextLeft, cipherTextRight, attachedCipherText);

                FinalPermutation(attachedCipherText, finalPermuatationCipherText);

                for (k = 0, j = i; j < (i + 64); ++j, ++k)
                {
                    binaryPlainText[j] = finalPermuatationCipherText[k];
                }
            }

            if(!isBinary)
                plaintext = ConvertBitsToText(binaryPlainText, fst);
            else
            {
                for(int i=0;i<fst;i++)
                {
                    plaintext += binaryPlainText[i];
                }
            }

            int plainLen = plaintext.Length;
            return plaintext;
        }

        public byte[] EncryptionDES(string plaintext, bool isBinary)//encrypt
        {
            BigInteger q = BigInteger.Parse("811625542898199028798761685010661735539490805771693537579527331057313324571568448399462365181797487809911948886255338244497001176540203235940735497429168085634281811232131790596475519535461973753959592314363650044494749470886414557182282939272027164975882401017428707277564380765257706818702936363131");

            BigInteger Xa = BigInteger.Parse("8388089106629031386100099554200373068182096190836051003383095251861768840967721916418379465371310651");

            BigInteger Yb = BigInteger.Parse("230237959934522967741530097375752932445043483760693170154138464628182098627044086247153407788848046772813737159138267907696695119504782409347934163882990113885516559307762800544992449061531788269912661508070321123426623021721801346476665547451961138893687075307093080400871898442263928653910158948572");

            BigInteger sessionKey = BigInteger.ModPow(Yb, Xa, q);

            byte[] binKab = sessionKey.ToByteArray();
            var hexStr = BitConverter.ToString(binKab);
            hexStr = hexStr.Replace("-", "");
            string key = String.Join(String.Empty,
            hexStr.Select(c => Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0')));

            if(!isBinary)
                plaintext = stringToBinString(plaintext);

            string cipherText = Encrypt(plaintext, key);

            if (isBinary)
            {
                int numOfBytes = cipherText.Length / 8;
                byte[] bytes = new byte[numOfBytes];
                for (int i = 0; i < numOfBytes; ++i)
                {
                    bytes[i] = Convert.ToByte(cipherText.Substring(8 * i, 8), 2);
                }
                return bytes;
            }
            System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
            byte[] cipherMsg;
            cipherMsg = enc.GetBytes(cipherText);
            return cipherMsg;

        }

        public string DecryptionDES(byte[] msg, bool isBinary)
        {

            BigInteger q = BigInteger.Parse("811625542898199028798761685010661735539490805771693537579527331057313324571568448399462365181797487809911948886255338244497001176540203235940735497429168085634281811232131790596475519535461973753959592314363650044494749470886414557182282939272027164975882401017428707277564380765257706818702936363131");

            BigInteger Xa = BigInteger.Parse("3688874613153740523692938629452814933745676512059566549790156200431548537521070337247213060898871487");

            BigInteger Yb = BigInteger.Parse("713923636676947356037684125678981155287396913112255968563472887250681508893982988287319988132745913073735797738073536141154653812665786981026200963119973536740180171845477642904160937390593751641752970948583482958051094872127630481619887837217393290385928680633586497596307429606398647891363403457152");

            BigInteger sessionKey = BigInteger.ModPow(Yb, Xa, q);


            string cipherText = "";

            if (!isBinary)
            {
                ASCIIEncoding eEncoding = new ASCIIEncoding();
                cipherText = eEncoding.GetString(msg);

                cipherText = cipherText.Replace("\0", "");
            }
            else
            {
                var hex = BitConverter.ToString(msg);
                hex = hex.Replace("-", "");
                cipherText = String.Join(String.Empty,
                hex.Select(c => Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0')));
            }

            //cipherText = binStringToString(cipherText);

            byte[] binSessionKey = sessionKey.ToByteArray();
            var hexStr = BitConverter.ToString(binSessionKey);
            hexStr = hexStr.Replace("-", "");
            string key = String.Join(String.Empty,
            hexStr.Select(c => Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0')));

            int cipherLength = cipherText.Length;
            
            string decryptedMessage = Decrypt(cipherText, key, isBinary);

            return decryptedMessage;
        }
    }
}