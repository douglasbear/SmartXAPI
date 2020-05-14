using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwAmortization
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("N_AmortizationID")]
        public int NAmortizationId { get; set; }
        [Column("N_AdditionID")]
        public int NAdditionId { get; set; }
        [Column("N_PolicyID")]
        public int NPolicyId { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("N_AmortizationDetailsID")]
        public int NAmortizationDetailsId { get; set; }
        [Column("N_EmpID")]
        public int NEmpId { get; set; }
        [Column("N_DependentID")]
        public int NDependentId { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal NAmount { get; set; }
    }
}
