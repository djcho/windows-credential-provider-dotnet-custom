using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using static Penta.EeWin.Cp.Base.Exception.ProviderError;

namespace Penta.EeWin.Cp.Base.Exception
{
    
    [Serializable]
    public class ProviderException : System.Exception
    {
        private ErrorCode errorCode;
        public ErrorCode ErrorCode { get { return errorCode; } }

        public ProviderException(ErrorCode errorCode, System.Exception innerException = null)
            : base(ProviderError.Parse(errorCode), innerException)
        {
            this.errorCode = errorCode;
        }

        protected ProviderException(in SerializationInfo info, in StreamingContext ctxt) : base(info, ctxt)
        {
        }
    }
}
