using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("___DAO_PaymentDetail")]
    public partial class DaoPaymentDetail
    {
        [Column("Sr_No")]
        [StringLength(100)]
        public string SrNo { get; set; }
        [Column("trn_date", TypeName = "date")]
        public DateTime? TrnDate { get; set; }
        [Column("payrunid")]
        [StringLength(100)]
        public string Payrunid { get; set; }
        [Column("X_Batch")]
        [StringLength(100)]
        public string XBatch { get; set; }
        [Column("X_PayRunText")]
        [StringLength(100)]
        public string XPayRunText { get; set; }
        [Column("Employee_Code")]
        [StringLength(100)]
        public string EmployeeCode { get; set; }
        [Column("Employee_Name")]
        [StringLength(100)]
        public string EmployeeName { get; set; }
        [StringLength(100)]
        public string Amount { get; set; }
    }
}
