using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
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

        public static string EncryptStr(this String plainText)
        {
            var base64Key = "+CffHxKmykUvCrrCILd4rZDBcrIoe3w89jnPNXYi0rg=";
            var Key = Convert.FromBase64String(base64Key);

            // Check arguments. 
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            byte[] returnValue;
            using (var aes = Aes.Create())
            {
                aes.KeySize = KeySize;
                aes.GenerateIV();
                aes.Mode = CipherMode.CBC;
                var iv = aes.IV;
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
        private const int KeySize = 256; // in bits
        public static string DecryptStr(this String cipherText)
        {
            var base64Key = "+CffHxKmykUvCrrCILd4rZDBcrIoe3w89jnPNXYi0rg=";
            var Key = Convert.FromBase64String(base64Key);

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
                aes.KeySize = KeySize;
                aes.Mode = CipherMode.CBC;

                // get our IV that we pre-pended to the data
                byte[] iv = new byte[aes.BlockSize / 8];
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

    }



}
