using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Configuration.Json;
using System.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace UDPserver
{
    /// <summary>
    /// 获取天气接口
    /// </summary>
    public interface IDataGetter
    {
        public string GetWeather(string city);
        public Task<int> UpdateWeatherAsync();
        public Task<int> InitWeatherAsync();
    }

    public class WeatherGetter:IDataGetter
    {
        private readonly AppSettingsReader Reader;
        private readonly WebClient Client;
        private readonly string ForecastURL;
        private readonly string LiveURL;
        private readonly DbApi.DbApi Api;
        public WeatherGetter(DbApi.DbApi api)
        {
            Api = api;
            Client = new WebClient();
            Reader = new AppSettingsReader();
            var key = Reader.GetValue("key", typeof(string)).ToString();
            var URL = Reader.GetValue("url", typeof(string)).ToString();
            ForecastURL = string.Format("{0}?key={1}&extensions=all&city=", URL, key);
            LiveURL = string.Format("{0}?key={1}&extensions=base&city=", URL, key);
        }
        /// <summary>
        /// return today and three days later weather,if connect fail ,return {}
        /// </summary>
        /// <param name="city"></param>
        /// <returns></returns>
        public string GetWeather(string city)
        {
            var result = new JObject();
            try
            {
              
                JObject ForecastJo = JsonConvert.DeserializeObject<JObject>(Client.DownloadString(ForecastURL + city));
                JObject LiveJo = JsonConvert.DeserializeObject<JObject>(Client.DownloadString(LiveURL + city));
                //result = jo["forecasts"].FirstOrDefault().ToString();
                result.Add("Forecast", ForecastJo["forecasts"]);

                result.Add("Live", LiveJo["lives"]);
            }
            catch
            {
                return null;
            }
            return JsonConvert.SerializeObject(result);
        }
        public async Task<int> UpdateWeatherAsync()
        {
            //查看参照表
            //遍历
            Logger.Info("weather start to update");
            var result = await Api.GetReferenceAsync();
            var jo_list = JsonConvert.DeserializeObject<List<JObject>>(result);
            foreach (JObject jo in jo_list)
            {

                var ret = GetWeather(jo["Adcode"].ToString());
                //todo 此处把所有内容都存入了content
                if (ret == null) continue;
                var content = new JObject(
                        new JProperty("content", ret)
                        );
                Api.UpdateWeather(jo["Adcode"].ToString(), content.ToString());
            }
            Logger.Info("finished update weather");
            return await Api.SaveChangeAsync();
        }

        public async Task<int> InitWeatherAsync()
        {
            var result = await Api.GetReferenceAsync();
           
            var jo_list = JsonConvert.DeserializeObject<List<JObject>>(result);
            foreach(var jo in jo_list)
            {
                jo.Add("Site", jo["Adcode"]);
                jo.Remove("Adcode");
                Api.AddWeather(jo.ToString());
            }
            Logger.Info("finished initial weather data");
            return await Api.SaveChangeAsync();
        }

    }
}
