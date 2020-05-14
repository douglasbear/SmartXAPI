using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_SalesBudget")]
    public partial class InvSalesBudget
    {
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearId")]
        public int? NFnYearId { get; set; }
        [Key]
        [Column("N_BudgetId")]
        public int NBudgetId { get; set; }
        [Required]
        [Column("X_BudgetCode")]
        [StringLength(100)]
        public string XBudgetCode { get; set; }
        [Column("D_Date", TypeName = "datetime")]
        public DateTime? DDate { get; set; }
        [Column("X_CustomerName")]
        [StringLength(1000)]
        public string XCustomerName { get; set; }
        [Column("X_ProjectName")]
        [StringLength(1000)]
        public string XProjectName { get; set; }
        [Column("N_ProjectType")]
        public int? NProjectType { get; set; }
        [Column("N_StreamID")]
        public int? NStreamId { get; set; }
        [Column("N_Q1", TypeName = "money")]
        public decimal? NQ1 { get; set; }
        [Column("N_Q2", TypeName = "money")]
        public decimal? NQ2 { get; set; }
        [Column("N_Q3", TypeName = "money")]
        public decimal? NQ3 { get; set; }
        [Column("N_Q4", TypeName = "money")]
        public decimal? NQ4 { get; set; }
        [Column("N_RAF1", TypeName = "money")]
        public decimal? NRaf1 { get; set; }
        [Column("N_RAF2", TypeName = "money")]
        public decimal? NRaf2 { get; set; }
        [Column("N_RAF3", TypeName = "money")]
        public decimal? NRaf3 { get; set; }
        [Column("N_RAF4", TypeName = "money")]
        public decimal? NRaf4 { get; set; }
    }
}
