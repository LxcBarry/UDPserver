using System.Threading.Tasks;
using UDPserver.Disposal;
using DbApi;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace UDPserver.Disposal
{
    public class GetRegisterDisposal : IDisposal
    {
        private readonly DbApi.DbApi dbApi;
        public GetRegisterDisposal(DbApi.DbApi dbApi)
        {
            this.dbApi = dbApi;
        }
        public string Run(string msg)
        {
            throw new System.NotImplementedException();
        }

        public async Task<string> RunAsync(string msg)
        {
            //throw new System.NotImplementedException();
            var msgJo = JsonConvert.DeserializeObject<JObject>(msg);
            var tmpJo = new JObject() {
                {"username",msgJo["username"] },
                {"password",msgJo["password"] }
            };
            if(await dbApi.CheckUserAsync(tmpJo.ToString())<= 0) return null;
            var result = await dbApi.GetUserRecordAsync(msgJo["username"].ToString());

            if (result == "null") return null;

            var resultJo = JsonConvert.DeserializeObject<List<JObject>>(result);
            var retList = new List<string>();
            foreach(var jo in resultJo)
            {
                retList.Add(jo["Site"].ToString());
            }
            return JsonConvert.SerializeObject(retList);
        }
    }
}