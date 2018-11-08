using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace QuickShare.Patch.Common.Util
{
    public static class Helper
    {
        public static void OpenFolder(string path)
        {
            Process.Start(path);
        }

        public static void OpenFile(string path, string target = "notepad")
        {
            if (!string.IsNullOrEmpty(target))
                Process.Start(target, path);
            else
                Process.Start(path);
        }
        public static DialogResult AbortRetryIgnore(IWin32Window owner, string message)
        {
            if (owner == null)
            {
                return MessageBox.Show(message, "提示", MessageBoxButtons.AbortRetryIgnore);
            }
            else
            {
                return MessageBox.Show(owner, message, "提示", MessageBoxButtons.AbortRetryIgnore);
            }
        }
        public static T AbortRetryIgnore<T>(IWin32Window owner, string message, Func<bool> func, Func<T> getResult, Func<T> ignore, Action abort)
        {
            bool retry = false;
            do
            {
                var flag = func();
                if (flag)
                {
                    return getResult();
                }
                else
                {
                    var da = Helper.AbortRetryIgnore(owner, message);
                    if (da == DialogResult.Ignore)
                    {
                        return ignore();
                    }
                    if (da == DialogResult.Retry)
                    {
                        retry = true;
                    }
                    else
                    {
                        abort();
                    }
                }

            } while (retry);

            return default(T);
        }
    }
}