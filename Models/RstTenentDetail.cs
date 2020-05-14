using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Rst_TenentDetail")]
    public partial class RstTenentDetail
    {
        [Key]
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_TenentID")]
        public int NTenentId { get; set; }
        [Key]
        [Column("N_TenentDetailID")]
        public int NTenentDetailId { get; set; }
        [Column("N_ItemID")]
        [StringLength(50)]
        public string NItemId { get; set; }
        [Column("X_ExtensionCode")]
        [StringLength(50)]
        public string XExtensionCode { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal? NAmount { get; set; }
    }
}
