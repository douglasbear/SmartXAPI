using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPayCodeItemwiseMappingDetailsRpt
    {
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearId")]
        public int? NFnYearId { get; set; }
        [Column("N_MappingId")]
        public int NMappingId { get; set; }
        [Column("X_MappingCode")]
        [StringLength(50)]
        public string XMappingCode { get; set; }
        [Column("X_MappingName")]
        [StringLength(50)]
        public string XMappingName { get; set; }
        [Column("N_MappingDetailsID")]
        public int NMappingDetailsId { get; set; }
        [Column("N_PaycodeID")]
        public int? NPaycodeId { get; set; }
        [Column("X_PayCode")]
        [StringLength(50)]
        public string XPayCode { get; set; }
        [Column("X_Description")]
        [StringLength(100)]
        public string XDescription { get; set; }
        [Column("X_PayType")]
        [StringLength(100)]
        public string XPayType { get; set; }
        [Column("X_ItemName")]
        [StringLength(800)]
        public string XItemName { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Column("X_ItemCode")]
        [StringLength(100)]
        public string XItemCode { get; set; }
    }
}
