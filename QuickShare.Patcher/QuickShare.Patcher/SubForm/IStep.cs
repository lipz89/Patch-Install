using System;
using QuickShare.Patch.Common;
using QuickShare.Patch.Common.Models;

namespace QuickShare.Patcher.SubForm
{
    interface IStep
    {
        string Title { get; }
        string SubTitle { get; }
        void SetChangeHandler(EventHandler handler);
        void SetContext(Context context, bool? isFromPrev = null);
        void ChangeContext();
        bool Check();
    }

    interface IProcess : ILog
    {
        void Begin();
        void End();
    }
}