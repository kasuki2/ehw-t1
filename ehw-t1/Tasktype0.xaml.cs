using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
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
    public sealed partial class Tasktype0 : Page
    {
        public Tasktype0()
        {
            this.InitializeComponent();
        }

        private void Process_Click(object sender, RoutedEventArgs e)
        {
            // ProcessText
            string toSend = rawSentence.Text;
            List<ListBoxItem> lb = toSend.ProcessText();

            wrapWords.Children.Clear();

            for(int i = 0; i< lb.Count; i++)
            {
                lb[i].Tapped += Lb_Tapped;
                wrapWords.Children.Add(lb[i]);
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
            if (tappedLb.Tag.ToString() == "0")
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



  //          List<string> lexicalItems = new List<string>();
//            lexicalItems.Clear();
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
                    if (temp != String.Empty)
                    {
                       // lexicalItems.Add(temp);
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
                wrapper.Orientation = Orientation.Vertical;
                wrapper.Tag = globLexi[c].id;
                wrapper.Name = globLexi[c].idsor.ToString();
                wrapper.Padding = new Thickness(4);
                wrapper.Background = new SolidColorBrush(Color.FromArgb(255, 200, 200, 200));
                wrapper.Margin = new Thickness(0, 0, 8, 8);
                wrapper.Width = 200;
                wrapper.BorderThickness = new Thickness(2, 2, 2, 2);
                wrapper.BorderBrush = new SolidColorBrush(Colors.Transparent);

                StackPanel expWrap = new StackPanel();
                expWrap.Orientation = Orientation.Vertical;
                expWrap.Tag = globLexi[c].id;
                expWrap.Name = globLexi[c].idsor.ToString();
                expWrap.Background = new SolidColorBrush(Colors.LightBlue);
                expWrap.Padding = new Thickness(4, 4, 4, 4);
                expWrap.Padding = new Thickness(4);
                expWrap.Margin = new Thickness(0, 0, 8, 8);
                expWrap.Width = 200;


                string currid = globLexi[c].id;

                Button plus = new Button();
                plus.Content = "+";
                plus.Click += Plus_Click;
                plus.IsTabStop = false;

                Button minus = new Button();
                minus.Content = "-";
                minus.Click += Minus_Click;
                minus.IsTabStop = false;

                Button connected = new Button();
                connected.Content = "c";
                connected.HorizontalAlignment = HorizontalAlignment.Right;
                connected.IsTabStop = false;

              
                
                

                //StackPanel head = new StackPanel();
                //head.Orientation = Orientation.Horizontal;
                //head.HorizontalAlignment = HorizontalAlignment.Stretch;
                //head.Background = new SolidColorBrush(Colors.PaleVioletRed);

                Grid head = new Grid();

                ColumnDefinition col1 = new ColumnDefinition();
                ColumnDefinition col2 = new ColumnDefinition();
                ColumnDefinition col3 = new ColumnDefinition();
                col1.Width = new GridLength(0, GridUnitType.Auto);
                col2.Width = new GridLength(0, GridUnitType.Auto);
                col3.Width = new GridLength(1, GridUnitType.Star);
                head.ColumnDefinitions.Add(col1);
                head.ColumnDefinitions.Add(col2);
                head.ColumnDefinitions.Add(col3);

                head.Children.Add(plus);
                head.Children.Add(minus);
                head.Children.Add(connected);

                Grid.SetColumn(plus, 0);
                Grid.SetColumn(minus, 1);
                Grid.SetColumn(connected, 2);

                wrapper.Children.Add(head);


                // distractors
                TextBox tb1 = new TextBox();
                tb1.HorizontalAlignment = HorizontalAlignment.Stretch;
            
                TextBox tb2 = new TextBox();
                tb2.HorizontalAlignment = HorizontalAlignment.Stretch;
               

                Button corr = new Button();
                corr.Click += Corr_Click;
                corr.Tag = 0;
                corr.IsTabStop = false;

                corr.Content = new FontIcon
                {
                    FontFamily = new FontFamily("Segoe MDL2 Assets"),
                    Glyph = "\xF13E",
                    Foreground = new SolidColorBrush(Colors.LightGray)
                };

                Button corr2 = new Button();
                corr2.Content = new FontIcon
                {
                    FontFamily = new FontFamily("Segoe MDL2 Assets"),
                    Glyph = "\xF13E",
                    Foreground = new SolidColorBrush(Colors.LightGray)
                };
                corr2.Tag = 0;
                corr2.Click += Corr_Click;
                corr2.IsTabStop = false;


                Grid distGrid = new Grid();
                ColumnDefinition cold1 = new ColumnDefinition();
                ColumnDefinition cold2 = new ColumnDefinition();
                cold1.Width = new GridLength(1, GridUnitType.Star);
                cold2.Width = new GridLength(0, GridUnitType.Auto);
                distGrid.ColumnDefinitions.Add(cold1);
                distGrid.ColumnDefinitions.Add(cold2);
                RowDefinition row1 = new RowDefinition();
                row1.Height = new GridLength(0, GridUnitType.Auto);
                RowDefinition row2 = new RowDefinition();
                row2.Height = new GridLength(0, GridUnitType.Auto);
                distGrid.RowDefinitions.Add(row1);
                distGrid.RowDefinitions.Add(row2);

                distGrid.Children.Add(tb1);
                distGrid.Children.Add(corr);
                distGrid.Children.Add(tb2);
                distGrid.Children.Add(corr2);
                Grid.SetColumn(tb1, 0);
                Grid.SetColumn(corr, 1);
                Grid.SetRow(tb1, 0);
                Grid.SetRow(corr, 0);

                Grid.SetColumn(tb2, 0);
                Grid.SetColumn(corr2, 1);
                Grid.SetRow(tb2, 1);
                Grid.SetRow(corr2, 1);



                //StackPanel distWrapper = new StackPanel();
                //distWrapper.Orientation = Orientation.Horizontal;
                //distWrapper.Margin = new Thickness(0, 4, 0, 0);

               


                //distWrapper.Children.Add(tb1);
                //distWrapper.Children.Add(corr);

              


                // explanation textboxes

                TextBox tbexp1 = new TextBox();
                TextBox tbexp2 = new TextBox();
                expWrap.Children.Add(tbexp1);
                expWrap.Children.Add(tbexp2);

                //StackPanel distWrapper2 = new StackPanel();
                //distWrapper2.Orientation = Orientation.Horizontal;

              

                //StackPanel dists = new StackPanel();
                //dists.Orientation = Orientation.Vertical;

                //dists.Children.Add(distWrapper);
                //dists.Children.Add(distWrapper2);

                wrapper.Children.Add(distGrid);





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

                    if (chosenWords.Children.Count > 0)
                    {
                        bool inserted = false;
                        for (int i = 0; i < chosenWords.Children.Count; i++)
                        {
                            StackPanel theWrapper = chosenWords.Children[i] as StackPanel;

                            if (Convert.ToInt16(theWrapper.Name) > Convert.ToInt16(wrapper.Name))
                            {
                                chosenWords.Children.Insert(i, wrapper);
                                explanationBoxes.Children.Insert(i, expWrap);
                                inserted = true;

                                break;
                            }

                        }

                        if (inserted == false)
                        {

                            chosenWords.Children.Add(wrapper);
                            explanationBoxes.Children.Add(expWrap);

                        }


                    }
                    else
                    {
                        chosenWords.Children.Add(wrapper);
                        explanationBoxes.Children.Add(expWrap);
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

        private void Corr_Click(object sender, RoutedEventArgs e)
        {
            Button okbutt = sender as Button;
            if((int)okbutt.Tag == 0)
            {
                okbutt.Content = new FontIcon
                {
                    FontFamily = new FontFamily("Segoe MDL2 Assets"),
                    Glyph = "\xF13E",
                    Foreground = new SolidColorBrush(Colors.Green)
                };
                okbutt.Tag = 1;
            }
            else
            {
                okbutt.Content = new FontIcon
                {
                    FontFamily = new FontFamily("Segoe MDL2 Assets"),
                    Glyph = "\xF13E",
                    Foreground = new SolidColorBrush(Colors.LightGray)
                };
                okbutt.Tag = 0;
            }

            WrapReset();
        }

        private void removeEditBox(string azid)
        {
            for (int u = 0; u < chosenWords.Children.Count; u++)
            {
                StackPanel aWrap = chosenWords.Children[u] as StackPanel;
                if (aWrap.Tag.ToString() == azid)
                {
                    chosenWords.Children.RemoveAt(u);
                }
            }


            for (int u = 0; u < explanationBoxes.Children.Count; u++)
            {
                StackPanel aWrap = explanationBoxes.Children[u] as StackPanel;
                if (aWrap.Tag.ToString() == azid)
                {
                    explanationBoxes.Children.RemoveAt(u);
                }
            }


        }

        private void Plus_Click(object sender, RoutedEventArgs e)
        {
            Button plusButton = sender as Button;
            Grid headWrap = plusButton.Parent as Grid;
            StackPanel allWrap = headWrap.Parent as StackPanel;

            string atag = allWrap.Tag.ToString();

            

            Grid contentGrid = allWrap.Children[1] as Grid;
            int rows = contentGrid.RowDefinitions.Count;
           

            int kids = contentGrid.Children.Count;
            if(kids >= 12)
            {
                ("You cannot insert more than 6 distractors.").Show();
                return;
            }

            RowDefinition nextRow = new RowDefinition();
            nextRow.Height = new GridLength(0, GridUnitType.Auto);

            TextBox tbuj = new TextBox();
            Button okbutt = new Button();
            okbutt.Tag = 0;
            okbutt.IsTabStop = false;
            okbutt.Content = new FontIcon
            {
                FontFamily = new FontFamily("Segoe MDL2 Assets"),
                Glyph = "\xF13E",
                Foreground = new SolidColorBrush(Colors.LightGray)
            };
            okbutt.Click += Corr_Click;

            contentGrid.RowDefinitions.Add(nextRow);
            contentGrid.Children.Add(tbuj);
            contentGrid.Children.Add(okbutt);

            Grid.SetColumn(tbuj, 0);
            Grid.SetColumn(okbutt, 1);
            Grid.SetRow(tbuj, rows);
            Grid.SetRow(okbutt, rows);




            // contentWrap.Children.Add(kisWrap);

            for (int i = 0; i < explanationBoxes.Children.Count; i++)
            {
                StackPanel explWrap = explanationBoxes.Children[i] as StackPanel;
                string expWrapTag = explWrap.Tag.ToString();
                if (expWrapTag == atag)
                {
                    TextBox tb = new TextBox();
                    explWrap.Children.Add(tb);
                }
            }


        }




        private void Minus_Click(object sender, RoutedEventArgs e)
        {
            Button plusButton = sender as Button;
            Grid headWrap = plusButton.Parent as Grid;
            StackPanel allWrap = headWrap.Parent as StackPanel;

            string atag = allWrap.Tag.ToString();

            Grid contentGrid = allWrap.Children[1] as Grid;
            int rows = contentGrid.RowDefinitions.Count;

           
            if (rows <= 2)
            {
                ("There must be at least two distractors.").Show();
                return;
            }

            // remove two last children
            contentGrid.Children.RemoveAt(contentGrid.Children.Count - 1);
            contentGrid.Children.RemoveAt(contentGrid.Children.Count - 1);

            contentGrid.RowDefinitions.RemoveAt(rows - 1);

            for (int i = 0; i < explanationBoxes.Children.Count; i++)
            {
                StackPanel explWrap = explanationBoxes.Children[i] as StackPanel;
                string expWrapTag = explWrap.Tag.ToString();
                if (expWrapTag == atag)
                {
                   
                  
                    explWrap.Children.RemoveAt(explWrap.Children.Count - 1);
                }
            }


        }


        Item azitem = new Item();

       


        private void Finish_Click(object sender, RoutedEventArgs e)
        {
            azitem.id = 0;

            // collect the sentence
            List<string> sentence = new List<string>();
            sentence.Clear();

            string temp = "";
            for (int i = 0;i < wrapWords.Children.Count; i++)
            {
                
                ListBoxItem lbitem = wrapWords.Children[i] as ListBoxItem;

                if(lbitem.Tag.ToString() != "1") // ha nem zöld
                {
                    TextBlock lbcontent = lbitem.Content as TextBlock;
                    if(lbitem.Tag.ToString() == "x")
                    {
                        temp += lbcontent.Text;
                    }
                    else
                    {
                        temp += " " + lbcontent.Text;
                    }
                  
                }
                else
                {
                    sentence.Add(temp.Trim());
                    temp = "";
                }
            }
            sentence.Add(temp);
            azitem.sentence = sentence;

         

          


            // write in the sentence
            List<string> solu = new List<string>();
            solu.Add("GGG"); solu.Add("GGG");
            azitem.solutions = solu;



           




            checkCheckMarks();

        }


        private void checkCheckMarks()
        {
            // 2. check if boxes have at leas one solution marked as correct
            bool markCorrect = false;
            bool distrContent = false;

            List<List<string>> distra = new List<List<string>>();
            List<string> sol = new List<string>();
            string s = "";
            string so = "";

            for (int u = 0; u < chosenWords.Children.Count; u++)
            {

                StackPanel aWrap = chosenWords.Children[u] as StackPanel;
                Grid contentGrid = aWrap.Children[1] as Grid;
                bool hasTick = false;
                int filledTb = 0;
                int buttonCount = 0;

                List<string> dist = new List<string>();

                for(int i = 0;i < contentGrid.Children.Count; i++)
                {

                    if( contentGrid.Children[i] is Button)
                    {
                        Button okbutt = contentGrid.Children[i] as Button;
                        if((int)okbutt.Tag == 1)
                        {
                            hasTick = true;
                            so += buttonCount.ToString(); // solu 0-1-1-0
                        }
                        buttonCount++;
                    }

                    if( contentGrid.Children[i] is TextBox)
                    {
                        TextBox tbcont = contentGrid.Children[i] as TextBox;
                        dist.Add(tbcont.Text.Trim());
                        if(tbcont.Text.Trim().Length > 0)
                        {
                            filledTb++;
                        }
                    }
                }

                if(u < chosenWords.Children.Count - 1)
                {
                    so += "-";
                }
               
                distra.Add(dist);
              
                if(filledTb < 2)
                {
                    aWrap.BorderBrush = new SolidColorBrush(Colors.Red);
                    distrContent = true;
                }
                if(hasTick == false)
                {
                    aWrap.BorderBrush = new SolidColorBrush(Colors.Red);
                    markCorrect = true;
                }

            }

           
            azitem.distractors = distra;

            sol.Add(so);
            azitem.solu = sol;

            string errorMessage = "";

            if (markCorrect)
            {
                errorMessage = "You need to mark at least one solution as correct in the boxes.";
            }

            if (distrContent)
            {
                errorMessage = "You need to fill in at least two distractors.";
            }

            if(errorMessage != "")
            {
                errorMessage.Show();
            }


            createJson();

        }

        private void createJson()
        {
            List<List<string>> explanations = new List<List<string>>();

            for (int i = 0; i < explanationBoxes.Children.Count; i++)
            {
                StackPanel explWrap = explanationBoxes.Children[i] as StackPanel;
                List<string> expl = new List<string>();

                for(int e = 0; e < explWrap.Children.Count; e++)
                {
                    TextBox exp = explWrap.Children[e] as TextBox;
                    expl.Add(exp.Text);
                }
                explanations.Add(expl);
            }


          

            azitem.remarks = explanations;

            FullFile fullfile = new FullFile();
            fullfile.path = "FELADATOK/valami/path";
            fullfile.title = "Past simple structures - 1";
            List<Item> egyitem = new List<Item>();
            egyitem.Add(azitem);
            fullfile.contents = egyitem;

            string json = Newtonsoft.Json.JsonConvert.SerializeObject(fullfile);
            result.Text = json;

        }

        public class FullFile
        {
            public string path { get; set; }
            public string title { get; set; }
            public List<Item> contents { get; set; }
        }

        public class Item
        {
            public int id { get; set; }
            public List<string> sentence { get; set; }
            public List<string> solutions { get; set; }
            public List<List<string>> distractors { get; set; }
            public List<string> solu { get; set; }
            public List<List<string>> remarks { get; set; }

        }

        public class WeatherForecast
        {
            public string Date { get; set; }
            public int TemperatureCelsius { get; set; }
            public string Summary { get; set; }
        }

        private void WrapReset()
        {
            for (int u = 0; u < chosenWords.Children.Count; u++)
            {
                StackPanel aWrap = chosenWords.Children[u] as StackPanel;
                aWrap.BorderBrush = new SolidColorBrush(Colors.Transparent);
            }
        }








    }
}
