using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwSchFeesSummaryRpt1
    {
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("N_AdmissionID")]
        public int? NAdmissionId { get; set; }
        [Column("N_DiscountAmt", TypeName = "money")]
        public decimal? NDiscountAmt { get; set; }
        [Column("N_SalesAmt", TypeName = "money")]
        public decimal? NSalesAmt { get; set; }
        [Column("N_ReceivedAmount", TypeName = "money")]
        public decimal? NReceivedAmount { get; set; }
        [Column("N_DueAmount", TypeName = "money")]
        public decimal? NDueAmount { get; set; }
        [Column("N_DivisionID")]
        public int? NDivisionId { get; set; }
        [Column("X_Name")]
        [StringLength(200)]
        public string XName { get; set; }
        [Column("X_Gender")]
        [StringLength(10)]
        public string XGender { get; set; }
        [Column("X_StudentMobile")]
        [StringLength(50)]
        public string XStudentMobile { get; set; }
        [Column("N_ClassID")]
        public int NClassId { get; set; }
    }
}
