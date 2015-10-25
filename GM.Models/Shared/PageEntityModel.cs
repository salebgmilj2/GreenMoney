using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GM.Models.Shared
{
    public class PageEntityModel
    {
        public virtual int Id { get; set; }

        public virtual SectionModel Section { get; set; }

        public virtual string Label { get; set; }

        public virtual string URLValue { get; set; }

        public virtual bool Hidden { get; set; }

        public virtual int OrderId { get; set; }

        public string BodyHtml { get; set; }
    }

}
