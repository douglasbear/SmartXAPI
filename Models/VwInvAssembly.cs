using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvAssembly
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_AssemblyID")]
        public int NAssemblyId { get; set; }
        [Column("X_Action")]
        [StringLength(20)]
        public string XAction { get; set; }
        [Column("X_ReferenceNo")]
        [StringLength(20)]
        public string XReferenceNo { get; set; }
        [Column("D_Date")]
        [StringLength(8000)]
        public string DDate { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Column("N_Qty")]
        public double? NQty { get; set; }
        [Column("N_Cost", TypeName = "money")]
        public decimal? NCost { get; set; }
        [Column("N_Rate", TypeName = "money")]
        public decimal? NRate { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("X_ItemCode")]
        [StringLength(100)]
        public string XItemCode { get; set; }
        [Column("X_ItemName")]
        [StringLength(600)]
        public string XItemName { get; set; }
        [Column("N_CategoryID")]
        public int? NCategoryId { get; set; }
        [Column("X_Category")]
        [StringLength(100)]
        public string XCategory { get; set; }
        [Column("N_OtherCharges", TypeName = "money")]
        public decimal? NOtherCharges { get; set; }
        [Column("X_LocationName")]
        public string XLocationName { get; set; }
        [Column("N_LocationID")]
        public int? NLocationId { get; set; }
        [Column("N_LaborCost", TypeName = "money")]
        public decimal? NLaborCost { get; set; }
        [Column("N_Machinery_Cost", TypeName = "money")]
        public decimal? NMachineryCost { get; set; }
        [Column("X_ItemUnit")]
        [StringLength(500)]
        public string XItemUnit { get; set; }
        [Column("B_IsProcess")]
        public bool BIsProcess { get; set; }
        [Required]
        [StringLength(5)]
        public string Status { get; set; }
        [Required]
        [Column("X_PRSNo")]
        [StringLength(50)]
        public string XPrsno { get; set; }
        [Column("N_ReqID")]
        public int? NReqId { get; set; }
        public bool? Expr1 { get; set; }
        [Column("X_Description")]
        [StringLength(500)]
        public string XDescription { get; set; }
        [Column("N_SalesOrderId")]
        public int? NSalesOrderId { get; set; }
        [Required]
        [Column("X_OrderNo")]
        [StringLength(50)]
        public string XOrderNo { get; set; }
        [Required]
        [Column("X_CustomerName")]
        [StringLength(100)]
        public string XCustomerName { get; set; }
        [Column("N_CustomerID")]
        public int? NCustomerId { get; set; }
        [Column("D_ReleaseDate", TypeName = "datetime")]
        public DateTime? DReleaseDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? FilDate { get; set; }
    }
}
