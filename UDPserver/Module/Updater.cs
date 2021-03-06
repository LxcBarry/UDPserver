﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace UDPserver
{
    public interface IUpdater
    {
        public void AutoUpdate();
    }

    public class WeatherUpdater:IUpdater
    {
        private readonly WeatherGetter weatherGetter;
        //private readonly UDPserver server;
        private readonly ServerBLL bLL;
        public WeatherUpdater(WeatherGetter dataGetter,ServerBLL bll)
        {
            weatherGetter = dataGetter;
            //server = uDPserver;
            bLL = bll;
        }
        public void AutoUpdate()
        {
            var reader = new AppSettingsReader();
            var updateTime = (Int32)reader.GetValue("updateWeatherTime", typeof(Int32));
            var t = new Task(() =>
            {
                //Console.WriteLine($"{DateTime.Now}: start update weather thread");
                Logger.Info("start update weather thread");
                while (true)
                {
                    UpdateFuncAsync();
                    Thread.Sleep(updateTime);
                }
            },
            TaskCreationOptions.LongRunning);
            t.Start();
        }

        private async void UpdateFuncAsync()
        {
            Logger.Info("开始更新天气信息");
            
            var result = await weatherGetter.UpdateWeatherAsync();
            if (result > 0)
            {
                Logger.Info("开始天气推送");
                //server.SendMessage();
                await bLL.SendWeather();
                Logger.Info("已经把天气推送到发送进程中");
            }
            else
            {
                Logger.Info("服务器天气信息未更新");
            }
        }
    }

}
