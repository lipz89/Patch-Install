using System;
using QuickShare.Patch.Common.Models;

namespace QuickShare.Patch.Common.Util
{
    public class InstallEventArgs : EventArgs
    {
        public bool AllowNext { get; set; } = true;
        public string ConfirmMessage { get; set; }
        public Action<InstallContext> ConfirmAction { get; set; }
    }
}