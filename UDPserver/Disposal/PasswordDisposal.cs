using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DbApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace UDPserver.Disposal
{
    public class PasswordDisposal : IDisposal
    {
        private readonly DbApi.DbApi dbApi;
        public PasswordDisposal(DbApi.DbApi dbApi)
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
            //return null;
            var msgJo = JsonConvert.DeserializeObject<JObject>(msg);
            var newPwd = msgJo["new"];
            msgJo.Remove("new");
            var count = await dbApi.CheckUserAsync(msgJo.ToString());
            if(count > 0)
            {
                msgJo["password"] = newPwd;
                await dbApi.UpdateUserAsync(msgJo["username"].ToString(),msgJo.ToString());
                return await dbApi.SaveChangeAsync() > 0 ? "sucess" : "no change";
            }
            else
            {
                Logger.Info(msgJo["username"].ToString()+"修改密码失败");
                return null;
            }
        }
    }
}
