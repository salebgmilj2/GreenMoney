using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GM.Models.Public
{
    public class TransactionModel
    {
        public UserModel User { get; set; }

        public int Id { get; set; }
        public DateTime Time { get; set; }
        public string Description { get; set; }
        public long Points { get; set; }
        public int? TransactionTypeID { get; set; }

    }
}
