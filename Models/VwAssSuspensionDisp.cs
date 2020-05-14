using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwAssSuspensionDisp
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [StringLength(50)]
        public string Code { get; set; }
        [Column("N_SuspendID")]
        public int NSuspendId { get; set; }
        [Column("D_FromDate")]
        [StringLength(8000)]
        public string DFromDate { get; set; }
        [Column("D_ToDate")]
        [StringLength(8000)]
        public string DToDate { get; set; }
        [Column("N_ItemID")]
        public int NItemId { get; set; }
        [Column("Item Code")]
        [StringLength(50)]
        public string ItemCode { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("X_ItemName")]
        [StringLength(100)]
        public string XItemName { get; set; }
    }
}
