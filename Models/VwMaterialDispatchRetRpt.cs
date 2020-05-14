using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwMaterialDispatchRetRpt
    {
        [Column("X_ItemUnit")]
        [StringLength(500)]
        public string XItemUnit { get; set; }
        [Column("N_ItemID")]
        public int NItemId { get; set; }
        [Column("X_ItemCode")]
        [StringLength(100)]
        public string XItemCode { get; set; }
        [Column("X_ItemName")]
        [StringLength(800)]
        public string XItemName { get; set; }
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("X_ProjectName")]
        [StringLength(50)]
        public string XProjectName { get; set; }
        [Column("N_DispatchReturnDetailsID")]
        public int NDispatchReturnDetailsId { get; set; }
        [Column("N_Qty")]
        public double? NQty { get; set; }
        [Column("N_Cost", TypeName = "money")]
        public decimal? NCost { get; set; }
        [Column("N_DispatchReturnId")]
        public int NDispatchReturnId { get; set; }
        [Column("X_DispatchReturnNo")]
        [StringLength(50)]
        public string XDispatchReturnNo { get; set; }
        [Column("D_ReturnDate", TypeName = "smalldatetime")]
        public DateTime? DReturnDate { get; set; }
        [Column("N_BillAmount", TypeName = "money")]
        public decimal? NBillAmount { get; set; }
        [Column("N_RetQty")]
        public double? NRetQty { get; set; }
        [Column("N_DispatchID")]
        public int? NDispatchId { get; set; }
    }
}
