using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Pay_DepartmentSettings")]
    public partial class PayDepartmentSettings
    {
        [Key]
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Key]
        [Column("N_SettingsID")]
        public int NSettingsId { get; set; }
        [Key]
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("N_DepartmentID")]
        public int? NDepartmentId { get; set; }
        [Column("N_PayID")]
        public int? NPayId { get; set; }
        [Column("N_LedgerID")]
        public int? NLedgerId { get; set; }
        [Column("N_Cr_LedgerID")]
        public int? NCrLedgerId { get; set; }
    }
}
