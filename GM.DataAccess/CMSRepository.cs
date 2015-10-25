using GM.DataAccess.Entities;
using GM.DataAccess.Interfaces;
using GM.Models;
using GM.Models.Shared;
using System.Collections.Generic;
using System.Linq;

namespace GM.DataAccess
{
    public class CMSRepository : ICMSRepository
    {
        public SectionModel GetSection(int id)
        {
            using (var context = new greenMoneyEntities())
            {

                Pages page = context.Pages.SingleOrDefault(w => w.Id == id);

                SectionModel model = new SectionModel();

                SectionEntityModel sectionModel = new SectionEntityModel();

                Sections section = context.Sections.SingleOrDefault(x => x.Id == page.Sections.Id);

                Utils.CopyProperties(section, sectionModel);
                model.Section = sectionModel;

                List<Pages> pages = context.Pages
                                            .Where(x => x.Sections.Id == section.Id)
                                            .OrderBy(o => o.OrderId)
                                            .ToList();

                List<PageModel> pagesList = new List<PageModel>();
                foreach (var item in pages)
                {
                    PageModel pmodel = new PageModel();
                    Utils.CopyProperties(item, pmodel);

                    pagesList.Add(pmodel);
                }

                model.Pages = pagesList;

                return model;
            }
        }

        public PageModel GetPage(string id, string inst)
        {
            PageModel model = new PageModel();
            using (var context = new greenMoneyEntities())
            {
                var candidatePages = context.Pages.Where(p => p.URLValue == id).ToList();

                Pages page;
                if (!string.IsNullOrWhiteSpace(inst))
                {
                    page = candidatePages.FirstOrDefault(
                        p => p.Sections.Instance_Id != null && p.Sections.Instance_Id.ToString() == inst);
                }
                else
                {
                    page = context.Pages.FirstOrDefault(p => p.URLValue == id && p.Sections.Instance_Id == null);
                }


                if (page == null)
                {
                    return null;
                }

                PageEntityModel pmodel = new PageEntityModel();

                Utils.CopyProperties(page, pmodel);
                var pageContentBodyHtml = page.Resources.FirstOrDefault(p => p.ResourceTypes.Name == "BodyContent");

                if (pageContentBodyHtml != null)
                {
                    pmodel.BodyHtml = pageContentBodyHtml.Value;
                }

                model.Page = pmodel;

                List<PageEntityModel> subPages = new List<PageEntityModel>();

                if (page.Sections != null && page.Sections.Pages.Any())
                {
                    foreach (var item in page.Sections.Pages.Where(p => !p.Hidden && p.Resources.Count>0).OrderBy(p=>p.OrderId))
                    {
                        PageEntityModel subPage = new PageEntityModel();
                        subPage.Label = item.Label;
                        subPage.URLValue = item.URLValue;

                        subPages.Add(subPage);
                    }
                }
                model.SubPages = subPages;


                List<Resources> resources = context.Resources
                                            .Where(x => x.Pages.Id == page.Id)
                                            .ToList();

                List<ResourceEntityModel> list = new List<ResourceEntityModel>();
                foreach (var item in resources)
                {
                    ResourceEntityModel rem = new ResourceEntityModel();
                    Utils.CopyProperties(item, rem);

                    list.Add(rem);
                }

                model.Resources = list;

                return model;
            }
        }

        public string GetSectionPageUrl(string id)
        {
            using (var context = new greenMoneyEntities())
            {
                Sections section = context.Sections.FirstOrDefault(s => s.URLValue.ToLower() == id.ToLower());
                IList<Pages> pages = context.Pages
                    .Where(x => x.Sections.Id == section.Id && x.Resources.Count >0 && !x.Hidden)
                    .OrderBy(o => o.OrderId).ToList();

                var landingSectionPage = pages.FirstOrDefault();
                if (landingSectionPage != null)
                {
                    return landingSectionPage.URLValue.ToLower();
                }

                return null;
            }
        }
    }
}
