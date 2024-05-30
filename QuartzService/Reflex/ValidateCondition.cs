using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuartzService.Reflex
{
    public static class ValidateCondition
    {
        public static bool IsNumric(string value)
        {
            var sss = int.TryParse(value, out var num2);
            return int.TryParse(value, out var num);
        }
    }
}
