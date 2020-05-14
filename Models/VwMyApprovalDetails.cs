using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwMyApprovalDetails
    {
        [Column("N_EmpID")]
        public int? NEmpId { get; set; }
        [Column("X_EmpCode")]
        [StringLength(50)]
        public string XEmpCode { get; set; }
        [Column("X_EmailID")]
        [StringLength(100)]
        public string XEmailId { get; set; }
        [Column("X_EmpName")]
        [StringLength(100)]
        public string XEmpName { get; set; }
        [Column("X_Position")]
        [StringLength(100)]
        public string XPosition { get; set; }
        [Column("D_RequestDate", TypeName = "datetime")]
        public DateTime? DRequestDate { get; set; }
        [Column("X_Type")]
        [StringLength(50)]
        public string XType { get; set; }
        [Column("X_TypeName")]
        [StringLength(50)]
        public string XTypeName { get; set; }
        [Column("D_Shift1_In")]
        public TimeSpan? DShift1In { get; set; }
        [Column("D_Shift1_Out")]
        public TimeSpan? DShift1Out { get; set; }
        [Column("D_Shift2_In")]
        public TimeSpan? DShift2In { get; set; }
        [Column("D_Shift2_Out")]
        public TimeSpan? DShift2Out { get; set; }
        [Column("X_Comment")]
        [StringLength(100)]
        public string XComment { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_TransID")]
        public int NTransId { get; set; }
        [Column("X_TransCode")]
        public int? XTransCode { get; set; }
        [Column("D_TransDate", TypeName = "datetime")]
        public DateTime? DTransDate { get; set; }
        [Column("DFActIn")]
        public TimeSpan? DfactIn { get; set; }
        [Column("DFActOut")]
        public TimeSpan? DfactOut { get; set; }
        [Column("DSActIn")]
        public TimeSpan? DsactIn { get; set; }
        [Column("DSActOut")]
        public TimeSpan? DsactOut { get; set; }
        [Column("X_Time")]
        public double? XTime { get; set; }
    }
}
