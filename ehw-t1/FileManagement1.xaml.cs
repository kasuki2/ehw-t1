﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
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
        }

        public class DirContent
        {
            public string name { get; set; }
            public bool dir { get; set; }
            public string path { get; set; }
        }

       
        
        


        private async void GetFiles_Click(object sender, RoutedEventArgs e)
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

                    TextBlock nameTb = new TextBlock();
                    nameTb.Text = FilesAndFolders[i].name;
                    nameTb.Tag = FilesAndFolders[i].path;
                    nameTb.Tapped += NameTb_Tapped;
                    Grid.SetColumn(nameTb, 1);

                    headGrid.Children.Add(nameTb);

                    Button addFolder = new Button();
                    addFolder.Content = "(+)";
                    addFolder.Tag = FilesAndFolders[i].path;
                    Grid.SetColumn(addFolder, 2);

                    headGrid.Children.Add(addFolder);

                    Button addFile = new Button();
                    addFile.Content = "+f";
                    addFile.Tag = FilesAndFolders[i].path;
                    Grid.SetColumn(addFile, 3);
                    headGrid.Children.Add(addFile);

                    // content stackpanel
                    StackPanel contentStack = new StackPanel();
                    Grid.SetRow(contentStack, 1);



                    Grid.SetRow(headGrid, 0);
                    wrapGrid.Children.Add(headGrid);
                    wrapGrid.Children.Add(contentStack);

                    contentSt.Children.Add(wrapGrid);

                }
                else
                {
                    ListBoxItem egydir = new ListBoxItem();
                    egydir.Background = new SolidColorBrush(Windows.UI.Colors.Yellow);
                    egydir.Content = FilesAndFolders[i].name + " path: " + FilesAndFolders[i].path;
                    contentSt.Children.Add(egydir);
                }

                
            }
        }

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
            contentStack.Margin = new Thickness(4, 0, 0, 0);

            string logindat = ("logindata").GetStore();
            if (logindat != null)
            {

                UserData userdata = JsonConvert.DeserializeObject<UserData>(logindat);
                userdata.code = 3;
                userdata.path = apath;

                result.Text = apath;

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

      
    }
}
