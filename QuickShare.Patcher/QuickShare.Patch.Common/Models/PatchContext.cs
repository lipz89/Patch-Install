using QuickShare.Patch.Common.Commands;

namespace QuickShare.Patch.Common.Models
{
    public class PatchContext
    {
        public bool IsNeedStore { get; set; } = true;
        public string Path { get; set; }
        public string ConnectionString { get; set; }
        public ScriptFiles Files { get; set; }
        public bool ShowDetails { get; set; } = true;
    }
}