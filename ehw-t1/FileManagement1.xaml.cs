using Newtonsoft.Json;
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

                DrawFilesAndFolders(globDirContents);

            }
            else
            {
                result.Text = "No authentication data was found. You need to register first.";
            }

          

           

        }


        private void DrawFilesAndFolders(List<DirContent> FilesAndFolders)
        {
            forFiles.Children.Clear();


            for (int i = 0; i < FilesAndFolders.Count; i++)
            {
                if(FilesAndFolders[i].dir == true)
                {
                    ListBoxItem egydir = new ListBoxItem();
                    egydir.Background = new SolidColorBrush(Windows.UI.Colors.Gray);
                    egydir.Content = FilesAndFolders[i].name;
                    egydir.Tag = FilesAndFolders[i].path;
                    egydir.Tapped += Egydir_Tapped;
                    forFiles.Children.Add(egydir);

                }
                else
                {
                    ListBoxItem egydir = new ListBoxItem();
                    egydir.Background = new SolidColorBrush(Windows.UI.Colors.Yellow);
                    egydir.Content = FilesAndFolders[i].name + " path: " + FilesAndFolders[i].path; 
                    forFiles.Children.Add(egydir);
                }

                
            }
        }

        private async void Egydir_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ListBoxItem a_folder = sender as ListBoxItem;
            string neve = a_folder.Content.ToString();
            string apath = a_folder.Tag.ToString() + "/" + neve;


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

                DrawFilesAndFolders(globDirContents);
            }


        }
    }
}
