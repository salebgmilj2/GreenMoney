using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GM.Models.Shared
{
    public class ResourceEntityModel
    {
        public virtual int Id { get; set; }

        public virtual PageEntityModel Page { get; set; }

        public virtual ResourceEntityType ResourceType { get; set; }

        public virtual string Value { get; set; }

    }

    public class ResourceEntityType
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
    }
}
