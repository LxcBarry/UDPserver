using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DbApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace UDPserver.Disposal
{
    public class SendDisposal : IDisposal
    {
        private readonly DbApi.DbApi dbApi;
        public SendDisposal(DbApi.DbApi db)
        {
            dbApi = db;
        }
        public string Run(string msg)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// get send data
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public async Task<string> RunAsync(string msg=null)
        {
            try
            {
                Logger.Info("开始获取推送数据");
                var siteAndIpInfo = await dbApi.GetCiteIpGroupAsync();
                var siteAndIpJo = JsonConvert.DeserializeObject<List<JObject>>(siteAndIpInfo);
                var siteAndContentInfo = await dbApi.GetWeatherAsync(null);
                //var siteAndContentJo = JsonConvert.DeserializeObject<List<JObject>>(siteAndContentInfo);
                var resultJo = new List<JObject>();
                foreach(var ipJo in siteAndIpJo)
                {
                    var weatherInfo = await dbApi.GetWeatherAsync(ipJo["Site"].ToString());
                    var weatherJoList = JsonConvert.DeserializeObject<List<JObject>>(weatherInfo);
                    if (weatherJoList.Count <= 0) continue;
                    var weatherJo = weatherJoList[0];
                    resultJo.Add(new JObject
                    {
                        { "Ip",ipJo["Ip"] },
                        { "Content",weatherJo["Content"]}
                    }
                        );
                }
               return JsonConvert.SerializeObject(resultJo);
            }
            catch(Exception ex)
            {
                Logger.Fatal($"获取发送数据发生异常: {ex.Message}");
                return null;
            }
            //throw new NotImplementedException();
            //return null;
        }
    }
}
