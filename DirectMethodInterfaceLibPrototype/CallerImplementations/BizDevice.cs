using DirectMethodCommunicationsLibPrototype;
using DirectMethodInterfaceLibPrototype.Data;
using DirectMethodInterfaceLibPrototype.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace DirectMethodInterfaceLibPrototype.CallerImplementations
{
    // TODO : Think more about how to make this foolproof
    // Copy paste allows functions to be routed incorrectly by accident
    // It might be nice to avoid that - but we can generate a runtime error
    public class BizDevice : IBizDevice
    {
        public void Heartbeat()
        {
            DirectMethodHelper.CallDirectMethod(this, Heartbeat);
        }

        public MethodOutput BizFunction1(MethodInput methodInput)
        {
            return DirectMethodHelper.CallDirectMethod(this, BizFunction1, methodInput);
        }

        public bool BizFunctionBool()
        {
            return DirectMethodHelper.CallDirectMethod(this, BizFunctionBool);
        }

        public void BizActionBool(bool b)
        {
            DirectMethodHelper.CallDirectMethod(this, BizActionBool, b);
        }
    }
}
