using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Acc_VoucherTypes")]
    public partial class AccVoucherTypes
    {
        [Key]
        [Column("X_ID")]
        [StringLength(20)]
        public string XId { get; set; }
        [Column("X_Code")]
        [StringLength(100)]
        public string XCode { get; set; }
        [Column("X_Description")]
        [StringLength(50)]
        public string XDescription { get; set; }
        [Column("X_VoucherRemarks")]
        [StringLength(50)]
        public string XVoucherRemarks { get; set; }
        [Column("N_TypeOrder")]
        public int? NTypeOrder { get; set; }
        [Column("X_Description_Ar")]
        [StringLength(50)]
        public string XDescriptionAr { get; set; }
        [Column("N_FormID")]
        public int? NFormId { get; set; }
    }
}
