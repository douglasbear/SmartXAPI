using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvAmountSplitType
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_TypeID")]
        public int NTypeId { get; set; }
        [Column("X_TypeCode")]
        [StringLength(50)]
        public string XTypeCode { get; set; }
        [Column("X_TypeName")]
        [StringLength(100)]
        public string XTypeName { get; set; }
        [Column("N_Amount")]
        [StringLength(100)]
        public string NAmount { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("N_CtegoryId")]
        public int? NCtegoryId { get; set; }
    }
}
