using CredentialProvider.Interop;
using Penta.EeWin.Cp.Base.CredentialContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Penta.EeWin.Cp.IdPwCp.CredentialContext
{
    public class IdPwPasswordChangeCredentialContext : PasswordChangeCredentialContext
    {
        public IdPwPasswordChangeCredentialContext(in Guid cpClsid, in _CREDENTIAL_PROVIDER_USAGE_SCENARIO cpus) : base(cpClsid, cpus)
        {
        }
    }
}
