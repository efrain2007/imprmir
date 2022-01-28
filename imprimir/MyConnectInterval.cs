using SocketIOClient.ConnectInterval;
using System;

namespace imprmir
{
    class MyConnectInterval : IConnectInterval
    {
        public MyConnectInterval()
        {
            delay = 1000;
        }

        double delay;

        public int GetDelay()
        {
           // Console.WriteLine("GetDelay: " + delay);
            return (int)delay;
        }

        public double NextDealy()
        {
           // Console.WriteLine("NextDealy: " + (delay + 1000));
            return delay += 1000;
        }
    }
}
