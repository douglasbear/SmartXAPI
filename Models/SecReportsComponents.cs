using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Sec_ReportsComponents")]
    public partial class SecReportsComponents
    {
        [Column("N_CompID")]
        public int NCompId { get; set; }
        [Column("N_MenuID")]
        public int NMenuId { get; set; }
        [Column("X_CompType")]
        [StringLength(50)]
        public string XCompType { get; set; }
        [Column("X_FieldType")]
        [StringLength(50)]
        public string XFieldType { get; set; }
        [Column("X_DataField")]
        [StringLength(250)]
        public string XDataField { get; set; }
        [Column("N_MaxLength")]
        public int? NMaxLength { get; set; }
        [Column("X_TableName")]
        [StringLength(50)]
        public string XTableName { get; set; }
        [Column("X_HideFieldList")]
        [StringLength(250)]
        public string XHideFieldList { get; set; }
        [Column("X_FieldtoReturn")]
        [StringLength(50)]
        public string XFieldtoReturn { get; set; }
        [Column("X_FieldList")]
        [StringLength(250)]
        public string XFieldList { get; set; }
        [Column("X_Criteria")]
        [StringLength(350)]
        public string XCriteria { get; set; }
        [Column("X_ProcCode")]
        [StringLength(50)]
        public string XProcCode { get; set; }
        [Column("X_DataFieldCompanyID")]
        [StringLength(50)]
        public string XDataFieldCompanyId { get; set; }
        [Column("X_DataFieldYearID")]
        [StringLength(50)]
        public string XDataFieldYearId { get; set; }
        [Column("B_StaticList")]
        public bool? BStaticList { get; set; }
        [Column("X_FixedValList")]
        [StringLength(500)]
        public string XFixedValList { get; set; }
        [Column("B_Range")]
        public bool? BRange { get; set; }
        [Column("X_DefVal1")]
        [StringLength(50)]
        public string XDefVal1 { get; set; }
        [Column("X_DefVal2")]
        [StringLength(50)]
        public string XDefVal2 { get; set; }
        [Column("N_SqlType")]
        public int? NSqlType { get; set; }
        [Column("X_rptFile")]
        [StringLength(50)]
        public string XRptFile { get; set; }
        [Column("X_LangControlNo")]
        [StringLength(50)]
        public string XLangControlNo { get; set; }
        [Column("N_ListOrder")]
        public int? NListOrder { get; set; }
        [Column("B_Active")]
        public bool? BActive { get; set; }
        [Column("X_ReportCode")]
        [StringLength(50)]
        public string XReportCode { get; set; }
        [Column("X_FieldWidth")]
        [StringLength(50)]
        public string XFieldWidth { get; set; }
        [Column("N_ComponentWidth")]
        public int? NComponentWidth { get; set; }
        [Column("X_LinkField")]
        [StringLength(50)]
        public string XLinkField { get; set; }
        [Column("N_LinkCompID")]
        public int? NLinkCompId { get; set; }
        [Column("X_OrderByField")]
        [StringLength(150)]
        public string XOrderByField { get; set; }
        [Column("B_EnableMultiselect")]
        public bool? BEnableMultiselect { get; set; }
        [Column("X_DataFieldUserID")]
        [StringLength(50)]
        public string XDataFieldUserId { get; set; }
        [Column("X_DataFieldBranchID")]
        [StringLength(50)]
        public string XDataFieldBranchId { get; set; }
        [Column("X_Operator")]
        [StringLength(5)]
        public string XOperator { get; set; }
    }
}
