using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Mnp_EmpPriceSettings")]
    public partial class MnpEmpPriceSettings
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("N_PriceSettingsID")]
        public int NPriceSettingsId { get; set; }
        [Column("N_CustomerID")]
        public int NCustomerId { get; set; }
        [Required]
        [Column("X_SettingsNo")]
        [StringLength(50)]
        public string XSettingsNo { get; set; }
        [Column("D_Date", TypeName = "datetime")]
        public DateTime DDate { get; set; }
        [Column("N_ProjectID")]
        public int? NProjectId { get; set; }
        [Column("N_BranchID")]
        public int NBranchId { get; set; }
        [Column("N_UserID")]
        public int NUserId { get; set; }
        [Column("N_TotalAmount", TypeName = "money")]
        public decimal NTotalAmount { get; set; }
        [Column("N_TaxAmount", TypeName = "money")]
        public decimal NTaxAmount { get; set; }
        [Column("N_NetAmount", TypeName = "money")]
        public decimal NNetAmount { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime DEntryDate { get; set; }
    }
}
