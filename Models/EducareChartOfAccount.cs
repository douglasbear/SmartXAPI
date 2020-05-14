using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("___Educare_ChartOfAccount")]
    public partial class EducareChartOfAccount
    {
        [Column("Account_Code")]
        [StringLength(100)]
        public string AccountCode { get; set; }
        [Column("Account_Name")]
        [StringLength(100)]
        public string AccountName { get; set; }
        [Column("Main_Sub")]
        [StringLength(100)]
        public string MainSub { get; set; }
        [StringLength(100)]
        public string Level { get; set; }
        [Column("Account_Type")]
        [StringLength(100)]
        public string AccountType { get; set; }
        [Column("Final_Report")]
        [StringLength(100)]
        public string FinalReport { get; set; }
        [Column("Debit_Credit")]
        [StringLength(100)]
        public string DebitCredit { get; set; }
        [StringLength(100)]
        public string Currency { get; set; }
    }
}
