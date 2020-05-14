using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwFfwPriceListDetails
    {
        [Column("X_PriceCode")]
        [StringLength(100)]
        public string XPriceCode { get; set; }
        [Column("N_PartyID")]
        public int? NPartyId { get; set; }
        [Column("D_DateFrom", TypeName = "datetime")]
        public DateTime? DDateFrom { get; set; }
        [Column("D_DateTo", TypeName = "datetime")]
        public DateTime? DDateTo { get; set; }
        [Column("N_LocTo")]
        public int? NLocTo { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Column("N_Price")]
        public int? NPrice { get; set; }
        [Column("N_PriceID")]
        public int NPriceId { get; set; }
        [Column("N_LocFrom")]
        public int? NLocFrom { get; set; }
        [Column("X_ItemName")]
        [StringLength(600)]
        public string XItemName { get; set; }
        [Column("N_TypeID")]
        public int? NTypeId { get; set; }
        [Column("FromID")]
        public int FromId { get; set; }
        [Column("ToID")]
        public int ToId { get; set; }
        [StringLength(100)]
        public string LocationFrom { get; set; }
        [StringLength(100)]
        public string LocationTo { get; set; }
        [Column("X_VendorName")]
        [StringLength(100)]
        public string XVendorName { get; set; }
        [Column("X_CustomerName")]
        [StringLength(100)]
        public string XCustomerName { get; set; }
    }
}
