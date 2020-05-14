using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_Opportunities")]
    public partial class InvOpportunities
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Key]
        [Column("N_ReferenceID")]
        public int NReferenceId { get; set; }
        [Column("X_ReferenceCode")]
        [StringLength(200)]
        public string XReferenceCode { get; set; }
        [Column("N_CustomerID")]
        public int NCustomerId { get; set; }
        [Column("X_ScopeOfWork")]
        [StringLength(200)]
        public string XScopeOfWork { get; set; }
        [Column("X_Status")]
        [StringLength(200)]
        public string XStatus { get; set; }
        [Column("D_Date", TypeName = "datetime")]
        public DateTime? DDate { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_TypeId")]
        public int? NTypeId { get; set; }
        [Column("N_Processed")]
        public int? NProcessed { get; set; }
    }
}
