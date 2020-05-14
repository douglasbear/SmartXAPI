using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwDispatch
    {
        [Column("B_Dispatched")]
        public bool? BDispatched { get; set; }
        [Column("N_SalesDetailsID")]
        public int NSalesDetailsId { get; set; }
    }
}
