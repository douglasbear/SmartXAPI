using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvWarrantyDisp
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_WarrantyID")]
        public int NWarrantyId { get; set; }
        [Column("X_WarrantyCode")]
        [StringLength(50)]
        public string XWarrantyCode { get; set; }
        [Column("X_Warranty")]
        [StringLength(100)]
        public string XWarranty { get; set; }
        [Column("N_WarrantyTypeID")]
        public int? NWarrantyTypeId { get; set; }
        [Column("X_Remarks")]
        [StringLength(100)]
        public string XRemarks { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("X_TypeName")]
        [StringLength(50)]
        public string XTypeName { get; set; }
        [Column("N_DefaultId")]
        public int? NDefaultId { get; set; }
        [Column("X_TypeCode")]
        [StringLength(5)]
        public string XTypeCode { get; set; }
    }
}
