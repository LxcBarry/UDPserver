using System;
using System.Collections.Generic;
using System.Linq;
using DbApi.Models;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Data;
using Microsoft.EntityFrameworkCore;

namespace DbApi
{
    
    public class DbApi
    {
        weathersysContext Context { get; set; }
        JsonSerializer Serializer { get; set; }
        JsonSerializerSettings JsonSetting;
        public DbApi()
        {
            this.Context = new weathersysContext();
            this.Serializer = new JsonSerializer();
            this.JsonSetting = new JsonSerializerSettings()
            {
                StringEscapeHandling = StringEscapeHandling.EscapeNonAscii
            };


        }

        public int CheckUser(string jsonInfo)
        {
            var jo = JsonConvert.DeserializeObject<JObject>(jsonInfo);

            var count = Context.UserTable
                .Where(u => u.Username == jo["username"].ToString() && u.Password == jo["password"].ToString())
                .Count();
            return count;
        }

        public async Task<int> CheckUserAsync(string jsonInfo)
        {
            var jo = JsonConvert.DeserializeObject<JObject>(jsonInfo);

            var count = await Context.UserTable
                .Where(u => u.Username == jo["username"].ToString() && u.Password == jo["password"].ToString())
                .CountAsync();
            return count;
        }
        public string GetReference()
        {
            var result = Context.ReferenceTable
                .ToList();
            return JsonConvert.SerializeObject(result,JsonSetting);
        }

        public async Task<string> GetReferenceAsync()
        {
            var result = await Context.ReferenceTable
                .ToListAsync();
            return JsonConvert.SerializeObject(result,JsonSetting);
        }

        public string GetUser(string username)
        {
            var user = Context.UserTable
                .SingleOrDefault(u => u.Username == username);
            return JsonConvert.SerializeObject(user,JsonSetting);
        }
        public async Task<string> GetUserAsync(string username)
        {
            var user = await Context.UserTable
                .SingleOrDefaultAsync(u => u.Username == username);
            return JsonConvert.SerializeObject(user,JsonSetting);
        }
        public string GetWeather(string site)
        {
            //if(site != null)
            //{
            var query = from w in Context.Set<WeatherTable>().Where(e => e.Site == site || site==null)
                        select new { Site = w.Site, Content = w.Content };
            return JsonConvert.SerializeObject(query.ToList(),JsonSetting);
            //}
            //return JsonConvert.SerializeObject(weather);

        }
        public async Task<string> GetWeatherAsync(string site)
        {
            var query = from w in Context.Set<WeatherTable>().Where(e => e.Site == site || site == null)
                        select new { Site = w.Site, Content = w.Content };
            return JsonConvert.SerializeObject(await query.ToListAsync(),JsonSetting);

        }
        public string GetUserRecord(string username)
        {
            var records = Context.RecordTable
                .Where(r => r.Username == username)
                .ToList();
            return JsonConvert.SerializeObject(records,JsonSetting);
        }

        public async Task<string> GetUserRecordAsync(string username)
        {
            var records = await Context.RecordTable
                .Where(r => r.Username == username)
                .ToListAsync();
            return JsonConvert.SerializeObject(records,JsonSetting);
        }

        public string GetSiteRecord(string sitename)
        {
            var records = Context.RecordTable
                .Where(r => r.Site == sitename)
                .ToList();
            return JsonConvert.SerializeObject(records,JsonSetting);
        }
        public async Task<string> GetSiteRecordAsync(string sitename)
        {
            var records = await Context.RecordTable
                .Where(r => r.Site == sitename)
                .ToListAsync();
            return JsonConvert.SerializeObject(records,JsonSetting);
        }
        public string GetUserRecordWeather(string username)
        {
            var query = from r in Context.Set<RecordTable>().Where(r => r.Username == username)
                        join w in Context.Set<WeatherTable>()
                            on r.Site equals w.Site
                        select new { w.Content };
            
            return JsonConvert.SerializeObject(query.ToList(),JsonSetting);
        }
        public async Task<string> GetUserRecordWeatherAsync(string username)
        {
            var query = from r in Context.Set<RecordTable>().Where(r => r.Username == username)
                        join w in Context.Set<WeatherTable>()
                            on r.Site equals w.Site
                        select new { w.Content };
            var result = await query.ToListAsync();
            return JsonConvert.SerializeObject(result,JsonSetting);
        }
        public string GetSiteRecordIp(string sitename)
        {
            var query = from r in Context.Set<RecordTable>().Where(r => r.Site == sitename)
                        join u in Context.Set<UserTable>()
                            on r.Username equals u.Username
                        select new { u.Ip };
            return JsonConvert.SerializeObject(query.ToList(),JsonSetting);

        }
        public async Task<string> GetSiteRecordIpAsync(string sitename)
        {
            var query = from r in Context.Set<RecordTable>().Where(r => r.Site == sitename)
                        join u in Context.Set<UserTable>()
                            on r.Username equals u.Username
                        select new { u.Ip };
            var result = await query.ToListAsync();
            return JsonConvert.SerializeObject(result,JsonSetting);

        }
        /// <summary>
        /// async function,get a list of {site,Ip}
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetCiteIpGroupAsync()
        {
            var query = from r in Context.Set<RecordTable>()
                        join u in Context.Set<UserTable>()
                            on r.Username equals u.Username
                        select new { Site=r.Site, Ip = u.Ip };
            return JsonConvert.SerializeObject(await query.ToListAsync(),JsonSetting);
            //return g;                        
        }
        /// <summary>
        /// update weather info by site 
        /// </summary>
        /// <param name="site"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public void UpdateWeather(string site,string jsonInfo)
        {
            var weather = Context.WeatherTable.Single(e => e.Site == site);
            var jo = JObject.Parse(jsonInfo);
            var tmp_weather = new WeatherTable();
            tmp_weather.Site = (string)jo.SelectToken("site");
            var tmp_content = jo.SelectToken("content");
            if(tmp_content != null)
                tmp_weather.Content = tmp_content.ToString();
            tmp_weather.Province = (string)jo.SelectToken("province");
            tmp_weather.Updatetime = DateTime.Parse((string)jo.SelectToken("updatetime")??DateTime.Now.ToString());
            tmp_weather.City = (string)jo.SelectToken("city");
            weather.Update(ref tmp_weather);
        }
        public async Task UpdateWeatherAsync(string site, string jsonInfo)
        {
            var weather = await Context.WeatherTable.SingleAsync(e => e.Site == site);
            var jo = JObject.Parse(jsonInfo);
            var tmp_weather = new WeatherTable();
            tmp_weather.Site = (string)jo.SelectToken("site");
            var tmp_content = jo.SelectToken("content");
            if (tmp_content != null)
                tmp_weather.Content = tmp_content.ToString();
            tmp_weather.Province = (string)jo.SelectToken("province");
            tmp_weather.Updatetime = DateTime.Parse((string)jo.SelectToken("updatetime") ?? DateTime.Now.ToString());
            tmp_weather.City = (string)jo.SelectToken("city");
            weather.Update(ref tmp_weather);
        }
        /// <summary>
        /// update user information,included password,Ip,Username
        /// </summary>
        /// <param name="username"></param>
        /// <param name="jsonInfo"></param>
        /// <returns></returns>
        public void UpdateUser(string username,string jsonInfo)
        {
            var user = Context.UserTable.Single(e => e.Username == username);
            if (user == null) return;
            var jo = JObject.Parse(jsonInfo);
            var tmp_user = new UserTable();
            tmp_user.Ip = (string)jo["ip"];
            tmp_user.Password = (string)jo["password"];
            tmp_user.Username = (string)jo["username"];
            //tmp_user.Updatetime = DateTime.Parse(jo["updatetime"].ToString());
            user.Update(ref tmp_user);
            
        }
        public async Task UpdateUserAsync(string username, string jsonInfo)
        {
            var user = await Context.UserTable.SingleAsync(e => e.Username == username);
            if (user == null) return;
            var jo = JObject.Parse(jsonInfo);
            var tmp_user = new UserTable();
            tmp_user.Ip = (string)jo["ip"];
            tmp_user.Password = (string)jo["password"];
            tmp_user.Username = (string)jo["username"];
            //tmp_user.Updatetime = DateTime.Parse(jo["updatetime"].ToString());
            user.Update(ref tmp_user);

        }
        /// <summary>
        /// update site that user set
        /// </summary>
        /// <param name="username"></param>
        /// <param name="jsonInfo"></param>
        /// <returns></returns>
        public void UpdateRecord(string username,string jsonInfo)
        {
            var recoredOfUser = Context.RecordTable
                                    .Where(r => r.Username == username)
                                    .ToList();
            var jo = JObject.Parse(jsonInfo);
            foreach(var record in recoredOfUser)
            {
                var obj = jo.SelectToken(record.Site);
                if ( obj== null) continue;
                record.Site=obj.ToString();
            }
            //return SaveChange();
        }

        public async Task UpdateRecordAsync(string username, string jsonInfo)
        {
            var recoredOfUser = await Context.RecordTable
                                    .Where(r => r.Username == username)
                                    .ToListAsync();
            var jo = JObject.Parse(jsonInfo);
            foreach (var record in recoredOfUser)
            {
                var obj = jo.SelectToken(record.Site);
                if (obj == null) continue;
                record.Site = obj.ToString();
            }
            //return SaveChange();
        }
        public void RemoveUser(string username)
        {
            var users = Context.UserTable
                .Where(u => u.Username==username)
                .ToList();
            Context.RemoveRange(users);
            //return SaveChange();
        }
        public async Task RemoveUserAsync(string username)
        {
            var users = await Context.UserTable
                .Where(u => u.Username == username)
                .ToListAsync();
            Context.RemoveRange(users);
            
            //return SaveChange();
        }
        public void RemoveRecord(string[] userAndSite)
        {
          
            var records = Context.RecordTable
                .Where(r => r.Username==userAndSite[0] && r.Site==userAndSite[1])
                .ToList();
          
            Context.RemoveRange(records);
            //return SaveChange();
        }
        public async Task RemoveRecordAsync(string[] userAndSite)
        {

            var records = await Context.RecordTable
                .Where(r => r.Username == userAndSite[0] && r.Site == userAndSite[1])
                .ToListAsync();

            Context.RemoveRange(records);
            //return SaveChange();
        }
        public void RemoveWeather(string sitename)
        {
            var sites = Context.WeatherTable
                .Where(s => s.Site==sitename)
                .ToList();
            Context.RemoveRange(sites);
        }
        public async Task RemoveWeatherAsync(string sitename)
        {
            var sites = await Context.WeatherTable
                .Where(s => s.Site == sitename)
                .ToListAsync();
            Context.RemoveRange(sites);
        }

        /// <summary>
        /// add funcitons,provide async version;
        /// async version will auto add SaveChange() function,
        /// and return Task<bool> indicate wheater sucess or not
        /// </summary>
        /// <param name="jsonInfo"></param>
        /// <returns></returns>
        public void AddUser(string jsonInfo)
        {
            Add<UserTable>(jsonInfo);
        }

        public async Task<int> AddUserAsync(string jsonInfo)
        {
      
            return await AddAsync<UserTable>(jsonInfo);
        }

        public void AddWeather(string jsonInfo)
        {
            Add<WeatherTable>(jsonInfo);
        }

        public async Task<int> AddWeatherAsync(string jsonInfo)
        {
            return await AddAsync<WeatherTable>(jsonInfo);
        }

        public void AddRecord(string jsonInfo)
        {
            Add<RecordTable>(jsonInfo);
        }

        public async Task<int> AddRecordAsync(string jsonInfo)
        {
            return await AddAsync<RecordTable>(jsonInfo);
        }


        //private template
        void Add<T>(string jsonInfo)
        {

            T Ts = JsonConvert.DeserializeObject<T>(jsonInfo);
            Context.Add(Ts);
        }
        async Task<int> AddAsync<T>(string jsonInfo)
        {
            T Ts = JsonConvert.DeserializeObject<T>(jsonInfo);
            await Context.AddAsync(Ts);
            return await Context.SaveChangesAsync();
        }


        public async Task<int> SaveChangeAsync()
        {
            return await Context.SaveChangesAsync();
        }
        public int SaveChange() => Context.SaveChanges();


    }
}
