using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwTimesheetImportDisp
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("D_Date", TypeName = "date")]
        public DateTime? DDate { get; set; }
        [Column("N_EmpID")]
        public int? NEmpId { get; set; }
        [Column("D_In")]
        public TimeSpan? DIn { get; set; }
        [Column("D_Out")]
        public TimeSpan? DOut { get; set; }
        [Column("X_PayrunText")]
        [StringLength(50)]
        public string XPayrunText { get; set; }
        [Column("D_Shift2_In")]
        public TimeSpan? DShift2In { get; set; }
        [Column("D_Shift2_Out")]
        public TimeSpan? DShift2Out { get; set; }
        [Column("N_SheetID")]
        public int? NSheetId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_Status")]
        public int? NStatus { get; set; }
        [Column("X_Remarks")]
        [StringLength(500)]
        public string XRemarks { get; set; }
    }
}
