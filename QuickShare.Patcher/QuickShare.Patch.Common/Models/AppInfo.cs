using System;

namespace QuickShare.Patch.Common.Models
{
    [Serializable]
    public class AppInfo
    {
        public AppInfo()
        {

        }

        public AppInfo(bool isFull)
        {
            AppName = "My Application";
            Version = isFull ? "1.0" : "1.1";
            Publisher = "快享医疗科技(上海)有限公司";//"Quick Share Healthcare Technology(ShangHai)Co.,Ltd."
            WebSite = "http://www.kxhealth.net";
            LastVersion = isFull ? "" : "1.0";
        }
        public string AppName { get; set; }
        public string Version { get; set; }
        public string Publisher { get; set; }
        public string WebSite { get; set; }
        public string LastVersion { get; set; }
        public string FullName
        {
            get
            {
                if (AppName != null && Version != null)
                {
                    return $"{AppName}.{Version}";
                }
                return null;
            }
        }
        public string TargetName
        {
            get
            {
                if (AppName != null && Version != null)
                {
                    if (!string.IsNullOrEmpty(LastVersion))
                    {
                        return $"{AppName}.{LastVersion}-{Version}";
                    }
                    else
                    {
                        return $"{AppName}.{Version}";
                    }
                }
                return null;
            }
        }
    }
}