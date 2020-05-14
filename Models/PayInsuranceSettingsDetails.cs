using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Pay_InsuranceSettingsDetails")]
    public partial class PayInsuranceSettingsDetails
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Key]
        [Column("N_InsuranceSettingsDetailsID")]
        public int NInsuranceSettingsDetailsId { get; set; }
        [Column("N_InsuranceSettingsID")]
        public int NInsuranceSettingsId { get; set; }
        [Column("N_InsuranceCategoryID")]
        public int NInsuranceCategoryId { get; set; }
        [Column("N_Cost", TypeName = "money")]
        public decimal NCost { get; set; }
        [Column("N_Price", TypeName = "money")]
        public decimal NPrice { get; set; }
    }
}
