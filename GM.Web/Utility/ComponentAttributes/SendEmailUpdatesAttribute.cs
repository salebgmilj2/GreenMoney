using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Web;

namespace GM.Utility.ComponentAttributes
{
    public class SendEmailUpdatesAttribute : DisplayNameAttribute
    {
        public override string DisplayName
        {
            get
            {
                return "Send " + ConfigurationManager.AppSettings["Application.CurrentCouncil"].ToString() + " Council updates and newsletters";
            }
        }

        public SendEmailUpdatesAttribute() : base() { }

    }
}