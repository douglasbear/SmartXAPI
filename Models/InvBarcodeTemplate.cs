using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_BarcodeTemplate")]
    public partial class InvBarcodeTemplate
    {
        [Key]
        [Column("N_TemplateID")]
        public int NTemplateId { get; set; }
        [Column("X_TemplateCode")]
        [StringLength(50)]
        public string XTemplateCode { get; set; }
        [Column("X_TemplateName")]
        [StringLength(50)]
        public string XTemplateName { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime? DEntryDate { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_MenuID")]
        public int? NMenuId { get; set; }
        [Column("N_FormID")]
        public int? NFormId { get; set; }
        [Column("N_DocTotalWidth")]
        public double? NDocTotalWidth { get; set; }
        [Column("N_LabelHeight")]
        public double? NLabelHeight { get; set; }
        [Column("N_LabelWidth")]
        public double? NLabelWidth { get; set; }
        [Column("N_LabelMarginLeft")]
        public double? NLabelMarginLeft { get; set; }
        [Column("N_LabelMarginRight")]
        public double? NLabelMarginRight { get; set; }
        [Column("N_LabelMarginTop")]
        public double? NLabelMarginTop { get; set; }
        [Column("N_LabelMarginBottom")]
        public double? NLabelMarginBottom { get; set; }
        [Column("N_ColumnsPerRow")]
        public int? NColumnsPerRow { get; set; }
        [Column("N_BarcodeHeight")]
        public double? NBarcodeHeight { get; set; }
        [Column("N_BarcodeSize")]
        public double? NBarcodeSize { get; set; }
        [Column("N_BarcodeBaseLine")]
        public double? NBarcodeBaseLine { get; set; }
        [Column("N_SpaceBetweenLabels")]
        public double? NSpaceBetweenLabels { get; set; }
    }
}
