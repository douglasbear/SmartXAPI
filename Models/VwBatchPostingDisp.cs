using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwBatchPostingDisp
    {
        [Required]
        [Column("X_ID")]
        [StringLength(20)]
        public string XId { get; set; }
        [Column("X_Description")]
        [StringLength(8000)]
        public string XDescription { get; set; }
        [Column("X_Description_Ar")]
        [StringLength(50)]
        public string XDescriptionAr { get; set; }
        [Column("X_TransType")]
        [StringLength(50)]
        public string XTransType { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("B_IsAccPosted")]
        public bool? BIsAccPosted { get; set; }
    }
}
