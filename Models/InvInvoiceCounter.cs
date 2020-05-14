using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_InvoiceCounter")]
    public partial class InvInvoiceCounter
    {
        [Key]
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Key]
        [Column("N_FormID")]
        public int NFormId { get; set; }
        [Column("X_Prefix")]
        [StringLength(100)]
        public string XPrefix { get; set; }
        [Column("N_StartNo")]
        public int? NStartNo { get; set; }
        [Column("N_LastUsedNo")]
        public int? NLastUsedNo { get; set; }
        [Column("B_AutoInvoiceEnabled")]
        public bool? BAutoInvoiceEnabled { get; set; }
        [Column("N_MenuID")]
        public int? NMenuId { get; set; }
        [Key]
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("B_Yearwise")]
        public bool? BYearwise { get; set; }
        [Key]
        [Column("N_BranchID")]
        public int NBranchId { get; set; }
        [Column("N_MinimumLen")]
        public int? NMinimumLen { get; set; }
        [Column("B_Suffix")]
        public bool? BSuffix { get; set; }
        [Column("X_Suffix")]
        [StringLength(100)]
        public string XSuffix { get; set; }
        [Column("B_ResetYearly")]
        public bool? BResetYearly { get; set; }
    }
}
