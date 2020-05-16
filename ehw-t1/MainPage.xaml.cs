using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.Cryptography;
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
            ("Semmi").Show();

        }

       
 





        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
          



            // PROCESS button

            string[] elv = { " " };
            string[] words1 = rawSentence.Text.ToString().Split(elv, StringSplitOptions.None);

            char[] sepas = { '.', ',', ':', '?', '!' };
            char[] textChars = rawSentence.Text.ToCharArray();

            List<string> processedText = new List<string>();
            processedText.Clear();

            string temp = "";
            for (int z = 0; z < textChars.Length; z++)
            {
                // írásjeleket külön hozzáadni
                if (Array.Exists(sepas, element => element == textChars[z])) {

                    processedText.Add(temp);
                    temp = "";

                    processedText.Add(textChars[z].ToString());
                    continue;
                }


                if (textChars[z] == ' ')
                {
                    processedText.Add(temp);
                    temp = "";
                }

                temp += textChars[z];
            }

            wrapWords.Children.Clear();

            // contains all the words without puncutations

          



            for (int k = 0; k < processedText.Count; k++)
            {
              
                

                TextBlock tb = new TextBlock();
                tb.Text = processedText[k];

                var padding = new Thickness(2, 0, 0, 0);
                var abg = new SolidColorBrush(Colors.White);

                var needTap = true;
                if (processedText[k] == "," || processedText[k] == "?" || processedText[k] == ";" || processedText[k] == ":" || processedText[k] == ".")
                {

                    abg = new SolidColorBrush(Color.FromArgb(255, 48, 179, 221));
                    padding = new Thickness(0, 0, 0, 0);
                    needTap = false;

                }
              


                ListBoxItem lb = new ListBoxItem();
                lb.Content = tb;
                lb.Padding = padding;
                lb.Background = abg;
                if (needTap)
                {
                    lb.Tag = "0";
                    lb.Name =  k.ToString(); // max 1000 words!
                    lb.Tapped += Lb_Tapped;
                }
                else
                {
                    lb.Tag = "x";
                    lb.Name = k.ToString();
                }
               
                wrapWords.Children.Add(lb);

               

            }




            chosenWords.Children.Clear();

        }


        public class Lexi
        {
            public string id { get; set; }
            public int idsor { get; set; }
            public string word { get; set; }
        }

        List<Lexi> globLexi = new List<Lexi>();

       



        private void Lb_Tapped(object sender, TappedRoutedEventArgs e)
        {
           

            ListBoxItem tappedLb = sender as ListBoxItem;

          

            if(tappedLb.Tag.ToString() == "0")
            {
                // új kiválasztás
                tappedLb.Tag = "1";
            }
            else
            {
                tappedLb.Tag = "0";
                // delete only this box
            }



            for (int i = 0; i < wrapWords.Children.Count; i++)
            {
                ListBoxItem item = wrapWords.Children[i] as ListBoxItem;
                if (item.Tag.ToString() == "1")
                {
                    item.Background = new SolidColorBrush(Colors.LightGreen);
                }
                else if (item.Tag.ToString() == "0")
                {
                    item.Background = new SolidColorBrush(Colors.White);
                }
            }



            List<string> lexicalItems = new List<string>();
            lexicalItems.Clear();
            globLexi.Clear();

            string temp = String.Empty;
          
            string ids = "";
            for (int z = 0; z < wrapWords.Children.Count; z++)
            {
                ListBoxItem theItem = wrapWords.Children[z] as ListBoxItem;
                TextBlock atb = theItem.Content as TextBlock;

                if (theItem.Tag.ToString() == "1")
                {

                   
                    temp += atb.Text.ToString();
                    ids += theItem.Name.ToString();
                  
                    
                }
                else
                {
                    if(temp != String.Empty)
                    {
                        lexicalItems.Add(temp);
                        Lexi egylexi = new Lexi();
                        egylexi.word = temp;
                        egylexi.id = ids;
                        egylexi.idsor = z;
                        globLexi.Add(egylexi);
                       
                        temp = String.Empty;
                        ids = "";
                    }
                }


            }
          
            // collect ids
            List<string> ides = new List<string>();
            ides.Clear();
            string mindides = "";
            for (int c = 0; c < globLexi.Count; c++)
            {
                ides.Add(globLexi[c].id);
                mindides += globLexi[c].id.ToString() + " -";
            }

            string marbeirt = "";
            for (int u = 0; u < chosenWords.Children.Count; u++)
            {
                StackPanel aWrap = chosenWords.Children[u] as StackPanel;
                marbeirt += aWrap.Tag.ToString() + " -";

            }

                // remove unused edit boxex

          

            // globLexi = globLexi.OrderBy(p => p.idsor).ToList();

            globlexiNum.Text = globLexi.Count.ToString() + " ides: " + mindides;

            // chosenWords distractor boxes 



            for (int c = 0;c < globLexi.Count; c++)
            {
             
                StackPanel wrapper = new StackPanel();
                wrapper.Orientation = Orientation.Vertical;
                wrapper.Tag = globLexi[c].id;
                wrapper.Name = globLexi[c].idsor.ToString();
                
                
                string currid = globLexi[c].id;
              
                Button plus = new Button();
                plus.Content = "+";
                plus.Click += Plus_Click;
                Button minus = new Button();
                minus.Content = "-";
                Button connected = new Button();
                connected.Content = "c";

                StackPanel head = new StackPanel();
                head.Orientation = Orientation.Horizontal;
             
                head.Children.Add(plus);
                head.Children.Add(minus);
                head.Children.Add(connected);

                wrapper.Children.Add(head);


                // distractors
                StackPanel distWrapper = new StackPanel();
                distWrapper.Orientation = Orientation.Horizontal;

                TextBox tb1 = new TextBox();
                tb1.Width = 100;
                tb1.Text = globLexi[c].word;



                Button corr = new Button();
                corr.Content = globLexi[c].id.ToString();


                distWrapper.Children.Add(tb1);
                distWrapper.Children.Add(corr);

                TextBox tb2 = new TextBox();
                tb2.Text = "";

                StackPanel distWrapper2 = new StackPanel();
                distWrapper2.Orientation = Orientation.Horizontal;

                Button corr2 = new Button();
                corr2.Content = "p";
                distWrapper2.Children.Add(tb2);
                distWrapper2.Children.Add(corr2);

                StackPanel dists = new StackPanel();
                dists.Orientation = Orientation.Vertical;

                dists.Children.Add(distWrapper);
                dists.Children.Add(distWrapper2);

                wrapper.Children.Add(dists);

                // Add the current box if there's no such and id

                // currid

                var van = false;
                for (int a = 0; a < chosenWords.Children.Count; a++)
                {
                    StackPanel theWrapper = chosenWords.Children[a] as StackPanel;
                    int asor = Convert.ToInt16(theWrapper.Tag);
                    if (theWrapper.Tag.ToString() == currid)
                    {
                        van = true;
                        break;
                    }
                }
                    
                // insert swhere

               


                // ha nincs ilyen, akkor beletenni, de hova
                if (van == false)
                {

                    if(chosenWords.Children.Count > 0)
                    {
                        bool inserted = false;
                        for (int i = 0; i < chosenWords.Children.Count; i++)
                        {
                            StackPanel theWrapper = chosenWords.Children[i] as StackPanel;

                            if (Convert.ToInt16(theWrapper.Name) > Convert.ToInt16(wrapper.Name))
                            {
                                chosenWords.Children.Insert(i, wrapper);
                                inserted = true;
                               
                                break;
                            }

                        }

                        if(inserted == false)
                        {

                            chosenWords.Children.Add(wrapper);
                           
                        }


                    }
                    else
                    {
                        chosenWords.Children.Add(wrapper);
                    }



                   
                }
            }

            

            // remove boxes
            List<string> beirtIds = new List<string>();
            beirtIds.Clear();
            for (int u = 0; u < chosenWords.Children.Count; u++)
            {
                StackPanel aWrap = chosenWords.Children[u] as StackPanel;
                beirtIds.Add(aWrap.Tag.ToString());
            }

            for(int u = 0; u < beirtIds.Count; u++)
            {
                bool vanez = false;
               
                for (int f = 0; f < ides.Count; f++)
                {

                    if (ides[f] == beirtIds[u])
                    {
                        vanez = true;

                    }
                }
                if(vanez == false)
                {
                    removeEditBox(beirtIds[u]);
                }
            }

          
        }

        private void removeEditBox(string azid)
        {
            for (int u = 0; u < chosenWords.Children.Count; u++)
            {
                StackPanel aWrap = chosenWords.Children[u] as StackPanel;
                if(aWrap.Tag.ToString() == azid)
                {
                    chosenWords.Children.RemoveAt(u);
                }
            }
        }

        private void Plus_Click(object sender, RoutedEventArgs e)
        {
            Button plusButton = sender as Button;
            StackPanel headWrap = plusButton.Parent as StackPanel;
            StackPanel allWrap = headWrap.Parent as StackPanel;

            StackPanel contentWrap = allWrap.Children[1] as StackPanel;



            TextBox tb1 = new TextBox();
            tb1.Text = "";

            Button corr = new Button();
            corr.Content = "p";

            StackPanel kisWrap = new StackPanel();
            kisWrap.Orientation = Orientation.Horizontal;
            kisWrap.Children.Add(tb1);
            kisWrap.Children.Add(corr);

            contentWrap.Children.Add(kisWrap);
           

           // int kids = contentWrap.Children.Count;
           // kids.ToString().Show();
        }

        private void PageNavi_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Tasktype0));
        }

        private void RegiButt_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Register));
        }

        private void ManageFiles_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(FileManagement1));
        }
    }
    }
