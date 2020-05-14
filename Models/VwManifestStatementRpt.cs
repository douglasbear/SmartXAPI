using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwManifestStatementRpt
    {
        [Column("N_CompanyId")]
        public int NCompanyId { get; set; }
        [Required]
        [Column("X_ManifestNo")]
        [StringLength(50)]
        public string XManifestNo { get; set; }
        [Column("N_TotAmt", TypeName = "money")]
        public decimal NTotAmt { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal NAmount { get; set; }
        [Column("D_EntryDate", TypeName = "smalldatetime")]
        public DateTime? DEntryDate { get; set; }
        [Column("X_CustomerName")]
        [StringLength(100)]
        public string XCustomerName { get; set; }
        [Column("N_ManifestID")]
        public int NManifestId { get; set; }
        [Column("N_CustomerID")]
        public int NCustomerId { get; set; }
        [Column("X_CustomerCode")]
        [StringLength(50)]
        public string XCustomerCode { get; set; }
    }
}
