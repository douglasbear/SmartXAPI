using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvPrsnoDeliverySearchNested
    {
        [Required]
        [StringLength(50)]
        public string FileNo { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_PRSID")]
        public int NPrsid { get; set; }
        [Column("PRS No")]
        [StringLength(50)]
        public string PrsNo { get; set; }
        [Column("D_PRSDate", TypeName = "datetime")]
        public DateTime? DPrsdate { get; set; }
        [Column("N_userID")]
        public int? NUserId { get; set; }
        [Column("N_LocationID")]
        public int? NLocationId { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_DepartmentID")]
        public int? NDepartmentId { get; set; }
        [Column("N_TotalPRSAmt", TypeName = "money")]
        public decimal? NTotalPrsamt { get; set; }
        [Column("X_DepartmentCode")]
        [StringLength(50)]
        public string XDepartmentCode { get; set; }
        [Column("N_CostCentreID")]
        public int? NCostCentreId { get; set; }
        [Column("X_Department")]
        public string XDepartment { get; set; }
        [Column("PRS Date")]
        [StringLength(8000)]
        public string PrsDate { get; set; }
        [Column("B_YearEndProcess")]
        public bool? BYearEndProcess { get; set; }
        [Column("N_Processed")]
        public int? NProcessed { get; set; }
        [Column("N_Type")]
        public int? NType { get; set; }
        [Column("X_requestType")]
        [StringLength(50)]
        public string XRequestType { get; set; }
        public string Location { get; set; }
        [Column("N_TransTypeID")]
        public int? NTransTypeId { get; set; }
        [Required]
        [Column("X_OrderNo")]
        [StringLength(50)]
        public string XOrderNo { get; set; }
        [Required]
        [Column("X_CustomerName")]
        [StringLength(100)]
        public string XCustomerName { get; set; }
        [StringLength(50)]
        public string Expr1 { get; set; }
        [Column("N_CustomerID")]
        public int NCustomerId { get; set; }
        [Column("X_Status")]
        [StringLength(20)]
        public string XStatus { get; set; }
        [Column("N_BalanceQty")]
        public double? NBalanceQty { get; set; }
        [Column("X_PurchaseOrderNo")]
        [StringLength(50)]
        public string XPurchaseOrderNo { get; set; }
    }
}
