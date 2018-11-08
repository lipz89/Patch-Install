using System;

namespace QuickShare.Patch.Common.Models
{
    [Serializable]
    public enum FileOverride
    {
        Always,
        Never,
        Try,
        Newer
    }
}