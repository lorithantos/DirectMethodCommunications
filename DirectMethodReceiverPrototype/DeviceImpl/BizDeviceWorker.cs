using DirectMethodInterfaceLibPrototype.Data;
using DirectMethodInterfaceLibPrototype.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace DirectMethodReceiverPrototype.DeviceImpl
{
    class BizDeviceWorker : IBizDevice
    {
        public void Heartbeat()
        {
            Debug.WriteLine("Heartbeat");
        }

        public MethodOutput BizFunction1(MethodInput methodInput)
        {
            return new MethodOutput
            {
                Data1 = $"Data1 {methodInput.Input1}",
                Data2 = $"Data2 {methodInput.Input2}",
            };
        }

        public bool BizFunctionBool()
        {
            return true;
        }

        public void BizActionBool(bool b)
        {
        }
    }
}
