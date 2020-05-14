using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_BarcodeTemplateDetails")]
    public partial class InvBarcodeTemplateDetails
    {
        [Key]
        [Column("N_TemplateDetailsID")]
        public int NTemplateDetailsId { get; set; }
        [Column("N_TemplateID")]
        public int? NTemplateId { get; set; }
        [Column("N_FieldID")]
        public int? NFieldId { get; set; }
        [Column("B_Show")]
        public bool? BShow { get; set; }
        [Column("N_Order")]
        public int? NOrder { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime? DEntryDate { get; set; }
        [Column("X_Font")]
        [StringLength(200)]
        public string XFont { get; set; }
        [Column("X_FontName")]
        [StringLength(100)]
        public string XFontName { get; set; }
        [Column("N_FontSize")]
        public double? NFontSize { get; set; }
        [Column("X_TextAlignment")]
        [StringLength(10)]
        public string XTextAlignment { get; set; }
        [Column("N_FieldSpace")]
        public double? NFieldSpace { get; set; }
        [Column("N_StartSpace")]
        public double? NStartSpace { get; set; }
        [Column("N_EndSpace")]
        public double? NEndSpace { get; set; }
        [Column("N_FieldLength")]
        public int? NFieldLength { get; set; }
        [Column("N_FontStyle")]
        public int? NFontStyle { get; set; }
    }
}
