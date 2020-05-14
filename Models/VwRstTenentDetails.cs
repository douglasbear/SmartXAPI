using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwRstTenentDetails
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_TenentID")]
        public int NTenentId { get; set; }
        [Column("N_TenentDetailID")]
        public int NTenentDetailId { get; set; }
        [Column("X_ItemName")]
        [StringLength(800)]
        public string XItemName { get; set; }
        [Column("X_TenentName")]
        [StringLength(400)]
        public string XTenentName { get; set; }
        [Column("X_PhoneNo")]
        [StringLength(50)]
        public string XPhoneNo { get; set; }
        [Column("N_ItemID")]
        [StringLength(50)]
        public string NItemId { get; set; }
        [Column("X_ExtensionCode")]
        [StringLength(50)]
        public string XExtensionCode { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal? NAmount { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("X_TenentCode")]
        [StringLength(50)]
        public string XTenentCode { get; set; }
        [Column("X_RoomNo")]
        [StringLength(50)]
        public string XRoomNo { get; set; }
        [Column("X_ItemCode")]
        [StringLength(100)]
        public string XItemCode { get; set; }
        [Column("N_Rate", TypeName = "money")]
        public decimal? NRate { get; set; }
        [Column("N_ItemCost", TypeName = "money")]
        public decimal? NItemCost { get; set; }
        [Column("N_PurchaseCost", TypeName = "money")]
        public decimal? NPurchaseCost { get; set; }
    }
}
