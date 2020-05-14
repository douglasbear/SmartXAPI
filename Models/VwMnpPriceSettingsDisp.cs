using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwMnpPriceSettingsDisp
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
        [Column("D_Date")]
        [StringLength(8000)]
        public string DDate { get; set; }
        [Column("N_TotalAmount")]
        [StringLength(30)]
        public string NTotalAmount { get; set; }
        [Column("N_TaxAmount")]
        [StringLength(30)]
        public string NTaxAmount { get; set; }
        [Column("N_NetAmount")]
        [StringLength(30)]
        public string NNetAmount { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime DEntryDate { get; set; }
        [Column("X_ProjectName")]
        [StringLength(50)]
        public string XProjectName { get; set; }
        [Column("X_CustomerCode")]
        [StringLength(50)]
        public string XCustomerCode { get; set; }
        [Column("X_CustomerName")]
        [StringLength(100)]
        public string XCustomerName { get; set; }
        [Column("X_ContactName")]
        [StringLength(100)]
        public string XContactName { get; set; }
        [Column("X_Address")]
        [StringLength(250)]
        public string XAddress { get; set; }
        [Column("X_Country")]
        [StringLength(50)]
        public string XCountry { get; set; }
        [Column("N_BranchID")]
        public int NBranchId { get; set; }
    }
}
