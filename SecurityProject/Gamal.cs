using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace SecurityProject
{
    [Serializable()]
    public class Gamal
    {
        BigInteger c1, c2;
        public byte[] BigIntegerToByte(BigInteger number)
        {
            string text = number.ToString();
            byte[] bytes = Encoding.ASCII.GetBytes(text);
            return bytes;
        }

        public byte[] Encrypt(Gamal gamal, string plainText)
        {
            BigInteger q = BigInteger.Parse("811625542898199028798761685010661735539490805771693537579527331057313324571568448399462365181797487809911948886255338244497001176540203235940735497429168085634281811232131790596475519535461973753959592314363650044494749470886414557182282939272027164975882401017428707277564380765257706818702936363131");
            BigInteger a = BigInteger.Parse("10");
            BigInteger Ya = BigInteger.Parse("713923636676947356037684125678981155287396913112255968563472887250681508893982988287319988132745913073735797738073536141154653812665786981026200963119973536740180171845477642904160937390593751641752970948583482958051094872127630481619887837217393290385928680633586497596307429606398647891363403457152");

            //Bob(Cipher)
            byte[] msg = Encoding.ASCII.GetBytes(plainText);

            var hexString = BitConverter.ToString(msg);
            hexString = hexString.Replace("-", "");
            string binaryString = String.Join(String.Empty,
            hexString.Select(c => Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0')));
            BigInteger M = BigInteger.Parse(binaryString);



            BigInteger Ksmall = BigInteger.Parse("74751729786219471113700411646762466406536193945869"); //k small
            BigInteger Kbig = BigInteger.ModPow(Ya, Ksmall, q);
            BigInteger C1 = BigInteger.ModPow(a, Ksmall, q);
            BigInteger KM = BigInteger.Multiply(Kbig, M);
            BigInteger C2 = BigInteger.ModPow(KM, 1, q);

            gamal.c1 = C1;
            gamal.c2 = C2;

            byte[] cipherObj = Serialize(gamal);

            return cipherObj;
        }

        public string Decrypt(Gamal gamal, byte[] cipherMsg)
        {
            gamal = Deserialize(cipherMsg);
            
            BigInteger q = BigInteger.Parse("811625542898199028798761685010661735539490805771693537579527331057313324571568448399462365181797487809911948886255338244497001176540203235940735497429168085634281811232131790596475519535461973753959592314363650044494749470886414557182282939272027164975882401017428707277564380765257706818702936363131");
            BigInteger a = BigInteger.Parse("10");

            //Alex(generation)
            BigInteger Xa = BigInteger.Parse("8388089106629031386100099554200373068182096190836051003383095251861768840967721916418379465371310651");

            BigInteger C1 = gamal.c1;
            BigInteger C2 = gamal.c2;

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
            string decryptedMessage = System.Text.Encoding.ASCII.GetString(bytes);

            return decryptedMessage;
        }

        public byte[] Serialize(Gamal obj)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }
        public Gamal Deserialize(byte[] arrBytes)
        {
            MemoryStream memStream = new MemoryStream();
            BinaryFormatter binForm = new BinaryFormatter();
            memStream.Write(arrBytes, 0, arrBytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            Gamal obj = (Gamal)binForm.Deserialize(memStream);

            return obj;
        }
    }
}
