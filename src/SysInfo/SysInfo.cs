using Penta.EeWin.SysInfo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Penta.EeWin.SysInfo
{
    public class SysInfo
    {
        public static string GetDomainName()
        {
            throw new NotImplementedException();
        }

        public static bool IsInDomain()
        {
            Win32.NetJoinStatus status = Win32.NetJoinStatus.NetSetupUnknownStatus;
            IntPtr pDomain = IntPtr.Zero;
            int result = Win32.NetGetJoinInformation(null, out pDomain, out status);
            if (pDomain != IntPtr.Zero)
            {
                Win32.NetApiBufferFree(pDomain);
            }
            if (result == Win32.ErrorSuccess)
            {
                return status == Win32.NetJoinStatus.NetSetupDomainName;
            }
            else
            {
                throw new Exception("Domain Info Get Failed", new Win32Exception());
            }
        }
    }
}
