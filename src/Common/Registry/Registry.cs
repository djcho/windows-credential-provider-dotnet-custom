using System;
using System.Collections.Generic;
using Microsoft.Win32;
using Penta.EeWin.Common.Registry.Data;

namespace Penta.EeWin.Common.Registry
{
    public static class Registry
    {
        public static void GetCpGuidList(out List<string> cpGuid)
        {
            cpGuid = new List<string>();
            RegistryKey regKey = default;

            try
            {
                regKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(KeyPathDefinition.EE_WIN_CP_LIST);
                if (regKey != null)
                {
                    string[] subKeyNames = regKey.GetSubKeyNames();
                    foreach (string item in subKeyNames)
                    {
                        cpGuid.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                if (regKey != null)
                    regKey.Close();
            }
        }

        public static void GetNetworkServiceWaitingTime(out string networkServiceWaitingTime)
        {
            networkServiceWaitingTime = "0";
            RegistryKey regKey = default;

            try
            {
                regKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(KeyPathDefinition.EE_WIN);
                if (regKey != null)
                {
                    object value = regKey.OpenSubKey(SubKeyNameDefinition.LOGON_MANAGER).GetValue(ValueNameDefinition.NETWORK_SERVICE_WAITING_TIME);
                    if (value != null)
                        networkServiceWaitingTime = value.ToString();
                }
            }
            catch(Exception ex)
            {

            }
            finally 
            {
                if (regKey != null)
                    regKey.Close();
            }
        }

        public static void getRetryCount(out string retryCount)
        {
            retryCount = "0";
            RegistryKey regKey = default;

            try
            {
                regKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(KeyPathDefinition.EE_WIN);
                if (regKey != null)
                {
                    object value = regKey.OpenSubKey(SubKeyNameDefinition.LOGON_MANAGER).GetValue(ValueNameDefinition.RETRY_COUNT);
                    if (value != null)
                        retryCount = value.ToString();
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                if (regKey != null)
                    regKey.Close();
            }
        }

        public static void GetApcServerInfo(out string ip, out string port)
        {
            ip = string.Empty;
            port = string.Empty;
            RegistryKey regKey = default;

            try
            {
                regKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(KeyPathDefinition.EE_WIN);
                if (regKey != null)
                {
                    object value = regKey.OpenSubKey(SubKeyNameDefinition.LOGON_MANAGER).GetValue(ValueNameDefinition.APC_ADDR);
                    if (value != null)
                        ip = value.ToString();

                    value = regKey.OpenSubKey(SubKeyNameDefinition.LOGON_MANAGER).GetValue(ValueNameDefinition.APC_PORT);
                    if (value != null)
                        port = value.ToString();
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                if (regKey != null)
                    regKey.Close();
            }
        }

        public static void GetIsignServerInfo(out string ip, out string port)
        {
            ip = string.Empty;
            port = string.Empty;
            RegistryKey regKey = default;

            try
            {
                regKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(KeyPathDefinition.EE_WIN);
                if (regKey != null)
                {
                    object value = regKey.OpenSubKey(SubKeyNameDefinition.LOGON_MANAGER).GetValue(ValueNameDefinition.HOST_ADDR);
                    if (value != null)
                        ip = value.ToString();

                    value = regKey.OpenSubKey(SubKeyNameDefinition.LOGON_MANAGER).GetValue(ValueNameDefinition.PORT_NUM);
                    if (value != null)
                        port = value.ToString();
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                if (regKey != null)
                    regKey.Close();
            }
        }

        public static bool isVmMode()
        {
            RegistryKey regKey = default;

            try
            {
                regKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Policies\VMware, Inc.\VMware VDM\Agent\Configuration");
                if (regKey == null)
                    return false;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                if (regKey != null)
                    regKey.Close();
            }

            return true;
        }

        public static void getLastLoggedOnSsoId(out string userId)
        {
            userId = string.Empty;
            RegistryKey regKey = default;

            try
            {
                regKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(KeyPathDefinition.EE_WIN);
                if (regKey != null)
                {
                    object value = regKey.OpenSubKey(SubKeyNameDefinition.LOGON_MANAGER).GetValue(ValueNameDefinition.LAST_LOGGED_ON_SSO_ID);
                    if (value != null)
                        userId = value.ToString();
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                if (regKey != null)
                    regKey.Close();
            }
        }

        public static void getLastLoggedOnWindowsUserId(out string userId)
        {
            userId = string.Empty;
            RegistryKey regKey = default;

            try
            { 
                regKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(KeyPathDefinition.WINDOWS_AUTHENTICATION);
                if (regKey != null)
                {
                    object value = regKey.OpenSubKey(SubKeyNameDefinition.LOGON_UI).GetValue(ValueNameDefinition.LAST_LOGGED_ON_USER);
                    if (value != null)
                        userId = value.ToString();
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                if (regKey != null)
                    regKey.Close();
            }
        }
    }
}
