using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Web_Pay_ApprovalSystemDetails")]
    public partial class WebPayApprovalSystemDetails
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_ApprovalID")]
        public int NApprovalId { get; set; }
        [Column("N_ApprovalDetailsID")]
        public int NApprovalDetailsId { get; set; }
        [Column("N_ActionID")]
        public int NActionId { get; set; }
        [Column("N_ApproverID")]
        public int NApproverId { get; set; }
        [Column("N_SequenceNo")]
        public int NSequenceNo { get; set; }
        [Column("X_Remarks")]
        [StringLength(100)]
        public string XRemarks { get; set; }
    }
}
