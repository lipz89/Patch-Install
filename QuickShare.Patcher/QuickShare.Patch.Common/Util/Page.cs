using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using QuickShare.Patch.Common.Models;

namespace QuickShare.Patch.Common.Util
{
    public class Page
    {
        public Form Form { get; set; }
        public PageInfo PageInfo { get; set; }

        public event EventHandler<InstallEventArgs> InstallChanged;

        internal void OnChanged(object control, InstallEventArgs args)
        {
            this.InstallChanged?.Invoke(control, args);
        }

        public void Init(bool enable = true)
        {
            var hasInvalid = enable && this.PageInfo.Items.Any(x => x.IsRequired && string.IsNullOrEmpty(x.Value));
            this.OnChanged(this.Form, new InstallEventArgs()
            {
                AllowNext = !hasInvalid
            });
        }
    }
}