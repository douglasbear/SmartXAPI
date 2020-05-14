using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Pay_PayType")]
    public partial class PayPayType
    {
        [Key]
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Key]
        [Column("N_PayTypeID")]
        public int NPayTypeId { get; set; }
        [Column("X_PayType")]
        [StringLength(100)]
        public string XPayType { get; set; }
        [Column("N_PerPayMethod")]
        public int? NPerPayMethod { get; set; }
        [Column("N_Type")]
        public int? NType { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("X_Cr_MappingLevel")]
        [StringLength(10)]
        public string XCrMappingLevel { get; set; }
        [Column("X_Dr_MappingLevel")]
        [StringLength(10)]
        public string XDrMappingLevel { get; set; }
        [Column("Cr_MappingLevel")]
        [StringLength(10)]
        public string CrMappingLevel { get; set; }
        [Column("Dr_MappingLevel")]
        [StringLength(10)]
        public string DrMappingLevel { get; set; }
    }
}
