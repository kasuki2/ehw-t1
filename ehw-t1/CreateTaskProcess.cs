using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace ehw_t1
{
    public static class CreateTaskProcess
    {

        public static List<ListBoxItem> ProcessText(this string inputText)
        {
            // PROCESS button
            List<ListBoxItem> ret = new List<ListBoxItem>();
          

            char[] sepas = { '.', ',', ':', '?', '!' };
            char[] textChars = inputText.ToCharArray();

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

          //  wrapWords.Children.Clear();


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
                  //  lb.Tapped += Lb_Tapped;
                }
                else
                {
                    lb.Tag = "x";
                    lb.Name = k.ToString();
                }
                ret.Add(lb);
             //   wrapWords.Children.Add(lb);



            }


            return ret;

           // chosenWords.Children.Clear();
        }

    }
}
