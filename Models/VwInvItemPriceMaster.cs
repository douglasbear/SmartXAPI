using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvItemPriceMaster
    {
        [Column("N_ItemID")]
        public int NItemId { get; set; }
        [Column("N_PriceID")]
        public int NPriceId { get; set; }
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_PriceVal", TypeName = "money")]
        public decimal? NPriceVal { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("Selling_Price")]
        [StringLength(10)]
        public string SellingPrice { get; set; }
    }
}
