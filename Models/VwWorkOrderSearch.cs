using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwWorkOrderSearch
    {
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearId")]
        public int? NFnYearId { get; set; }
        [Column("N_WorkOrderId")]
        public int NWorkOrderId { get; set; }
        [Column("X_WorkOrderNo")]
        [StringLength(50)]
        public string XWorkOrderNo { get; set; }
        [Column("N_ProjectId")]
        public int? NProjectId { get; set; }
        [Column("N_VendorId")]
        public int? NVendorId { get; set; }
        [Column("N_BillAmt")]
        [StringLength(30)]
        public string NBillAmt { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("D_OrderDate")]
        [StringLength(30)]
        public string DOrderDate { get; set; }
        [Column("x_Notes")]
        [StringLength(1000)]
        public string XNotes { get; set; }
        [Column("X_VendorCode")]
        [StringLength(50)]
        public string XVendorCode { get; set; }
        [Column("X_VendorName")]
        [StringLength(100)]
        public string XVendorName { get; set; }
        [Column("X_ProjectName")]
        [StringLength(50)]
        public string XProjectName { get; set; }
    }
}
