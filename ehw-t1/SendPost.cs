using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Http;

namespace ehw_t1
{
    public static class SendPost
    {
        public static async Task<string> PostJsonAsync(this Dictionary<string, string> pairs, String azurl)
        {
            try
            {
                // Construct the HttpClient and Uri. This endpoint is for test purposes only.
                HttpClient httpClient = new HttpClient();
                //   Uri uri = new Uri("http://kashusoft.org/uwpehw/resp.php");

              

                HttpFormUrlEncodedContent formContent = new HttpFormUrlEncodedContent(pairs);

                HttpClient client = new HttpClient();
                Uri uri = new Uri(azurl);
                HttpResponseMessage httpResponseMessage = await client.PostAsync(uri, formContent);
                httpResponseMessage.EnsureSuccessStatusCode();
                var httpResponseBody = await httpResponseMessage.Content.ReadAsStringAsync();

                return httpResponseBody.ToString();

                // response.Text = valasz.ToString();

            }
            catch (Exception ex)
            {
                // Write out any exceptions.
                return ex.ToString();
            }
        }
    }
}
