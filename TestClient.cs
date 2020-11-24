using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UDPSendTest
{
    class Program
    {
        /// <summary>
        /// 用于UDP发送的网络服务类
        /// </summary>
        private static UdpClient udpcSend = null;

        static IPEndPoint localIpep = null;

        public static void TestMain(string[] args)
        {
            localIpep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8889); // 本机IP和监听端口号
            udpcSend = new UdpClient(localIpep);


            // 实名发送
            string msg = null;
            while ((msg = Console.ReadLine()) != null)
            {
                Thread thrSend = new Thread(SendMessage);
                thrSend.Start(msg);
            }
            Console.ReadKey();
        }

        /// <summary>
        /// 发送信息
        /// </summary>
        /// <param name="obj"></param>
        private static void SendMessage(object obj)
        {
            try
            {
                string message = (string)obj;
                byte[] sendbytes = Encoding.Unicode.GetBytes(message);
                IPEndPoint remoteIpep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8899); // 发送到的IP地址和端口号
                udpcSend.Send(sendbytes, sendbytes.Length, remoteIpep);
                //udpcSend.Close();
            }
            catch { }
        }

    }
}
