using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GM.Models
{
    public class Utils
    {
        public static void CopyProperties(object copyFrom, object copyTo)
        {
            foreach (var p in copyFrom.GetType().GetProperties())
            {
                var x = copyTo.GetType().GetProperty(p.Name);
                if (x != null && p.GetValue(copyFrom, null) != null)
                {
                    //if (x.GetType() == p.GetValue(copyFrom, null).GetType())
                    //{
                    x.SetValue(copyTo, p.GetValue(copyFrom, null), null);
                    //}
                }
            }
        }
    }
}
