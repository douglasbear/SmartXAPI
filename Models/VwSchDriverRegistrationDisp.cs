using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwSchDriverRegistrationDisp
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("Driver ID")]
        [StringLength(30)]
        public string DriverId { get; set; }
        [Column("Driver Name")]
        [StringLength(50)]
        public string DriverName { get; set; }
    }
}
