using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvItemPriceListDisp
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("X_ItemCode")]
        [StringLength(50)]
        public string XItemCode { get; set; }
        [Column("X_ItemName")]
        [StringLength(600)]
        public string XItemName { get; set; }
        [Column("N_ItemID")]
        public int NItemId { get; set; }
        [Column("N_ID")]
        public int? NId { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime? DEntrydate { get; set; }
        public int? Expr1 { get; set; }
        [Column("N_PriceID")]
        public int? NPriceId { get; set; }
        [Column("N_PriceVal", TypeName = "money")]
        public decimal NPriceVal { get; set; }
        [Column("N_ReferId")]
        public int? NReferId { get; set; }
        [Column("X_Name")]
        [StringLength(50)]
        public string XName { get; set; }
        [Column("N_PkeyId")]
        public int? NPkeyId { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
    }
}
