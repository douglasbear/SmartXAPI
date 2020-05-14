using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Ffw_PriceListDetails")]
    public partial class FfwPriceListDetails
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Key]
        [Column("N_PriceID")]
        public int NPriceId { get; set; }
        [Key]
        [Column("N_PriceDetailsID")]
        public int NPriceDetailsId { get; set; }
        [Column("N_LocFrom")]
        public int? NLocFrom { get; set; }
        [Column("N_LocTo")]
        public int? NLocTo { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Column("N_Price")]
        public int? NPrice { get; set; }
    }
}
