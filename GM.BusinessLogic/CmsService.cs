using GM.BusinessLogic.Interfaces;
using GM.DataAccess;
using GM.Models.Shared;

namespace GM.BusinessLogic
{
    public class CmsService : ICmsService
    {
        public SectionModel GetSection(int id)
        {
            return new CMSRepository().GetSection(id);
        }

        public PageModel GetPage(string id, string inst)
        {
            return new CMSRepository().GetPage(id, inst);
        }

        public string GetSectionPageUrl(string id)
        {
            return new CMSRepository().GetSectionPageUrl(id);
        }
    }
}
