using System;
using System.Diagnostics;
using System.Security.Principal;
using System.Windows.Forms;

namespace QuickShare.Installer
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (IsAdministrator())
            {//如果具有管理员权限，直接运行程序
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new FrmMain());
            }
            else
            {//如果不具有管理员权限，则以管理员身份执行可执行程序
                var startInfo = new ProcessStartInfo
                {
                    FileName = Application.ExecutablePath,//进程目标文件
                    Verb = "runas"//管理员权限
                };
                Process.Start(startInfo);
            }
        }

        private static bool IsAdministrator()
        {
            var identity = WindowsIdentity.GetCurrent();
            if (identity != null)
            {
                var principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            return false;
        }
    }
}
