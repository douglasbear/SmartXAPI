using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("__Mig_BatchStock")]
    public partial class MigBatchStock
    {
        [StringLength(200)]
        public string Batch { get; set; }
        [Column("Drug_Id")]
        [StringLength(300)]
        public string DrugId { get; set; }
        [StringLength(100)]
        public string BatchNo { get; set; }
        [Column("Exp_Date")]
        [StringLength(100)]
        public string ExpDate { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal? Stock { get; set; }
        [StringLength(100)]
        public string Activate { get; set; }
        [StringLength(100)]
        public string Retail { get; set; }
        [Column("cost")]
        [StringLength(100)]
        public string Cost { get; set; }
        [StringLength(100)]
        public string Date { get; set; }
        [StringLength(100)]
        public string User { get; set; }
    }
}
