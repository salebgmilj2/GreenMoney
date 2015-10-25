using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GM.Models.Shared
{
    public class SectionEntityModel
    {
        public virtual int Id { get; set; }

        public virtual string Label { get; set; }

        public virtual string URLValue { get; set; }

        public virtual bool Hidden { get; set; }

        public virtual int OrderId { get; set; }

    }
}
