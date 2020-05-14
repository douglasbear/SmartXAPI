﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvSalesOrderDeliveryStatusRpt
    {
        [Column("N_SalesOrderId")]
        public int NSalesOrderId { get; set; }
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
        [Required]
        [StringLength(50)]
        public string SalesInvNo { get; set; }
        [Required]
        [Column("N_DelNo")]
        [StringLength(50)]
        public string NDelNo { get; set; }
        [Column("D_DeliveryDate", TypeName = "smalldatetime")]
        public DateTime? DDeliveryDate { get; set; }
        [Column("N_FnYearId")]
        public int? NFnYearId { get; set; }
        [Column("D_SalesDate", TypeName = "smalldatetime")]
        public DateTime? DSalesDate { get; set; }
        [Column("X_Status")]
        [StringLength(20)]
        public string XStatus { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        public double? Expr1 { get; set; }
        [Column("D_ReturnDate", TypeName = "datetime")]
        public DateTime? DReturnDate { get; set; }
    }
}
