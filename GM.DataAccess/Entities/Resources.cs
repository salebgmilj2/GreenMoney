//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GM.DataAccess.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class Resources
    {
        public int Id { get; set; }
        public string Value { get; set; }
        public Nullable<int> Page_Id { get; set; }
        public int ResourceType_Id { get; set; }
    
        public virtual Pages Pages { get; set; }
        public virtual ResourceTypes ResourceTypes { get; set; }
    }
}