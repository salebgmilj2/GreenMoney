using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GM.Models.Public
{
    public class UpdateProfileModel
    {

        public bool Success { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public DateTime BirthDate { get; set; }

        public string Sex { get; set; }

        public bool SendEmailUpdates { get; set; }
        public bool PostToFacebook { get; set; }
        public bool PushNotifications { get; set; }


        public string OldPassword { get; set; }

        public string NewPassword { get; set; }

        public string ConfirmNewPassword { get; set; }


        //Auspost
        public int? AddressId { get; set; }
        public string EmploymentType { get; set; }
    }
}