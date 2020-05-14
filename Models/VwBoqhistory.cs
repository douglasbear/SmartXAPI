﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwBoqhistory
    {
        [Required]
        [Column("X_Code")]
        [StringLength(50)]
        public string XCode { get; set; }
        [Required]
        [Column("X_ShortDesc")]
        [StringLength(200)]
        public string XShortDesc { get; set; }
        [Column("X_LongDesc")]
        [StringLength(2000)]
        public string XLongDesc { get; set; }
        [Column("N_OrderListID")]
        public int NOrderListId { get; set; }
        [Column("X_PartNo")]
        [StringLength(2000)]
        public string XPartNo { get; set; }
        [Column("N_WorkTypeID")]
        public int? NWorkTypeId { get; set; }
        [Column("N_ItemUnitID")]
        public int? NItemUnitId { get; set; }
        [Column("N_Sprice")]
        [StringLength(30)]
        public string NSprice { get; set; }
        [Column("X_FDTNo")]
        [StringLength(2000)]
        public string XFdtno { get; set; }
        [Column("N_Qty")]
        public double? NQty { get; set; }
        [Column("N_Cost")]
        [StringLength(30)]
        public string NCost { get; set; }
        [Column("N_MarginPerc")]
        public double? NMarginPerc { get; set; }
        [Column("N_VendorID")]
        public int? NVendorId { get; set; }
        [Column("N_ReqDays")]
        public int? NReqDays { get; set; }
        [Column("N_WOStatusID")]
        public int? NWostatusId { get; set; }
        [Column("N_Weightage")]
        public int? NWeightage { get; set; }
        [Column("N_ProgressPerc")]
        public int? NProgressPerc { get; set; }
        [Column("X_LocationName")]
        [StringLength(2000)]
        public string XLocationName { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("X_VendorCode")]
        [StringLength(50)]
        public string XVendorCode { get; set; }
        [Column("X_VendorName")]
        [StringLength(100)]
        public string XVendorName { get; set; }
        [Column("X_ItemUnit")]
        [StringLength(500)]
        public string XItemUnit { get; set; }
        [Column("X_PkeyCode")]
        [StringLength(5)]
        public string XPkeyCode { get; set; }
        [Column("X_Name")]
        [StringLength(50)]
        public string XName { get; set; }
        [Column("N_BOQID")]
        public int NBoqid { get; set; }
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("X_TypeName")]
        [StringLength(50)]
        public string XTypeName { get; set; }
        [Column("N_FormID")]
        public int? NFormId { get; set; }
        [Column("N_FormType")]
        public int? NFormType { get; set; }
        [Column("N_ProjectID")]
        public int NProjectId { get; set; }
        [Required]
        [Column("X_ProjectName")]
        [StringLength(50)]
        public string XProjectName { get; set; }
    }
}
