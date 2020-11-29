using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UDPserver.Disposal;

namespace UDPserver
{
    public class ServerBLL
    {
        private readonly LoginDisposal _loginDisposal;
        private readonly SendDisposal _sendDisposal;
        private readonly RegisterDisposal _registerDisposal;
        private readonly PasswordDisposal _passwordDisposal;
        private readonly UdpClient UdpcSend;
        private readonly GetRegisterDisposal _getRegisterDisposal;
        private readonly RemoveRegDisposal _removeRegDisposal;
        public ServerBLL(
            LoginDisposal login,
            SendDisposal send,
            RegisterDisposal register,
            PasswordDisposal password,
            GetRegisterDisposal getRegister,
            RemoveRegDisposal removeRegDisposal
            )
        {
            _loginDisposal = login;
            _sendDisposal = send;
            _registerDisposal = register;
            _passwordDisposal = password;
            _getRegisterDisposal = getRegister;
            _removeRegDisposal = removeRegDisposal;
            //内置发送客户端
            UdpcSend = new UdpClient(8887);
        }
        
        public async Task<string> Login(string msg)
        {
            return await _loginDisposal.RunAsync(msg);
        }

        public async Task<string> Register(string msg)
        {
            return await _registerDisposal.RunAsync(msg);
        }

        public async Task<string> PwdUpdate(string msg)
        {
            return await _passwordDisposal.RunAsync(msg);
        }

        public async Task<string> GetRegister(string msg)
        {
            return await _getRegisterDisposal.RunAsync(msg);
        }
        public async Task<string> RemoveReg(string msg)
        {
            return await _removeRegDisposal.RunAsync(msg);
        }
        public async Task<string> DisposalAsync(string msg)
        {
            var str = msg.Substring(0, 3);
            string result = null;
            if ( str == "LOG")
            {
                result = await Login(msg[3..])==null?null:"LOG";
            }
            else if (str == "REG")
            {
                result = await Register(msg[3..])==null?null:"REG";
            }
            else if (str == "PWD")
            {
                result = await PwdUpdate(msg[3..])==null?null:"PWD";
            }
            else if(str == "GRE")
            {
                result = await GetRegister(msg[3..]);
                if (result != null) result = "GRE" + result;
            }
            else if(str == "DER")
            {
                result = await RemoveReg(msg[3..]);
            }
            else
            {
                Logger.Info("receive invalid command");
                return null;
                //return null;
            }
            // 可优化
            var jo = JsonConvert.DeserializeObject<JObject>(msg[3..]);
            if(result !=null)
                return await SendAsync("SUC"+result, jo["ip"].ToString())>0?"sucess":null;
            else
            {
                return await SendAsync("FAL"+result, jo["ip"].ToString()) > 0 ? "sucess" : null;
            }
        }

        private async Task<int> SendAsync(string msg,string host)
        {
            byte[] sendbytes = Encoding.Unicode.GetBytes(msg);
            var ipAndPort = host.Split(":");
            IPEndPoint remoteIpep = new IPEndPoint(IPAddress.Parse(ipAndPort[0]), int.Parse(ipAndPort[1])); // 发送到的IP地址和端口号
            return await UdpcSend.SendAsync(sendbytes, sendbytes.Length, remoteIpep);
        }
        /// <summary>
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public async Task<string> GetSendDataAsync(string msg = null)
        {
            return await _sendDisposal.RunAsync(msg);
        }
        public async Task SendWeather()
        {
            var msgs = await _sendDisposal.RunAsync();
            var msgsJo = JsonConvert.DeserializeObject<List<JObject>>(msgs);
            foreach(var jo in msgsJo)
            {
                var result = SendAsync("WEA"+jo["Content"].ToString(),jo["Ip"].ToString());
            }
            //return null;
        }

    }
}
