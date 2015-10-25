using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Text.RegularExpressions;

namespace GM.Utility.ComponentAttributes
{
    public class ValidateEmailAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var aupostEmailDomain = ConfigurationManager.AppSettings["AuspostEmailDomain"] == null
                 ? "auspost.com.au"
                 : ConfigurationManager.AppSettings["AuspostEmailDomain"];

 
            Regex re = new Regex(@"\w.@" + aupostEmailDomain);

            return value != null && re.IsMatch(value.ToString());
        }
    }
}