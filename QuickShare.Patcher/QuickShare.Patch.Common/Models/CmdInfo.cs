using System;
using System.Collections.Generic;
using System.Linq;

namespace QuickShare.Patch.Common.Models
{
    [Serializable]
    public class CmdInfo
    {
        public int Index { get; set; }
        public string Name { get; set; }
        public string Target { get; set; }

        public string Args
        {
            get
            {
                if (ArgList != null && ArgList.Any())
                {
                    return string.Join(" ", ArgList.Select(x => "\"" + x + "\"").ToArray());
                }

                return null;
            }
        }

        public string[] ArgList { get; set; }
        public string Display
        {
            get
            {
                if (ArgList != null && ArgList.Any())
                {
                    if (ArgList.Length == 1)
                    {
                        return Target + " " + ArgList.FirstOrDefault();
                    }
                    else
                    {
                        return Target + " " + ArgList.FirstOrDefault() + " ...";
                    }
                }
                return Target;
            }
        }
    }
}