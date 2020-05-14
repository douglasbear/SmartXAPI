using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvLocationDisp
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_LocationID")]
        public int NLocationId { get; set; }
        [Column("X_LocationName")]
        public string XLocationName { get; set; }
    }
}
