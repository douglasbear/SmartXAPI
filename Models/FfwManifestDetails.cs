﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Ffw_ManifestDetails")]
    public partial class FfwManifestDetails
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_ManifestID")]
        public int? NManifestId { get; set; }
        [Key]
        [Column("N_ManifestDetailID")]
        public int NManifestDetailId { get; set; }
        [Column("X_OrderNo")]
        [StringLength(100)]
        public string XOrderNo { get; set; }
        [Column("N_CustomerID")]
        public int? NCustomerId { get; set; }
        [Column("N_ShipperID")]
        public int? NShipperId { get; set; }
        [Column("N_ConsigneeID")]
        public int? NConsigneeId { get; set; }
        [Column("N_Qty")]
        public int? NQty { get; set; }
        [Column("N_Weight")]
        public double? NWeight { get; set; }
        [Column("N_Price", TypeName = "money")]
        public decimal? NPrice { get; set; }
        [Column("B_IsSelect")]
        public bool? BIsSelect { get; set; }
        [Column("N_OrderID")]
        public int? NOrderId { get; set; }
    }
}
