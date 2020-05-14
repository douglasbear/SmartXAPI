using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvStockImeiRpt
    {
        [Column("X_ItemCode")]
        [StringLength(50)]
        public string XItemCode { get; set; }
        [Column("X_ItemName")]
        [StringLength(600)]
        public string XItemName { get; set; }
        [Column("N_IMEI")]
        [StringLength(50)]
        public string NImei { get; set; }
        [Column("N_Status")]
        public int? NStatus { get; set; }
        [Column("X_Category")]
        [StringLength(100)]
        public string XCategory { get; set; }
        [Column("X_CompanyName")]
        [StringLength(250)]
        public string XCompanyName { get; set; }
        [Column("X_Country")]
        [StringLength(50)]
        public string XCountry { get; set; }
        [Column("N_LPrice", TypeName = "money")]
        public decimal? NLprice { get; set; }
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("X_LocationName")]
        public string XLocationName { get; set; }
        [Column("N_CategoryID")]
        public int NCategoryId { get; set; }
    }
}
