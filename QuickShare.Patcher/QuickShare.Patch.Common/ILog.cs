using System.Linq;
using System.IO;

namespace QuickShare.Patch.Common
{
    public interface ILog
    {
        void Info(string message);
        void Warn(string message);
        void Error(string message);
    }

    public interface IWatch : ILog
    {
        void SetStep(string stepInfo);
        void SetMaxValue(int maxValue, int value);
        void AddValue(int value);
        void Complete();
        void Faild();
    }
}