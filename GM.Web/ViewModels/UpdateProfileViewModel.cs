using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;
using GM.Utility.ComponentAttributes;
using GM.ViewModels.Shared;

namespace GM.ViewModels
{
    public class UpdateProfileViewModel : LayoutViewModel
    {
        public bool Success { get; set; }

        [Required]
        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last name")]
        public string LastName { get; set; }

        [Display(Name = "Phone number")]
        //[RegularExpression(@"[0-9\-\(\)\+#\*,;]+", ErrorMessage = "Phone number is invalid.")]
        public string PhoneNumber { get; set; }

        [Required, DataType(DataType.EmailAddress)]
        [Display(Name = "Email address")]
        public string Email { get; set; }

        public string BirthDate { get; set; }

        public HttpPostedFileBase Photo { get; set; }

        public string Address { get; set; }

        public string Sex { get; set; }
        public IEnumerable<SelectListItem> Genders
        {
            get
            {
                yield return new SelectListItem { Value = "Male", Text = "Male" };
                yield return new SelectListItem { Value = "Female", Text = "Female" };
                yield return new SelectListItem { Value = "Other", Text = "Other" };
            }
        }


        [Display(Name = "Birthday")]
        public int? DateOfBirthDay { get; set; }
        public int? DateOfBirthMonth { get; set; }
        public string DateOfBirthMonthText { get; set; }
        public int? DateOfBirthYear { get; set; }

        public IEnumerable<SelectListItem> Days
        {
            get
            {
                yield return new SelectListItem { Text = "01", Value = "1" };
                yield return new SelectListItem { Text = "02", Value = "2" };
                yield return new SelectListItem { Text = "03", Value = "3" };
                yield return new SelectListItem { Text = "04", Value = "4" };
                yield return new SelectListItem { Text = "05", Value = "5" };
                yield return new SelectListItem { Text = "06", Value = "6" };
                yield return new SelectListItem { Text = "07", Value = "7" };
                yield return new SelectListItem { Text = "08", Value = "8" };
                yield return new SelectListItem { Text = "09", Value = "9" };
                yield return new SelectListItem { Text = "10", Value = "10" };
                yield return new SelectListItem { Text = "11", Value = "11" };
                yield return new SelectListItem { Text = "12", Value = "12" };
                yield return new SelectListItem { Text = "13", Value = "13" };
                yield return new SelectListItem { Text = "14", Value = "14" };
                yield return new SelectListItem { Text = "15", Value = "15" };
                yield return new SelectListItem { Text = "16", Value = "16" };
                yield return new SelectListItem { Text = "17", Value = "17" };
                yield return new SelectListItem { Text = "18", Value = "18" };
                yield return new SelectListItem { Text = "19", Value = "19" };
                yield return new SelectListItem { Text = "20", Value = "20" };
                yield return new SelectListItem { Text = "21", Value = "21" };
                yield return new SelectListItem { Text = "22", Value = "22" };
                yield return new SelectListItem { Text = "23", Value = "23" };
                yield return new SelectListItem { Text = "24", Value = "24" };
                yield return new SelectListItem { Text = "25", Value = "25" };
                yield return new SelectListItem { Text = "26", Value = "26" };
                yield return new SelectListItem { Text = "27", Value = "27" };
                yield return new SelectListItem { Text = "28", Value = "28" };
                yield return new SelectListItem { Text = "29", Value = "29" };
                yield return new SelectListItem { Text = "30", Value = "30" };
                yield return new SelectListItem { Text = "31", Value = "31" };
            }
        }

        public IEnumerable<SelectListItem> Months
        {
            get
            {
                yield return new SelectListItem { Value = "1", Text = "Jan" };
                yield return new SelectListItem { Value = "2", Text = "Feb" };
                yield return new SelectListItem { Value = "3", Text = "Mar" };
                yield return new SelectListItem { Value = "4", Text = "Apr" };
                yield return new SelectListItem { Value = "5", Text = "May" };
                yield return new SelectListItem { Value = "6", Text = "June" };
                yield return new SelectListItem { Value = "7", Text = "July" };
                yield return new SelectListItem { Value = "8", Text = "Aug" };
                yield return new SelectListItem { Value = "9", Text = "Sept" };
                yield return new SelectListItem { Value = "10", Text = "Oct" };
                yield return new SelectListItem { Value = "11", Text = "Nov" };
                yield return new SelectListItem { Value = "12", Text = "Dec" };
            }
        }

        //public IEnumerable<SelectListItem> Months
        //{
        //    get
        //    {
        //        yield return new SelectListItem { Text = "01", Value = "1" };
        //        yield return new SelectListItem { Text = "02", Value = "2" };
        //        yield return new SelectListItem { Text = "03", Value = "3" };
        //        yield return new SelectListItem { Text = "04", Value = "4" };
        //        yield return new SelectListItem { Text = "05", Value = "5" };
        //        yield return new SelectListItem { Text = "06", Value = "6" };
        //        yield return new SelectListItem { Text = "07", Value = "7" };
        //        yield return new SelectListItem { Text = "08", Value = "8" };
        //        yield return new SelectListItem { Text = "09", Value = "9" };
        //        yield return new SelectListItem { Text = "10", Value = "10" };
        //        yield return new SelectListItem { Text = "11", Value = "11" };
        //        yield return new SelectListItem { Text = "12", Value = "12" };
        //    }
        //}

        public IEnumerable<SelectListItem> Year
        {
            get
            {
                yield return new SelectListItem { Text = "2005" };
                yield return new SelectListItem { Text = "2004" };
                yield return new SelectListItem { Text = "2003" };
                yield return new SelectListItem { Text = "2002" };
                yield return new SelectListItem { Text = "2001" };
                yield return new SelectListItem { Text = "2000" };
                yield return new SelectListItem { Text = "1999" };
                yield return new SelectListItem { Text = "1998" };
                yield return new SelectListItem { Text = "1997" };
                yield return new SelectListItem { Text = "1996" };
                yield return new SelectListItem { Text = "1995" };
                yield return new SelectListItem { Text = "1994" };
                yield return new SelectListItem { Text = "1993" };
                yield return new SelectListItem { Text = "1992" };
                yield return new SelectListItem { Text = "1991" };
                yield return new SelectListItem { Text = "1990" };
                yield return new SelectListItem { Text = "1989" };
                yield return new SelectListItem { Text = "1988" };
                yield return new SelectListItem { Text = "1987" };
                yield return new SelectListItem { Text = "1986" };
                yield return new SelectListItem { Text = "1985" };
                yield return new SelectListItem { Text = "1984" };
                yield return new SelectListItem { Text = "1983" };
                yield return new SelectListItem { Text = "1982" };
                yield return new SelectListItem { Text = "1981" };
                yield return new SelectListItem { Text = "1980" };
                yield return new SelectListItem { Text = "1979" };
                yield return new SelectListItem { Text = "1978" };
                yield return new SelectListItem { Text = "1977" };
                yield return new SelectListItem { Text = "1976" };
                yield return new SelectListItem { Text = "1975" };
                yield return new SelectListItem { Text = "1974" };
                yield return new SelectListItem { Text = "1973" };
                yield return new SelectListItem { Text = "1972" };
                yield return new SelectListItem { Text = "1971" };
                yield return new SelectListItem { Text = "1970" };
                yield return new SelectListItem { Text = "1969" };
                yield return new SelectListItem { Text = "1968" };
                yield return new SelectListItem { Text = "1967" };
                yield return new SelectListItem { Text = "1966" };
                yield return new SelectListItem { Text = "1965" };
                yield return new SelectListItem { Text = "1964" };
                yield return new SelectListItem { Text = "1963" };
                yield return new SelectListItem { Text = "1962" };
                yield return new SelectListItem { Text = "1961" };
                yield return new SelectListItem { Text = "1960" };
                yield return new SelectListItem { Text = "1959" };
                yield return new SelectListItem { Text = "1958" };
                yield return new SelectListItem { Text = "1957" };
                yield return new SelectListItem { Text = "1956" };
                yield return new SelectListItem { Text = "1955" };
                yield return new SelectListItem { Text = "1954" };
                yield return new SelectListItem { Text = "1953" };
                yield return new SelectListItem { Text = "1952" };
                yield return new SelectListItem { Text = "1951" };
                yield return new SelectListItem { Text = "1950" };
                yield return new SelectListItem { Text = "1949" };
                yield return new SelectListItem { Text = "1948" };
                yield return new SelectListItem { Text = "1947" };
                yield return new SelectListItem { Text = "1946" };
                yield return new SelectListItem { Text = "1945" };
                yield return new SelectListItem { Text = "1944" };
                yield return new SelectListItem { Text = "1943" };
                yield return new SelectListItem { Text = "1942" };
                yield return new SelectListItem { Text = "1941" };
                yield return new SelectListItem { Text = "1940" };
                yield return new SelectListItem { Text = "1939" };
                yield return new SelectListItem { Text = "1938" };
                yield return new SelectListItem { Text = "1937" };
                yield return new SelectListItem { Text = "1936" };
                yield return new SelectListItem { Text = "1935" };
                yield return new SelectListItem { Text = "1934" };
                yield return new SelectListItem { Text = "1933" };
                yield return new SelectListItem { Text = "1932" };
                yield return new SelectListItem { Text = "1931" };
                yield return new SelectListItem { Text = "1930" };
                yield return new SelectListItem { Text = "1929" };
                yield return new SelectListItem { Text = "1928" };
                yield return new SelectListItem { Text = "1927" };
                yield return new SelectListItem { Text = "1926" };
                yield return new SelectListItem { Text = "1925" };
                yield return new SelectListItem { Text = "1924" };
                yield return new SelectListItem { Text = "1923" };
                yield return new SelectListItem { Text = "1922" };
                yield return new SelectListItem { Text = "1921" };
                yield return new SelectListItem { Text = "1920" };
                yield return new SelectListItem { Text = "1919" };
                yield return new SelectListItem { Text = "1918" };
                yield return new SelectListItem { Text = "1917" };
                yield return new SelectListItem { Text = "1916" };
                yield return new SelectListItem { Text = "1915" };
                yield return new SelectListItem { Text = "1914" };
                yield return new SelectListItem { Text = "1913" };
                yield return new SelectListItem { Text = "1912" };
                yield return new SelectListItem { Text = "1911" };
                yield return new SelectListItem { Text = "1910" };
            }
        }


        [DataType(DataType.Password)]
        [Display(Name = "Old password")]
        public string OldPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        [StringLength(int.MaxValue, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        public string ConfirmNewPassword { get; set; }

        [Display(Name = "Send me latest GreenMoney bonus offers and updates via email")]
        public bool SendEmailOffers { get; set; }

        [SendEmailUpdates]
        public bool SendEmailUpdates { get; set; }

        public bool PostToFacebook { get; set; }
        public bool PushNotifications { get; set; }
        
        public int? PhotoId { get; set; }

        public string SupportEmail { get; set; }

    }

    public class UpdateProfileAuspostViewModel : UpdateProfileViewModel
    {

        [Required/*, DataType(DataType.EmailAddress)*/]
        [Display(Name = "Email address")]
        public string CompanyEmail { get; set; }
        public string CompanyEmailDomain { get; set; }

        //[Required]
        public string StreetType { get; set; }
        public IEnumerable<SelectListItem> StreetTypes { get; set; }

        //[Required]
        public string State { get; set; }
        public IEnumerable<SelectListItem> States { get; set; }

        //[Required]
        public string StreetName { get; set; }
        public IEnumerable<SelectListItem> StreetNames { get; set; }
        
        //[Required]
        public string Subrp { get; set; }
        public IEnumerable<SelectListItem> Subrps { get; set; }

        //Company - aus post
        [Display(Name = "Employment type")]
        public string EmploymentType { get; set; }
        public IEnumerable<SelectListItem> EmploymentTypes
        {
            get
            {
                yield return new SelectListItem { Value = "Full time", Text = "Full time" };
                yield return new SelectListItem { Value = "Part time", Text = "Part time" };
                yield return new SelectListItem { Value = "Casual", Text = "Casual" };
                yield return new SelectListItem { Value = "Contractor", Text = "Contractor" };
            }
        }
    }
}