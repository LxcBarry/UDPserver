using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace UDPserver.Disposal
{
    public interface IDisposal
    {
        public string Run(string msg);
        public Task<string> RunAsync(string msg);
    }
}
