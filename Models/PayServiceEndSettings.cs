using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Pay_ServiceEndSettings")]
    public partial class PayServiceEndSettings
    {
        [Key]
        [Column("N_EndSettiingsID")]
        public int NEndSettiingsId { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_Status")]
        public int? NStatus { get; set; }
        [Column("N_FromYear")]
        public int? NFromYear { get; set; }
        [Column("N_ToYear")]
        public int? NToYear { get; set; }
        [Column("N_SalaryRange")]
        public double? NSalaryRange { get; set; }
        [Column("N_ApplyFrom")]
        public int? NApplyFrom { get; set; }
        [Column("N_AmtPerc", TypeName = "money")]
        public decimal? NAmtPerc { get; set; }
        [Column("N_ServiceEndID")]
        public int? NServiceEndId { get; set; }
    }
}
