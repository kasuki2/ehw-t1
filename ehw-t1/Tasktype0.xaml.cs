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
                if (Array.Exists(sepas, element => element == textChars[z]))
                {

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
                    lb.Name = k.ToString(); // max 1000 words!
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
                    if (temp != String.Empty)
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

                StackPanel expWrap = new StackPanel();
                expWrap.Orientation = Orientation.Vertical;
                expWrap.Tag = globLexi[c].id;
                expWrap.Name = globLexi[c].idsor.ToString();
                expWrap.Background = new SolidColorBrush(Colors.LightBlue);
                expWrap.Padding = new Thickness(4, 4, 4, 4);


                string currid = globLexi[c].id;

                Button plus = new Button();
                plus.Content = "+";
                plus.Click += Plus_Click;

                Button minus = new Button();
                minus.Content = "-";
                minus.Click += Minus_Click;

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


                // explanation textboxes

                TextBox tbexp1 = new TextBox();
                TextBox tbexp2 = new TextBox();
                expWrap.Children.Add(tbexp1);
                expWrap.Children.Add(tbexp2);

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
            StackPanel headWrap = plusButton.Parent as StackPanel;
            StackPanel allWrap = headWrap.Parent as StackPanel;

            string atag = allWrap.Tag.ToString();
           

            StackPanel contentWrap = allWrap.Children[1] as StackPanel;
            int kids = contentWrap.Children.Count;
            if(kids >= 6)
            {
                ("You cannot put in more than 6 distractors.").Show();
                return;
            }


            TextBox tb1 = new TextBox();
            tb1.Text = "";

            Button corr = new Button();
            corr.Content = "p";

            StackPanel kisWrap = new StackPanel();

          

            kisWrap.Orientation = Orientation.Horizontal;
            kisWrap.Children.Add(tb1);
            kisWrap.Children.Add(corr);

            contentWrap.Children.Add(kisWrap);

            for(int i = 0;i< explanationBoxes.Children.Count; i++)
            {
                StackPanel explWrap = explanationBoxes.Children[i] as StackPanel;
                string expWrapTag = explWrap.Tag.ToString();
                if(expWrapTag == atag)
                {
                    TextBox tb = new TextBox();
                    explWrap.Children.Add(tb);
                }
            }

            // int kids = contentWrap.Children.Count;
            // kids.ToString().Show();
        }




        private void Minus_Click(object sender, RoutedEventArgs e)
        {
            Button plusButton = sender as Button;
            StackPanel headWrap = plusButton.Parent as StackPanel;
            StackPanel allWrap = headWrap.Parent as StackPanel;

            string atag = allWrap.Tag.ToString();


            StackPanel contentWrap = allWrap.Children[1] as StackPanel;
            int kids = contentWrap.Children.Count;
            if (kids <= 2)
            {
                ("There must be at least two distractors.").Show();
                return;
            }

            contentWrap.Children.RemoveAt(contentWrap.Children.Count - 1);

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







    }
}
