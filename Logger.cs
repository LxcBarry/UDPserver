using System;
using System.Collections.Generic;
using System.Text;

namespace UDPserver
{
    class Logger
    {
        public static void Debug(string str)
        {
            Console.WriteLine($"{DateTime.Now}: {str}");
        }

        public static void Info(string str)
        {
            Console.WriteLine($"{DateTime.Now}: {str}");
        }

        public static void Trace(string str)
        {
            Console.WriteLine($"{DateTime.Now}: {str}");
        }

        public static void Fatal(string str)
        {
            Console.WriteLine($"{DateTime.Now}: {str}");
        }
    }
}
