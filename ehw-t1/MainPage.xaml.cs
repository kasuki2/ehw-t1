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
using Windows.UI;
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
            string textToSave = text.EncryptStr();
            
           // var buffer = await Windows.Storage.FileIO.ReadBufferAsync(sampleFile);


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
                await Windows.Storage.FileIO.WriteTextAsync(file, textToSave);
              //  await Windows.Storage.FileIO.WriteBufferAsync(file, buffer);


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






        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
         
          
            var plainText = "{\"title\":\"Conditionals, All Types -1\",\"instructions\":\"Complete the sentences to make correct conditional sentences.\"}";
            var encryptedText = plainText.EncryptStr();
            var decryptedText = encryptedText.DecryptStr();

            aesresult.Text = encryptedText + " -- " + decryptedText;

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            char[] elv = { ' ', '.', '!', '?', ':', ';', ',' };
            string[] words1 = rawSentence.Text.ToString().Split(elv, StringSplitOptions.None);

            for(int i = 0; i < words1.Length; i++)
            {
                string theWord = words1[i];

                TextBlock tb = new TextBlock();
                tb.Text = theWord;


                var padding = new Thickness(0, 0, 0, 0);
                var abg = new SolidColorBrush(Color.FromArgb(255, 48, 179, 221));


                if (theWord == " " || theWord == "," || theWord == "?" || theWord == ";" || theWord == ":" || theWord == ".")
                {

                    // var abg = new SolidColorBrush(Color.FromArgb(255, 48, 179, 221));
                    abg = new SolidColorBrush(Colors.AliceBlue);

                } else
                {
                    padding = new Thickness(2, 0, 0, 0);
                    abg = new SolidColorBrush(Colors.White);
                }
               

                ListBoxItem lb = new ListBoxItem();
                lb.Content = tb;
                lb.Padding = padding;
                lb.Background = abg;

                wrapWords.Children.Add(lb);
            }

            






        }
    }
}
