using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LasciviousnessVideoBotApp
{
    public static class DMMApiCaller
    {
        private const string DMM_FLOOR_API = @"https://api.dmm.com/affiliate/v3/GenreSearch?api_id={0}&affiliate_id={1}&initial={2}&floor_id=44&hits=220&offset=1&output=json";
        private const string DMM_SEARCH_VIDEO_API = @"https://api.dmm.com/affiliate/v3/ItemList?api_id={0}&affiliate_id={1}&site=DMM.R18&service=digital&floor=videoa&hits=10&sort=rank&output=json&keyword={2}";
        private static readonly string APP_ID = string.Empty;
        private static readonly string AFFILIATE_ID = string.Empty;
        private static readonly HttpClient client = new HttpClient();
        private static readonly List<string> ALL_GENRE_LIST = new List<string>();

        static DMMApiCaller()
        {
            // read configuration from Web.config
            APP_ID = ConfigurationManager.AppSettings["DMMWebAppId"];
            AFFILIATE_ID = ConfigurationManager.AppSettings["DMMAffiliateId"];

            // can't use await in static constructor
            var response = client.GetStringAsync(string.Format(DMM_FLOOR_API, APP_ID, AFFILIATE_ID, string.Empty)).Result;
            var model = JsonConvert.DeserializeObject<DMMFloorAPIModel>(response);
            if (model.result.genre != null)
            {
                foreach (var genre in model.result.genre)
                {
                    ALL_GENRE_LIST.Add(genre.name);
                }
            }
        }

        public static async Task<List<string>> GetGenreFromInput(string input)
        {
            // take keyphrases from input 
            var retPhrases = new List<string>();
            foreach (var keyPhrase in await TextAnalyticsCaller.DoRequest(input))
            {
                if (ALL_GENRE_LIST.Contains(keyPhrase))
                {
                    retPhrases.Add(keyPhrase);
                }
            }
            return retPhrases;
        }

        public static async Task<DMMVideoAPIModel> SeachVideo(string input)
        {
            var response = await client.GetStringAsync(string.Format(DMM_SEARCH_VIDEO_API, APP_ID, AFFILIATE_ID, input));
            return JsonConvert.DeserializeObject<DMMVideoAPIModel>(response);
        }
    }


    public class DMMFloorAPIModel
    {
        public Request request { get; set; }
        public Result result { get; set; }

        public class Request
        {
            public Parameters parameters { get; set; }
        }

        public class Parameters
        {
            public string Genre { get; set; }
            public string api_id { get; set; }
            public string affiliate_id { get; set; }
            public string initial { get; set; }
            public string floor_id { get; set; }
            public string hits { get; set; }
            public string offset { get; set; }
            public string output { get; set; }
        }

        public class Result
        {
            public string status { get; set; }
            public int result_count { get; set; }
            public string total_count { get; set; }
            public int first_position { get; set; }
            public string site_name { get; set; }
            public string site_code { get; set; }
            public string service_name { get; set; }
            public string service_code { get; set; }
            public string floor_id { get; set; }
            public string floor_name { get; set; }
            public string floor_code { get; set; }
            public Genre[] genre { get; set; }
        }

        public class Genre
        {
            public string genre_id { get; set; }
            public string name { get; set; }
            public string ruby { get; set; }
            public string list_url { get; set; }
        }
    }


    public class DMMVideoAPIModel
    {
        public Request request { get; set; }
        public Result result { get; set; }

        public class Request
        {
            public Parameters parameters { get; set; }
        }

        public class Parameters
        {
            public string api_id { get; set; }
            public string affiliate_id { get; set; }
            public string site { get; set; }
            public string service { get; set; }
            public string floor { get; set; }
            public string hits { get; set; }
            public string sort { get; set; }
            public string output { get; set; }
            public string keyword { get; set; }
        }

        public class Result
        {
            public int status { get; set; }
            public int result_count { get; set; }
            public int total_count { get; set; }
            public int first_position { get; set; }
            public Item[] items { get; set; }
        }

        public class Item
        {
            public string service_code { get; set; }
            public string service_name { get; set; }
            public string floor_code { get; set; }
            public string floor_name { get; set; }
            public string category_name { get; set; }
            public string content_id { get; set; }
            public string product_id { get; set; }
            public string title { get; set; }
            public string volume { get; set; }
            public string URL { get; set; }
            public string URLsp { get; set; }
            public string affiliateURL { get; set; }
            public string affiliateURLsp { get; set; }
            public Imageurl imageURL { get; set; }
            public Sampleimageurl sampleImageURL { get; set; }
            public Samplemovieurl sampleMovieURL { get; set; }
            public Prices prices { get; set; }
            public string date { get; set; }
            public Iteminfo iteminfo { get; set; }
            public Review review { get; set; }
        }

        public class Imageurl
        {
            public string list { get; set; }
            public string small { get; set; }
            public string large { get; set; }
        }

        public class Sampleimageurl
        {
            public Sample_S sample_s { get; set; }
        }

        public class Sample_S
        {
            public string[] image { get; set; }
        }

        public class Samplemovieurl
        {
            public string size_476_306 { get; set; }
            public string size_560_360 { get; set; }
            public string size_644_414 { get; set; }
            public string size_720_480 { get; set; }
            public int pc_flag { get; set; }
            public int sp_flag { get; set; }
        }

        public class Prices
        {
            public string price { get; set; }
            public Deliveries deliveries { get; set; }
        }

        public class Deliveries
        {
            public Delivery[] delivery { get; set; }
        }

        public class Delivery
        {
            public string type { get; set; }
            public string price { get; set; }
        }

        public class Iteminfo
        {
            public Genre[] genre { get; set; }
            public Series[] series { get; set; }
            public Maker[] maker { get; set; }
            public Actress[] actress { get; set; }
            public Director[] director { get; set; }
            public Label[] label { get; set; }
        }

        public class Genre
        {
            public int id { get; set; }
            public string name { get; set; }
        }

        public class Series
        {
            public int id { get; set; }
            public string name { get; set; }
        }

        public class Maker
        {
            public int id { get; set; }
            public string name { get; set; }
        }

        public class Actress
        {
            public object id { get; set; }
            public string name { get; set; }
        }

        public class Director
        {
            public object id { get; set; }
            public string name { get; set; }
        }

        public class Label
        {
            public int id { get; set; }
            public string name { get; set; }
        }

        public class Review
        {
            public int count { get; set; }
            public string average { get; set; }
        }
    }
}
