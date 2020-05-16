using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage;
using Windows.UI.Popups;

namespace ehw_t1
{
    public static class External_1
    {
        public async static void Show(this String str)
        {
            var dialog = new MessageDialog(str);
            await dialog.ShowAsync();
        }



        private const int KeySize = 256; // in bits

        public static string EncryptStr(this String plainText)
        {
            var utf8 = new UTF8Encoding();

          //  var base64Key = "+CffHxKmykUvCrrCILd4rZDBcrIoe3w89jnPNXYi0rg=";
            var base64Key = "1234567812345678";
            var Key2 = Convert.FromBase64String(base64Key);
            var Key = utf8.GetBytes(base64Key.Substring(0,16));

          
           // byte[] Key = utf8.GetBytes(aKey);

            string aIv = "1234567812345678";
            byte[] iv = utf8.GetBytes(aIv.Substring(0,16));
           

            byte[] ivArr = { 1, 2, 3, 4, 5, 6, 6, 5, 4, 3, 2, 1, 7, 7, 7, 7 };
            byte[] IVBytes16Value = new byte[16];
            Array.Copy(ivArr, IVBytes16Value, Math.Min(ivArr.Length, IVBytes16Value.Length));

            // Check arguments. 
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            byte[] returnValue;
            using (var aes = Aes.Create())
            {
                aes.KeySize = 128;
                aes.BlockSize = 128;
                aes.Padding = PaddingMode.PKCS7;
                aes.IV = IVBytes16Value;
              //  aes.GenerateIV();
                aes.Mode = CipherMode.CBC;
              //  var iv = aes.IV;
              
                if (string.IsNullOrEmpty(plainText))
                    return Convert.ToBase64String(iv);
                var encryptor = aes.CreateEncryptor(Key, iv);

                // Create the streams used for encryption. 
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        // this is just our encrypted data
                        var encrypted = msEncrypt.ToArray();
                        returnValue = new byte[encrypted.Length + iv.Length];
                        // append our IV so our decrypt can get it
                        Array.Copy(iv, returnValue, iv.Length);
                        // append our encrypted data
                        Array.Copy(encrypted, 0, returnValue, iv.Length, encrypted.Length);
                    }
                }
            }

            // return encrypted bytes converted to Base64String
            return Convert.ToBase64String(returnValue);
        }

       

        public static string DecryptStr(this String cipherText)
        {
            var utf8 = new UTF8Encoding();


            //  var base64Key = "+CffHxKmykUvCrrCILd4rZDBcrIoe3w89jnPNXYi0rg=";
            var base64Key = "1234567812345678";
            var Key2 = Convert.FromBase64String(base64Key);
            var Key = utf8.GetBytes(base64Key.Substring(0,16));
         
        
           
            string aIv = "1234567812345678";
            byte[] iv = utf8.GetBytes(aIv.Substring(0, 16));
           

            byte[] ivArr = { 1, 2, 3, 4, 5, 6, 6, 5, 4, 3, 2, 1, 7, 7, 7, 7 };
            byte[] IVBytes16Value = new byte[16];
            Array.Copy(ivArr, IVBytes16Value, Math.Min(ivArr.Length, IVBytes16Value.Length));

            //    byte[] Key = utf8.GetBytes(aKey);
            // Check arguments. 
            if (string.IsNullOrEmpty(cipherText))
                return string.Empty;
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");

            string plaintext = null;
            // this is all of the bytes
            var allBytes = Convert.FromBase64String(cipherText);

            using (var aes = Aes.Create())
            {
                aes.KeySize = 128;
                aes.BlockSize = 128;
                aes.Padding = PaddingMode.PKCS7;
                aes.Mode = CipherMode.CBC;
                aes.IV = IVBytes16Value;

                // get our IV that we pre-pended to the data
                //  byte[] iv = new byte[aes.BlockSize / 8];
                if (allBytes.Length < iv.Length)
                    throw new ArgumentException("Message was less than IV size.");
                Array.Copy(allBytes, iv, iv.Length);
                // get the data we need to decrypt
                byte[] cipherBytes = new byte[allBytes.Length - iv.Length];
                Array.Copy(allBytes, iv.Length, cipherBytes, 0, cipherBytes.Length);

                // Create a decrytor to perform the stream transform.
                var decryptor = aes.CreateDecryptor(Key, iv);

                // Create the streams used for decryption. 
                using (MemoryStream msDecrypt = new MemoryStream(cipherBytes))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            // Read the decrypted bytes from the decrypting stream 
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }


        public static void SaveMe(this String value, String key)
        {
            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;

            localSettings.Values[key] = value;
        }


        public static string GetStore(this String key)
        {
            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;

            if (localSettings.Values.ContainsKey(key))
            {
                return localSettings.Values[key].ToString();
            }
            else
            {
                return null;
            }


        }




    }



}
