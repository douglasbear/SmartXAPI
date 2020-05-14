using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Sec_FormComponents")]
    public partial class SecFormComponents
    {
        [Key]
        [Column("N_CtrlID")]
        public int NCtrlId { get; set; }
        [Column("N_MenuID")]
        public int? NMenuId { get; set; }
        [Column("X_Control")]
        [StringLength(50)]
        public string XControl { get; set; }
        [Column("X_CtrlName")]
        [StringLength(50)]
        public string XCtrlName { get; set; }
        [Column("X_CtrlText")]
        [StringLength(50)]
        public string XCtrlText { get; set; }
        [Column("N_X")]
        public int? NX { get; set; }
        [Column("N_Y")]
        public int? NY { get; set; }
        [Column("X_Tag")]
        [StringLength(2000)]
        public string XTag { get; set; }
        [Column("N_GroupBoxID")]
        public int? NGroupBoxId { get; set; }
        [Column("N_Width")]
        public int? NWidth { get; set; }
        [Column("N_Height")]
        public int? NHeight { get; set; }
        [Column("X_TableName")]
        [StringLength(50)]
        public string XTableName { get; set; }
        [Column("X_FieldName")]
        [StringLength(50)]
        public string XFieldName { get; set; }
        [Column("X_ProcCode")]
        [StringLength(50)]
        public string XProcCode { get; set; }
        [Column("X_RptCompanyIDField")]
        [StringLength(200)]
        public string XRptCompanyIdfield { get; set; }
        [Column("X_RptYearIDField")]
        [StringLength(50)]
        public string XRptYearIdfield { get; set; }
        [Column("N_LabelCtrlID")]
        public int? NLabelCtrlId { get; set; }
    }
}
