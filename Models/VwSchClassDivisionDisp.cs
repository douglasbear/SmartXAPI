using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwSchClassDivisionDisp
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        public int Code { get; set; }
        [Column("Class Division")]
        [StringLength(50)]
        public string ClassDivision { get; set; }
    }
}
