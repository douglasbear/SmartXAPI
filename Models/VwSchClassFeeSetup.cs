using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwSchClassFeeSetup
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("X_ClassType")]
        [StringLength(50)]
        public string XClassType { get; set; }
        [Column("N_ClassID")]
        public int NClassId { get; set; }
        [Column("N_FeeTypeID")]
        public int NFeeTypeId { get; set; }
        [Required]
        [Column("X_FeeType")]
        [StringLength(50)]
        public string XFeeType { get; set; }
        [Column("N_FeeCategoryID")]
        public int? NFeeCategoryId { get; set; }
        [Column("X_FeeCategory")]
        [StringLength(50)]
        public string XFeeCategory { get; set; }
    }
}
