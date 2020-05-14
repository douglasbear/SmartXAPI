using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPayEmployeeClearanceSettings
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_ClearanceSettingsID")]
        public int NClearanceSettingsId { get; set; }
        [Column("X_ClearanceCode")]
        [StringLength(50)]
        public string XClearanceCode { get; set; }
        [Column("X_PurposeName")]
        [StringLength(50)]
        public string XPurposeName { get; set; }
        [Column("N_PurposeID")]
        public int NPurposeId { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime? DEntryDate { get; set; }
        [Column("X_Name")]
        [StringLength(50)]
        public string XName { get; set; }
        [Column("X_PkeyCode")]
        [StringLength(5)]
        public string XPkeyCode { get; set; }
        [Column("N_ReferId")]
        public int? NReferId { get; set; }
        [Column("X_Name_Ar")]
        [StringLength(250)]
        public string XNameAr { get; set; }
    }
}
