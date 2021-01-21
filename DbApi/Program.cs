using System;
using DbApi.Models;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace DbApi
{
    /* 类名为 HelloWorld */
    class Program
    {
        /* main函数 */
        static void Main(string[] args)
        {
            //add function checked!
            // GetUser checked!
            //GetSiteRecord checked!
            //GetSiteRecordIp checked!
            //GetUserRecordWeather checked!
            //GetWeather checked!
            //Update function checked!
            //Remove function checked!
            ///* 我的第一个 C# 程序 */
            //Console.WriteLine("Hello World!");
            //Console.ReadKey();

            //!todo async not checked!
            DbApi api = new DbApi();

            //api.UpdateRecord("lxc2", "{\"xiamen\":\"guangzhou\"}");
            //api.UpdateUser("lxc2", "{\"Username\":\"lxc\",\"Ip\":\"10.0.0.0\"}");
            //api.UpdateWeather("guangzhou", "{}");
            //api.RemoveRecord(new string[2] { "lxc", "xiamen" } );
            //api.RemoveUser("lxc");
            //api.RemoveWeather("xiamen");
            //var result = api.GetUser("lxc3");
            //var result = api.GetWeather("xiamen");
            //var result = api.GetReference();
            //var result = api.AddUserAsync("{\"Username\":\"lxc11\"}");
            //api.SaveChange();
            //var result = api.GetReferenceAsync();
            //api.UpdateUser("lxc1", "{\"username\":\"lxc1\",\"ip\":\"0.0.0.0:8899\"}");

            //var result = api.SaveChangeAsync();
            var result = api.GetWeatherAsync("110105");
            //var g = api.GetCiteIpGroup().Result;
            //var result = g.GroupBy(e => e[0]);
            //foreach(var r in result)
            //{
            //    Console.WriteLine(r.Key);
            //    foreach (var item in r)
            //    {
            //        Console.WriteLine(item[1]);
            //    }
            //}
            //var result = api.CheckUserAsync("{\"username\":\"lxc\",\"password\":\"123\"}");

            //Console.WriteLine(result.Result);
            //result = api.CheckUserAsync("{\"username\":\"lxc1\",\"password\":\"123\"}");

            Console.WriteLine(result.Result);

            while (true)
            {

            }
            //await api.SaveChangeAsync();
            //Console.WriteLine(result.Result);
        }
    }
}