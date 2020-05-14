using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPayApprovalCodeDtls
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_ApprovalID")]
        public int NApprovalId { get; set; }
        [Required]
        [Column("X_ApprovalCode")]
        [StringLength(50)]
        public string XApprovalCode { get; set; }
        [Required]
        [Column("X_ApprovalDescription")]
        [StringLength(50)]
        public string XApprovalDescription { get; set; }
        [Column("D_Date", TypeName = "datetime")]
        public DateTime? DDate { get; set; }
        [Column("N_UserID")]
        public int NUserId { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime DEntryDate { get; set; }
        [Column("N_ApproverID")]
        public int NApproverId { get; set; }
        [Column("N_ActionID")]
        public int NActionId { get; set; }
        [Column("N_ApprovalDetailsID")]
        public int NApprovalDetailsId { get; set; }
        [Column("N_SequenceNo")]
        public int NSequenceNo { get; set; }
        [Column("X_Remarks")]
        [StringLength(100)]
        public string XRemarks { get; set; }
        public int Expr1 { get; set; }
        [Column("X_ActionDesc")]
        [StringLength(50)]
        public string XActionDesc { get; set; }
        [Column("X_EmpName")]
        [StringLength(100)]
        public string XEmpName { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
    }
}
