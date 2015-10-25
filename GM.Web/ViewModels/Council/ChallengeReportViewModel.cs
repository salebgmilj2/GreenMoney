using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GM.Models.Public;
using GM.ViewModels.Shared;

namespace GM.ViewModels.Council
{
    public class ChallengeReportViewModel : LayoutViewModel
    {
        public bool IsInitialRegistrationStep { get; set; }
        public ChallengeModel Challenge  { get; set; }

        public UserModel User { get; set; }
        public int ChallengeId { get; set; }
        public Guid UserId { get; set; }

        public DateTime LastActivityDate { get; set; }

       // public List <UserChallengeModel> UsersList { get; set; }
        public UserChallengeListModel UsersList { get; set; }

        public ChallengesListModel ChallengesList;
       // public SelectList Challenges { get; set; }
        //public int SelectedChallenge { get; set; }

        public SelectList OrderBy { get; set; }
        public int SelectedOrderBy { get; set; }

        public SelectList ChallengeStatus { get; set; }
        public int SelectedChallengeStatus { get; set; }

        public SelectList Suburbs { get; set; }
        public string SelectedSuburb { get; set; }

        public string SearchString { get; set; }

        public SelectList DownloadPdf { get; set; }
        public int SelectedDownloadPdf { get; set; }

        public SelectList DownloadCsv { get; set; }
        public int SelectedDownloadCsv { get; set; }

        public string DateRange { get; set; }

        //public int Page { get; set; }

        //public int NumPages { get; set; }

        //public int NumChallenges { get; set; }
    }
}