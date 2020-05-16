using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace ehw_t1
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Register : Page
    {
        public Register()
        {
            this.InitializeComponent();
        }



        private async Task<string> TryPostJsonAsync(Dictionary<string, string> pairs)
        {
            try
            {
                // Construct the HttpClient and Uri. This endpoint is for test purposes only.
                HttpClient httpClient = new HttpClient();
             //   Uri uri = new Uri("http://kashusoft.org/uwpehw/resp.php");

                // Construct the JSON to post.
                //  HttpStringContent content = new HttpStringContent("{ \"firstName\": \"Eliot\" }", UnicodeEncoding.Utf8,"application/json");
               // HttpStringContent content = new HttpStringContent("{ \"firstName\": \"Eliot\" }", Windows.Storage.Streams.UnicodeEncoding.Utf8, "application/json");

                // Post the JSON and wait for a response.
               // HttpResponseMessage httpResponseMessage = await httpClient.PostAsync(
                 //   uri,
                  //  content);

                // Make sure the post succeeded, and write out the response.
               // httpResponseMessage.EnsureSuccessStatusCode();
               // var httpResponseBody = await httpResponseMessage.Content.ReadAsStringAsync();

            //    response.Text = httpResponseBody.ToString();


                //Dictionary<string, string> pairs = new Dictionary<string, string>();
                //pairs.Add("Name", "Bob");
                //pairs.Add("Age", "18");
                //pairs.Add("Gender", "Male");

                HttpFormUrlEncodedContent formContent = new HttpFormUrlEncodedContent(pairs);

                HttpClient client = new HttpClient();
                Uri uri = new Uri("http://kashusoft.org/uwpehw/resp.php");
                HttpResponseMessage httpResponseMessage = await client.PostAsync(uri, formContent);
                httpResponseMessage.EnsureSuccessStatusCode();
                var httpResponseBody = await httpResponseMessage.Content.ReadAsStringAsync();

                return  httpResponseBody.ToString();

               // response.Text = valasz.ToString();

            }
            catch (Exception ex)
            {
                // Write out any exceptions.
              return  ex.ToString();
            }
        }

        public class Weather
        {
            public string Date { get; set; }
            public int TemperatureCelsius { get; set; }
            public string Summary { get; set; }
        }

        public class RegisterUser
        {
            public string username { get; set; }
            public string azemail { get; set; }
            public string pw { get; set; }
        }

        private async void Register_Click(object sender, RoutedEventArgs e)
        {
            string Username = username.Text.Trim();
            string Email = email.Text.Trim();
            string Password = password.Password.ToString();

            RegisterUser regi = new RegisterUser();
            regi.username = Username;
            regi.azemail = Email;
            regi.pw = Password;

            string thejson = Newtonsoft.Json.JsonConvert.SerializeObject(regi);
          


            Dictionary<string, string> pairs = new Dictionary<string, string>();
           
            pairs.Add("json", thejson);

            //string valasz = await TryPostJsonAsync(pairs);
            string valasz = await pairs.PostJsonAsync("http://kashusoft.org/uwpehw/resp.php");
            response.Text = valasz;

        }
    }
}
