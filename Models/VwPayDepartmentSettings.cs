using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPayDepartmentSettings
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_DepartmentID")]
        public int? NDepartmentId { get; set; }
        [Column("X_DepartmentCode")]
        [StringLength(50)]
        public string XDepartmentCode { get; set; }
        [Column("X_Department")]
        [StringLength(100)]
        public string XDepartment { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_SettingsID")]
        public int? NSettingsId { get; set; }
        [Column("N_LedgerID")]
        public int? NLedgerId { get; set; }
        [Column("X_LedgerCode")]
        [StringLength(50)]
        public string XLedgerCode { get; set; }
        [Column("X_LedgerName")]
        [StringLength(500)]
        public string XLedgerName { get; set; }
        [Column("N_PayID")]
        public int NPayId { get; set; }
        [Column("X_PayCode")]
        [StringLength(50)]
        public string XPayCode { get; set; }
        [Column("X_Description")]
        [StringLength(100)]
        public string XDescription { get; set; }
        [Column("N_Cr_LedgerID")]
        public int? NCrLedgerId { get; set; }
        [Column("X_Cr_LedgerCode")]
        [StringLength(50)]
        public string XCrLedgerCode { get; set; }
        [Column("X_Cr_LedgerName")]
        [StringLength(500)]
        public string XCrLedgerName { get; set; }
        [Column("X_Cr_MappingLevel")]
        [StringLength(10)]
        public string XCrMappingLevel { get; set; }
        [Column("X_Dr_MappingLevel")]
        [StringLength(10)]
        public string XDrMappingLevel { get; set; }
    }
}
