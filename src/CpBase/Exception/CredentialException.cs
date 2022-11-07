using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using static Penta.EeWin.Cp.Base.Exception.CredentialError;

namespace Penta.EeWin.Cp.Base.Exception
{
    
    [Serializable]
    public class CredentialException : System.Exception
    {
        private ErrorCode errorCode;
        public ErrorCode ErrorCode { get { return errorCode; } }

        public CredentialException(ErrorCode errorCode, System.Exception innerException = null)
            : base(CredentialError.Parse(errorCode), innerException)
        {
            this.errorCode = errorCode;
        }

        protected CredentialException(in SerializationInfo info, in StreamingContext ctxt) : base(info, ctxt)
        {
        }
    }
}
