using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GM.Models.Public
{
    public class InstanceModel
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public int order { get; set; }

        //pipe delimited list of postcodes valid for this instance
        public string postcodes { get; set; }


        // New fields
        public int Id { get; set; }
        public string Photo { get; set; }

    }
}
