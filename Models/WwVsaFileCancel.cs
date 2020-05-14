using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class WwVsaFileCancel
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_FileID")]
        public int NFileId { get; set; }
        [Column("N_ItemCode")]
        public int? NItemCode { get; set; }
        [Column("X_Description")]
        [StringLength(100)]
        public string XDescription { get; set; }
        [Column("D_Date", TypeName = "datetime")]
        public DateTime DDate { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal NAmount { get; set; }
        [Column("N_Discount", TypeName = "money")]
        public decimal NDiscount { get; set; }
        [Column("X_FileCode")]
        [StringLength(50)]
        public string XFileCode { get; set; }
        [Column("X_Name")]
        [StringLength(50)]
        public string XName { get; set; }
        [Column("X_FileNo")]
        [StringLength(50)]
        public string XFileNo { get; set; }
        [Column("X_CancelNo")]
        [StringLength(50)]
        public string XCancelNo { get; set; }
        [Column("N_RfdAmt", TypeName = "money")]
        public decimal? NRfdAmt { get; set; }
        [Column("D_PayDate", TypeName = "smalldatetime")]
        public DateTime? DPayDate { get; set; }
        [StringLength(250)]
        public string Expr1 { get; set; }
        [Column("N_PaidAmount", TypeName = "money")]
        public decimal? NPaidAmount { get; set; }
        [Column("N_bal", TypeName = "money")]
        public decimal? NBal { get; set; }
    }
}
