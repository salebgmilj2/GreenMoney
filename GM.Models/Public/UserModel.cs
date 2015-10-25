using System;

namespace GM.Models.Public
{
    public class UserModel
    {

        public Guid Id { get; set; }

        public bool IsFBAccount;
        public long FBUserId;

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Sex { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string PhoneNumber { get; set; }

        public int AddressId { get; set; }


        public string Email { get; set; }
        public string Postcode { get; set; }

        /// <summary>
        /// Whether or not user wants to receive promotional emails.
        /// </summary>
        public virtual bool SendEmailOffers { get; set; }
        public virtual bool SendEmailUpdates { get; set; }


        // New fields
        public int Instance_Id { get; set; }
        public int? PhotoID { get; set; }
        
        /// <summary>
        /// This field is used just in the mobile app but has to be editable on the web too
        /// </summary>
        public bool PushNotifications { get; set; }

        /// <summary>
        /// Auto post activities on facebook if selected
        /// </summary>
        public bool PostToFacebook { get; set; }

        public bool IsAdditionalAccountHolder { get; set; }


        //Supplier specific fields
        public string BusinessName { get; set; }
        public string BusinessNumberABN { get; set; }
        public string BusinessType { get; set; }

        public string BussinesWebSite { get; set; }
        public string BussinesEmail { get; set; }

        public string BussinesPhone { get; set; }
        public string BussinesPhoneArea { get; set; }
        public string BussinesPhoneMobile { get; set; }

        public string BussinesLocation { get; set; }

        public bool EmailBussinesOnVoucherRedeem { get; set; }

        public AddressModel AddressModel { get; set; }


        //Private company - auspost
        public string EmploymentType { get; set; }

    }
}
