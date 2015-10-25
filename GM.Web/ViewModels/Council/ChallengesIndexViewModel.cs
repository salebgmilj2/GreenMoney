using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GM.Models.Public;
using GM.ViewModels.Shared;
using GM.ViewModels.Council;

namespace GM.ViewModels.Council
{
    public class ChallengesIndexViewModel : LayoutViewModel
    {
        public bool IsInitialRegistrationStep { get; set; }

        public ChallengesListModel Challenges { get; set; }
        public List<UploadModel> Uploads { get; set; }

        public SelectList OrderBy { get; set; }
        public int SelectedOrderBy { get; set; }

        public SelectList Status { get; set; }
        public int? SelectedStatus { get; set; }

        public SelectList ChallengeCategory { get; set; }
        public int? SelectedChallengeCategory { get; set; }

        public string SearchString { get; set; }
    }
}