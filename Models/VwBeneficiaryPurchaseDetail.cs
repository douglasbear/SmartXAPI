using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwBeneficiaryPurchaseDetail
    {
        [Column("N_PurchaseID")]
        public int NPurchaseId { get; set; }
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_BeneficiaryID")]
        public int NBeneficiaryId { get; set; }
        [Column("X_BeneficiaryCode")]
        [StringLength(50)]
        public string XBeneficiaryCode { get; set; }
        [Column("X_BeneficiaryName")]
        [StringLength(100)]
        public string XBeneficiaryName { get; set; }
    }
}
