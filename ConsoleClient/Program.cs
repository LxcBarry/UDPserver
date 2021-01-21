using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ConsoleTables;
using System.IO;
using System.Configuration;

namespace ConsoleClient
{
    class Program
    {
        private static UDPutil udpClient;
        private static string WEACache;
        private static Dictionary<String, String> CodeAdMap;
        private static Dictionary<String, String> AdCodeMap;
        private static string username;
        private static string password;
        private static int maxtry;

        private static void LoadMap()
        {
            CodeAdMap = new Dictionary<string, string>();
            AdCodeMap = new Dictionary<string, string>();
            //TODO 加载CodeAdMap和AdCodeMap;
            var strs = ConsoleClient.Properties.Resources.CodeMap.Split("\r\n");
            foreach(var str in strs)
            {
                var tmp = str.Split(',');
                CodeAdMap.Add(tmp[1], tmp[0]);
                AdCodeMap.Add(tmp[0], tmp[1]);
            }
        }
        private static string GetPass()
        {
            string ret = null;
            while (true)
            {
                //存储用户输入的按键，并且在输入的位置不显示字符
                var ck = Console.ReadKey(true);

                //判断用户是否按下的Enter键
                if (ck.Key != ConsoleKey.Enter)
                {
                    if (ck.Key != ConsoleKey.Backspace)
                    {
                        //将用户输入的字符存入字符串中
                        ret += ck.KeyChar.ToString();
                        //将用户输入的字符替换为*
                        Console.Write("*");
                    }
                    else
                    {
                        //删除错误的字符
                        Console.Write("\b \b");
                    }
                }
                else
                {
                    Console.WriteLine();

                    break;
                }
            }
            return ret;
        }
        static void Main(string[] args)
        {
            var reader = new AppSettingsReader();
            maxtry = (int)reader.GetValue("maxtry", typeof(int));

            string tmp = "欢迎使用!";
            //Console.WriteLine("");
            Console.SetCursorPosition((Console.WindowWidth - tmp.Length) / 2, 1);
            Console.WriteLine(tmp);
            #region 客户端初始化
            udpClient = new UDPutil();
            //加载codemap
            LoadMap();
            #endregion

            #region 登录
            bool stopLoop = false;
            while (true)
            {

                if (stopLoop) break;
                Console.Write("你的用户名:");
                username = Console.ReadLine();
                Console.Write("你的密码:");
                password = GetPass();

                var LoginJo = new JObject() {
                {"username",username },
                {"password",password },
                {"ip",udpClient.localIpep.ToString() }
                };
                udpClient.SendMessage("LOG" + LoginJo.ToString()).Wait();
                var result = udpClient.ReceiveMessage().Result;
                if (result == "FALLOG")
                {
                    Console.WriteLine("登录失败!服务器繁忙或密码不正确");
                    continue;
                }
                else if (result == "SUCLOG") stopLoop = true;
                else
                {
                    result = udpClient.ReceiveMessage().Result;
                    if (result == "SUCLOG") stopLoop = true;
                    else return;
                }

            }

            #endregion
            #region 选择操作
            Console.Clear();

            stopLoop = false;
            PrintChoise();
            while (true)
            {
                if (stopLoop) break;
                var choise = Console.ReadKey(true);
                switch (choise.Key)
                {
                    case ConsoleKey.C:
                        Console.Clear();
                        PrintRegister();
                        break;
                    case ConsoleKey.R:
                        //Console.Clear();
                        RegisterCity(); //TODO待测试
                        break;
                    case ConsoleKey.U:
                        //Console.Clear();
                        UpdatePassword(); 
                        break;
                    case ConsoleKey.D:
                        //Console.Clear();
                        RemoveReg(); //TODO待测试
                        break;
                    case ConsoleKey.Tab:
                        Console.Clear();
                        PrintChoise();
                        //stopLoop = true;
                        break;
                    case ConsoleKey.Escape:
                        Console.Clear();
                        Console.WriteLine("正在等待订阅信息");
                        stopLoop = true;
                        break;

                    default:
                        break;
                }
            }

            #endregion
            #region 接收订阅
            // 获得订阅后清屏
            PrintRegisterInfo();
            #endregion
        }
        private static async Task RemoveReg()
        {
            Console.WriteLine("请一行中输入取消订阅的城市/市区，以逗号分隔");
            var sitesAd = Console.ReadLine().Split(',');
            List<string> sites = new List<string>();
            foreach (var ad in sitesAd) sites.Add(AdCodeMap[ad]);
            var msgJo = new { username, password, ip= udpClient.localIpep.ToString(), sites };
            await udpClient.SendMessage("DER" + JsonConvert.SerializeObject(msgJo));
            Console.WriteLine("已成功发送,按tab返回");

        }
        private static async Task UpdatePassword()
        {
            Console.WriteLine("请输入当前密码: ");
            var tmpPass = GetPass();
            if(tmpPass != password)
            {
                Console.WriteLine("密码错误!");
                return;
            }
            Console.WriteLine("请输入新的密码: ");
            var newPass = GetPass();
            Console.WriteLine("再次确认新的密码:");
            int c = 3;
            while (newPass != GetPass()&&c>0)
            {
                Console.WriteLine("密码不对应，请重试");
                c--;
            }
            if (c == 0) return;
            var msgJo = new JObject()
            {
                {"username",username },
                {"password",password },
                {"ip",udpClient.localIpep.ToString() },
                { "new",newPass}
            };
            await udpClient.SendMessage("PWD"+msgJo.ToString());
        }
        private static async Task RegisterCity()
        {
            Console.WriteLine("请在一行中以','为分隔输入城市/市区:");
            var sitesAd = Console.ReadLine().Split(",");
            List<string> sites = new List<string>();
            foreach (var ad in sitesAd)
            {
                if (AdCodeMap.TryGetValue(ad, out string adCode))
                    sites.Add(adCode);
                else
                    Console.WriteLine($"不提供{ad}的订阅");
            }
            var msgJo = new
            {
                username,
                password,
                ip = udpClient.localIpep.ToString(),
                sites
            };
            await udpClient.SendMessage("REG" + JsonConvert.SerializeObject(msgJo));
            Console.WriteLine("已成功发送,按tab返回");

        }
        private static async Task PrintRegister()
        {
            var msgJo = new JObject()
            {
                {"username",username },
                {"password",password },
                {"ip",udpClient.localIpep.ToString() }
            };
            await udpClient.SendMessage("GRE" + msgJo.ToString());
            var result = await udpClient.ReceiveMessage();
            int tryNum = 1;
            while (result.Substring(0, 6) != "SUCGRE" && tryNum < maxtry) { result = await udpClient.ReceiveMessage(); tryNum++; }
            if (tryNum >= maxtry) 
            {
                Console.WriteLine("请重试");
                return;
            }
            {
                var resultJo = JsonConvert.DeserializeObject<List<string>>(result[6..]);
                Console.WriteLine("已经订阅的城市/市区如下:");
                foreach(var jo in resultJo)
                {
                    
                    Console.Write(CodeAdMap.GetValueOrDefault(jo) + "\t");

                }
                Console.Write("\n");
            
                Console.WriteLine("按tab返回...");
            }
        }
        private static void ParseRegisterInfo(string msg)
        {
            var msgJo = JsonConvert.DeserializeObject<JObject>(msg);
            Console.WriteLine("城市/市区:" + msgJo["Live"][0]["city"]);
            var LiveJo = msgJo["Live"][0].ToObject<JObject>();                   //<JObject>
            var ForecastJo = msgJo["Forecast"][0]["casts"].ToObject<List<JObject>>(); //list<JObject>
            Console.WriteLine("今日天气:)");
            #region Live数据打印
            LiveJo.Remove("city");
            LiveJo.Remove("adcode");
            var table = new ConsoleTable("天气", "气温", "湿度", "风向", "风速", "更新时间");
            table.AddRow(
                LiveJo["weather"],
                LiveJo["temperature"]+"℃",
                LiveJo["humidity"],
                LiveJo["winddirection"],
                LiveJo["windpower"],
                LiveJo["reporttime"]
                );
            table.Write();
            #endregion
            #region Forecast数据打印
            Console.WriteLine("后面四天天气:)");
            table = new ConsoleTable("日期", "白天天气", "白天气温", "白天风向", "夜间天气", "夜间气温", "夜间风向");
            foreach(var jo in ForecastJo)
            {
                table.AddRow(
                    jo["date"],
                    jo["dayweather"],
                    jo["daytemp"] + "℃",
                    jo["daywind"],
                    jo["nightweather"],
                    jo["nighttemp"] + "℃",
                    jo["nightwind"]
                    );
            }
            table.Write();
            #endregion
        }
        private static void PrintChoise()
        {
            Console.Write("======= 可选操作 =======\n" +
                "C：查看订阅的城市\n" +
                "R：订阅\n" +
                "U：更改密码\n" +
                "D：退订城市\n"+
                "esc：开始接收订阅\n");
        }
        private static void PrintRegisterInfo()
        {
            Console.Clear(); //清屏  
            while (true)
            {
                if (WEACache == null)
                {
                    var result = udpClient.ReceiveMessage().Result;
                    if (result.Substring(0, 3) == "WEA")
                    {
                        ParseRegisterInfo(result[3..]);
                        Console.WriteLine("\n");
                    }
                }
                else
                {
                    Console.Clear(); 
                    ParseRegisterInfo(WEACache[3..]);
                    WEACache = null;
                }
            }
        }

    }

}
