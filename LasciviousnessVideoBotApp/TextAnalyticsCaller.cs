using System;
using System.Globalization;
using System.Net.Http.Headers;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;

namespace LasciviousnessVideoBotApp
{
    /// <summary>
    /// This is a sample program that shows how to use the Azure ML Text Analytics app: key phrases, language and sentiment detection. 
    /// </summary>
    public static class TextAnalyticsCaller
    {
        /// <summary>
        /// Azure portal URL.
        /// </summary>
        private const string BaseUrl = "https://westus.api.cognitive.microsoft.com/";
        //https://westus.api.cognitive.microsoft.com/text/analytics/v2.0

        /// <summary>
        /// Your account key goes here.
        /// </summary>
        private readonly static string ACCOUNT_KEY = string.Empty;

        /// <summary>
        /// Maximum number of languages to return in language detection API.
        /// </summary>
        private const string MEDIA_TYPE = "application/json";

        static TextAnalyticsCaller()
        {
            ACCOUNT_KEY = ConfigurationManager.AppSettings["AzureSearchAccountKey"];
        }

        public static async Task<List<string>> DoRequest(string input)
        {
            KeyPhraseModel ret = null;
            HttpResponseMessage response;
            using (var client = new HttpClient())
            {
                // Request headers
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", ACCOUNT_KEY);
                var uri = "https://westus.api.cognitive.microsoft.com/text/analytics/v2.0/keyPhrases?";

                // Request body
                byte[] byteData = Encoding.UTF8.GetBytes("{\"documents\":[" +
                    "{\"language\": \"ja\", \"id\":\"1\",\"text\":\"" + input + "\"}" +
                    "]}");

                var str = string.Empty;
                using (var content = new ByteArrayContent(byteData))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue(MEDIA_TYPE);
                    response = await client.PostAsync(uri, content);
                    str = await response.Content.ReadAsStringAsync();
                }

                ret = JsonConvert.DeserializeObject<KeyPhraseModel>(str);
            }

            return new List<string>(ret.documents.FirstOrDefault().keyPhrases);
        }
    }


    class KeyPhraseModel
    {
        public Document[] documents { get; set; }
        public object[] errors { get; set; }
    }

    class Document
    {
        public string[] keyPhrases { get; set; }
        public string id { get; set; }
    }

}
