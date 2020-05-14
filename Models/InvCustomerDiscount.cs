using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("inv_CustomerDiscount")]
    public partial class InvCustomerDiscount
    {
        [Key]
        [Column("N_CustDiscountId")]
        public int NCustDiscountId { get; set; }
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_CustomerID")]
        public int NCustomerId { get; set; }
        [Column("N_ProductID")]
        public int NProductId { get; set; }
        [Column("N_ItemPrice", TypeName = "money")]
        public decimal? NItemPrice { get; set; }
        [Column("N_DiscPerc")]
        public int? NDiscPerc { get; set; }
        [Column("N_Status")]
        public int? NStatus { get; set; }
        [Column("N_CDMID")]
        public int? NCdmid { get; set; }
        [Column("N_ItemUnitID")]
        public int? NItemUnitId { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
    }
}
