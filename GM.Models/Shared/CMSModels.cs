using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GM.Models.Shared
{
	public class SectionsModel
	{
		public IList<SectionModel> Sections { get; set; }
	}

	public class SectionModel
	{
		public SectionEntityModel Section { get; set; }
		public IList<PageModel> Pages { get; set; }
	}

	public class PageModel 
	{
		public PageEntityModel Page { get; set; }
        public List<PageEntityModel> SubPages { get; set; }

        public IList<ResourceEntityModel> Resources { get; set; }
		public IList<SelectListItem> SectionsSelections { get; set; }
	}

	public class ResourceModel
	{
		public ResourceEntityModel Resource { get; set; }
		public IList<SelectListItem> ResourceTypesSelections { get; set; }
	}

    public class SelectListItem
    {

        public bool Selected { get; set; }
        public string Text { get; set; }
        public string Value { get; set; }
    }

 
}