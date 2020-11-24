using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using DbApi;

namespace UDPserver.Disposal
{
    public class RegisterDisposal : IDisposal
    {
        private readonly DbApi.DbApi dbApi;
        public RegisterDisposal(DbApi.DbApi api)
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
            //throw new NotImplementedException();
            //TODO 待测试
            var jo = JsonConvert.DeserializeObject<JObject>(msg);
            var checkJo = new JObject
            {
                { "username", jo["username"] },
                { "password", jo["password"] }
            };
            int count = await dbApi.CheckUserAsync(checkJo.ToString());
            if (count <= 0) return null;
            var tempJo = new JObject
            {
                { "username", jo["username"] },
                { "site", "" }
            };
            count = 0;
      
            try
            {
                foreach(var o in jo["sites"])
                {
                    tempJo["site"] = o;
                    //records.Add(new JObject(tempJo));
                    dbApi.AddRecord(tempJo.ToString());
                    //tempJo["site"] = "";
                }
                // 待优化，这一步只是为了保持接口特性统一而做的
                //foreach (var r in records)
                //{
                //    count += r.Result;
                //}
                return await dbApi.SaveChangeAsync() > 0 ? "sucess" : "no change";
            }
            catch(Exception ex)
            {
                Logger.Fatal($"订阅地点异常: {ex.Message}");
                return null;
            }
        }
    }
}
