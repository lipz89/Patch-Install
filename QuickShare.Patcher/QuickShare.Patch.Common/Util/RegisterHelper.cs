using System;
using System.Xml.Serialization;
using Microsoft.Win32;
using QuickShare.Patch.Common.Models;

namespace QuickShare.Patch.Common.Util
{
    public static class RegisterHelper
    {

        public static bool Write(RegInfo info)
        {
            var flag = WriteAppInfo(info);
            flag &= WriteUninstInfo(info);
            return flag;
        }
        private static bool WriteAppInfo(RegInfo info)
        {
            if (string.IsNullOrEmpty(info.ExeKey))
                return true;
            using (var softWare = RegInfo.Root.OpenSubKey(RegInfo.ProductDirRegkey, true))
            {
                if (CheckKey(softWare, "打开注册表项"))
                {
                    try
                    {
                        using (var appKey = softWare.OpenSubKey(info.ExeKey, true) ?? softWare.CreateSubKey(info.ExeKey))
                        {
                            if (CheckKey(appKey, "创建注册表项"))
                            {
                                return appKey.AddOrUpdateValue("", info.ExePath, RegistryValueKind.String);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Global.Log?.Error(e.Message);
                        return false;
                    }
                }
            }
            return false;
        }
        private static bool WriteUninstInfo(RegInfo info)
        {
            using (var softWare = RegInfo.Root.OpenSubKey(RegInfo.ProductUninstKey, true))
            {
                if (CheckKey(softWare, "打开注册表项"))
                {
                    try
                    {
                        using (var appKey = softWare.OpenSubKey(info.AppKey, true) ?? softWare.CreateSubKey(info.AppKey))
                        {
                            if (CheckKey(appKey, "创建注册表项"))
                            {
                                appKey.AddOrUpdateValue("DisplayName", info.DisplayName, RegistryValueKind.String);
                                appKey.AddOrUpdateValue("DisplayIcon", info.DisplayIcon, RegistryValueKind.String);
                                appKey.AddOrUpdateValue("DisplayVersion", info.DisplayVersion, RegistryValueKind.String);
                                appKey.AddOrUpdateValue("Publisher", info.Publisher, RegistryValueKind.String);
                                appKey.AddOrUpdateValue("IconGroupName", info.IconGroupName, RegistryValueKind.String);
                                appKey.AddOrUpdateValue("URLInfoAbout", info.URLInfoAbout, RegistryValueKind.String);
                                appKey.AddOrUpdateValue("UninstallString", info.UninstallString, RegistryValueKind.String);
                                appKey.AddOrUpdateValue("ExeKey", info.ExeKey, RegistryValueKind.String);
                                appKey.AddOrUpdateValue("ExePath", info.ExePath, RegistryValueKind.String);
                                appKey.AddOrUpdateValue("ExeArgs", info.ExeArgs, RegistryValueKind.String);
                                appKey.AddOrUpdateValue("InstallPath", info.InstallPath, RegistryValueKind.String);
                                appKey.AddOrUpdateValue("EstimatedSize", info.EstimatedSize, RegistryValueKind.DWord);
                                appKey?.Close();
                                return true;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Global.Log?.Error(e.Message);
                        return false;
                    }
                }
            }
            return false;
        }

        private static bool CheckKey(RegistryKey key, string info)
        {
            if (key == null)
            {
                Global.Log?.Error($"{info}失败");
                return false;
            }

            return true;
        }

        public static bool Read(string key, out RegInfo info)
        {
            info = null;
            using (var root = RegInfo.Root.OpenSubKey(RegInfo.ProductUninstKey))
            {
                var uninst = root.OpenSubKey(key);
                if (uninst == null)
                {
                    return true;
                }

                try
                {
                    info = new RegInfo();
                    info.AppKey = key;
                    info.DisplayIcon = uninst.GetValue("DisplayIcon")?.ToString();
                    info.DisplayName = uninst.GetValue("DisplayName")?.ToString();
                    info.DisplayVersion = uninst.GetValue("DisplayVersion")?.ToString();
                    info.IconGroupName = uninst.GetValue("IconGroupName")?.ToString();
                    info.UninstallString = uninst.GetValue("UninstallTarget")?.ToString();
                    info.URLInfoAbout = uninst.GetValue("URLInfoAbout")?.ToString();
                    info.Publisher = uninst.GetValue("Publisher")?.ToString();
                    info.ExeKey = uninst.GetValue("ExeKey")?.ToString();
                    info.InstallPath = uninst.GetValue("InstallPath")?.ToString();
                    info.ExePath = uninst.GetValue("ExePath")?.ToString();
                    info.ExeArgs = uninst.GetValue("ExeArgs")?.ToString();
                    return true;
                }
                catch (Exception e)
                {
                    Global.Log?.Error(e.Message);
                    return false;
                }
            }
        }

        public static void Delete(string path, string key)
        {
            using (var root = RegInfo.Root.OpenSubKey(path, true))
            {
                root?.DeleteSubKeyTree(key);
            }
        }

        private static bool AddOrUpdateValue(this RegistryKey key, string name, object value, RegistryValueKind? valueKind)
        {
            if (key == null || value == null)
            {
                return false;
            }

            var oldValue = key.GetValue(name);
            if (oldValue != null)
            {
                if (oldValue.ToString().Equals(value))
                {
                    return true;
                }
                key.DeleteValue(name);
            }

            if (valueKind.HasValue)
            {
                key.SetValue(name, value, valueKind.Value);
            }
            else
            {
                key.SetValue(name, value);
            }
            return true;
        }
    }
}
