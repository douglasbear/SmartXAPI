using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("inv_CustomerDiscountMaster")]
    public partial class InvCustomerDiscountMaster
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Key]
        [Column("N_CDMID")]
        public int NCdmid { get; set; }
        [Required]
        [Column("X_CDMCode")]
        [StringLength(100)]
        public string XCdmcode { get; set; }
        [Column("D_DateFrom", TypeName = "smalldatetime")]
        public DateTime? DDateFrom { get; set; }
        [Column("D_DateTo", TypeName = "smalldatetime")]
        public DateTime? DDateTo { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
    }
}
