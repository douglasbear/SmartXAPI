using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Acc_CashFlowCategory")]
    public partial class AccCashFlowCategory
    {
        [Key]
        [Column("N_CategoryID")]
        public int NCategoryId { get; set; }
        [Column("X_Description")]
        [StringLength(50)]
        public string XDescription { get; set; }
        [Column("N_GenTypeId")]
        public int? NGenTypeId { get; set; }
        [Column("N_SubCategoryID")]
        public int? NSubCategoryId { get; set; }
        [Column("N_Order")]
        public int? NOrder { get; set; }
    }
}
