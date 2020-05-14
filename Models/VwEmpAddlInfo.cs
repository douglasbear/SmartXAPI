using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwEmpAddlInfo
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_InfoID")]
        public int NInfoId { get; set; }
        [Column("X_ClgCourse")]
        [StringLength(50)]
        public string XClgCourse { get; set; }
        [Column("X_ClgLevel")]
        [StringLength(50)]
        public string XClgLevel { get; set; }
        [Column("X_HSchool")]
        [StringLength(50)]
        public string XHschool { get; set; }
        [Column("X_HsLevel")]
        [StringLength(50)]
        public string XHsLevel { get; set; }
        [Column("X_DGUniverity")]
        [StringLength(50)]
        public string XDguniverity { get; set; }
        [Column("X_DGInclusiveDate")]
        [StringLength(15)]
        public string XDginclusiveDate { get; set; }
        [Column("D_DGDate", TypeName = "datetime")]
        public DateTime? DDgdate { get; set; }
        [Column("X_PGCourse")]
        [StringLength(50)]
        public string XPgcourse { get; set; }
        [Column("X_PGUniverity")]
        [StringLength(50)]
        public string XPguniverity { get; set; }
        [Column("X_PGUnits")]
        [StringLength(50)]
        public string XPgunits { get; set; }
        [Column("X_PGLastSchool")]
        [StringLength(50)]
        public string XPglastSchool { get; set; }
        [Column("X_PGInclusiveDate")]
        [StringLength(50)]
        public string XPginclusiveDate { get; set; }
        [Column("D_PGDate", TypeName = "datetime")]
        public DateTime? DPgdate { get; set; }
        [Column("X_DrCourse")]
        [StringLength(50)]
        public string XDrCourse { get; set; }
        [Column("X_DrUniverity")]
        [StringLength(50)]
        public string XDrUniverity { get; set; }
        [Column("X_DrUnits")]
        [StringLength(50)]
        public string XDrUnits { get; set; }
        [Column("X_DrInclusiveDate")]
        [StringLength(50)]
        public string XDrInclusiveDate { get; set; }
        [Column("D_DrDate", TypeName = "datetime")]
        public DateTime? DDrDate { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("N_BranchID")]
        public int NBranchId { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("N_EmpID")]
        public int? NEmpId { get; set; }
        [Column("X_EmpCode")]
        [StringLength(50)]
        public string XEmpCode { get; set; }
        [Column("X_EmpName")]
        [StringLength(100)]
        public string XEmpName { get; set; }
        [Column("N_PRCRate", TypeName = "money")]
        public decimal? NPrcrate { get; set; }
        [Column("X_PRCRegNo")]
        [StringLength(20)]
        public string XPrcregNo { get; set; }
        [Column("X_PRCReg")]
        [StringLength(50)]
        public string XPrcreg { get; set; }
        [Column("D_PRCDate", TypeName = "datetime")]
        public DateTime? DPrcdate { get; set; }
        [Column("X_PRCValidity")]
        [StringLength(50)]
        public string XPrcvalidity { get; set; }
        [Column("N_Exp")]
        public double? NExp { get; set; }
        [Column("X_AddEduInfo")]
        [StringLength(100)]
        public string XAddEduInfo { get; set; }
        [Column("X_Achivmnts")]
        [StringLength(100)]
        public string XAchivmnts { get; set; }
    }
}
