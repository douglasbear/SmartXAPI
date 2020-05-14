using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInsuranceAmountCategoryWise
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Required]
        [Column("X_CategoryCode")]
        [StringLength(50)]
        public string XCategoryCode { get; set; }
        [Required]
        [Column("X_CategoryName")]
        [StringLength(150)]
        public string XCategoryName { get; set; }
        [Column("X_InsuranceClass")]
        [StringLength(50)]
        public string XInsuranceClass { get; set; }
        [Column("N_Cost")]
        [StringLength(30)]
        public string NCost { get; set; }
        [Column("N_Price")]
        [StringLength(30)]
        public string NPrice { get; set; }
        [Column("N_InsuranceID")]
        public int? NInsuranceId { get; set; }
        [Column("N_PkeyId")]
        public int NPkeyId { get; set; }
        public int EmpType { get; set; }
        [Column("X_Price")]
        [StringLength(30)]
        public string XPrice { get; set; }
    }
}
