using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SF_CS.IntegrationApp.DALayer
{
    public class cryptoutil
    {
        //8 bytes randomly selected for both the Key and the Initialization Vector 
        //the IV is used to encrypt the first block of text so that any repetitive 
        //patterns are not apparent 
        private static byte[] KEY_64 = { 42, 16, 93, 156, 78, 4, 218, 32 };
        private static byte[] IV_64 = { 55, 103, 246, 79, 36, 99, 167, 3 };

        //24 byte or 192 bit key and IV for TripleDES 
        private static byte[] KEY_192 = { 42, 16, 93, 156, 78, 4, 218, 32, 15, 167,
    44, 80, 26, 250, 155, 112, 2, 94, 11, 204,
    119, 35, 184, 197 };
        private static byte[] IV_192 = { 55, 103, 246, 79, 36, 99, 167, 3, 42, 5,
    62, 83, 184, 7, 209, 13, 145, 23, 200, 58,
    173, 10, 121, 222 };

        //Standard DES encryption 
        public static string Encrypt(string value)
        {

            if (!string.IsNullOrEmpty(value))
            {
                DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, cryptoProvider.CreateEncryptor(KEY_64, IV_64), CryptoStreamMode.Write);
                StreamWriter sw = new StreamWriter(cs);
                sw.Write(value);
                sw.Flush();
                cs.FlushFinalBlock();
                ms.Flush();
                //convert back to a string 
                return Convert.ToBase64String(ms.GetBuffer(), 0, Convert.ToInt16(ms.Length));
            }
            return "";

        }

        //Standard DES decryption 
        public static string Decrypt(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();

                //convert from string to byte array 
                byte[] buffer = Convert.FromBase64String(value);
                MemoryStream ms = new MemoryStream(buffer);
                CryptoStream cs = new CryptoStream(ms, cryptoProvider.CreateDecryptor(KEY_64, IV_64), CryptoStreamMode.Read);
                StreamReader sr = new StreamReader(cs);

                return sr.ReadToEnd();
            }
            return "";
        }

        //TRIPLE DES encryption 
        public static string EncryptTripleDES(string value)
        {

            if (!string.IsNullOrEmpty(value))
            {
                TripleDESCryptoServiceProvider cryptoProvider = new TripleDESCryptoServiceProvider();
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, cryptoProvider.CreateEncryptor(KEY_192, IV_192), CryptoStreamMode.Write);
                StreamWriter sw = new StreamWriter(cs);

                sw.Write(value);
                sw.Flush();
                cs.FlushFinalBlock();
                ms.Flush();

                //convert back to a string 
                return Convert.ToBase64String(ms.GetBuffer(), 0, Convert.ToInt16(ms.Length));
            }
            return "";

        }

        //TRIPLE DES decryption 
        public static string DecryptTripleDES(string value)
        {

            if (!string.IsNullOrEmpty(value))
            {
                TripleDESCryptoServiceProvider cryptoProvider = new TripleDESCryptoServiceProvider();

                //convert from string to byte array 
                byte[] buffer = Convert.FromBase64String(value);
                MemoryStream ms = new MemoryStream(buffer);
                CryptoStream cs = new CryptoStream(ms, cryptoProvider.CreateDecryptor(KEY_192, IV_192), CryptoStreamMode.Read);
                StreamReader sr = new StreamReader(cs);
                return sr.ReadToEnd();
            }
            return "";

        }
    }
}
