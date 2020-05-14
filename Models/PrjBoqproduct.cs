using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Prj_BOQProduct")]
    public partial class PrjBoqproduct
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_BOQID")]
        public int NBoqid { get; set; }
        [Key]
        [Column("N_BOQProductID")]
        public int NBoqproductId { get; set; }
        [Column("X_ProductName")]
        [StringLength(200)]
        public string XProductName { get; set; }
        [Column("N_Qty")]
        public int? NQty { get; set; }
        [Column("N_Cost", TypeName = "money")]
        public decimal? NCost { get; set; }
    }
}
