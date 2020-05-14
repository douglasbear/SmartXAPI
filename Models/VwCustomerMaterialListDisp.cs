using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwCustomerMaterialListDisp
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("X_CustomerCode")]
        [StringLength(50)]
        public string XCustomerCode { get; set; }
        [Column("X_CustomerName")]
        [StringLength(100)]
        public string XCustomerName { get; set; }
        [Column("N_CustomerID")]
        public int? NCustomerId { get; set; }
        [Column("X_ItemName")]
        [StringLength(500)]
        public string XItemName { get; set; }
        [Column("X_ItemCode")]
        [StringLength(500)]
        public string XItemCode { get; set; }
        [Column("X_ItemUnit")]
        [StringLength(500)]
        public string XItemUnit { get; set; }
        [Column("N_Received")]
        public double? NReceived { get; set; }
        [Column("N_Returnd")]
        public double? NReturnd { get; set; }
        [Column("N_Balance")]
        public double? NBalance { get; set; }
        [Column("N_Damage")]
        public double? NDamage { get; set; }
        [Column("N_Issue")]
        public double? NIssue { get; set; }
        [Column("N_Transfer")]
        public double? NTransfer { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
    }
}
