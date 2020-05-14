using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_AmountSplitType")]
    public partial class InvAmountSplitType
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Key]
        [Column("N_TypeID")]
        public int NTypeId { get; set; }
        [Column("X_TypeCode")]
        [StringLength(50)]
        public string XTypeCode { get; set; }
        [Column("X_TypeName")]
        [StringLength(100)]
        public string XTypeName { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal? NAmount { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("N_CtegoryId")]
        public int? NCtegoryId { get; set; }
    }
}
