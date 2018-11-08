using System;
using QuickShare.Patch.Common.Models;
using QuickShare.Patch.Common.Util;

namespace QuickShare.ScriptPatcher.SubForm
{
    interface IStep
    {
        string Title { get; }
        string SubTitle { get; }
        void SetContext(PatchContext context, EventHandler<InstallEventArgs> handler);
        void ChangeContext();
        void ActiveStep();
    }
}