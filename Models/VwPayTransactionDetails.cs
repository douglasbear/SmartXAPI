using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPayTransactionDetails
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_TransID")]
        public int NTransId { get; set; }
        [Column("N_EmpID")]
        public int? NEmpId { get; set; }
        [Column("N_PayID")]
        public int? NPayId { get; set; }
        [Column("N_PayRunID")]
        public int? NPayRunId { get; set; }
        [Column("D_TransDate", TypeName = "datetime")]
        public DateTime? DTransDate { get; set; }
        [Column("X_Batch")]
        [StringLength(100)]
        public string XBatch { get; set; }
        [Column("X_PayrunText")]
        [StringLength(50)]
        public string XPayrunText { get; set; }
        [Column("B_PostedAccount")]
        public bool? BPostedAccount { get; set; }
        [Column("X_EmpCode")]
        [StringLength(50)]
        public string XEmpCode { get; set; }
        [Column("X_PayCode")]
        [StringLength(50)]
        public string XPayCode { get; set; }
        [Column("N_Payrate", TypeName = "money")]
        public decimal? NPayrate { get; set; }
        [Column("N_PayFactor")]
        public double? NPayFactor { get; set; }
        [Column("X_PayGroup")]
        [StringLength(50)]
        public string XPayGroup { get; set; }
        [Column("N_PositionID")]
        public int? NPositionId { get; set; }
        [Column("N_DepartmentID")]
        public int? NDepartmentId { get; set; }
        [Column("N_TransDetailsID")]
        public int NTransDetailsId { get; set; }
        [Column("N_Status")]
        public int NStatus { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_FormID")]
        public int? NFormId { get; set; }
    }
}
