using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class MnpMobilizationDisp
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_MobilizationID")]
        public int NMobilizationId { get; set; }
        [Required]
        [Column("X_MobilizationCode")]
        [StringLength(20)]
        public string XMobilizationCode { get; set; }
        [Column("D_Date")]
        [StringLength(8000)]
        public string DDate { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("N_CustomerID")]
        public int NCustomerId { get; set; }
        [Required]
        [Column("X_OrderNo")]
        [StringLength(10)]
        public string XOrderNo { get; set; }
        [Column("N_BranchID")]
        public int NBranchId { get; set; }
        [Column("N_UserID")]
        public int NUserId { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime DEntryDate { get; set; }
        [Column("X_CustomerCode")]
        [StringLength(50)]
        public string XCustomerCode { get; set; }
        [Column("X_CustomerName")]
        [StringLength(100)]
        public string XCustomerName { get; set; }
        [Column("X_ProjectName")]
        [StringLength(50)]
        public string XProjectName { get; set; }
        [Column("N_FromPrj")]
        public int NFromPrj { get; set; }
        [Column("N_ProjectID")]
        public int? NProjectId { get; set; }
        [Column("X_UserName")]
        [StringLength(60)]
        public string XUserName { get; set; }
        [Column("B_IssaveDraft")]
        public bool BIssaveDraft { get; set; }
        [Column("N_ProcStatus")]
        public int? NProcStatus { get; set; }
        [Column("N_NextApprovalID")]
        public int? NNextApprovalId { get; set; }
        [Column("N_ApprovalLevelId")]
        public int? NApprovalLevelId { get; set; }
    }
}
