using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvPendingDeliveryNotesRpt
    {
        [Column("N_SalesOrderId")]
        public int NSalesOrderId { get; set; }
        [Column("N_FnYearId")]
        public int? NFnYearId { get; set; }
        [Column("N_CustomerId")]
        public int? NCustomerId { get; set; }
        [Column("X_OrderNo")]
        [StringLength(50)]
        public string XOrderNo { get; set; }
        [Column("D_OrderDate", TypeName = "smalldatetime")]
        public DateTime? DOrderDate { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Column("SOQty")]
        public double? Soqty { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("N_SOQty")]
        public double? NSoqty { get; set; }
        [Column("N_ItemUnitID")]
        public int? NItemUnitId { get; set; }
        [Column("N_deliveredQty")]
        public double? NDeliveredQty { get; set; }
        [Column("N_ClassID")]
        public int? NClassId { get; set; }
        [Column("X_CustomerCode")]
        [StringLength(50)]
        public string XCustomerCode { get; set; }
        [Column("X_CustomerName")]
        [StringLength(100)]
        public string XCustomerName { get; set; }
        [Required]
        [Column("N_Status")]
        [StringLength(7)]
        public string NStatus { get; set; }
        [Column("RemaingQTY")]
        public double? RemaingQty { get; set; }
        [Column("PQty")]
        public double? Pqty { get; set; }
        [Column("N_SalesOrderDetailsID")]
        public int NSalesOrderDetailsId { get; set; }
        [Column("B_IsSaveDraft")]
        public bool? BIsSaveDraft { get; set; }
        [Column("N_Sprice", TypeName = "money")]
        public decimal? NSprice { get; set; }
    }
}
