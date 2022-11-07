using CredentialProvider.Interop;
using System;
using System.ServiceProcess;
using Penta.EeWin.Common;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Penta.EeWin.ServerApi;
using Penta.EeWin.Common.Registry;
using Penta.EeWin.ServerApi.Data;
using Penta.EeWin.ServerApi.Data.Response;

namespace Penta.EeWin.Cp.Filter
{
    [ComVisible(true)]
    [Guid("C071F065-E347-4F68-9219-E09DA48DF919")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("Isign.CpFilter")]
    public class ProviderFilter : ICredentialProviderFilter
    {
        enum CP_ID
        {
            OTP,
            FACE,
            QR,
            SIMPLE
        }

        public int Filter(_CREDENTIAL_PROVIDER_USAGE_SCENARIO cpus, uint dwFlags, IntPtr rgclsidProviders, IntPtr rgbAllow, uint cProviders)
        {
#if DEBUG
            // break point
            MessageBox.Show("Filter Test");
#endif
            List<string> cpGuid;

            try
            {
                // 네트워크 상태 확인하기
                CheckNetworkStatus();

                // 서버에서 cp 목록 얻기
                GetCpGuidListFromServer(out cpGuid);
            }
            catch(Exception ex)
            {
                // 레지스트리에서 cp 목록 얻기                
                Registry.GetCpGuidList(out cpGuid);
            }

            // 필터 값 적용
            var size = Marshal.SizeOf(typeof(System.Guid));
            System.Guid[] rgclsidProviderArray = new System.Guid[cProviders];
            for (int index = 0; index < cProviders; index++)
            {
                rgclsidProviderArray[index] = Marshal.PtrToStructure<System.Guid>(rgclsidProviders + index * size);
            }

            int[] rgbAllowArray = new int[cProviders];
            Marshal.Copy(rgbAllow, rgbAllowArray, 0, (int)cProviders);

            switch (cpus)
            {
                case _CREDENTIAL_PROVIDER_USAGE_SCENARIO.CPUS_LOGON:
                case _CREDENTIAL_PROVIDER_USAGE_SCENARIO.CPUS_UNLOCK_WORKSTATION:
                case _CREDENTIAL_PROVIDER_USAGE_SCENARIO.CPUS_CHANGE_PASSWORD:
                {
                    if (cpGuid.Count > 0)
                    {
                        for (uint index = 0; index < cProviders; index++)
                        {
                            if (cpGuid.Contains(rgclsidProviderArray[index].ToString()))
                                rgbAllowArray[index] = Convert.ToInt32(true);
                            else
                                rgbAllowArray[index] = Convert.ToInt32(false);
                        }

                        Marshal.Copy(rgbAllowArray, 0, rgbAllow, (int)cProviders);
                    }

                    return HResultValues.S_OK;
                }
                case _CREDENTIAL_PROVIDER_USAGE_SCENARIO.CPUS_CREDUI:
                case _CREDENTIAL_PROVIDER_USAGE_SCENARIO.CPUS_PLAP:
                    return HResultValues.S_OK;
                default:
                    return HResultValues.E_INVALIDARG;
            }
        }

        public int UpdateRemoteCredential(ref _CREDENTIAL_PROVIDER_CREDENTIAL_SERIALIZATION pcpcsIn, out _CREDENTIAL_PROVIDER_CREDENTIAL_SERIALIZATION pcpcsOut)
        {
            pcpcsOut = default;

            return HResultValues.E_NOTIMPL;
        }

        const string EEWIN_AGENT_ID = "-1";
        void GetCpGuidListFromServer(out List<string> cpGuid)
        {
            cpGuid = new List<string>();

            // 서버와의 통신 확인
            string url, port, retryCount;
            Registry.GetApcServerInfo(out url, out port);
            Registry.getRetryCount(out retryCount);

            string userId;
            Registry.getLastLoggedOnSsoId(out userId);
            if (userId == string.Empty)
                Registry.getLastLoggedOnWindowsUserId(out userId);

            AuthList response;
            RequestInfo requestInfo = new RequestInfo(url, port, Convert.ToInt32(retryCount));
            GetAuthList getAuthList = new GetAuthList(requestInfo);            
            getAuthList.Request(EEWIN_AGENT_ID, userId, out response);

            if (response.policy.factorList == null)
                throw new ArgumentNullException();

            // Converting
            if (response.policy.factorList[0] == null)
                throw new ArgumentNullException();

            foreach (FactorList item in response.policy.factorList[0])
            {
                if (item.type == CP_ID.OTP.ToString())
                {
                    cpGuid.Add(Common.Guid.motpCp.ToString(Common.Guid.GUID_FORMAT_BRACES));
                }
                else if (item.type == CP_ID.FACE.ToString())
                {
                    cpGuid.Add(Common.Guid.faceCp.ToString(Common.Guid.GUID_FORMAT_BRACES));
                }
                else if (item.type == CP_ID.QR.ToString())
                {
                    cpGuid.Add(Common.Guid.qrCp.ToString(Common.Guid.GUID_FORMAT_BRACES));
                }
                else if (item.type == CP_ID.SIMPLE.ToString())
                {
                    cpGuid.Add(Common.Guid.simpleCp.ToString(Common.Guid.GUID_FORMAT_BRACES));
                }
            }
        }

        const string NETWORK_SERVICE_NAME = "nsi";
        void CheckNetworkStatus()
        {
            // 네트워크 서비스 상태 확인
            string networkServiceWaitingTime;

            Registry.GetNetworkServiceWaitingTime(out networkServiceWaitingTime);
            TimeSpan networkServiceWaitingTimeSpan = TimeSpan.Parse(networkServiceWaitingTime);

            ServiceController sc = new ServiceController(NETWORK_SERVICE_NAME);
            sc.WaitForStatus(ServiceControllerStatus.Running, networkServiceWaitingTimeSpan);
            if (sc.Status != ServiceControllerStatus.Running)
                throw new System.ServiceProcess.TimeoutException();

            // 서버와의 통신 확인
            string url, port, retryCount;
            Registry.GetApcServerInfo(out url, out port);
            Registry.getRetryCount(out retryCount);

            RequestInfo requestInfo = new RequestInfo(url, port, Convert.ToInt32(retryCount));
            CheckServer checkServer = new CheckServer(requestInfo);
            checkServer.Request();            
        }
    }
}
