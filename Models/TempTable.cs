using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("temp_table")]
    public partial class TempTable
    {
        [StringLength(100)]
        public string Col1 { get; set; }
        [StringLength(100)]
        public string Col2 { get; set; }
        [StringLength(100)]
        public string Col3 { get; set; }
        [StringLength(100)]
        public string Col4 { get; set; }
        [StringLength(100)]
        public string Col5 { get; set; }
        [StringLength(100)]
        public string Col6 { get; set; }
        [StringLength(100)]
        public string Col7 { get; set; }
        [StringLength(100)]
        public string Col8 { get; set; }
        [StringLength(50)]
        public string Col9 { get; set; }
    }
}
