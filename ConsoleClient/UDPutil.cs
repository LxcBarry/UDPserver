using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;

namespace ConsoleClient
{
    public class UDPutil
    {
        private readonly UdpClient udpcSend;
        private readonly UdpClient udpcRecv;
        public  IPEndPoint localIpep;
        private readonly string ServerIp;
        private readonly int ServerPort;
        public UDPutil()
        {
            Logger.Info("初始化udp客户端");
            var reader = new AppSettingsReader();
            var ipAndHost = reader.GetValue("serverHost", typeof(string)).ToString().Split(":");
            var localIp = reader.GetValue("localIp", typeof(string)).ToString();
            ServerIp = ipAndHost[0]; ServerPort = int.Parse(ipAndHost[1]);
            Logger.Info($"监听地址: {string.Join(":", ipAndHost)}");
            var localPort = FreePort.GetFirstAvailablePort();
            localIpep = new IPEndPoint(IPAddress.Parse(localIp), localPort);
            Logger.Info($"获取本地套接字:{localIpep}");
            Logger.Info($"初始化udp客户端完成");
            udpcSend = new UdpClient(localIpep);
        }

        public async Task<int> SendMessage(string message)
        {
            try
            {
                byte[] sendbytes = Encoding.Unicode.GetBytes(message);
                IPEndPoint remoteIpep = new IPEndPoint(IPAddress.Parse(ServerIp), ServerPort); // 发送到的IP地址和端口号
                return await udpcSend.SendAsync(sendbytes, sendbytes.Length, remoteIpep);
                //udpcSend.Close();
            }
            catch {
                return -1;
            }
        }
        public async Task<string> ReceiveMessage()
        {

            try
            {
                //byte[] bytRecv = udpcSend.Receive(ref localIpep);
                var bytRecv = await udpcSend.ReceiveAsync();
                //udpcSend.ReceiveAsync()
                //byte[] bytRecv = await udpcSend.ReceiveAsync(ref localIpep);
                string message = Encoding.Unicode.GetString(bytRecv.Buffer, 0, bytRecv.Buffer.Length);
                message = message.Replace("\0", "");
                //Console.WriteLine(message);
                return message;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

        }

    }
}
