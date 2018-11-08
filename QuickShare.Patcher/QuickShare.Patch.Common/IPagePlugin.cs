using System;
using System.Collections.Generic;
using System.Windows.Forms;
using QuickShare.Patch.Common.Models;
using QuickShare.Patch.Common.Util;

namespace QuickShare.Patch.Common
{
    public interface IPagePlugin
    {
        string Title { get; }
        string SubTitle { get; }
        List<PageItem> GetItems();
        Form GelForm();
        event EventHandler<InstallEventArgs> InstallChanged;
        //Func<List<PageItem>, InstallEventArgs> GetInitEvent();
        string Privoder { get; }
    }
}