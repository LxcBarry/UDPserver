using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using DbApi;

//todo 待测试
namespace UDPserver.Disposal
{
    public class RemoveRegDisposal : IDisposal
    {
        private readonly DbApi.DbApi dbApi;
        public RemoveRegDisposal(DbApi.DbApi api)
        {
            dbApi = api;
        }
        public string Run(string msg)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 登记用户注册的地点，也就是加入观察者，其结果就是在数据库recordtable加入一行
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public async Task<string> RunAsync(string msg)
        {
            var msgJo = JsonConvert.DeserializeObject<JObject>(msg);
            var username = msgJo["username"].ToString();
            foreach(var site in msgJo["sites"])
            {
                string[] tmp = { username, site.ToString() };
                await dbApi.RemoveRecordAsync(tmp);
            }
            return await dbApi.SaveChangeAsync() > 0 ? "sucess" : "no change";

        }
    }
}
