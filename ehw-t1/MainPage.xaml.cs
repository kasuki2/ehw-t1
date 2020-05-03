using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.Cryptography.Certificates;
using Windows.Security.Cryptography.Core;
using Windows.Storage;
using Windows.Storage.Search;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ehw_t1
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            ReadAllFiles();
        }

        private async void AddFile_Click(object sender, RoutedEventArgs e)
        {
            var FileToCreateName = Filename.Text.ToString();

            if(FileToCreateName.Length < 3)
            {
                ("File name is too short. Min. 3 characters.").Show();
                return;
            }

            // van-e már ilyen file?

           
            Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            StorageFileQueryResult results = storageFolder.CreateFileQuery();

           
            IReadOnlyList<StorageFile> filesInFolder = await results.GetFilesAsync();
            bool van = false;
            foreach (StorageFile item in filesInFolder)
            {
                if(item.Name == FileToCreateName)
                {
                    van = true;

                }
               
            }

            if (van)
            {
                "Error: there is a file with the same name.".Show();
                return;
            }



            // Create sample file; replace if exists.
         //   Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            Windows.Storage.StorageFile sampleFile = await storageFolder.CreateFileAsync(FileToCreateName, Windows.Storage.CreationCollisionOption.ReplaceExisting);

            ("File created successfully.").Show();

        }

        private async void ReadFile_Click(object sender, RoutedEventArgs e)
        {

            ReadAllFiles();
        }

        private async void ReadAllFiles()
        {
            // Get the app's installation folder.
            Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;

            // Get the files in the current folder.
            StorageFileQueryResult results = storageFolder.CreateFileQuery();

            // Iterate over the results and print the list of files
            // to the Visual Studio Output window.
            IReadOnlyList<StorageFile> filesInFolder = await results.GetFilesAsync();

            foreach (StorageFile item in filesInFolder)
            {
                TextBlock tb = new TextBlock();
                tb.Text = item.Name;

                StackPanel sp = new StackPanel();
                sp.Tag = item.Name;
                sp.Orientation = Orientation.Vertical;
                sp.Tapped += Sp_Tapped;
                sp.Children.Add(tb);

                outerWrapper.Children.Add(sp);
            }
        }

        private void Sp_Tapped(object sender, TappedRoutedEventArgs e)
        {
            StackPanel tappedSp = sender as StackPanel;

            SelectedFile.Text = tappedSp.Tag.ToString();
        }

        private async void SaveText_Click(object sender, RoutedEventArgs e)
        {
            

            string textToWrite = TextForFile.Text.ToString();

            if(textToWrite.Length < 1)
            {
                ("Error: Nothing to write to the file.").Show();
                return;
            }


            string fileName = SelectedFile.Text;
            if(fileName.Length < 1)
            {
                ("Error: You have not selected a file.").Show();
                return;
            }

            Windows.Storage.StorageFolder storageFolder =
    Windows.Storage.ApplicationData.Current.LocalFolder;
            Windows.Storage.StorageFile sampleFile =
                await storageFolder.GetFileAsync(fileName);


            await Windows.Storage.FileIO.WriteTextAsync(sampleFile, textToWrite);
            ("File saved").Show();

        }

        private async void ReadFile_Click_1(object sender, RoutedEventArgs e)
        {

            string fileName = SelectedFile.Text;
            if (fileName.Length < 1)
            {
                ("Error: You have not selected a file.").Show();
                return;
            }


            Windows.Storage.StorageFolder storageFolder =
    Windows.Storage.ApplicationData.Current.LocalFolder;
            Windows.Storage.StorageFile sampleFile =
                await storageFolder.GetFileAsync(fileName);



            // string text = await Windows.Storage.FileIO.ReadTextAsync(sampleFile);

            string atext = "";
            var buffer = await Windows.Storage.FileIO.ReadBufferAsync(sampleFile);
            using (var dataReader = Windows.Storage.Streams.DataReader.FromBuffer(buffer))
            {
                atext = dataReader.ReadString(buffer.Length);
            }


            FileContents.Text = atext;


        }



        public static async Task<bool> EncryptAesFileAsync(StorageFile fileForEncryption, string aesKey256, string iv16lenght)
        {
            //private string AES_Key = "Y+3xQDLPWalRKK3U/JuabsJNnuEO91zRiOH5gjgOqck=";
            //private string AES_IV = "15CV1/ZOnVI3rY4wk4INBg==";
            // private IBuffer m_iv = null;
            //private CryptographicKey m_key;


        bool success = false;
            try
            {
                //Initialize key
                IBuffer key = Convert.FromBase64String(aesKey256).AsBuffer();
                var m_iv = Convert.FromBase64String(iv16lenght).AsBuffer();
                SymmetricKeyAlgorithmProvider provider = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesCbcPkcs7);
                var m_key = provider.CreateSymmetricKey(key);

                //secured data
                IBuffer data = await FileIO.ReadBufferAsync(fileForEncryption);
                IBuffer SecuredData = CryptographicEngine.Encrypt(m_key, data, m_iv);
                await FileIO.WriteBufferAsync(fileForEncryption, SecuredData);
                success = true;
            }
            catch (Exception ex)
            {
                success = false;
                ex.ToString().Show();
            }
            return success;

        }



        public static async Task<bool> DecryptAesFileAsync(StorageFile EncryptedFile, string aesKey256, string iv16lenght)
        {

            bool success = false;
            try
            {
                //Initialize key
                IBuffer key = Convert.FromBase64String(aesKey256).AsBuffer();
                var m_iv = Convert.FromBase64String(iv16lenght).AsBuffer();
                SymmetricKeyAlgorithmProvider provider = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesCbcPkcs7);
                var m_key = provider.CreateSymmetricKey(key);

                //Unsecured Data
                IBuffer data = await FileIO.ReadBufferAsync(EncryptedFile);
                IBuffer UnSecuredData = CryptographicEngine.Decrypt(m_key, data, m_iv);
                await FileIO.WriteBufferAsync(EncryptedFile, UnSecuredData);
                success = true;
            }
            catch (Exception ex)
            {
                success = false;
                ex.ToString().Show();
            }
            return success;

        }

        private async void EncFile_Click(object sender, RoutedEventArgs e)
        {
            string fileName = SelectedFile.Text;
            if (fileName.Length < 1)
            {
                ("Error: You have not selected a file.").Show();
                return;
            }


            Windows.Storage.StorageFolder storageFolder =
    Windows.Storage.ApplicationData.Current.LocalFolder;
            Windows.Storage.StorageFile sampleFile =
                await storageFolder.GetFileAsync(fileName);

            string AES_Key = "Y+3xQDLPWalRKK3U/JuabsJNnuEO91zRiOH5gjgOqck=";
            string AES_IV = "15CV1/ZOnVI3rY4wk4INBg==";

            bool siker = await  EncryptAesFileAsync(sampleFile, AES_Key, AES_IV);
            if (siker == true)
            {
                ("Sikerült titkosítani").Show();
            }

        }

        private async void DecFile_Click(object sender, RoutedEventArgs e)
        {
            string fileName = SelectedFile.Text;
            if (fileName.Length < 1)
            {
                ("Error: You have not selected a file.").Show();
                return;
            }


            Windows.Storage.StorageFolder storageFolder =
    Windows.Storage.ApplicationData.Current.LocalFolder;
            Windows.Storage.StorageFile sampleFile =
                await storageFolder.GetFileAsync(fileName);

            string AES_Key = "Y+3xQDLPWalRKK3U/JuabsJNnuEO91zRiOH5gjgOqck=";
            string AES_IV = "15CV1/ZOnVI3rY4wk4INBg==";

            bool siker = await DecryptAesFileAsync(sampleFile, AES_Key, AES_IV);
            if(siker == true)
            {
                ("Sikerült megfejteni").Show();
            }
        }

        private async void SaveFileOnPc_Click(object sender, RoutedEventArgs e)
        {


            var savePicker = new Windows.Storage.Pickers.FileSavePicker();
            savePicker.SuggestedStartLocation =
                Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            // Dropdown of file types the user can save the file as
            savePicker.FileTypeChoices.Add("ehw file", new List<string>() { ".ehw" });
            // Default file name if the user does not type one in or select a file to replace
            savePicker.SuggestedFileName = "New Document";


            // read my file which I want to save

            string fileName = SelectedFile.Text;
            if (fileName.Length < 1)
            {
                ("Error: You have not selected a file.").Show();
                return;
            }


            Windows.Storage.StorageFolder storageFolder =
    Windows.Storage.ApplicationData.Current.LocalFolder;
            Windows.Storage.StorageFile sampleFile =
                await storageFolder.GetFileAsync(fileName);


            string text = await Windows.Storage.FileIO.ReadTextAsync(sampleFile);
            var buffer = await Windows.Storage.FileIO.ReadBufferAsync(sampleFile);


            Windows.Storage.StorageFile file = await savePicker.PickSaveFileAsync();

            if (sampleFile != null)
            {
               
               await sampleFile.CopyAndReplaceAsync(file);
            }


            if (file != null)
            {
                // Prevent updates to the remote version of the file until
                // we finish making changes and call CompleteUpdatesAsync.
                Windows.Storage.CachedFileManager.DeferUpdates(file);
                // write to file
              //  await Windows.Storage.FileIO.WriteTextAsync(file, text);
                await Windows.Storage.FileIO.WriteBufferAsync(file, buffer);
               
                
                // Let Windows know that we're finished changing the file so
                // the other app can update the remote version of the file.
                // Completing updates may require Windows to ask for user input.
                Windows.Storage.Provider.FileUpdateStatus status =
                    await Windows.Storage.CachedFileManager.CompleteUpdatesAsync(file);
                if (status == Windows.Storage.Provider.FileUpdateStatus.Complete)
                {
                    FileContents.Text = "File " + file.Name + " was saved.";
                }
                else
                {
                    FileContents.Text = "File " + file.Name + " couldn't be saved.";
                }
            }
            else
            {
                FileContents.Text = "Operation cancelled.";
            }

        }




        static string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
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
        static byte[] EncryptStringToBytes_Aes(string plainText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encrypted;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
               


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
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }

        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string original = "Here is some data to encrypt!";
            string AES_Key = "Y+3xQDLPWalRKK3U/JuabsJNnuEO91zRiOH5gjgOqck=";
            string AES_IV = "15CV1/ZOnVI3rY4wk4INBg==";

            // you get the base64 encoded key from somewhere
            var base64Key = "+CffHxKmykUvCrrCILd4rZDBcrIoe3w89jnPNXYi0rg=";
            // convert it to byte[] or alternatively you could store your key as a byte[] 
            //   but that depends on how you set things up.
            var key = Convert.FromBase64String(base64Key);
            var plainText = "{\"title\":\"Conditionals, All Types -1\",\"instructions\":\"Complete the sentences to make correct conditional sentences.\"}";
            var encryptedText = EncryptStringToBase64String(plainText, key);
            var decryptedText = DecryptStringFromBase64String(encryptedText, key);

            aesresult.Text = encryptedText + " -- " + decryptedText;

        }

        static string EncryptStringToBase64String(string plainText, byte[] Key)
        {
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
        static string DecryptStringFromBase64String(string cipherText, byte[] Key)
        {

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
