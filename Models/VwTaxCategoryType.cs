using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwTaxCategoryType
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_TypeID")]
        public int NTypeId { get; set; }
        [Column("N_TransType")]
        public int NTransType { get; set; }
        [Column("X_Description")]
        [StringLength(250)]
        public string XDescription { get; set; }
        [Required]
        [Column("B_TaxApplicable")]
        [StringLength(5)]
        public string BTaxApplicable { get; set; }
        [Column("B_ToBeChecked")]
        public int? BToBeChecked { get; set; }
    }
}
