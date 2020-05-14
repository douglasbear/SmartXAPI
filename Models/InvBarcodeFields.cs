using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_BarcodeFields")]
    public partial class InvBarcodeFields
    {
        [Key]
        [Column("N_FieldID")]
        public int NFieldId { get; set; }
        [Column("X_FieldName")]
        [StringLength(50)]
        public string XFieldName { get; set; }
        [Column("X_DisplayName")]
        [StringLength(50)]
        public string XDisplayName { get; set; }
        [Column("X_TableName")]
        [StringLength(50)]
        public string XTableName { get; set; }
        [Column("B_Enabled")]
        public bool? BEnabled { get; set; }
        [Column("N_Order")]
        public int? NOrder { get; set; }
        [Column("N_FormID")]
        public int? NFormId { get; set; }
    }
}
