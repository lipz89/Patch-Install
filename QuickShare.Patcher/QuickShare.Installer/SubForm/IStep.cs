using System;
using QuickShare.Patch.Common;
using QuickShare.Patch.Common.Models;
using QuickShare.Patch.Common.Util;

namespace QuickShare.Installer.SubForm
{
    interface IStep
    {
        string Title { get; }
        string SubTitle { get; }
        void SetContext(InstallContext context, EventHandler<InstallEventArgs> handler);
        void ChangeContext();
        void ActiveStep(bool fromPrev);
    }
}