using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_TaxCategoryType")]
    public partial class InvTaxCategoryType
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Key]
        [Column("N_TypeID")]
        public int NTypeId { get; set; }
        [Column("N_TransType")]
        public int NTransType { get; set; }
        [Column("X_Description")]
        [StringLength(250)]
        public string XDescription { get; set; }
        [Column("B_TaxApplicable")]
        public int? BTaxApplicable { get; set; }
        [Column("B_ToBeChecked")]
        public int? BToBeChecked { get; set; }
    }
}
