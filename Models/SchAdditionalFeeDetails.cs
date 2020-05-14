using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Sch_AdditionalFeeDetails")]
    public partial class SchAdditionalFeeDetails
    {
        [Key]
        [Column("N_FeeDetailsId")]
        public int NFeeDetailsId { get; set; }
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("X_FeeDescription")]
        [StringLength(100)]
        public string XFeeDescription { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal? NAmount { get; set; }
        [Column("N_ReceiptID")]
        public int? NReceiptId { get; set; }
        [Column("N_ACYearID")]
        public int? NAcyearId { get; set; }
    }
}
