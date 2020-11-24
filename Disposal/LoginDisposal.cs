using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DbApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace UDPserver.Disposal
{
    public class LoginDisposal : IDisposal
    {
        private readonly DbApi.DbApi dbApi;
        public LoginDisposal(DbApi.DbApi dbApi)
        {
            this.dbApi = dbApi;
        }
        public string Run(string msg)
        {
            throw new NotImplementedException();
        }

        public async Task<string> RunAsync(string msg)
        {
            //throw new NotImplementedException();
            //TODO 待优化
            // 有该用户，更新ip
            // 无该用户，加入
            var jo = JsonConvert.DeserializeObject<JObject>(msg);
            var user = await dbApi.GetUserAsync(jo["username"].ToString());
            var tmpJo = new JObject(jo);
            tmpJo.Remove("ip");
            int count = dbApi.CheckUser(tmpJo.ToString());
            try
            {
                if (count > 0)
                {
                    //var user = await dbApi.GetUserAsync(jo["username"].ToString());
                    var userJo = JsonConvert.DeserializeObject<JObject>(user);
                    var updateJo = new JObject
                    {
                        { "ip", jo["ip"] },
                    };
                    dbApi.UpdateUser(userJo["Username"].ToString(), updateJo.ToString());

                    await dbApi.UpdateUserAsync(userJo["Username"].ToString(), updateJo.ToString());
                    return await dbApi.SaveChangeAsync() > 0 ? "sucess" : "no change";
                }
                else if (user == "null")
                {
                    return await dbApi.AddUserAsync(jo.ToString()) > 0 ? "sucess" : "no change";
                    //Logger.Info(jo["username"].ToString() + "试图登陆，但密码错误,将不会收到推送");
                    //jo.Remove("password");
                    //jo["ip"] = "";
                    //await dbApi.UpdateUserAsync(jo["Username"].ToString(), jo.ToString());
                    //return await dbApi.SaveChangeAsync() > 0 ? "sucess" : "no change";
                    //return null;
                }
                else return null;
           
            }
            catch(Exception ex)
            {
                Logger.Fatal($"用户登陆异常: {ex.Message}");
                return null;
            }

        }
    }
}
