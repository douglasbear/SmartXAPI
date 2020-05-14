using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwItemDisplaySalesOrder
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_ItemID")]
        public int NItemId { get; set; }
        [Column("Item Code")]
        [StringLength(50)]
        public string ItemCode { get; set; }
        [Column("Product Code")]
        [StringLength(50)]
        public string ProductCode { get; set; }
        [StringLength(600)]
        public string Description { get; set; }
        [Column("Part No")]
        [StringLength(250)]
        public string PartNo { get; set; }
        [Column("B_InActive")]
        public bool? BInActive { get; set; }
        [Column("N_LocationID")]
        public int NLocationId { get; set; }
        [StringLength(100)]
        public string Category { get; set; }
        [Column("Product Class")]
        [StringLength(25)]
        public string ProductClass { get; set; }
        [Column("Item Class")]
        [StringLength(25)]
        public string ItemClass { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_WarehouseID")]
        public int? NWarehouseId { get; set; }
        public int? Expr1 { get; set; }
        [Column("N_SalesOrderId")]
        public int NSalesOrderId { get; set; }
        [Column("N_SalesOrderDetailsID")]
        public int NSalesOrderDetailsId { get; set; }
    }
}
