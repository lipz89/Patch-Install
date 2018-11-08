using QuickShare.Patch.Common.Commands;

namespace QuickShare.Patcher.IISPlugin
{
    public class CommandPlugin : ICommandPlugin
    {
        public string CmdKey { get; } = "$(IISConfiger)";
        public string CmdDesc { get; } = "通过“部署IIS应用程序”页面配置的参数配置IIS，所有参数会自动获取。";
        public ICommand GetCommand()
        {
            return new Command();
        }
    }
}