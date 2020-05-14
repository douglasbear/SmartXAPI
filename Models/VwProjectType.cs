using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwProjectType
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_PkeyId")]
        public int NPkeyId { get; set; }
        [Column("X_PkeyCode")]
        [StringLength(5)]
        public string XPkeyCode { get; set; }
        [Column("X_Name")]
        [StringLength(50)]
        public string XName { get; set; }
        [Column("N_ReferId")]
        public int? NReferId { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Required]
        [Column("X_Name_Ar")]
        [StringLength(250)]
        public string XNameAr { get; set; }
    }
}
