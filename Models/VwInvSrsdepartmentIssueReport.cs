using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvSrsdepartmentIssueReport
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("X_PRSNo")]
        [StringLength(50)]
        public string XPrsno { get; set; }
        [Column("X_ItemCode")]
        [StringLength(50)]
        public string XItemCode { get; set; }
        [Column("X_ItemName")]
        [StringLength(600)]
        public string XItemName { get; set; }
        [Column("X_ItemUnit")]
        [StringLength(500)]
        public string XItemUnit { get; set; }
        [Column("X_Department")]
        public string XDepartment { get; set; }
        [Column("D_TransferDate", TypeName = "smalldatetime")]
        public DateTime? DTransferDate { get; set; }
        [Column("N_Qty")]
        public double? NQty { get; set; }
        [Column("X_TransName")]
        [StringLength(50)]
        public string XTransName { get; set; }
        [Column("N_Cost")]
        public double? NCost { get; set; }
        [Column("N_DepartmentID")]
        public int? NDepartmentId { get; set; }
    }
}
