using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using DbApi;
using UDPserver.Disposal;
using Autofac;
namespace UDPserver
{
    class Program
    {
        static void Main(string[] args)
        {

            //InitWeather checked!
            //Console.WriteLine("hello world!");
            //var api = new DbApi.DbApi();
            //WeatherGetter weatherGetter = new WeatherGetter(ref api);
            //TimerUpdater updater = new TimerUpdater();
            //updater.RunUpdateWeather(weatherGetter.UpdateWeather);
            //var autoEvent = new AutoResetEvent(false);
            //var result = api.GetWeather("110000");
            //var result = api.GetReferenceAsync();
            //Console.WriteLine(result.Result);
            //var result = weatherGetter.GetWeather("北京市");
            //var result = weatherGetter.InitWeather();
            //var result = weatherGetter.UpdateWeather();
            //Console.WriteLine(result);
            var cb = new ContainerBuilder();
            //数据接口层
            cb.RegisterType<DbApi.DbApi>()
                .AsSelf()
                .SingleInstance();
            //数据更新
            cb.RegisterType<WeatherUpdater>()
                .Named<IUpdater>("weatherUpdater")
                .SingleInstance();
            cb.RegisterType<WeatherGetter>()
                .AsSelf()
                .SingleInstance();
            //服务逻辑
            cb.RegisterType<ServerBLL>()   //需要新增命令时，在这个类中增加服务实例
                .AsSelf()
                .SingleInstance();
            cb.RegisterType<LoginDisposal>() //登陆处理
                .AsSelf()
                .SingleInstance();
            cb.RegisterType<SendDisposal>()  //推送处理
                .AsSelf()
                .SingleInstance();
            cb.RegisterType<RegisterDisposal>()  //订阅处理
                .AsSelf()
                .SingleInstance();
            cb.RegisterType<PasswordDisposal>()  //修改密码
                .AsSelf()
                .SingleInstance();
            //服务器
            cb.RegisterType<UDPserver>()
                .AsSelf()
                .SingleInstance();
            
            var container = cb.Build();

            //var updater = WeatherGetter();

            //var weatherUpdater = container.ResolveNamed<IUpdater>("weatherUpdater");
            //weatherUpdater.AutoUpdate();
            //var result = weatherUpdater.
            //var updater = container.
            //weatherUpdater.AutoUpdate();
            var server = container.Resolve<UDPserver>();
            server.StartRecv();
            //server.SendMessage();
            //var log = new Log();
            //Logger.Info("hello world!");
            //Logger.Debug("Debug");
            while (true) { } //主线程

            //Console.WriteLine("end");

        }
    }
}
