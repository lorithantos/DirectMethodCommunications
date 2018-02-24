using System;
using System.Collections.Generic;
using System.Text;

namespace DirectMethodCommunicationsLibPrototype
{
    [DirectMethod(Name ="BizAI")]
    public interface IDirectMethodBase
    {
        void Heartbeat();
    }
}
