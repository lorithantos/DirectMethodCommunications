using DirectMethodInterfaceLibPrototype.CallerImplementations;
using DirectMethodInterfaceLibPrototype.Data;
using System;

namespace DirectMethodCallerPrototype
{
    class Program
    {
        static void Main(string[] args)
        {
            BizDevice bizDevices = new BizDevice { };

            bizDevices.Heartbeat();

            bizDevices.BizFunction1(new MethodInput
            {
                Input1 = 0,
                Input2 = -1
            });
        }
    }
}
