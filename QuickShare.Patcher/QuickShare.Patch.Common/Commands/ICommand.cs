using System.Windows.Forms;
using QuickShare.Patch.Common.Models;

namespace QuickShare.Patch.Common.Commands
{
    public interface ICommand
    {
        void Init();
        bool Run();
        bool Rollback();
        bool CanRun { get; }
        bool CanRollback { get; }
        void SetOwner(IWin32Window owner);
        void SetContext(InstallContext context, IWatch watch, string name = null, params string[] args);
        int GetProcessValue();
        InstallContext InstallContext { get; }
        IWatch Watch { get; }
        string Name { get; }
        CommandStatus CommandStatus { get; }
        RollbackStatus RollbackStatus { get; }
    }

    public interface ICommandPlugin
    {
        string CmdKey { get; }
        string CmdDesc { get; }
        ICommand GetCommand();
    }
}