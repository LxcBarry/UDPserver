using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace UDPserver
{
    /// <summary>
    /// 接受
    /// </summary>

    public class UDPserver
    {
        UdpClient udpcRecv = null;
        UdpClient udpcSend = null;
        IPEndPoint localIpep = null;
        bool IsUdpcRecvStart = false;
        Task thrRecv;
        //Task thrSend;
        private readonly ServerBLL _disposal;
        
        public UDPserver(ServerBLL disposal)
        {
            _disposal = disposal;
            udpcSend = new UdpClient(8888);
            var reader = new AppSettingsReader();
            var data = reader.GetValue("listenSocket", typeof(string)).ToString();
            var ipAndPort = data.Split(":");

            localIpep = new IPEndPoint(IPAddress.Parse(ipAndPort[0]), int.Parse(ipAndPort[1])); // 本机IP和监听端口号
        }

        public void StartRecv()
        {
            //if (!IsUdpcRecvStart) // 未监听的情况，开始监听
            //{
            udpcRecv = new UdpClient(localIpep);
            Logger.Info($"UDP开始监听{localIpep}");
            //thrRecv = new Thread(ReceiveMessage);
            //thrRecv.Start();
            thrRecv = new Task(() =>
            {
                Logger.Info("UDP服务器开始监听");
                ReceiveMessage();
            },
            TaskCreationOptions.LongRunning);
            thrRecv.Start();
            IsUdpcRecvStart = true;

            //Console.WriteLine(": UDP监听器已成功启动");
            //}
        }
           
        //public void StartSend()
        //{
        //    udpcSend = new UdpClient(8888);
        //    var reader = new AppSettingsReader();
        //    var sendCycle = (Int32)reader.GetValue("sendDataTime", typeof(Int32));
        //    thrSend = new Task(() =>
        //    {
        //        Logger.Info("UDP服务器开始推送数据");
        //        SendMessage();
        //    },
        //    TaskCreationOptions.LongRunning);
        //    thrSend.Start();
        //}
        //public  void Stop()
        //{
        //    if (IsUdpcRecvStart)
        //    {
                
        //        thrRecv.Dispose(); // 必须先关闭这个线程，否则会异常
        //        udpcRecv.Close();
        //        IsUdpcRecvStart = false;
        //        Console.WriteLine(": UDP监听器已成功关闭");
        //    }
        //}

        /// <summary>
        /// 接收数据
        /// </summary>
        private  void ReceiveMessage()
        {
            while (IsUdpcRecvStart)
            {
                try
                {
                    byte[] bytRecv = udpcRecv.Receive(ref localIpep);
                    string message = Encoding.UTF8.GetString(bytRecv, 0, bytRecv.Length);
                    message = message.Replace("\0","");
                    Logger.Info($"receive message {message}");
                    var result = _disposal.DisposalAsync(message);
                    //Console.WriteLine(string.Format("{0}[{1}]", localIpep, message));
                    //TODO 后续删除
                    //Thread.Sleep(1000);
                    //Console.WriteLine(result.Result);
                }
                catch (Exception ex)
                {
                    //Console.WriteLine(ex.Message);
                    Logger.Fatal(ex.Message);
                    break;
                }
            }
        }
        //public async void SendMessage()
        //{
        //    try
        //    {
        //        var data = await _disposal.GetSendDataAsync();
        //        var msgs = JsonConvert.DeserializeObject<List<JObject>>(data);
        //        foreach (var msg in msgs)
        //        {
        //            byte[] sendbytes = Encoding.Unicode.GetBytes(msg["Content"].ToString());
        //            var ipAndPort = msg["Ip"].ToString().Split(":");
        //            IPEndPoint remoteIpep = new IPEndPoint(IPAddress.Parse(ipAndPort[0]), int.Parse(ipAndPort[1])); // 发送到的IP地址和端口号
        //            var result = udpcSend.SendAsync(sendbytes, sendbytes.Length, remoteIpep);
        //        }
        //        Logger.Info("已经推送数据");
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Fatal($"发送数据异常: {ex.Message}");
        //    }
        //}

    }
}