using System;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace pdx_ymltranslator.Util
{
    public static class BaiduAPI
    {
        public const string BaiduAPIUrl = "http://fanyi.baidu.com/transapi?from=en&to=zh&query=";

        public static string GetTranslatedResult(string queryString)
        {
            WebClient wc = new WebClient();
            string json = wc.DownloadString(BaiduAPIUrl + queryString);
            JObject jsonarr = (JObject)JsonConvert.DeserializeObject(json);
            return jsonarr["data"][0]["dst"].ToString();
        }
    }
}
