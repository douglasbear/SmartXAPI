using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("___PSD_ChartOfAccount")]
    public partial class PsdChartOfAccount
    {
        [Column("Account_Code")]
        [StringLength(100)]
        public string AccountCode { get; set; }
        [Column("Account_Name")]
        [StringLength(100)]
        public string AccountName { get; set; }
    }
}
