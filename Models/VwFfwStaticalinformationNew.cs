using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwFfwStaticalinformationNew
    {
        [Column("N_CompanyId")]
        public int NCompanyId { get; set; }
        [Column("N_Month")]
        [StringLength(30)]
        public string NMonth { get; set; }
        [Column("N_FnYearId")]
        public int? NFnYearId { get; set; }
        [Column("X_ChgWt", TypeName = "money")]
        public decimal? XChgWt { get; set; }
        [Column("NOOrders")]
        public int? Noorders { get; set; }
        [Column("GPFTE", TypeName = "money")]
        public decimal? Gpfte { get; set; }
        [Column("GPKilo", TypeName = "money")]
        public decimal? Gpkilo { get; set; }
        [Column("GPOrder", TypeName = "money")]
        public decimal? Gporder { get; set; }
        [Column("X_MonthName")]
        [StringLength(30)]
        public string XMonthName { get; set; }
        [Column("N_MonthInt")]
        public int? NMonthInt { get; set; }
    }
}
