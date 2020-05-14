using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Api_Transactions")]
    public partial class ApiTransactions
    {
        [Column("N_TransactionID")]
        public int? NTransactionId { get; set; }
        [Column("X_CompanyCode")]
        [StringLength(50)]
        public string XCompanyCode { get; set; }
        [Column("X_TransType")]
        [StringLength(50)]
        public string XTransType { get; set; }
        [Column("X_AccountCode")]
        [StringLength(50)]
        public string XAccountCode { get; set; }
        [Column("D_TransDate", TypeName = "datetime")]
        public DateTime? DTransDate { get; set; }
        [Column("X_RefNo")]
        [StringLength(50)]
        public string XRefNo { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal? NAmount { get; set; }
        [Column("X_Remarks")]
        [StringLength(1000)]
        public string XRemarks { get; set; }
        [Column("X_BranchCode")]
        [StringLength(50)]
        public string XBranchCode { get; set; }
        [Column("X_CostCenterCode")]
        [StringLength(50)]
        public string XCostCenterCode { get; set; }
        [Column("X_ProjectCode")]
        [StringLength(50)]
        public string XProjectCode { get; set; }
        [Column("X_EmployeeCode")]
        [StringLength(50)]
        public string XEmployeeCode { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime? DEntryDate { get; set; }
        [Column("B_IsProcessed")]
        public bool? BIsProcessed { get; set; }
    }
}
