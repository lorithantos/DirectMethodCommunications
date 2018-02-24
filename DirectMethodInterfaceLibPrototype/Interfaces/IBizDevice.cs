using DirectMethodCommunicationsLibPrototype;
using DirectMethodInterfaceLibPrototype.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace DirectMethodInterfaceLibPrototype.Interfaces
{
    public interface IBizDevice : IDirectMethodBase
    {
        MethodOutput BizFunction1(MethodInput methodInput);
        bool BizFunctionBool();
        void BizActionBool(bool b);
    }
}
