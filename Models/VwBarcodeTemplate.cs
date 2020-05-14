using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwBarcodeTemplate
    {
        [Column("N_TemplateID")]
        public int NTemplateId { get; set; }
        [Column("X_TemplateCode")]
        [StringLength(50)]
        public string XTemplateCode { get; set; }
        [Column("X_TemplateName")]
        [StringLength(50)]
        public string XTemplateName { get; set; }
        [Column("N_TemplateDetailsID")]
        public int NTemplateDetailsId { get; set; }
        [Column("N_FieldID")]
        public int? NFieldId { get; set; }
        [Column("B_Show")]
        public bool? BShow { get; set; }
        [Column("N_Order")]
        public int? NOrder { get; set; }
        [Column("X_DisplayName")]
        [StringLength(50)]
        public string XDisplayName { get; set; }
        [Column("X_TableName")]
        [StringLength(50)]
        public string XTableName { get; set; }
        [Column("N_FormID")]
        public int? NFormId { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_MenuID")]
        public int? NMenuId { get; set; }
        [Column("N_ScreenID")]
        public int? NScreenId { get; set; }
        [Column("X_Font")]
        [StringLength(200)]
        public string XFont { get; set; }
        [Column("X_FontName")]
        [StringLength(100)]
        public string XFontName { get; set; }
        [Column("N_FontSize")]
        public double? NFontSize { get; set; }
        [Column("X_FieldName")]
        [StringLength(50)]
        public string XFieldName { get; set; }
        [Column("N_DocTotalWidth")]
        public double NDocTotalWidth { get; set; }
        [Column("N_LabelHeight")]
        public double NLabelHeight { get; set; }
        [Column("N_LabelWidth")]
        public double NLabelWidth { get; set; }
        [Column("N_LabelMarginLeft")]
        public double NLabelMarginLeft { get; set; }
        [Column("N_LabelMarginRight")]
        public double NLabelMarginRight { get; set; }
        [Column("N_LabelMarginTop")]
        public double NLabelMarginTop { get; set; }
        [Column("N_LabelMarginBottom")]
        public double NLabelMarginBottom { get; set; }
        [Column("N_ColumnsPerRow")]
        public int NColumnsPerRow { get; set; }
        [Column("N_BarcodeHeight")]
        public double NBarcodeHeight { get; set; }
        [Column("N_BarcodeSize")]
        public double NBarcodeSize { get; set; }
        [Column("N_BarcodeBaseLine")]
        public double NBarcodeBaseLine { get; set; }
        [Required]
        [Column("X_TextAlignment")]
        [StringLength(10)]
        public string XTextAlignment { get; set; }
        [Column("N_SpaceBetweenLabels")]
        public double NSpaceBetweenLabels { get; set; }
        [Column("N_FieldSpace")]
        public double NFieldSpace { get; set; }
        [Column("N_StartSpace")]
        public double NStartSpace { get; set; }
        [Column("N_EndSpace")]
        public double NEndSpace { get; set; }
        [Column("N_FieldLength")]
        public int NFieldLength { get; set; }
        [Column("N_FontStyle")]
        public int NFontStyle { get; set; }
    }
}
