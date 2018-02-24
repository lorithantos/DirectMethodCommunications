using DirectMethodCommunicationsLibPrototype;
using DirectMethodReceiverPrototype.DeviceImpl;
using System;

namespace DirectMethodReceiverPrototype
{
    class Program
    {
        static void Main(string[] args)
        {
            DirectMethodHelper.AddDirectMethodImplementation<BizDeviceWorker>();

            Console.WriteLine("Press 'q' to exit");
            while (char.ToLowerInvariant(Console.ReadKey(true).KeyChar) != 'q')
            {
            }
        }
    }
}
