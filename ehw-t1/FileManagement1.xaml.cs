﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace ehw_t1
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FileManagement1 : Page
    {
        public FileManagement1()
        {
            this.InitializeComponent();
            getTheFiles();
        }

        public class DirContent
        {
            public string name { get; set; }
            public bool dir { get; set; }
            public string path { get; set; }
        }

       
        public class FolderResponse
        {
            public string name { get; set; }
            public int code { get; set; }
        }
        


        private void GetFiles_Click(object sender, RoutedEventArgs e)
        {

            getTheFiles();

        }

        private async void getTheFiles()
        {
            string logindat = ("logindata").GetStore();

            if (logindat != null)
            {
                UserData userdata = JsonConvert.DeserializeObject<UserData>(logindat);
                userdata.code = 3;
                userdata.path = "";
                string thejson = JsonConvert.SerializeObject(userdata);

                Dictionary<string, string> pairs = new Dictionary<string, string>();

                pairs.Add("json", thejson);

                //string valasz = await TryPostJsonAsync(pairs);
                string valasz = await pairs.PostJsonAsync("http://kashusoft.org/uwpehw/src/client_teacher.php");

                //  DirContent[] globDirContents = JsonConvert.Deserialize <DirContent>(valasz);
                List<DirContent> globDirContents = JsonConvert.DeserializeObject<List<DirContent>>(valasz);
                result.Text = globDirContents.Count.ToString();

                DrawFilesAndFolders(globDirContents, forFiles);

            }
            else
            {
                result.Text = "No authentication data was found. You need to register first.";
            }
        }

        private void DrawFilesAndFolders(List<DirContent> FilesAndFolders, StackPanel contentSt)
        {
         //   contentSt.Children.Clear();
            if(FilesAndFolders.Count == 0)
            {
                ("Empty folder").Show();
                return;
            }

            

            contentSt.Children.Clear();
            for (int i = 0; i < FilesAndFolders.Count; i++)
            {
                if(FilesAndFolders[i].dir == true)
                {
                    // wrap grid
                    Grid wrapGrid = new Grid();
                    RowDefinition row1 = new RowDefinition();
                    RowDefinition row2 = new RowDefinition();
                    wrapGrid.RowDefinitions.Add(row1);
                    wrapGrid.RowDefinitions.Add(row2);
                    wrapGrid.Tag = "valami";

                    // head grid
                    Grid headGrid = new Grid();
                    RowDefinition hrow = new RowDefinition();
                    headGrid.RowDefinitions.Add(hrow);
                    ColumnDefinition col1 = new ColumnDefinition();
                    col1.Width = new GridLength(0, GridUnitType.Auto);
                    ColumnDefinition col2 = new ColumnDefinition();
                    col2.Width = new GridLength(1, GridUnitType.Star);
                    ColumnDefinition col3 = new ColumnDefinition();
                    col3.Width = new GridLength(0, GridUnitType.Auto);
                    ColumnDefinition col4 = new ColumnDefinition();
                    col4.Width = new GridLength(0, GridUnitType.Auto);
                    headGrid.ColumnDefinitions.Add(col1);
                    headGrid.ColumnDefinitions.Add(col2);
                    headGrid.ColumnDefinitions.Add(col3);
                    headGrid.ColumnDefinitions.Add(col4);

                    //E188
                    FontIcon onlyFolder = new FontIcon();
                    onlyFolder.FontFamily = new FontFamily("Segoe MDL2 Assets");
                    onlyFolder.Glyph = "\xE188";
                    onlyFolder.Foreground = new SolidColorBrush(Color.FromArgb(255,30,30,30));
                    Grid.SetColumn(onlyFolder, 0);
                    headGrid.Children.Add(onlyFolder);

                    TextBlock nameTb = new TextBlock();
                    nameTb.Text = FilesAndFolders[i].name;
                    nameTb.Tag = FilesAndFolders[i].path;
                    nameTb.Margin = new Thickness(4, 0, 0, 0);
                    nameTb.Tapped += NameTb_Tapped;
                    Grid.SetColumn(nameTb, 1);

                    headGrid.Children.Add(nameTb);

                    Button addFolder = new Button();
                    addFolder.Content = "[+]";

                    addFolder.Tag = FilesAndFolders[i].path + "/" + FilesAndFolders[i].name; 
                    addFolder.Tapped += AddFolder_Tapped;
                    Grid.SetColumn(addFolder, 2);

                    headGrid.Children.Add(addFolder);

                    Button addFile = new Button();
                    addFile.Content = "+f";
                    addFile.Tag = FilesAndFolders[i].path + "/" + FilesAndFolders[i].name;
                    addFile.Tapped += AddFile_Tapped;
                    Grid.SetColumn(addFile, 3);
                    headGrid.Children.Add(addFile);

                    // content stackpanel
                    StackPanel contentStack = new StackPanel();
                    contentStack.Margin = new Thickness(24, 0, 0, 0);
                    Grid.SetRow(contentStack, 1);



                    Grid.SetRow(headGrid, 0);
                    wrapGrid.Children.Add(headGrid);
                    wrapGrid.Children.Add(contentStack);

                    contentSt.Children.Add(wrapGrid);

                }
                else
                {
                    ListBoxItem egydir = new ListBoxItem();
                    egydir.Padding = new Thickness(0);
                    egydir.Background = new SolidColorBrush(Windows.UI.Colors.AliceBlue);
                    egydir.Content = FilesAndFolders[i].name;
                    egydir.Tag = FilesAndFolders[i].path + "/" + FilesAndFolders[i].name;
                    egydir.Tapped += Egydir_Tapped; // file
                    contentSt.Children.Add(egydir);
                }

                
            }
        }

        public class TaskFrame
        {
            public string title { get; set; }
            public string uid { get; set; }
            public string instructions { get; set; }
            public int type { get; set; }
            public object[] contents { get; set; }
            public string weight;
        }

        public class TaskMainFrame
        {
            public string title { get; set; }
            public string instructions { get; set; }
            public int type { get; set; }
            public object[] contents { get; set; }
            public string weight;
        }

        public class Tasktype0
        {
            public int id { get; set; }
            public List<string> sentence { get; set; }
            public List<List<string>> distractors { get; set; }
            public List<string> solu { get; set; }
            public List<List<string>> remarks { get; set; }
        }

        private async void Egydir_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //file tapped
            ListBoxItem fileTapped = sender as ListBoxItem;
            string filePath = fileTapped.Tag.ToString();

            filePath.Show();

            string logindat = ("logindata").GetStore();
            if (logindat != null)
            {
                UserData userdata = JsonConvert.DeserializeObject<UserData>(logindat);
                string azemail = userdata.email;
                string pw = userdata.pw;


                await Windows.UI.Xaml.Controls.WebView.ClearTemporaryWebDataAsync();
                webView1.Navigate(new Uri("https://kashusoft.org/uwpehw/src/gettaskfile.php?apikey=32&mail=" + azemail + "&pw=" + pw + "&task=" + filePath));

                // get file data, 
                

                userdata.code = 7;
                userdata.path = filePath;

                string thejson = JsonConvert.SerializeObject(userdata);

                Dictionary<string, string> pairs = new Dictionary<string, string>();
                pairs.Add("json", thejson);

                string valasz = await pairs.PostJsonAsync("http://kashusoft.org/uwpehw/src/client_teacher.php");
               
                if(valasz != "")
                {
                    TaskFrame taskFrame = JsonConvert.DeserializeObject<TaskFrame>(valasz);
                    jsonType10.Text = taskFrame.instructions;


                    File_name.Text = filePath;
                    Task_title.Text = taskFrame.title;
                    Task_instructions.Text = taskFrame.instructions;

                    string taskTypeText = "No type specified error.";
                    if(taskFrame.type == 0)
                    {
                        taskTypeText = "Type 1 - Multiple choice with popup.";
                    }
                    else if(taskFrame.type == 1)
                    {
                        taskTypeText = "Type 2 - Fill in the gaps.";
                    }
                    else if (taskFrame.type == 2)
                    {
                        taskTypeText = "Type 3 - Multiple choice.";
                    }
                    else if (taskFrame.type == 3)
                    {
                        taskTypeText = "Type 4 - Vocabulary.";
                    }
                    else if (taskFrame.type == 6)
                    {
                        taskTypeText = "Type 5 - Rewrite the sentences.";
                    }
                    else if (taskFrame.type == 9)
                    {
                        taskTypeText = "Type 6 - Complete the sentences.";
                    }
                    else if (taskFrame.type == 10)
                    {
                        taskTypeText = "Type 7 - Complete the text.";
                    }

                        selected_task_type.Text = taskTypeText;

                    string lev = "Level" + taskFrame.weight.ToString();
                    RadioButton leve = this.FindName(lev) as RadioButton;
                    leve.IsChecked = true;
                } 
               





                // UserData userdata = JsonConvert.DeserializeObject<UserData>(logindat);
                // userdata.code = 6;
                // userdata.path = filePath;

                // //   result.Text = apath;

                // string thejson = JsonConvert.SerializeObject(userdata);

                // Dictionary<string, string> pairs = new Dictionary<string, string>();

                // pairs.Add("json", thejson);

                // //string valasz = await TryPostJsonAsync(pairs);
                // string valasz = await pairs.PostJsonAsync("http://kashusoft.org/uwpehw/src/client_teacher.php");

                //TaskFrame taskFrame = JsonConvert.DeserializeObject<TaskFrame>(valasz);
                // //  List<DirContent> globDirContents = JsonConvert.DeserializeObject<List<DirContent>>(valasz);
                // if(taskFrame != null)
                // {
                //     Task_title.Text = taskFrame.title;
                //     Task_instructions.Text = taskFrame.instructions;

                //     if( taskFrame.weight == "0")
                //     {
                //         Level0.IsChecked = true;
                //     }
                //     else if(taskFrame.weight == "1")
                //     {
                //         Level1.IsChecked = true;
                //     }
                //     else if(taskFrame.weight == "2")
                //     {
                //         Level2.IsChecked = true;
                //     }
                //     else if(taskFrame.weight == "3")
                //     {
                //         Level3.IsChecked = true;
                //     }
                //     else if(taskFrame.weight == "4")
                //     {
                //         Level4.IsChecked = true;
                //     }
                //     else if(taskFrame.weight == "5")
                //     {
                //         Level5.IsChecked = true;
                //     }


                // }
                // else
                // {
                //     ("Empty file").Show();

                // }

                // // taskContent.Text = valasz;
                // string contentObj = JsonConvert.SerializeObject(taskFrame.contents);
                // if(taskFrame.type == 0)
                // {
                //     List<Tasktype0> taskContents = JsonConvert.DeserializeObject<List<Tasktype0>>(contentObj);
                //     taskProp.Text = taskContents.Count.ToString();
                // }

                // taskContent.Text = contentObj;
            }



        }



        private void AddNewFileMode()
        {
            File_name.Visibility = Visibility.Collapsed;
            save_base_data.Visibility = Visibility.Collapsed;

            NewFileName.Visibility = Visibility.Visible;
            NewFilePath.Visibility = Visibility.Visible;
            newFileStack.Visibility = Visibility.Visible;
            tasktypeGrid.Visibility = Visibility.Visible;
        }

        private void Cancel_newfile_Click(object sender, RoutedEventArgs e)
        {
            ChangeBaseDateMode();
        }

        private void ChangeBaseDateMode()
        {
            NewFileName.Visibility = Visibility.Collapsed;
            NewFilePath.Visibility = Visibility.Collapsed;
            newFileStack.Visibility = Visibility.Collapsed;
            tasktypeGrid.Visibility = Visibility.Collapsed;

            File_name.Visibility = Visibility.Visible;
            save_base_data.Visibility = Visibility.Visible;
        }


        private void AddFile_Tapped(object sender, TappedRoutedEventArgs e)
        {
            AddNewFileMode();

            Button addFoldButt = sender as Button;
            string apath = addFoldButt.Tag.ToString();

            NewFilePath.Text = apath;

            Grid headGr = addFoldButt.Parent as Grid;
            Grid wrapGr = headGr.Parent as Grid;
            globContentSt = wrapGr.Children[1] as StackPanel;

            //popupText.Text = "Give a name to the file:";
            //addFolderPopup.Tag = "file";
            //Button addFoldButt = sender as Button;
            //string apath = addFoldButt.Tag.ToString();
            //file_name.Tag = apath; // give over the folder name in which we create the new folder

            //Grid headGr = addFoldButt.Parent as Grid;
            //Grid wrapGr = headGr.Parent as Grid;

            //globContentSt = wrapGr.Children[1] as StackPanel;

            //popup.Visibility = Visibility.Visible;
        }

        private StackPanel globContentSt;
        private void AddFolder_Tapped(object sender, TappedRoutedEventArgs e)
        {
            popupText.Text = "Give a name to the folder:";
            addFolderPopup.Tag = "folder";
            Button addFoldButt = sender as Button;
            string apath = addFoldButt.Tag.ToString();
            file_name.Tag = apath; // give over the folder name in which we create the new folder

            Grid headGr = addFoldButt.Parent as Grid;
            Grid wrapGr = headGr.Parent as Grid;

            globContentSt = wrapGr.Children[1] as StackPanel;

            popup.Visibility = Visibility.Visible;
        }
        private void ClosePopup_Click(object sender, RoutedEventArgs e)
        {
            popup.Visibility = Visibility.Collapsed;
        }


        // folder tapped

        private async void NameTb_Tapped(object sender, TappedRoutedEventArgs e)
        {
            TextBlock tappedFolder = sender as TextBlock;
            string neve = tappedFolder.Text;
            string apath = tappedFolder.Tag.ToString() + "/" + neve; 
         

            Grid headGr = tappedFolder.Parent as Grid;
            Grid wrapGr = headGr.Parent as Grid;

            StackPanel contentStack = wrapGr.Children[1] as StackPanel;
            int kids = contentStack.Children.Count;
            if(kids > 0)
            {
                contentStack.Children.Clear();
                return;
            }
         //   contentStack.Margin = new Thickness(12, 0, 0, 0);

            string logindat = ("logindata").GetStore();
            if (logindat != null)
            {

                UserData userdata = JsonConvert.DeserializeObject<UserData>(logindat);
                userdata.code = 3;
                userdata.path = apath;

             //   result.Text = apath;

                string thejson = JsonConvert.SerializeObject(userdata);

                Dictionary<string, string> pairs = new Dictionary<string, string>();

                pairs.Add("json", thejson);

                //string valasz = await TryPostJsonAsync(pairs);
                string valasz = await pairs.PostJsonAsync("http://kashusoft.org/uwpehw/src/client_teacher.php");

                //  DirContent[] globDirContents = JsonConvert.Deserialize <DirContent>(valasz);
                List<DirContent> globDirContents = JsonConvert.DeserializeObject<List<DirContent>>(valasz);
                result.Text = valasz;

                DrawFilesAndFolders(globDirContents, contentStack);
            }

        }

       

        private async void AddFolderPopup_Click(object sender, RoutedEventArgs e)
        {

            Button okbutt = sender as Button;

            int foldOrfile = 4; // folder
            if(okbutt.Tag.ToString() == "file")
            {
                foldOrfile = 5;
            }

            string ujFolderName = file_name.Text.Trim();
            string apath = file_name.Tag.ToString();

            if(ujFolderName.Length < 1 || ujFolderName.Length > 20)
            {
                ("Invalid folder name. Min. 1 max. 20 characters.").Show();
                return;
            }

            string logindat = ("logindata").GetStore();
            if (logindat != null)
            {

                UserData userdata = JsonConvert.DeserializeObject<UserData>(logindat);
                userdata.code = foldOrfile; // 4 folder, 5 file

                userdata.path = apath + "/" + ujFolderName;
              
                userdata.foldername = ujFolderName;
              
                result.Text = apath;

                string thejson = JsonConvert.SerializeObject(userdata);

                Dictionary<string, string> pairs = new Dictionary<string, string>();

                pairs.Add("json", thejson);

             
                string valasz = await pairs.PostJsonAsync("http://kashusoft.org/uwpehw/src/client_teacher.php");

                //  DirContent[] globDirContents = JsonConvert.Deserialize <DirContent>(valasz);
                FolderResponse folderResponse = JsonConvert.DeserializeObject<FolderResponse>(valasz);
                result.Text = valasz;
               // valasz.Show();


                // update file tree - only the one needed to update

               
                userdata.code = 3;
                userdata.path = apath;


                //   result.Text = apath;

                string thejson2 = JsonConvert.SerializeObject(userdata);

                Dictionary<string, string> pairs2 = new Dictionary<string, string>();

                pairs2.Add("json", thejson2);

             
                string valasz2 = await pairs2.PostJsonAsync("http://kashusoft.org/uwpehw/src/client_teacher.php");

              
                List<DirContent> globDirContents = JsonConvert.DeserializeObject<List<DirContent>>(valasz2);
             //   result.Text = valasz2;
                globContentSt.Children.Clear();
                DrawFilesAndFolders(globDirContents, globContentSt);

             
            }

        }



        private void drawType0(List<Tasktype0> contents)
        {
            Run run1 = new Run();
            run1.Text = "Hello majom.";
            Paragraph pari = new Paragraph();
            pari.Inlines.Add(run1);
            

            RichTextBlock rtb = new RichTextBlock();
            rtb.Blocks.Add(pari);

            textFrame.Children.Add(rtb);
        }

        private void drawSg()
        {
            textFrame.Children.Clear();

            List<string> sentence = new List<string>();
            sentence.Add("If we ");
            sentence.Add("more time, we ");
            sentence.Add("have been able to visit you in your home.");

            List<string> butts = new List<string>();
            butts.Add("had");
            butts.Add("would");

            string[] sepa = { " " };

            for (int i = 0; i < sentence.Count; i++)
            {
                
                string[] words = sentence[i].Split(sepa, StringSplitOptions.None);
                for(int w = 0; w < words.Length; w++)
                {
                    TextBlock lb = new TextBlock();
                    lb.Text = words[w];
                    lb.Margin = new Thickness(4, 0, 0, 0);

                    textFrame.Children.Add(lb);
                }
                if (i < sentence.Count - 1)
                {

                    Flyout flb = new Flyout();
                    

                    Style s = new Style { TargetType = typeof(FlyoutPresenter) };
                    s.Setters.Add(new Setter(BackgroundProperty, new SolidColorBrush(Color.FromArgb(255, 240, 255, 255))));
                    flb.FlyoutPresenterStyle = s;

                    TextBlock tbf = new TextBlock();
                    tbf.Text = "was";
                    tbf.Tapped += Tbf_Tapped;
                    TextBlock tbf2 = new TextBlock();
                    tbf2.Text = "were";

                    StackPanel flyStack = new StackPanel();
                    flyStack.Orientation = Orientation.Horizontal;
                    flyStack.Children.Add(tbf);
                    flyStack.Children.Add(tbf2);

                    flb.Content = flyStack;



                    Button butt = new Button();
                    butt.Content = butts[i];
                    butt.FontWeight = FontWeights.Bold;
                    butt.Padding = new Thickness(0);
                    butt.Margin = new Thickness(4, 0, 0, 0);
                    butt.Tapped += Butt_Tapped;
                    butt.ContextFlyout = flb;

                    textFrame.Children.Add(butt);
                }
               
            }


             

         

           // textFrame.Children.Add(rtb);
        }

        private void Tbf_Tapped(object sender, TappedRoutedEventArgs e)
        {
            TextBlock tappedFly = sender as TextBlock;
            StackPanel par1 = tappedFly.Parent as StackPanel;
            Flyout afly = par1.Parent as Flyout;
           


        }

        private void Butt_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Button tappedText = sender as Button;
            tappedText.ContextFlyout.ShowAt(tappedText);
        }

        private void Drawtext_Click(object sender, RoutedEventArgs e)
        {
            drawSg();
        }

        private async void FillWeb_Click(object sender, RoutedEventArgs e)
        {

            string logindat = ("logindata").GetStore();
            if (logindat != null)
            {

                UserData userdata = JsonConvert.DeserializeObject<UserData>(logindat);
                string azemail = userdata.email;
                string pw = userdata.pw;
                await Windows.UI.Xaml.Controls.WebView.ClearTemporaryWebDataAsync();
                webView1.Navigate(new Uri("http://kashusoft.org/uwpehw/src/gettaskfile.php?apikey=32&mail=" + azemail +"&pw=" + pw + "&task=" + "task"));
            }
               
        }

        private class Action
        {
            public string action { get; set; }
            public string num { get; set; }
        }
        private void WebView1_ScriptNotify(object sender, NotifyEventArgs e)
        {
            Action action = JsonConvert.DeserializeObject<Action>(e.Value);
            //  e.Value.Show();
            action.action.Show();
            
        }

        private void ProcessText10_Click(object sender, RoutedEventArgs e)
        {
            string textInp = rawText.Text;
            if(textInp.Length < 10)
            {
                ("The text is too short.").Show();
                return;
            }

            List<ListBoxItem> lb = textInp.ProcessText();

            chosenWordsT_10.Children.Clear();

            for (int i = 0; i < lb.Count; i++)
            {
                lb[i].Tapped += FileManagement1_Tapped;
                chosenWordsT_10.Children.Add(lb[i]);
            }

           



        }
        public class Lexi
        {
            public string id { get; set; }
            public int idsor { get; set; }
            public string word { get; set; }
        }

        List<Lexi> globLexi = new List<Lexi>();

        private void FileManagement1_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ListBoxItem tappedLb = sender as ListBoxItem;
            if (tappedLb.Tag.ToString() == "0")
            {
                tappedLb.Tag = "1";
            }
            else
            {
                tappedLb.Tag = "0";   
            }

            for (int i = 0; i < chosenWordsT_10.Children.Count; i++)
            {
                ListBoxItem item = chosenWordsT_10.Children[i] as ListBoxItem;
                if (item.Tag.ToString() == "1")
                {
                    item.Background = new SolidColorBrush(Colors.LightGreen);
                }
                else if (item.Tag.ToString() == "0")
                {
                    item.Background = new SolidColorBrush(Colors.White);
                }
            }


            globLexi.Clear();

            string temp = String.Empty;
            string ids = "";
            for (int z = 0; z < chosenWordsT_10.Children.Count; z++)
            {
                ListBoxItem theItem = chosenWordsT_10.Children[z] as ListBoxItem;
                TextBlock atb = theItem.Content as TextBlock;

                if (theItem.Tag.ToString() == "1")
                {
                    temp += atb.Text.ToString();
                    ids += theItem.Name.ToString();
                }
                else
                {
                    if (temp != String.Empty)
                    {                
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
            for (int c = 0; c < globLexi.Count; c++)
            {
                ides.Add(globLexi[c].id);
            }

            for (int c = 0; c < globLexi.Count; c++)
            {

                StackPanel wrapper = new StackPanel();
                wrapper.Orientation = Orientation.Horizontal;
                wrapper.Tag = globLexi[c].id;
                wrapper.Name = globLexi[c].idsor.ToString();
                wrapper.Padding = new Thickness(4);
                wrapper.Background = new SolidColorBrush(Color.FromArgb(255, 200, 200, 200));
                wrapper.Margin = new Thickness(0, 0, 8, 8);
               
               
                wrapper.BorderThickness = new Thickness(2, 2, 2, 2);
                wrapper.BorderBrush = new SolidColorBrush(Colors.Transparent);

                Grid wideGrid = new Grid();
                wideGrid.HorizontalAlignment = HorizontalAlignment.Stretch;
                wideGrid.MinWidth = 260;
             

                RowDefinition row1 = new RowDefinition();
                RowDefinition row2 = new RowDefinition();
                RowDefinition row3 = new RowDefinition();
                RowDefinition row4 = new RowDefinition();
                wideGrid.RowDefinitions.Add(row1);
                wideGrid.RowDefinitions.Add(row2);
                wideGrid.RowDefinitions.Add(row3);
                wideGrid.RowDefinitions.Add(row4);



                
                TextBlock clue = new TextBlock();
                if(globLexi[c].word.Length >= 15)
                {
                    clue.Text = globLexi[c].word.Substring(5, 9);
                }
                else if(globLexi[c].word.Length >= 10)
                {
                    clue.Text = globLexi[c].word.Substring(2, 5);
                }
                else if(globLexi[c].word.Length >= 5)
                {
                    clue.Text = globLexi[c].word.Substring(0, 4);
                }
                else
                {
                    clue.Text = globLexi[c].word;
                }
                
                clue.HorizontalAlignment = HorizontalAlignment.Stretch;

                
               


                wideGrid.Children.Add(clue);
                wideGrid.Background = new SolidColorBrush(Colors.AliceBlue);
                wideGrid.Padding = new Thickness(2);
                // Grid.SetColumn(clue, 0);
                Grid.SetRow(clue, 0);


                StackPanel stackBase = new StackPanel();
                stackBase.Orientation = Orientation.Vertical;

                TextBlock tbf = new TextBlock();
                tbf.Text = "Base form of the word:";
                TextBox baseForm = new TextBox();
                baseForm.Background = new SolidColorBrush(Colors.DarkRed);
                baseForm.Foreground = new SolidColorBrush(Colors.White);
                baseForm.HorizontalAlignment = HorizontalAlignment.Stretch;

                stackBase.Children.Add(tbf);
                stackBase.Children.Add(baseForm);

                wideGrid.Children.Add(stackBase);
                //Grid.SetColumn(baseForm, 1);
                Grid.SetRow(stackBase, 1);


                StackPanel alternatives = new StackPanel();
                alternatives.Orientation = Orientation.Vertical;
                alternatives.HorizontalAlignment = HorizontalAlignment.Stretch;

                TextBlock alt0 = new TextBlock();
                alt0.Text = "Correct alternatives:";

                StackPanel buttons = new StackPanel();
                buttons.Orientation = Orientation.Horizontal;
                Button pl = new Button();
                pl.Click += Pl_Click;
                pl.Content = "+";
                Button mi = new Button();
                mi.Click += Mi_Click;
                mi.Content = "-";
                buttons.Children.Add(pl);
                buttons.Children.Add(mi);
              

                TextBox alt1 = new TextBox();
                alt1.Text = globLexi[c].word;
                alt1.HorizontalAlignment = HorizontalAlignment.Stretch;
                alternatives.Children.Add(alt0);
                alternatives.Children.Add(buttons);
                alternatives.Children.Add(alt1);

                wideGrid.Children.Add(alternatives);
                Grid.SetRow(alternatives, 2);

                StackPanel stackExpl = new StackPanel();
                stackExpl.Orientation = Orientation.Vertical;

                TextBlock tex = new TextBlock();
                tex.Text = "Short explanation:";
                stackExpl.Children.Add(tex);

                TextBox expl = new TextBox();
                expl.HorizontalAlignment = HorizontalAlignment.Stretch;
                expl.Text = "Correct: '" + globLexi[c].word.Trim() + "'. ";
                stackExpl.Children.Add(expl);

                wideGrid.Children.Add(stackExpl);
               
                Grid.SetRow(stackExpl, 3);

                wrapper.Children.Add(wideGrid);
                string currid = globLexi[c].id;

              

                // Add the current box if there's no such and id

                // currid

                var van = false;
                for (int a = 0; a < editBoxes.Children.Count; a++)
                {
                    StackPanel theWrapper = editBoxes.Children[a] as StackPanel;
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

                    if (editBoxes.Children.Count > 0)
                    {
                        bool inserted = false;
                        for (int i = 0; i < editBoxes.Children.Count; i++)
                        {
                            StackPanel theWrapper = editBoxes.Children[i] as StackPanel;

                            if (Convert.ToInt16(theWrapper.Name) > Convert.ToInt16(wrapper.Name))
                            {
                                editBoxes.Children.Insert(i, wrapper);
                                inserted = true;
                                break;
                            }
                        }
                        if (inserted == false)
                        {
                            editBoxes.Children.Add(wrapper);
                        }
                    }
                    else
                    {
                        editBoxes.Children.Add(wrapper);
                    }

                }



            }

            // remove boxes
            List<string> beirtIds = new List<string>();
            beirtIds.Clear();
            for (int u = 0; u < editBoxes.Children.Count; u++)
            {
                StackPanel aWrap = editBoxes.Children[u] as StackPanel;
                beirtIds.Add(aWrap.Tag.ToString());
            }

            for (int u = 0; u < beirtIds.Count; u++)
            {
                bool vanez = false;

                for (int f = 0; f < ides.Count; f++)
                {

                    if (ides[f] == beirtIds[u])
                    {
                        vanez = true;

                    }
                }
                if (vanez == false)
                {
                    removeEditBox(beirtIds[u]);
                }
            }
        }

        private void Mi_Click(object sender, RoutedEventArgs e)
        {
            Button plusButt = sender as Button;
            StackPanel buttPar = plusButt.Parent as StackPanel;
            StackPanel altern = buttPar.Parent as StackPanel;
            int kids = altern.Children.Count;

            if(kids < 4)
            {
                ("There must be at least one correct solution.").Show();
                return;
            }

            altern.Children.RemoveAt(altern.Children.Count - 1);
        }

        private void Pl_Click(object sender, RoutedEventArgs e)
        {
            Button plusButt = sender as Button;
            StackPanel buttPar = plusButt.Parent as StackPanel;
            StackPanel altern = buttPar.Parent as StackPanel;
            int kids = altern.Children.Count;

            if(kids > 6)
            {
                ("The max. number of alternatives is 5.").Show();
                return;
            }

            TextBox corrAlt = new TextBox();
            altern.Children.Add(corrAlt);
        }

        private void removeEditBox(string azid)
        {
            for (int u = 0; u < editBoxes.Children.Count; u++)
            {
                StackPanel aWrap = editBoxes.Children[u] as StackPanel;
                if (aWrap.Tag.ToString() == azid)
                {
                    editBoxes.Children.RemoveAt(u);
                }
            }
        }


        public class TaskType10
        {
            public int id { get; set; }
            public List<string> sentence { get; set; }
            public List<string> words { get; set; }
            public List<string> clues { get; set; }
            public List<List<string>> alternatives { get; set; }
            public List<string> expls { get; set;  }
        }

        private async void Type10Save_Click(object sender, RoutedEventArgs e)
        {
            TaskType10 taskContent = new TaskType10();
            taskContent.id = 0;

            // checkings
            ClearErrors();
            string mind = "";
            List<string> theClues = new List<string>();
            theClues.Clear();
            List<string> baseForms = new List<string>();
            baseForms.Clear();
            List<List<string>> alterns = new List<List<string>>();
            List<string> explans = new List<string>();

            for (int u = 0; u < editBoxes.Children.Count; u++)
            {
                List<string> altern = new List<string>();

                StackPanel aWrap = editBoxes.Children[u] as StackPanel;
                // clues - are there any clues that are the same?
                Grid wideGr = aWrap.Children[0] as Grid;

                // baseform
                StackPanel baseStack = wideGr.Children[1] as StackPanel;
                TextBox baseF = baseStack.Children[1] as TextBox;
                if(baseF.Text.Length < 1)
                {
                    MarkError(u);
                    mind = "Error: You have not filled in at least one base form.";
                    break;
                }
                baseForms.Add(baseF.Text.Trim());

                // clues
                TextBlock theClue = wideGr.Children[0] as TextBlock;
                theClues.Add(theClue.Text.Trim());

                // alternatives
                StackPanel alternativeStack = wideGr.Children[2] as StackPanel;
                for(int a = 2; a < alternativeStack.Children.Count; a++)
                {
                    TextBox oneAlt = alternativeStack.Children[a] as TextBox;
                    altern.Add(oneAlt.Text.Trim());
                }

                alterns.Add(altern);

                // explanations
                StackPanel explStack = wideGr.Children[3] as StackPanel;
                TextBox theExpl = explStack.Children[1] as TextBox;


                explans.Add(theExpl.Text.Trim());



            }
          //  mind.Show();

            // get the sentence



            List<string> sentence = new List<string>();
            sentence.Clear();
            string ch = "";
            int ha = 0;
            string temp = "";
            for (int i = 0; i < chosenWordsT_10.Children.Count; i++)
            {

                ListBoxItem lbitem = chosenWordsT_10.Children[i] as ListBoxItem;
                if(lbitem.Tag.ToString() == "0" || lbitem.Tag.ToString() == "x")
                {
                    string eloke = " ";
                    if(lbitem.Tag.ToString() == "x")
                    {
                        eloke = "";
                    }

                    TextBlock lbcontent = lbitem.Content as TextBlock;
                    temp += lbcontent.Text;
                }
                else
                {
                    if(temp != "")
                    {
                        sentence.Add(temp.Trim());
                        ha++;
                        temp = "";
                    }
                  
                }

                //if (lbitem.Tag.ToString() != "1") // ha nem zöld
                //{
                //    TextBlock lbcontent = lbitem.Content as TextBlock;
                //    if (lbitem.Tag.ToString() == "x")
                //    {
                //        temp += lbcontent.Text;
                //    }
                //    else
                //    {
                //        temp += " " + lbcontent.Text;
                //    }

                //}
                //else
                //{
                //    sentence.Add(temp.Trim());
                //    temp = "";
                //}
            }
            sentence.Add(temp);
            ha++;
         //   ha.ToString().Show();

            taskContent.sentence = sentence;
            taskContent.words = baseForms;
            taskContent.clues = theClues;
            taskContent.alternatives = alterns;
            taskContent.expls = explans;

            string json = Newtonsoft.Json.JsonConvert.SerializeObject(taskContent);
            jsonType10.Text = json;


            string logindat = ("logindata").GetStore();
            if (logindat != null)
            {

                UserData userdata = JsonConvert.DeserializeObject<UserData>(logindat);
                userdata.code = 6; // 6 send new item for a file

              //  userdata.path = apath + "/" + newFileName;

              //  userdata.foldername = newFileName;

               // result.Text = apath;

                string thejson = JsonConvert.SerializeObject(userdata);

                Dictionary<string, string> pairs = new Dictionary<string, string>();

                //pairs.Add("json", thejson);
                //pairs.Add("filetitle", newFileTitle);
                //pairs.Add("instructions", instru);
                //pairs.Add("weight", weight.ToString());
                //pairs.Add("type", "10"); // task type


                string valasz = await pairs.PostJsonAsync("http://kashusoft.org/uwpehw/src/client_teacher.php");

            }

        }

        private void MarkError(int wrapper)
        {
            StackPanel theWrapper = editBoxes.Children[wrapper] as StackPanel;
            theWrapper.BorderBrush = new SolidColorBrush(Colors.Red);
        }

        private void ClearErrors()
        {
            for (int u = 0; u < editBoxes.Children.Count; u++)
            {
                StackPanel aWrap = editBoxes.Children[u] as StackPanel;
                aWrap.BorderBrush = new SolidColorBrush(Colors.Transparent);
            }
        }

        private async void Send10_Click(object sender, RoutedEventArgs e)
        {
            string filePath = File_name.Text;
            
            string contentStr = jsonType10.Text;
            if(contentStr.Length < 10)
            {
                ("The task is too short.").Show();
                return;
            }

            string logindat = ("logindata").GetStore();
            if (logindat != null)
            {

                UserData userdata = JsonConvert.DeserializeObject<UserData>(logindat);
                userdata.code = 8; // 6 send new item for a file

                userdata.path = filePath;

                //  userdata.foldername = newFileName;

                // result.Text = apath;

                string thejson = JsonConvert.SerializeObject(userdata);

                Dictionary<string, string> pairs = new Dictionary<string, string>();

                pairs.Add("json", thejson);
                pairs.Add("content", contentStr);
                //pairs.Add("instructions", instru);
                //pairs.Add("weight", weight.ToString());
                //pairs.Add("type", "10"); // task type
               
                string valasz = await pairs.PostJsonAsync("http://kashusoft.org/uwpehw/src/client_teacher.php");
                jsonType10.Text = valasz;

            }

        }

        private async void Save_new_file_Click(object sender, RoutedEventArgs e)
        {
            // get new file data and send

            string newFileName = NewFileName.Text.Trim();
            if(newFileName.Length < 3 || newFileName.Length > 60)
            {
                ("Invalid file name. Min. 3, max. 60 characters are allowed.").Show();
                return;
            }
            
            string newFileTitle = Task_title.Text.Trim();
            if(newFileTitle.Length < 3 || newFileTitle.Length > 80)
            {
                ("Invalid file title. Min. 3, max. 80 characters are allowed.").Show();
            }

            string instru = Task_instructions.Text.Trim();
            if(instru.Length < 3 || instru.Length > 300)
            {
                ("Invalid instructions. Min. 3, max. 300 characters are allowed").Show();
            }


            int weight = -1;
            if (Level0.IsChecked == true)
            {
                weight = 0;
            }
            else if(Level1.IsChecked == true)
            {
                weight = 1;
            }
            else if (Level2.IsChecked == true)
            {
                weight = 2;
            }
            else if (Level3.IsChecked == true)
            {
                weight = 3;
            }
            else if (Level4.IsChecked == true)
            {
                weight = 4;
            }
            else if (Level5.IsChecked == true)
            {
                weight = 5;
            }
            if(weight < 0)
            {
                ("You have not selected a level for the task.").Show();
                return;
            }



            string apath = NewFilePath.Text;

            string logindat = ("logindata").GetStore();
            if (logindat != null)
            {

                UserData userdata = JsonConvert.DeserializeObject<UserData>(logindat);
                userdata.code = 5; // 4 folder, 5 file

                userdata.path = apath + "/" + newFileName;

                userdata.foldername = newFileName;

                result.Text = apath;

                string thejson = JsonConvert.SerializeObject(userdata);

                Dictionary<string, string> pairs = new Dictionary<string, string>();
                selected_task_type.Tag.ToString().Show();
                pairs.Add("json", thejson);
                pairs.Add("filetitle", newFileTitle);
                pairs.Add("instructions", instru);
                pairs.Add("weight", weight.ToString());
                pairs.Add("type", selected_task_type.Tag.ToString()); // task type

               
                string valasz = await pairs.PostJsonAsync("http://kashusoft.org/uwpehw/src/client_teacher.php");

                //  DirContent[] globDirContents = JsonConvert.Deserialize <DirContent>(valasz);
                FolderResponse folderResponse = JsonConvert.DeserializeObject<FolderResponse>(valasz);
                result.Text = valasz;
                // valasz.Show();
               

                // update file tree - only the one needed to update


                userdata.code = 3;
                userdata.path = apath;


                //   result.Text = apath;

                string thejson2 = JsonConvert.SerializeObject(userdata);

                Dictionary<string, string> pairs2 = new Dictionary<string, string>();

                pairs2.Add("json", thejson2);


                string valasz2 = await pairs2.PostJsonAsync("http://kashusoft.org/uwpehw/src/client_teacher.php");


                List<DirContent> globDirContents = JsonConvert.DeserializeObject<List<DirContent>>(valasz2);
                //   result.Text = valasz2;
                globContentSt.Children.Clear();
                DrawFilesAndFolders(globDirContents, globContentSt);


            }

        }

        private void Type_0_Click(object sender, RoutedEventArgs e)
        {
            string typ = "";
            string tip = "0";
            RadioButton type_radio_butt = sender as RadioButton;
            if(type_radio_butt.Name == "type_0")
            {
                typ = "Type 1 - Multiple choice with popup.";
                tip = "0";
            }
            else if (type_radio_butt.Name == "type_1")
            {
                typ = "Type 2 - Fill in the gaps.";
                tip = "1";
            }
            else if (type_radio_butt.Name == "type_2")
            {
                typ = "Type 3 - Multiple choice.";
                tip = "2";
            }
            else if (type_radio_butt.Name == "type_3")
            {
                typ = "Type 4 - Vocabulary.";
                tip = "3";
            }
            else if (type_radio_butt.Name == "type_6")
            {
                typ = "Type 5 - Rewrite the sentences.";
                tip = "6";
            }
            else if (type_radio_butt.Name == "type_9")
            {
                typ = "Type 6 - Complete the sentences.";
                tip = "9";
            }
            else if (type_radio_butt.Name == "type_10")
            {
                typ = "Type 7 - Complete the text.";
                tip = "10";
            }
            selected_task_type.Text = typ;
            selected_task_type.Tag = tip;
        }
    }
}
