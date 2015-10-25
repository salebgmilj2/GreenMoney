using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GM.Models.Public
{
    public class AddressModel
    {
        public virtual int Id { get; set; }

        /// <summary>
        /// Unit number. May just be any old random string, eg. 801E.
        /// </summary>
        public virtual string UnitNumber { get; set; }

        /// <summary>
        /// Street number. May contain a hyphen to indicate a range, eg. 9-11 for Unit 7 9-11 Carmen St Bankstown. Could
        /// also be just any old random string, eg. 6A
        /// </summary>
        public virtual string StreetNumber { get; set; }

        /// <summary>
        /// Street name, eg. Carmen for Unit 7 9-11 Carmen St Bankstown.
        /// </summary>
        public virtual string StreetName { get; set; }

        /// <summary>
        /// Street type, eg. St for Unit 7 9-11 Carmen St Bankstown.
        /// </summary>
        public virtual string StreetType { get; set; }

        /// <summary>
        /// Suburb, eg. Bankstown.
        /// </summary>
        public virtual string Suburb { get; set; }

        /// <summary>
        /// State, eg. NSW.
        /// </summary>
        public virtual string State { get; set; }

        /// <summary>
        /// Postcode, eg. 2300.
        /// </summary>
        public virtual string Postcode { get; set; }


        //New field
        public int Instance_Id { get; set; }

    }
}
