using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using QuickShare.Patch.Common.Models;

namespace QuickShare.Patch.Common.Commands
{
    public abstract class BaseCommand : ICommand
    {
        private static readonly List<ICommandPlugin> commandPlugins;
        protected string[] initArgs;

        static BaseCommand()
        {
            commandPlugins = IocContainer.Get<ICommandPlugin>();
        }
        public InstallContext InstallContext { get; protected set; }
        public IWatch Watch { get; protected set; }
        public string Name { get; protected set; }
        public CommandStatus CommandStatus { get; protected set; } = CommandStatus.UnInited;
        public RollbackStatus RollbackStatus { get; protected set; } = RollbackStatus.None;

        public void SetOwner(IWin32Window owner)
        {
            this.Owner = owner;
        }

        public IWin32Window Owner { get; protected set; }

        public virtual void SetContext(InstallContext context, IWatch watch, string name = null, params string[] args)
        {
            this.InstallContext = context;
            this.Watch = watch;
            this.Name = name;
            this.initArgs = args;
        }
        public abstract int GetProcessValue();
        public abstract bool Rollback();
        public abstract bool Run();
        public abstract void Init();
        public virtual bool CanRun { get; protected set; }
        public virtual bool CanRollback { get; protected set; }

        public static string[] GetCommands()
        {
            var internals = new[] { TextReplacer.CmdKey, SqlAdoExecutor.CmdKey, SqlCmdExecutor.CmdKey };
            var plugins = commandPlugins.Select(x => x.CmdKey).ToArray();
            return internals.Concat(plugins).ToArray();
        }
        public static ICommand GetCommand(string key)
        {
            if (key == SqlCmdExecutor.CmdKey)
            {
                return new SqlCmdExecutor();
            }
            if (key == SqlAdoExecutor.CmdKey)
            {
                return new SqlAdoExecutor();
            }
            if (key == TextReplacer.CmdKey)
            {
                return new TextReplacer();
            }

            var cmd = commandPlugins.FirstOrDefault(x => x.CmdKey == key);
            return cmd?.GetCommand();
        }
        public static string GetCommandDesc(string key)
        {
            if (key == SqlCmdExecutor.CmdKey)
            {
                return SqlCmdExecutor.CmdDesc;
            }
            if (key == SqlAdoExecutor.CmdKey)
            {
                return SqlAdoExecutor.CmdDesc;
            }
            if (key == TextReplacer.CmdKey)
            {
                return TextReplacer.CmdDesc;
            }

            var cmd = commandPlugins.FirstOrDefault(x => x.CmdKey == key);
            if (cmd != null)
            {
                return cmd.CmdDesc;
            }

            return "每一行是一个参数。\r\n该命令未定义其他参数说明。";
        }
    }

    public enum CommandStatus
    {
        UnInited,
        Inited,
        Running,
        Complete,
        Failed,
    }

    public enum RollbackStatus
    {
        None,
        Rollbacking,
        Complete,
        Failed,
    }
}
