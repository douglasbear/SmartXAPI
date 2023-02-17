// using System;
// using System.Collections.Generic;
// using System.ComponentModel;
// using System.Data;
// using System.Drawing;
// using System.Text;
// using System.Windows.Forms;  using OlivoDataAccess; using OlivoGlobal;
// using System.Net.Mail;
// using System.IO;
// using Microsoft.Win32;
// namespace SmartAccounting
// {
//     public partial class Pay_TimeSheetVerify : Form
//     {
//         myMessage msg = new myMessage(); GeneralFunctions MYG = new GeneralFunctions(myCompanyID._dsPayroll, myCompanyID._RightToLeft);
//         DBAccess dba = new DBAccess();
//         ToolTip ts = new ToolTip();

//         DataSet dsItemCategory = new DataSet();
//         DataSet dsTimesheetDetails = new DataSet();
//         DataSet dsMail = new DataSet();
//         DataSet dsCategory = new DataSet();
//         DataSet dsTrans = new DataSet();

//         DataSet dsMaster = new DataSet();

//         DateTime D_In2Casual;
//         int N_PayID = 0, N_Navigation = -1, N_EmpID = 0, N_TimeSheetId = 0, N_FormId = 0;
//         double N_Payrate = 0, N_MinHours = 0;
//         string X_GridPrevVal = "", X_GlobalVal = "";
//         bool B_EscapePressed = false, B_UcLeaveCalled = false, B_ClosedFlag = false;
//         const int mcSlNo = 0, McDis_Date = 1, mcIn = 2, mcOut = 3, mcIn2 = 4, mcOut2 = 5, mcTotHrs = 6, mcDutyHours = 7, mcDiff = 8, mcOt = 9, mcDeduct = 10, mcBrkHr = 11, mcCompDeduct = 12, mcType = 13, mcAttendence = 14, mcRemarks = 15, mcApproved = 16, mcSheetID = 17, mcDate = 18, mcPayID = 19, mcIsVacation = 20, mcWorkingHrs = 21, mcIncHoliday = 22;
//         bool B_DoubleEntry = false;
//         string X_PrevValue = "";
//         double N_Diffrence = 0;
//         double N_TotalDays = 0;
//         bool B_AutoInvoice = false;
//         int N_BranchID = 0;
//         bool B_SaveChanges = false;
//         bool B_CompansateTime = false;
//         bool B_MonthlyaddordedProcess = false;
//         bool B_ManualEntry_InGrid = false;
//         bool B_CategoryWiseAddition = false;
//         bool B_CategoryWiseDeduction = false;
//         bool B_CategoryWiseComp = false;
//         double N_WorkHours = 0, N_WorkdHrs = 0;
//         double Netdays = 0;
//         double N_NonDedApp = 0;
//         string X_Additions = "", X_Deductions = "", X_DefaultAbsentCode = "";
//         int N_AdditionPayID = 0, N_DeductionPayID, N_AdjTypeID = 0, N_DefaultAbsentID = 0;
//         int N_TimeSheetID = 0;
//         int N_BatchID = 0;
//         int N_CatagoryId = 0;
//         double CompansateTime = 0, CompansateaddlTime = 0, NetCompansateaddlTime = 0, NetCompansatededTime = 0, N_totalDedution = 0;

//         int N_EntryUserID = 0, N_ProcStatus = 0, N_ApprovalLevelID = 0, N_SaveDraft = 0;
//         int N_IsApprovalSystem = -1;
//         int N_NextApprovalLevel = 0,IsEditable=1;
//         string X_TransType = "Timesheet Approval", X_Action = "", X_Code = "";


//         public string _BatchCode
//         {
//             get { return X_Code; }
//             set { X_Code = value; }
//         }

//         public Pay_TimeSheetVerify()
//         {
//             InitializeComponent();
//         }

//         private void MsgDisplay(string Action, string Descr)
//         {
//             lblResult.Visible = true;
//             lblResultDescr.Visible = true;
//             lblResult.Text = Action;
//             lblResultDescr.Text = Descr;
//         }
//         private void FillData()
//         {
//             // Collecting Data
//             if (dsItemCategory.Tables.Contains("Pay_PayMaster"))
//                 dsItemCategory.Tables.Remove("Pay_PayMaster");
//             string Sql = "Select * from Pay_PayMaster Where Pay_PayMaster.N_CompanyID =" + myCompanyID._CompanyID;

//             dba.FillDataSet(ref dsItemCategory, "Pay_PayMaster", Sql, "TEXT", new DataTable());

//             Sql = "Select * from Pay_PayType Order By N_PayTypeID";
//             dba.FillDataSet(ref dsItemCategory, "Pay_PayType", Sql, "TEXT", new DataTable());
//         }

//         private void ViewData()
//         {
//             string PayrunID = dtpPayrunText.Value.Year.ToString("00##") + dtpPayrunText.Value.Month.ToString("0#");
//             object obj = dba.ExecuteSclar("Select isnull(Count(X_BatchCode),0) from Pay_TimeSheetMaster where N_CompanyID=" + myCompanyID._CompanyID + " and N_FnYearID=" + myCompanyID._FnYearID + " and N_EmpID=" + N_EmpID.ToString() + " and N_BatchID=" + PayrunID, "TEXT", new DataTable());
//             //object N_AddorDedID = dba.ExecuteSclar("Select ISNULL(N_AddorDedID,0) from Pay_TimeSheetMaster where N_CompanyID=" + myCompanyID._CompanyID + " and N_FnYearID=" + myCompanyID._FnYearID + " and N_EmpID=" + N_EmpID.ToString() + " and N_BatchID=" + PayrunID, "TEXT", new DataTable());
//             //if (N_AddorDedID != null)
//             //{
//             //    if (myFunctions.getIntVAL(N_AddorDedID.ToString()) > 0)
//             //    {
//             //        object TransDetailID = dba.ExecuteSclar("SELECT isnull(Count(Pay_MonthlyAddOrDedDetails.N_TransDetailsID),0) FROM Pay_MonthlyAddOrDedDetails INNER JOIN Pay_MonthlyAddOrDed ON Pay_MonthlyAddOrDedDetails.N_CompanyID = Pay_MonthlyAddOrDed.N_CompanyID AND Pay_MonthlyAddOrDedDetails.N_TransID = Pay_MonthlyAddOrDed.N_TransID WHERE Pay_MonthlyAddOrDed.N_CompanyID=" + myCompanyID._CompanyID + "  AND Pay_MonthlyAddOrDedDetails.N_EmpID =" + N_EmpID.ToString() + "  AND Pay_MonthlyAddOrDed.N_PayRunID =" + PayrunID + " AND Pay_MonthlyAddOrDed.N_TransID=" + N_AddorDedID.ToString(), "TEXT", new DataTable());
//             //        if (myFunctions.getIntVAL(TransDetailID.ToString()) == 0)
//             //            obj = 0;
//             //    }
//             //}
//             if (obj != null)
//             {
//                 if (myFunctions.getIntVAL(obj.ToString()) > 0)
//                 {
//                     N_BatchID = myFunctions.getIntVAL(PayrunID);
//                     ViewDetails();
//                 }
//                 else
//                     Get_Attendence();
//             }
//         }

//         private void Navigation(int position)
//         {
//             // 1 : First, 2 : Next, 3 : Prev, 4 : Last
//             if (!dsItemCategory.Tables.Contains("Pay_PayMaster"))
//                 FillData();

//             if (dsItemCategory.Tables["Pay_PayMaster"].Rows.Count <= 0)
//                 return;

//             // Row Index
//             int rowcount = dsItemCategory.Tables["Pay_PayMaster"].Rows.Count - 1;
//             switch (position)
//             {
//                 case 1:
//                     N_Navigation = 0;
//                     break;
//                 case 2:
//                     N_Navigation += 1;
//                     if (N_Navigation > rowcount)
//                         N_Navigation = 0;
//                     break;
//                 case 3:
//                     N_Navigation -= 1;
//                     if (N_Navigation < 0)
//                         N_Navigation = rowcount;
//                     break;
//                 default:
//                     N_Navigation = rowcount;
//                     break;
//             }

//             // Display Data

//             DataRow drow = dsItemCategory.Tables["Pay_PayMaster"].Rows[N_Navigation];
//             DisplayData(drow);

//         }
//         private void DisplayData(DataRow drow)
//         {
//             ////btnGridAccount.Visible = false;
//             ////txtGridTextBox.Visible = false;
//             ////txtGridTextBox.Text = "";
//             //btnSave.Enabled = true;
//             //btnDelete.Enabled = true;
//             //if (!MYG.HavePermission(this.Text, "Edit", lblResult, lblResultDescr))
//             //{
//             //    btnSave.Enabled = false;
//             //}
//             //if (!MYG.HavePermission(this.Text, "Delete", lblResult, lblResultDescr))
//             //{
//             //    btnDelete.Enabled = false;
//             //}
//             //if (!btnSave.Enabled && !btnDelete.Enabled)
//             //    MYG.ResultMessage(lblResult, lblResultDescr, MYG.ReturnMultiLingualVal("-1111", "X_ControlNo", "Alert"), MYG.ReturnMultiLingualVal("-1111", "X_ControlNo", "EditDeleteAlert"));

//             //N_PayID = myFunctions.getIntVAL(drow["N_PayID"].ToString());
//             //txtClassName.Text = drow["X_Description"].ToString();
//             //txtClassnameLocale.Text = drow["X_DescriptionLocale"].ToString();
//             //txtCode.Text = drow["X_Paycode"].ToString();
//             //txtPayRate.Text = myFunctions.getVAL(drow["N_Value"].ToString()).ToString(myCompanyID.DecimalPlaceString);
//             //cmbPayType.SelectedIndex = myFunctions.getIntVAL(drow["N_PayType"].ToString());
//             //cmbPayMethod.SelectedIndex = myFunctions.getIntVAL(drow["N_PayMethod"].ToString());
//             //cmbConfigLevel.SelectedIndex = myFunctions.getIntVAL(drow["N_ConfigLevel"].ToString());

//             //if (myFunctions.getVAL(dba.ExecuteSclarNoErrorCatch("Select Count(N_TransDetailsID) from Pay_PaymentDetails Where n_CompanyID= " + myCompanyID._CompanyID + " and N_PayID = " + N_PayID.ToString(), "TEXT", new DataTable()).ToString()) > 0)
//             //{
//             //    cmbPayType.Enabled = false;
//             //    ts.SetToolTip(label4, "Pay Type not possible to edit. This Pay Element has been used in transactions");
//             //}
//             //else
//             //{
//             //    cmbPayType.Enabled = true;
//             //    ts.SetToolTip(label4, "");
//             //}

//             //if (dsItemCategory.Tables.Contains("Pay_SummaryPercentage"))
//             //    dsItemCategory.Tables.Remove("Pay_SummaryPercentage");
//             //dba.FillDataSet(ref dsItemCategory, "Pay_SummaryPercentage", "SELECT    * From Pay_SummaryPercentage inner join Pay_PayType on Pay_SummaryPercentage.N_PayTypeID = Pay_PayType.N_PayTypeID and Pay_SummaryPercentage.N_CompanyID = Pay_PayType.N_CompanyID  Where Pay_SummaryPercentage.N_PayID =" + N_PayID + " and Pay_SummaryPercentage.N_CompanyID=" + myCompanyID._CompanyID, "TEXT", new DataTable());

//             //if (!dsItemCategory.Tables.Contains("Pay_SummaryPercentage")) return;
//             //foreach (DataRow drowSummary in dsItemCategory.Tables["Pay_SummaryPercentage"].Rows)
//             //{
//             //    lstSummaryList.SetItemChecked(lstSummaryList.Items.IndexOf(drowSummary["X_PayType"]), true);
//             //}

//             //if (dsItemCategory.Tables.Contains("Pay_PayFormulae"))
//             //    dsItemCategory.Tables.Remove("Pay_PayFormulae");
//             //dba.FillDataSet(ref dsItemCategory, "Pay_PayFormulae", "SELECT    Pay_PayFormulae.*, Pay_Paymaster.X_Paycode,Pay_Paymaster.X_Description From Pay_PayFormulae inner join Pay_Paymaster on Pay_PayFormulae.N_PayItemID = Pay_Paymaster.N_PayID  Where Pay_PayFormulae.N_PayID =" + N_PayID + " and Pay_PayFormulae.N_CompanyID=" + myCompanyID._CompanyID, "TEXT", new DataTable());

//             //if (!dsItemCategory.Tables.Contains("Pay_PayFormulae")) return;
//             //int SLNo = 1;
//             //flxFeeDetails.Rows = SLNo;
//             //foreach (DataRow drowFee in dsItemCategory.Tables["Pay_PayFormulae"].Rows)
//             //{
//             //    flxFeeDetails.Rows = SLNo + 1;
//             //    flxFeeDetails.set_TextMatrix(SLNo, mcSrl, SLNo.ToString());
//             //    flxFeeDetails.set_TextMatrix(SLNo, mcValue, drowFee["N_Percentage"].ToString());
//             //    flxFeeDetails.set_TextMatrix(SLNo, mcPayElementID, drowFee["N_PayItemID"].ToString());
//             //    flxFeeDetails.set_TextMatrix(SLNo, mcPayElement, drowFee["X_Description"].ToString());
//             //    flxFeeDetails.set_TextMatrix(SLNo, mcPayFormulaeID, drowFee["N_FormulaeID"].ToString());
//             //    flxFeeDetails.set_TextMatrix(SLNo, mcOperator, drowFee["X_Operator"].ToString());
//             //    SLNo += 1;
//             //}
//             //flxFeeDetails.Rows = SLNo + 1;
//             //flxFeeDetails.Row = SLNo;
//             //flxFeeDetails.Col = mcPayElement;
//         }
//         private bool Validation(int Action)
//         {
//             // 1: Save/Edit, 2 : Delete

//             if (Action == 1)
//             {
//                 //if (MYG.check4Null(txtCode, MYG.ReturnMultiLingualVal("-1111", "X_ControlNo", "InvalidEntry")))
//                 //    return false;
//                 //if (MYG.check4Null(txtClassName, MYG.ReturnMultiLingualVal("-1111", "X_ControlNo", "InvalidEntry")))
//                 //    return false;
//                 //if (MYG.check4Null(cmbPayType, MYG.ReturnMultiLingualVal("-1111", "X_ControlNo", "InvalidEntry")))
//                 //    return false;
//                 //if (MYG.check4Null(cmbPayMethod, MYG.ReturnMultiLingualVal("-1111", "X_ControlNo", "InvalidEntry")))
//                 //    return false;
//                 return true;
//             }
//             else
//             {
//                 //if (txtCode.Text == "@Auto") { msg.msgInformation(MYG.ReturnMultiLingualVal("-1111", "X_ControlNo", "InvalidEntry")); txtCode.Focus(); return false; }
//                 //if (MYG.check4Null(txtCode, MYG.ReturnMultiLingualVal("-1111", "X_ControlNo", "InvalidEntry")))
//                 //    return false;
//                 return true;
//             }
//         }
//         private void SetFields()
//         {


//             //flxFeeDetails.set_ColWidth(mcPayFormulaeID, 0);
//         }




//         private void flxTimesheetVerify_SizeChanged(object sender, EventArgs e)
//         {
//             int N_TotalWidth = (flxTimesheetVerify.Width * 15) - 360;
//             flxTimesheetVerify.set_ColWidth(mcSlNo, 0);
//             flxTimesheetVerify.set_ColWidth(mcSheetID, 0);
//             flxTimesheetVerify.set_ColWidth(mcPayID, 0);
//             flxTimesheetVerify.set_ColWidth(mcDate, 0);
//             flxTimesheetVerify.set_ColWidth(mcApproved, 0);
//             flxTimesheetVerify.set_ColWidth(mcIsVacation, 0);
//             flxTimesheetVerify.set_ColWidth(mcWorkingHrs, 0);
//             flxTimesheetVerify.set_ColWidth(mcDiff, 0);
//             flxTimesheetVerify.set_ColWidth(mcIncHoliday, 0);

//             flxTimesheetVerify.set_ColWidth(mcIn, 0);
//             flxTimesheetVerify.set_ColWidth(mcOut, 0);
//             flxTimesheetVerify.set_ColWidth(mcIn2, 0);
//             flxTimesheetVerify.set_ColWidth(mcOut2, 0);
//             flxTimesheetVerify.set_ColWidth(mcTotHrs, 0);
//             flxTimesheetVerify.set_ColWidth(mcOt, 0);
//             flxTimesheetVerify.set_ColWidth(mcDeduct, 0);
//             flxTimesheetVerify.set_ColWidth(mcCompDeduct, 0);
//             flxTimesheetVerify.set_ColWidth(mcType, 0);
//             flxTimesheetVerify.set_ColWidth(mcAttendence, 0);
//             flxTimesheetVerify.set_ColWidth(mcRemarks, 0);
//             flxTimesheetVerify.set_ColWidth(mcDutyHours, 0);

//             flxTimesheetVerify.set_ColWidth(McDis_Date, N_TotalWidth * 11 / 100);
//             if (B_CompansateTime)
//                 flxTimesheetVerify.set_ColWidth(mcCompDeduct, N_TotalWidth * 7 / 100);
//             else
//                 flxTimesheetVerify.set_ColWidth(mcCompDeduct, 0);

//             if (B_DoubleEntry)
//             {
//                 flxTimesheetVerify.set_ColWidth(mcIn, N_TotalWidth * 7 / 100);
//                 flxTimesheetVerify.set_ColWidth(mcOut, N_TotalWidth * 7 / 100);
//                 flxTimesheetVerify.set_ColWidth(mcIn2, N_TotalWidth * 7 / 100);
//                 flxTimesheetVerify.set_ColWidth(mcOut2, N_TotalWidth * 7 / 100);
//             }
//             else
//             {
//                 flxTimesheetVerify.set_ColWidth(mcIn, N_TotalWidth * 14 / 100);
//                 flxTimesheetVerify.set_ColWidth(mcOut, N_TotalWidth * 16 / 100);
//                 flxTimesheetVerify.set_ColWidth(mcIn2, 0);
//                 flxTimesheetVerify.set_ColWidth(mcOut2, 0);
//             }
//             flxTimesheetVerify.set_ColWidth(mcTotHrs, N_TotalWidth * 6 / 100);

//             flxTimesheetVerify.set_ColWidth(mcOt, N_TotalWidth * 5 / 100);
//             flxTimesheetVerify.set_ColWidth(mcDeduct, N_TotalWidth * 5 / 100);
//             flxTimesheetVerify.set_ColWidth(mcType, N_TotalWidth * 11 / 100);
//             flxTimesheetVerify.set_ColWidth(mcAttendence, N_TotalWidth * 7 / 100);
//             flxTimesheetVerify.set_ColWidth(mcRemarks, N_TotalWidth * 6 / 100);
//             flxTimesheetVerify.set_ColWidth(mcBrkHr, N_TotalWidth * 5 / 100);


//             myFunctions.ResetGrid(flxTimesheetVerify);

//         }




//         private void Pay_TimeSheetVerify_Load(object sender, EventArgs e)
//         {
//             //---- MenUID = 216

//             N_FormId = 216;
//             Cursor.Current = Cursors.WaitCursor;
//             ts.IsBalloon = true;
//             MYG.MultiLingual(this, "216");
//             get_Offdays();
//             get_Workingdays();
//             checkperiod();
//             if (!MYG.HavePermission(this.Text, "Save"))
//                 btnSave.Enabled = false;
//             CompansateTime = myFunctions.getVAL(MYG.ReturnSettings(myCompanyID._CompanyID.ToString(), "HR", "Compansate_Minutes", "N_Value")) / 100;
//             CompansateaddlTime = myFunctions.getVAL(MYG.ReturnSettings(myCompanyID._CompanyID.ToString(), "HR", "Compansate_addlMinutes", "N_Value")) / 100;
//             NetCompansateaddlTime = myFunctions.getVAL(MYG.ReturnSettings(myCompanyID._CompanyID.ToString(), "HR", "NetCompansate_addlMinutes", "N_Value")) / 100;
//             NetCompansatededTime = myFunctions.getVAL(MYG.ReturnSettings(myCompanyID._CompanyID.ToString(), "HR", "NetCompansate_dedMinutes", "N_Value")) / 100;
//             if (CompansateTime > 0)
//                 B_CompansateTime = true;
//             if (B_CompansateTime)
//             {
//                 lblCompnDed.Visible = false;
//                 lblTotlCmpnDed.Visible = false;
//             }
//             else
//             {
//                 lblCompnDed.Visible = false;
//                 lblTotlCmpnDed.Visible = false;
//             }
//             dtpPayrunText.Value = myFunctions.GetFormatedDate(myCompanyID._SystemDate);
//             DefaultPayCodes();
//             flxTimesheetVerify_SizeChanged(sender, e);
//             Set_GridSize();
//             setStatus();

//             B_AutoInvoice = Convert.ToBoolean(dba.ExecuteSclar("Select Isnull(B_AutoInvoiceEnabled,0) from Inv_InvoiceCounter where N_MenuID=" + MYG.ReturnFormID(this.Text) + " and N_CompanyID=" + myCompanyID._CompanyID + " and N_FnYearID=" + myCompanyID._FnYearID, "TEXT", new DataTable()));
//             if (B_AutoInvoice)
//                 txtBatchID.Text = "@Auto";
//             else
//                 txtBatchID.Text = "";


//             txtEmpCode.Focus();
//             ToolTipSettings();
//             lbl_EmpDetails.Text = "";

//             myFunctions.SetApprovalSystem(ref N_IsApprovalSystem, myFunctions.getIntVAL(MYG.ReturnFormID(this.Text)), N_TimeSheetID, N_EntryUserID, N_ProcStatus, N_ApprovalLevelID, ref N_NextApprovalLevel, btnSave, btnDelete, lblmsg, lblResultDescr, 0, 1, ref IsEditable);

//             Cursor.Current = Cursors.Default;
//         }
//         private void CategorywiseSettings(int empid, int groupid)
//         {
//             if (dsCategory.Tables.Contains("EmpGroup"))
//                 dsCategory.Tables.Remove("EmpGroup");
//             string Sql = "";
//             Sql = "Select B_Addition,B_Deduction,B_Compensation from Pay_EmployeeGroup where N_CompanyID=" + myCompanyID._CompanyID + " and N_PkeyId=" + groupid;
//             dba.FillDataSet(ref dsCategory, "EmpGroup", Sql, "TEXT", new DataTable());
//             if (dsCategory.Tables["EmpGroup"].Rows.Count == 0)
//             {
//                 B_CategoryWiseAddition = true;
//                 B_CategoryWiseDeduction = true;
//                 B_CategoryWiseComp = false;
//             }
//             else
//             {
//                 B_CategoryWiseAddition = myFunctions.getBoolVAL(dsCategory.Tables["EmpGroup"].Rows[0]["B_Addition"].ToString());
//                 B_CategoryWiseDeduction = myFunctions.getBoolVAL(dsCategory.Tables["EmpGroup"].Rows[0]["B_Deduction"].ToString());
//                 B_CategoryWiseComp = myFunctions.getBoolVAL(dsCategory.Tables["EmpGroup"].Rows[0]["B_Compensation"].ToString());
//             }
//         }

//         private void setStatus()
//         {
//             if (B_MonthlyaddordedProcess)
//             {
//                 if (!B_CompansateTime)
//                 {

//                     chkSProcessType.Checked = true;
//                     chkSProcessType.Visible = false;
//                     Btnadjpaycode.Visible = true;
//                     txtAdjpaycode.Visible = true;
//                     lbladjustment.Visible = true;
//                     txtAdjustment.Visible = true;
//                     lblBalance.Visible = false;
//                     lblBalName.Visible = false;
//                 }
//                 else
//                 {
//                     chkSProcessType.Checked = false;
//                     chkSProcessType.Visible = false;
//                     Btnadjpaycode.Visible = false;
//                     txtAdjpaycode.Visible = false;
//                     lbladjustment.Visible = false;
//                     txtAdjustment.Visible = false;
//                     lblBalance.Visible = false;
//                     lblBalName.Visible = false;
//                 }
//             }
//             else
//             {
//                 chkSProcessType.Checked = false;
//                 chkSProcessType.Visible = false;
//                 Btnadjpaycode.Visible = false;
//                 txtAdjpaycode.Visible = false;
//                 lbladjustment.Visible = false;
//                 txtAdjustment.Visible = false;
//                 lblBalance.Visible = false;
//                 lblBalName.Visible = false;
//             }
//         }
//         private void Set_GridSize()
//         {
//             string GridSize = "";
//             try
//             {
//                 RegistryKey reg;
//                 reg = Registry.CurrentUser.OpenSubKey("SOFTWARE\\" + myCompanyID._Project + dba.RegSubFolder, true);
//                 GridSize = reg.GetValue("HR_" + this.Text + "_grid", "").ToString();
//                 reg.Close();

//                 if (GridSize != "")
//                 {
//                     string[] temp = GridSize.Split(',');
//                     for (int i = 0; i < flxTimesheetVerify.Cols; i++)
//                     {
//                         if (myFunctions.getIntVAL(temp[i]) != 0 && flxTimesheetVerify.get_ColWidth(i) != 0)
//                             flxTimesheetVerify.set_ColWidth(i, myFunctions.getIntVAL(temp[i]));
//                     }
//                     return;
//                 }

//             }
//             catch (Exception ex)
//             {
//                 int N_TotalWidth = (flxTimesheetVerify.Width * 15) - 360;
//                 flxTimesheetVerify.set_ColWidth(mcSlNo, 0);
//                 flxTimesheetVerify.set_ColWidth(mcSheetID, 0);
//                 flxTimesheetVerify.set_ColWidth(mcPayID, 0);
//                 flxTimesheetVerify.set_ColWidth(mcDate, 0);
//                 flxTimesheetVerify.set_ColWidth(mcDiff, 0);
//                 flxTimesheetVerify.set_ColWidth(McDis_Date, N_TotalWidth * 11 / 100);
//                 flxTimesheetVerify.set_ColWidth(mcIn, N_TotalWidth * 7 / 100);
//                 flxTimesheetVerify.set_ColWidth(mcOut, N_TotalWidth * 8 / 100);
//                 flxTimesheetVerify.set_ColWidth(mcIn2, N_TotalWidth * 7 / 100);
//                 flxTimesheetVerify.set_ColWidth(mcOut2, N_TotalWidth * 8 / 100);
//                 flxTimesheetVerify.set_ColWidth(mcTotHrs, N_TotalWidth * 5 / 100);
//                 flxTimesheetVerify.set_ColWidth(mcDutyHours, 0);
//                 flxTimesheetVerify.set_ColWidth(mcOt, N_TotalWidth * 8 / 100);
//                 flxTimesheetVerify.set_ColWidth(mcDeduct, N_TotalWidth * 7 / 100);
//                 flxTimesheetVerify.set_ColWidth(mcType, N_TotalWidth * 12 / 100);
//                 flxTimesheetVerify.set_ColWidth(mcAttendence, N_TotalWidth * 5 / 100);
//                 flxTimesheetVerify.set_ColWidth(mcRemarks, N_TotalWidth * 24 / 100);
//                 flxTimesheetVerify.set_ColWidth(mcApproved, 0);
//                 flxTimesheetVerify.set_ColWidth(mcIsVacation, 0);
//                 flxTimesheetVerify.set_ColWidth(mcWorkingHrs, 0);
//                 flxTimesheetVerify.set_ColWidth(mcIncHoliday, 0);
//             }
//         }
//         private void ToolTipSettings()
//         {
//             // Create the ToolTip and associate with the Form container.
//             ToolTip toolTip1 = new ToolTip();

//             // Set up the delays for the ToolTip.
//             toolTip1.AutoPopDelay = 5000;
//             toolTip1.InitialDelay = 100;
//             toolTip1.ReshowDelay = 500;
//             // Force the ToolTip text to be displayed whether or not the form is active.
//             toolTip1.ShowAlways = true;

//             // Set up the ToolTip text for the Button and Checkbox.

//         }

//         public void DefaultPayCodes()
//         {
//             object N_Result;

//             N_Result = dba.ExecuteSclarNoErrorCatch("Select N_Value from Gen_Settings Where X_Description ='Default Addition' and N_CompanyID= " + myCompanyID._CompanyID + " and X_Group='HR'", "TEXT", new DataTable());

//             if (N_Result != null)
//             {
//                 if (myFunctions.getIntVAL(N_Result.ToString()) != 0)
//                 {
//                     N_AdditionPayID = myFunctions.getIntVAL(N_Result.ToString());
//                     object additions = dba.ExecuteSclarNoErrorCatch("Select X_Description from Pay_PayMaster Where N_PayID =" + N_AdditionPayID + "and N_CompanyID= " + myCompanyID._CompanyID + " and N_FnYearID=" + myCompanyID._FnYearID + "", "TEXT", new DataTable());
//                     if (additions != null)
//                         X_Additions = additions.ToString();
//                 }
//             }
//             else
//                 X_Additions = "";


//             N_Result = dba.ExecuteSclarNoErrorCatch("Select N_Value from Gen_Settings Where X_Description ='Default Deduction' and N_CompanyID= " + myCompanyID._CompanyID + " and X_Group='HR'", "TEXT", new DataTable());
//             if (N_Result != null)
//             {
//                 if (myFunctions.getIntVAL(N_Result.ToString()) != 0)
//                 {
//                     N_DeductionPayID = myFunctions.getIntVAL(N_Result.ToString());
//                     object deductions = dba.ExecuteSclarNoErrorCatch("Select X_Description from Pay_PayMaster Where N_PayID =" + N_DeductionPayID + "and N_CompanyID= " + myCompanyID._CompanyID + " and N_FnYearID=" + myCompanyID._FnYearID + "", "TEXT", new DataTable());
//                     if (deductions != null)
//                         X_Deductions = deductions.ToString();
//                 }
//             }
//             else

//                 X_Deductions = "";



//             N_Result = dba.ExecuteSclarNoErrorCatch("Select N_Value from Gen_Settings Where X_Description ='Default AbsentType' and N_CompanyID= " + myCompanyID._CompanyID + " and X_Group='HR'", "TEXT", new DataTable());
//             if (N_Result != null)
//             {
//                 if (myFunctions.getIntVAL(N_Result.ToString()) != 0)
//                 {
//                     N_DefaultAbsentID = myFunctions.getIntVAL(N_Result.ToString());
//                     object AbsentCode = dba.ExecuteSclarNoErrorCatch("Select X_VacType from Pay_VacationType Where N_VacTypeID =" + N_DefaultAbsentID + "and N_CompanyID= " + myCompanyID._CompanyID, "TEXT", new DataTable());
//                     if (AbsentCode != null)
//                         X_DefaultAbsentCode = AbsentCode.ToString();

//                 }
//             }
//             else

//                 X_DefaultAbsentCode = "";
//             B_MonthlyaddordedProcess = Convert.ToBoolean(dba.ExecuteSclarNoErrorCatch("Select N_Value from Gen_Settings Where X_Description ='Salary Process' and N_CompanyID= " + myCompanyID._CompanyID + " and X_Group='HR'", "TEXT", new DataTable()));
//             B_ManualEntry_InGrid = Convert.ToBoolean(dba.ExecuteSclarNoErrorCatch("Select N_Value from Gen_Settings Where X_Description ='ManualEntryInGrid' and N_CompanyID= " + myCompanyID._CompanyID + " and X_Group='HR'", "TEXT", new DataTable()));
//             B_DoubleEntry = Convert.ToBoolean(dba.ExecuteSclarNoErrorCatch("Select N_Value from Gen_Settings Where X_Description ='DoubleShiftEntry' and N_CompanyID= " + myCompanyID._CompanyID + " and X_Group='HR'", "TEXT", new DataTable()));
//         }

//         public void checkperiod()
//         {
//             // object Periodvalue = myFunctions.getIntVAL(MYG.ReturnSettings(myCompanyID._CompanyID.ToString(), "Payroll", "Period Settings", "N_Value"));
//             object PeriodType = dba.ExecuteSclarNoErrorCatch("Select X_Value from Gen_Settings Where X_Description ='Period Settings' and N_CompanyID= " + myCompanyID._CompanyID + " and X_Group='Payroll'", "TEXT", new DataTable()); 
//             object Periodvalue = dba.ExecuteSclarNoErrorCatch("Select N_Value from Gen_Settings Where X_Description ='Period Settings' and N_CompanyID= " + myCompanyID._CompanyID + " and X_Group='Payroll'", "TEXT", new DataTable()); 
//             if (Periodvalue == null) return;
//             if (PeriodType != null && PeriodType.ToString() == "M")
//             {
//                 DateTime dtStartDate = new DateTime(dtpPayrunText.Value.Year, dtpPayrunText.Value.Month,1);
//                 int days = DateTime.DaysInMonth(dtpPayrunText.Value.Year, dtpPayrunText.Value.Month)  - myFunctions.getIntVAL(Periodvalue.ToString());
//                 dtptodate.Value = dtStartDate.AddDays( myFunctions.getIntVAL(Periodvalue.ToString()) - 2);
//                 int lastdays = myFunctions.getIntVAL(Periodvalue.ToString());
//                 dtpfromdate.Value = dtStartDate.AddMonths(-1).AddDays(lastdays-1);
//             }
//             else
//             {
//                 DateTime dtStartDate = new DateTime(dtpPayrunText.Value.Year, dtpPayrunText.Value.Month, 1);
//                 int days = DateTime.DaysInMonth(dtpPayrunText.Value.Year, dtpPayrunText.Value.Month) - myFunctions.getIntVAL(Periodvalue.ToString());
//                 dtptodate.Value = dtStartDate.AddDays(myFunctions.getIntVAL(days.ToString()) - 1);
//                 int lastdays = myFunctions.getIntVAL(Periodvalue.ToString());
//                 dtpfromdate.Value = dtStartDate.AddDays(-lastdays);
//             }//  dtptodate.Value.AddMonths(-1).AddDays(1);
//         }

//         private void btnClose_Click(object sender, EventArgs e)
//         {
//             this.Close();
//         }

//         private void Pay_TimeSheetVerify_FormClosed(object sender, FormClosedEventArgs e)
//         {
//             B_ClosedFlag = true;
//             myFunctions.RemoveOpenedForms(this.Text);
//         }

//         //--- Attendence Import
//         private void get_Offdays()
//         {
//             if (dsItemCategory.Tables.Contains("pay_Offdays"))
//                 dsItemCategory.Tables.Remove("Pay_OffDays");
//             string Sql = "Select * from vw_pay_OffDays Where N_CompanyID =" + myCompanyID._CompanyID + " and (N_FNyearID= " + myCompanyID._FnYearID + " or N_FNyearID=0)  ";

//             dba.FillDataSet(ref dsItemCategory, "Pay_OffDays", Sql, "TEXT", new DataTable());


//         }

//         private void get_Workingdays()
//         {
//             if (dsItemCategory.Tables.Contains("Pay_WorkingHours"))
//                 dsItemCategory.Tables.Remove("Pay_WorkingHours");
//             string Sql = "Select * from vw_pay_WorkingHours Where N_CompanyID =" + myCompanyID._CompanyID;

//             dba.FillDataSet(ref dsItemCategory, "Pay_WorkingHours", Sql, "TEXT", new DataTable());
//         }

//         private void get_Payrate()
//         {
//             if (dsItemCategory.Tables.Contains("Pay_Payrate"))
//                 dsItemCategory.Tables.Remove("Pay_Payrate");
//             string Sql = "SP_Pay_SelAddOrDed_Emp " + myCompanyID._CompanyID + "," + dtpPayrunText.Value.Year.ToString("00##") + dtpPayrunText.Value.Month.ToString("0#") + "," + myCompanyID._FnYearID + "," + N_EmpID;
//             dba.FillDataSet(ref dsItemCategory, "Pay_Payrate", Sql, "TEXT", new DataTable());
//         }

//         private void Get_Attendence()
//         {
//             int result = 0;
//             DateTime D_Out1ForDay;
//             string Sql1 = "";
//             N_WorkHours = 0;
//             N_WorkdHrs = 0;
//             N_TimeSheetID = 0;
//             N_NonDedApp = 0;
//             lbl_msg.Text = "";
//             dtpfromdate.Enabled = true;
//             dtptodate.Enabled = true;
//             if (N_EmpID > 0)
//             {
//                 if (dsItemCategory.Tables.Contains("EmpGrp_Workhours"))
//                     dsItemCategory.Tables.Remove("EmpGrp_Workhours");
//                 dba.FillDataSet(ref dsItemCategory, "EmpGrp_Workhours", "Select * From vw_EmpGrp_Workhours Where N_CompanyID=" + myCompanyID._CompanyID + " and N_PkeyId=" + N_CatagoryId + "", "TEXT", new DataTable());
//                 if (dsItemCategory.Tables["EmpGrp_Workhours"].Rows.Count != 0)
//                 {
//                     DataRow drow1 = dsItemCategory.Tables["EmpGrp_Workhours"].Rows[0];
//                     if (myFunctions.getBoolVAL(drow1["B_Compensation"].ToString()) == false)
//                     {
//                         lblExtraHrs.Visible = false;
//                         label12.Visible = false;
//                         lblTotalNonCompMins.Visible = false;
//                         label7.Visible = false;
//                     }
//                 }

//                 CategorywiseSettings(N_EmpID, N_CatagoryId);
//                 if (dsItemCategory.Tables.Contains("Pay_Attendence"))
//                     dsItemCategory.Tables.Remove("Pay_Attendence");
//                 string Sql = "";
//                 Sql = "SP_Pay_TimeSheet " + myCompanyID._CompanyID + " , " + myCompanyID._FnYearID + " , '" + myFunctions.getDateVAL(dtpfromdate.Value.Date) + "','" + myFunctions.getDateVAL(dtptodate.Value.Date) + "'," + N_EmpID;
//                 dba.FillDataSet(ref dsItemCategory, "Pay_Attendence", Sql, "TEXT", new DataTable());
//                 DateTime Date = dtpfromdate.Value.Date;
//                 flxTimesheetVerify.Rows = 1;
//                 int row = 1;
//                 lblNonAppDed.Text = "0.00";
//                 // ----- Period Calender  //--- Isvacation -0-Normal ,1-leave,2 -Holidays or wkoff
//                 do
//                 {


//                     flxTimesheetVerify.Rows = row + 1;
//                     flxTimesheetVerify.set_TextMatrix(row, mcSlNo, row.ToString());
//                     string s = DateTime.Now.Date.ToString("ddd");
//                     flxTimesheetVerify.set_TextMatrix(row, mcDate, Date.ToShortDateString());
//                     flxTimesheetVerify.set_TextMatrix(row, McDis_Date, Date.ToString("dd-MMM-yyy ddd"));



//                     foreach (DataRow Var1 in dsItemCategory.Tables["Pay_OffDays"].Rows)
//                     {
//                         if (N_CatagoryId == myFunctions.getIntVAL(Var1["N_CategoryID"].ToString()) && ((int)Date.DayOfWeek) + 1 == myFunctions.getIntVAL(Var1["N_DayID"].ToString()) || myFunctions.getDateVAL(Date) == myFunctions.getDateVAL(Convert.ToDateTime(Var1["D_Date"].ToString())))
//                         {
                            
//                             flxTimesheetVerify.set_TextMatrix(row, mcRemarks, Var1["X_Remarks"].ToString());
//                             flxTimesheetVerify.set_TextMatrix(row, mcIsVacation, "2");
//                         }

//                     }


//                     foreach (DataRow Var2 in dsItemCategory.Tables["Pay_WorkingHours"].Rows)
//                     {
//                         if (((int)Date.DayOfWeek) + 1 == myFunctions.getIntVAL(Var2["N_WHID"].ToString()))
//                         {
//                             flxTimesheetVerify.set_TextMatrix(row, mcWorkingHrs, Var2["N_Workhours"].ToString());
//                         }
//                     }
//                     if (dsItemCategory.Tables["Pay_Attendence"].Rows.Count > 0)
//                     {
//                         lbl_msg.Text = "Timesheet not yet approved";
//                         lbl_msg.ForeColor = Color.Gray;
//                         foreach (DataRow var in dsItemCategory.Tables["Pay_Attendence"].Rows)
//                         {


//                             // MessageBox.Show(var["D_Date"].ToString());
//                             if (myFunctions.getDateVAL(Convert.ToDateTime(var["D_Date"].ToString())) == myFunctions.getDateVAL(Date))
//                             {
//                                 string defaultTime = "00:00:00";

//                                 if (var["D_In"].ToString() != "")
//                                 {
//                                     flxTimesheetVerify.Row = row;
//                                     flxTimesheetVerify.Col = mcIn;
//                                     if (var["D_In"].ToString() == defaultTime)
//                                     {

//                                         flxTimesheetVerify.set_TextMatrix(row, mcIn, defaultTime);
//                                         flxTimesheetVerify.CellForeColor = Color.Red;
//                                     }
//                                     else
//                                     {
//                                         flxTimesheetVerify.set_TextMatrix(row, mcIn, var["D_In"].ToString());
//                                         flxTimesheetVerify.CellForeColor = Color.FromArgb(51, 51, 51);
//                                     }

//                                 }
//                                 else
//                                 {
//                                     flxTimesheetVerify.set_TextMatrix(row, mcIn, defaultTime);
//                                     flxTimesheetVerify.CellForeColor = Color.Red;
//                                 }
//                                 if (var["D_Out"].ToString() != "")
//                                 {
//                                     flxTimesheetVerify.Row = row;
//                                     flxTimesheetVerify.Col = mcOut;
//                                     if (var["D_Out"].ToString() == defaultTime)
//                                     {
//                                         flxTimesheetVerify.Row = row;
//                                         flxTimesheetVerify.Col = mcOut;
//                                         flxTimesheetVerify.set_TextMatrix(row, mcOut, defaultTime);
//                                         flxTimesheetVerify.CellForeColor = Color.Red;
//                                     }
//                                     else
//                                     {
//                                         flxTimesheetVerify.set_TextMatrix(row, mcOut, var["D_Out"].ToString());
//                                         flxTimesheetVerify.CellForeColor = Color.FromArgb(51, 51, 51);
//                                     }
//                                 }
//                                 else
//                                 {
//                                     flxTimesheetVerify.set_TextMatrix(row, mcOut, defaultTime);
//                                     flxTimesheetVerify.CellForeColor = Color.Red;
//                                 }
//                                 if (var["D_Shift2_In"].ToString() != "")
//                                 {
//                                     flxTimesheetVerify.Row = row;
//                                     flxTimesheetVerify.Col = mcIn2;
//                                     if (var["D_Shift2_In"].ToString() == defaultTime)
//                                     {
//                                         flxTimesheetVerify.set_TextMatrix(row, mcIn2, defaultTime);
//                                         flxTimesheetVerify.CellForeColor = Color.Red;
//                                     }
//                                     else
//                                     {
//                                         flxTimesheetVerify.set_TextMatrix(row, mcIn2, var["D_Shift2_In"].ToString());
//                                         flxTimesheetVerify.CellForeColor = Color.FromArgb(51, 51, 51);
//                                     }
//                                 }
//                                 else
//                                 {
//                                     flxTimesheetVerify.set_TextMatrix(row, mcIn2, defaultTime);
//                                     flxTimesheetVerify.CellForeColor = Color.Red;
//                                 }
//                                 if (var["D_Shift2_Out"].ToString() != "")
//                                 {
//                                     flxTimesheetVerify.Row = row;
//                                     flxTimesheetVerify.Col = mcOut2;
//                                     if (var["D_Shift2_Out"].ToString() == defaultTime)
//                                     {
//                                         flxTimesheetVerify.set_TextMatrix(row, mcOut2, defaultTime);
//                                         flxTimesheetVerify.CellForeColor = Color.Red;
//                                     }
//                                     else
//                                     {
//                                         flxTimesheetVerify.set_TextMatrix(row, mcOut2, var["D_Shift2_Out"].ToString());
//                                         flxTimesheetVerify.CellForeColor = Color.FromArgb(51, 51, 51);
//                                     }

//                                 }
//                                 else
//                                 {
//                                     flxTimesheetVerify.set_TextMatrix(row, mcOut2, defaultTime);
//                                     flxTimesheetVerify.CellForeColor = Color.Red;
//                                 }


//                                 flxTimesheetVerify.set_TextMatrix(row, mcTotHrs, myFunctions.getFloatVAL(var["N_TotHours"].ToString()).ToString("0.00")); //CMTDON07102020
//                                 //CalculateTotalWorkedHours(row);
//                                 flxTimesheetVerify.set_TextMatrix(row, mcDutyHours, myFunctions.getFloatVAL(var["N_DutyHours"].ToString()).ToString("0.00"));
//                                 flxTimesheetVerify.set_TextMatrix(row, mcBrkHr, myFunctions.getFloatVAL(var["N_DedHour"].ToString()).ToString("0.00"));
//                                 flxTimesheetVerify.set_TextMatrix(row, mcRemarks, var["X_YearlyOffDay"].ToString());
//                                 flxTimesheetVerify.set_TextMatrix(row, mcType, var["X_Description"].ToString());
//                                 flxTimesheetVerify.set_TextMatrix(row, mcSheetID, var["N_SheetID"].ToString());
//                                 flxTimesheetVerify.set_TextMatrix(row, mcPayID, var["N_OTPayID"].ToString());
//                                 flxTimesheetVerify.set_TextMatrix(row, mcIsVacation, var["B_Isvacation"].ToString());
//                                 if (flxTimesheetVerify.get_TextMatrix(row, mcRemarks).ToString() == "" || flxTimesheetVerify.get_TextMatrix(row, mcRemarks).ToString() == null)
//                                 {
//                                     if( var["X_Remarks"].ToString().ToLower() != "remarks")
//                                     flxTimesheetVerify.set_TextMatrix(row, mcRemarks, var["X_Remarks"].ToString());
//                                 }
//                                 //if (var["X_PayrunText"].ToString() != "" && var["X_PayrunText"].ToString() != "0")
//                                 //    dtpPayrunText.Text = var["X_PayrunText"].ToString();
//                                 if (B_MonthlyaddordedProcess)
//                                 {
//                                     if (txtAdjustment.Text != "")
//                                     {
//                                         string transcode = dtpPayrunText.Value.Year.ToString("00##") + dtpPayrunText.Value.Month.ToString("0#");
//                                         object obj = dba.ExecuteSclarNoErrorCatch("Select X_Description from vw_PayCodelist_MonthWise Where N_PayRunID =" + transcode + " and N_CompanyID= " + myCompanyID._CompanyID + " and N_EmpID=" + N_EmpID, "TEXT", new DataTable());
//                                         if (obj != null)
//                                             txtAdjpaycode.Text = obj.ToString();
//                                     }
//                                 }

//                                 //N_WorkHours += (((int)(myFunctions.getVAL(var["N_Workhours"].ToString())) / 1) + ((myFunctions.getVAL(var["N_Workhours"].ToString())) % 1) / 0.60);
//                                 //N_WorkdHrs += (((int)(myFunctions.getVAL(var["N_Tothours"].ToString())) / 1) + ((myFunctions.getVAL(var["N_Tothours"].ToString())) % 1) / 0.60);
//                                 int a = flxTimesheetVerify.Rows;
//                                 N_Diffrence = myFunctions.getVAL(var["N_Diff"].ToString());
//                                 flxTimesheetVerify.set_TextMatrix(row, mcDiff, N_Diffrence.ToString("0.00"));

//                                 if (!B_CategoryWiseDeduction && N_Diffrence < 0)
//                                 {
//                                     N_Diffrence = myFunctions.HoursToMinutes(N_Diffrence);
//                                     N_NonDedApp = myFunctions.HoursToMinutes(N_NonDedApp);

//                                     N_NonDedApp += N_Diffrence;

//                                     N_NonDedApp = myFunctions.MinutesToHours(N_NonDedApp);
//                                     lblNonAppDed.Text = N_NonDedApp.ToString("0.00");
//                                     lblNonAppDed.Visible = true;
//                                 }

//                                 //else
//                                 //{
//                                 //    lblNonAppDed.Visible = false;
//                                 //    lblNonDedApplic.Visible = false;
//                                 //    lblNonAppDed.Text = "0.00";
//                                 //}


//                                 if (B_CategoryWiseAddition)
//                                 {
//                                     flxTimesheetVerify.set_TextMatrix(row, mcOt, myFunctions.getVAL(var["OverTime"].ToString()).ToString("0.00"));

//                                 }
//                                 else
//                                 {
//                                     flxTimesheetVerify.set_TextMatrix(row, mcOt, "0.00");
//                                 }
//                                 if (B_CategoryWiseDeduction)
//                                     flxTimesheetVerify.set_TextMatrix(row, mcDeduct, myFunctions.getVAL(var["Deduction"].ToString()).ToString("0.00"));
//                                 else
//                                     flxTimesheetVerify.set_TextMatrix(row, mcDeduct, "0.00");
//                                 if (B_CategoryWiseDeduction)
//                                     flxTimesheetVerify.set_TextMatrix(row, mcCompDeduct, myFunctions.getVAL(var["CompMinutes"].ToString()).ToString("0.00"));
//                                 else
//                                     flxTimesheetVerify.set_TextMatrix(row, mcCompDeduct, "0.00");
//                                 if (var["B_Isvacation"].ToString() == "1")
//                                 {
//                                     flxTimesheetVerify.Col = mcAttendence;
//                                     flxTimesheetVerify.Row = flxTimesheetVerify.Rows - 1;
//                                     flxTimesheetVerify.CellForeColor = Color.Red;
//                                     flxTimesheetVerify.set_TextMatrix(row, mcAttendence, "A");
//                                     flxTimesheetVerify.set_TextMatrix(row, mcIncHoliday, var["B_HolidayFlag"].ToString());
//                                     if(myFunctions.getIntVAL(var["B_HolidayFlag"].ToString())!=0)
//                                         flxTimesheetVerify.set_TextMatrix(row, mcRemarks, var["X_Remarks"].ToString());
//                                 }

//                                 else
//                                 {
//                                     flxTimesheetVerify.Col = mcAttendence;
//                                     flxTimesheetVerify.Row = flxTimesheetVerify.Rows - 1;
//                                     if (myFunctions.getVAL(flxTimesheetVerify.get_TextMatrix(row, mcTotHrs)) < myFunctions.getVAL(var["N_MinWorkhours"].ToString()))
//                                     {
//                                         if (myFunctions.getBoolVAL(var["B_IsApproved"].ToString()) == true)
//                                         {
//                                             flxTimesheetVerify.set_TextMatrix(row, mcType, var["X_Description"].ToString());
//                                             flxTimesheetVerify.set_TextMatrix(row, mcPayID, var["N_OTPayID"].ToString());
//                                             flxTimesheetVerify.Col = mcAttendence;
//                                             flxTimesheetVerify.CellForeColor = Color.Green;

//                                             flxTimesheetVerify.set_TextMatrix(row, mcAttendence, "P");
//                                         }
//                                         else
//                                         {
//                                             flxTimesheetVerify.set_TextMatrix(row, mcType, X_Deductions);
//                                             flxTimesheetVerify.set_TextMatrix(row, mcPayID, N_DeductionPayID.ToString());
//                                             flxTimesheetVerify.Col = mcAttendence;
//                                             flxTimesheetVerify.CellForeColor = Color.Red;

//                                             flxTimesheetVerify.set_TextMatrix(row, mcAttendence, "A");
//                                         }

//                                     }
//                                     else if (myFunctions.getVAL(flxTimesheetVerify.get_TextMatrix(row, mcTotHrs)) > myFunctions.getVAL(var["N_MinWorkhours"].ToString()))
//                                     {
//                                         if (myFunctions.getBoolVAL(var["B_IsApproved"].ToString()) == true)
//                                         {
//                                             flxTimesheetVerify.set_TextMatrix(row, mcType, var["X_Description"].ToString());
//                                             flxTimesheetVerify.set_TextMatrix(row, mcPayID, var["N_OTPayID"].ToString());
//                                             flxTimesheetVerify.CellForeColor = Color.Green;
//                                             flxTimesheetVerify.set_TextMatrix(row, mcAttendence, "P");
//                                         }
//                                         else
//                                         {
//                                             if (myFunctions.getVAL(flxTimesheetVerify.get_TextMatrix(row, mcOt)) > 0)
//                                             {
//                                                 flxTimesheetVerify.set_TextMatrix(row, mcType, X_Additions);
//                                                 flxTimesheetVerify.set_TextMatrix(row, mcPayID, N_AdditionPayID.ToString());
//                                                 flxTimesheetVerify.CellForeColor = Color.Green;
//                                                 flxTimesheetVerify.set_TextMatrix(row, mcAttendence, "P");
//                                             }
//                                             else if (myFunctions.getVAL(flxTimesheetVerify.get_TextMatrix(row, mcDeduct)) > 0)
//                                             {
//                                                 flxTimesheetVerify.set_TextMatrix(row, mcType, X_Deductions);
//                                                 flxTimesheetVerify.set_TextMatrix(row, mcPayID, N_DeductionPayID.ToString());
//                                                 flxTimesheetVerify.CellForeColor = Color.Green;
//                                                 flxTimesheetVerify.set_TextMatrix(row, mcAttendence, "P");
//                                             }
//                                             else
//                                             {

//                                                 flxTimesheetVerify.set_TextMatrix(row, mcType, "");
//                                                 flxTimesheetVerify.set_TextMatrix(row, mcPayID, "0");
//                                                 flxTimesheetVerify.CellForeColor = Color.Green;
//                                                 flxTimesheetVerify.set_TextMatrix(row, mcAttendence, "P");
//                                             }
//                                         }

//                                     }
//                                     else
//                                     {

//                                         flxTimesheetVerify.set_TextMatrix(row, mcType, "");
//                                         flxTimesheetVerify.set_TextMatrix(row, mcPayID, "0");
//                                         flxTimesheetVerify.CellForeColor = Color.Green;
//                                         flxTimesheetVerify.set_TextMatrix(row, mcAttendence, "P");
//                                     }
//                                 }
//                                 if (myFunctions.getBoolVAL(var["B_IsApproved"].ToString()) == true)
//                                     flxTimesheetVerify.set_TextMatrix(row, mcApproved, "P");
//                                 else
//                                     flxTimesheetVerify.set_TextMatrix(row, mcApproved, "O");
//                             }

//                         }

//                     }
//                     if (flxTimesheetVerify.get_TextMatrix(row, mcRemarks) == "" && flxTimesheetVerify.get_TextMatrix(row, mcAttendence) != "P")
//                     {
                        
//                         flxTimesheetVerify.Col = mcAttendence;
//                         flxTimesheetVerify.Row = flxTimesheetVerify.Rows - 1;
//                         flxTimesheetVerify.CellForeColor = Color.Red;
//                         if (Date > Convert.ToDateTime(myFunctions.GetFormatedDate(myCompanyID._SystemDate)))
//                             flxTimesheetVerify.set_TextMatrix(row, mcAttendence, "");
//                         else
//                         {
//                             object obj = dba.ExecuteSclar("select X_GroupName from Pay_EmpShiftDetails where N_EmpID=" + N_EmpID + " and N_CompanyID=" + myCompanyID._CompanyID + " and N_FnYearID=" + myCompanyID._FnYearID + " and D_Date='" + myFunctions.getDateVAL(Date) + "' and D_In1='00:00:00.0000000' and D_Out1='00:00:00.0000000' and D_In2='00:00:00.0000000' and D_Out2='00:00:00.0000000'", "TEXT", new DataTable());
//                             if (obj != null)
//                             {
//                                 flxTimesheetVerify.set_TextMatrix(row, mcRemarks, obj.ToString());
//                                 flxTimesheetVerify.set_TextMatrix(row, mcAttendence, "");
//                             }
//                             else
//                             {
//                                 flxTimesheetVerify.set_TextMatrix(row, mcType, X_DefaultAbsentCode);
//                                 flxTimesheetVerify.set_TextMatrix(row, mcPayID, N_DefaultAbsentID.ToString());
//                                 flxTimesheetVerify.set_TextMatrix(row, mcAttendence, "A");
//                             }
//                         }
//                     }

//                     foreach (DataRow Var1 in dsItemCategory.Tables["Pay_OffDays"].Rows)
//                     {
//                         if (N_CatagoryId == myFunctions.getIntVAL(Var1["N_CategoryID"].ToString()) && ((int)Date.DayOfWeek) + 1 == myFunctions.getIntVAL(Var1["N_DayID"].ToString()) || myFunctions.getDateVAL(Date) == myFunctions.getDateVAL(Convert.ToDateTime(Var1["D_Date"].ToString())))
//                         {
//                             object obj = dba.ExecuteSclar("Select N_Workhours from Pay_AdditionalWorkingDays Where D_WorkingDate='" + Date.ToString("yyyy-MM-dd") + "' and N_CatagoryID=" + N_CatagoryId + " and N_CompanyID=" + myCompanyID._CompanyID, "TEXT", new DataTable());
//                             if (obj != null) continue;
//                             if (myFunctions.getIntVAL(flxTimesheetVerify.get_TextMatrix(row, mcIncHoliday)) != 1)
//                             {
//                                 flxTimesheetVerify.set_TextMatrix(row, mcRemarks, Var1["X_Remarks"].ToString());
//                                 flxTimesheetVerify.set_TextMatrix(row, mcIsVacation, "2");
//                             }
//                             if (myFunctions.getVAL(flxTimesheetVerify.get_TextMatrix(row, mcTotHrs)) != 0)
//                             {
//                                 double hours = myFunctions.getVAL(flxTimesheetVerify.get_TextMatrix(row, mcTotHrs));
//                                 flxTimesheetVerify.set_TextMatrix(row, mcType, X_Additions);
//                                 flxTimesheetVerify.set_TextMatrix(row, mcPayID, N_AdditionPayID.ToString());
//                                 flxTimesheetVerify.CellForeColor = Color.Green;
//                                 flxTimesheetVerify.set_TextMatrix(row, mcAttendence, "P");
//                                 flxTimesheetVerify.set_TextMatrix(row, mcDeduct, "");
//                             }
//                             else if (myFunctions.getIntVAL(flxTimesheetVerify.get_TextMatrix(row, mcIncHoliday))!=1)
//                             {
//                                 flxTimesheetVerify.Col = mcAttendence;
//                                 flxTimesheetVerify.CellForeColor = Color.Red;
//                                 flxTimesheetVerify.set_TextMatrix(row, mcAttendence, "");
//                                 flxTimesheetVerify.set_TextMatrix(row, mcType, "");
//                                 flxTimesheetVerify.set_TextMatrix(row, mcDeduct, "");
//                                 flxTimesheetVerify.set_TextMatrix(row, mcIn, "");
//                                 flxTimesheetVerify.set_TextMatrix(row, mcOut, "");
//                                 flxTimesheetVerify.set_TextMatrix(row, mcIn2, "");
//                                 flxTimesheetVerify.set_TextMatrix(row, mcOut2, "");
//                             }
//                             if (myFunctions.getVAL(flxTimesheetVerify.get_TextMatrix(row, mcCompDeduct)) < 0)
//                                 flxTimesheetVerify.set_TextMatrix(row, mcCompDeduct, "");

//                         }
//                     }
//                     //N_WorkHours = 0;
//                     foreach (DataRow var in dsItemCategory.Tables["Pay_Attendence"].Rows)
//                     {
//                         if (myFunctions.getDateVAL(Convert.ToDateTime(var["D_Date"].ToString())) == myFunctions.getDateVAL(Date))
//                         {
//                             if (flxTimesheetVerify.get_TextMatrix(row, mcAttendence) != "A")
//                             {
//                                 //N_WorkHours += (((int)(myFunctions.getVAL(var["N_Workhours"].ToString())) / 1) + ((myFunctions.getVAL(var["N_Workhours"].ToString())) % 1) / 0.60);
//                                 //N_WorkdHrs += (((int)(myFunctions.getVAL(var["N_Tothours"].ToString())) / 1) + ((myFunctions.getVAL(var["N_Tothours"].ToString())) % 1) / 0.60);
//                                 N_WorkdHrs += myFunctions.HoursToMinutes(myFunctions.getVAL(var["N_Tothours"].ToString()));
//                                 N_WorkHours += myFunctions.HoursToMinutes(myFunctions.getVAL(var["N_Workhours"].ToString()));
//                             }
//                         }
                        
//                     }
//                     flxTimesheetVerify.Col = mcApproved;
//                     flxTimesheetVerify.Row = flxTimesheetVerify.Rows - 1;
//                     flxTimesheetVerify.CellFontName = "Wingdings 2";
//                     flxTimesheetVerify.CellBackColor = Color.AliceBlue;
//                     flxTimesheetVerify.CellForeColor = Color.Red;
//                     row += 1;
//                     Date = Date.AddDays(1);

//                     // lblTotWrkDayval.Text = (((int)(N_WorkHours) / 1) + ((N_WorkHours) % 1) * .60).ToString("0.00");
//                     // lblWrkdDaysVal.Text = (((int)(N_WorkdHrs) / 1) + ((N_WorkdHrs) % 1) * .60).ToString("0.00");

//                     lblTotWrkDayval.Text = myFunctions.MinutesToHours(N_WorkHours).ToString("0.00");
//                     lblWrkdDaysVal.Text = myFunctions.MinutesToHours(N_WorkdHrs).ToString("0.00");

//                 }

//                 while (Date <= dtptodate.Value.Date);
//                 get_Payrate();
//                 CalcAdditionDeduction();
//                 CalculateAbsent();
//             }
//         }
//         private void ViewDetails()
//         {
//             int result = 0;
//             DateTime D_Out1ForDay;
//             string Sql1 = "";
//             N_WorkHours = 0;
//             N_WorkdHrs = 0;
//             int N_Status = 0;
//             if (N_BatchID > 0 && N_EmpID > 0)
//             {
//                 if (dsItemCategory.Tables.Contains("TimeSheetMaster"))
//                     dsItemCategory.Tables.Remove("TimeSheetMaster");
//                 string Sql = "";
//                 Sql = "Select * from vw_TimeSheetMaster_Disp where N_CompanyID=" + myCompanyID._CompanyID + " and N_FnYearID=" + myCompanyID._FnYearID + " and N_BatchID=" + N_BatchID.ToString() + " and N_EmpID=" + N_EmpID.ToString();
//                 dba.FillDataSet(ref dsItemCategory, "TimeSheetMaster", Sql, "TEXT", new DataTable());
//                 if (dsItemCategory.Tables["TimeSheetMaster"].Rows.Count == 0)
//                 {
//                     ClearScrean();
//                     btnSave.Enabled = true;
//                     if (!MYG.HavePermission(this.Text, "Save", lblResult, lblResultDescr))
//                         btnSave.Enabled = false;
//                     return;
//                 }
//                 DataRow drow = dsItemCategory.Tables["TimeSheetMaster"].Rows[0];
//                 txtEmpCode.Text = drow["X_EmpCode"].ToString();
//                 txtEmpName.Text = drow["X_EmpName"].ToString();
//                 N_TimeSheetID = myFunctions.getIntVAL(drow["N_TimeSheetID"].ToString());
//                 N_CatagoryId = myFunctions.getIntVAL(drow["N_CatagoryId"].ToString());
//                 CategorywiseSettings(N_EmpID, N_CatagoryId);
//                 dtpPayrunText.Text = drow["X_PayrunText"].ToString();
//                 txtBatchID.Text = drow["X_BatchCode"].ToString();

//                 dtpfromdate.Value = Convert.ToDateTime(drow["D_DateFrom"]);

//                 dtptodate.Value = Convert.ToDateTime(drow["D_DateTo"]);
//                 dtpfromdate.Enabled = false;
//                 dtptodate.Enabled = false;
//                 get_Payrate();
//                 lblDeductTotal.Text = myFunctions.getVAL(drow["N_GridDedTotal"].ToString()).ToString("0.00");
//                 lblTotalNonCompMins.Text = myFunctions.getVAL(drow["N_CompMinutes"].ToString()).ToString("0.00");
//                 lblNetDeduction.Text = myFunctions.getVAL(drow["N_Ded"].ToString()).ToString("0.00");
//                 txtNetDedApplicable.Text = myFunctions.getVAL(drow["N_Ded"].ToString()).ToString("0.00");
//                 lblTotAddition.Text = myFunctions.getVAL(drow["N_Ot"].ToString()).ToString("0.00");
//                 txtNetAddApplicable.Text = myFunctions.getVAL(drow["N_Ot"].ToString()).ToString("0.00");

//                 N_EntryUserID = myFunctions.getIntVAL(drow["N_UserID"].ToString());
//                 N_ProcStatus = myFunctions.getIntVAL(drow["N_ProcStatus"].ToString());
//                 N_ApprovalLevelID = myFunctions.getIntVAL(drow["N_ApprovalLevelID"].ToString());
//                 N_SaveDraft = myFunctions.getIntVAL(drow["N_SaveDraft"].ToString());

//                 if (dsItemCategory.Tables.Contains("EmpGrp_Workhours"))
//                     dsItemCategory.Tables.Remove("EmpGrp_Workhours");
//                 dba.FillDataSet(ref dsItemCategory, "EmpGrp_Workhours", "Select * From vw_EmpGrp_Workhours Where N_CompanyID=" + myCompanyID._CompanyID + " and N_PkeyId=" + myFunctions.getIntVAL(drow["N_CatagoryId"].ToString()) + "", "TEXT", new DataTable());
//                 if (dsItemCategory.Tables["EmpGrp_Workhours"].Rows.Count != 0)
//                 {
//                     DataRow drow1 = dsItemCategory.Tables["EmpGrp_Workhours"].Rows[0];
//                     if (myFunctions.getBoolVAL(drow1["B_Compensation"].ToString()) == false)
//                     {
//                         lblExtraHrs.Visible = false;
//                         label12.Visible = false;
//                         lblTotalNonCompMins.Visible = false;
//                         label7.Visible = false;
//                     }
//                 }


//                 int N_AddorDedID = 0;
//                 if (MYG.HavePermission(MYG.ReturnFormCaption("1017"), "Visible"))
//                 {
//                     object OTRequestCount = dba.ExecuteSclar("Select MAX(Pay_OvertimeRequestMaster.X_ReferenceCode) AS X_ReferenceCode from Pay_OvertimeRequestDetail INNER JOIN Pay_OvertimeRequestMaster ON Pay_OvertimeRequestDetail.N_OvertimeRequestID = Pay_OvertimeRequestMaster.N_OvertimeRequestID AND Pay_OvertimeRequestDetail.N_CompanyID = Pay_OvertimeRequestMaster.N_CompanyID WHERE Pay_OvertimeRequestDetail.N_EmpID=" + N_EmpID.ToString() + " and Pay_OvertimeRequestDetail.N_PayrunID=" + dtpPayrunText.Value.Year.ToString("00##") + dtpPayrunText.Value.Month.ToString("0#") + " and Pay_OvertimeRequestDetail.N_CompanyID=" + myCompanyID._CompanyID, "TEXT", new DataTable());
//                     if (OTRequestCount != null)
//                     {
//                         if (OTRequestCount.ToString().Trim() != "")
//                         {
//                             btnSave.Enabled = false;
//                             btnDelete.Enabled = false;
//                             MYG.ResultMessage(lblResult, lblResultDescr, "Alert!", "OT Request processed. Ref No:" + OTRequestCount.ToString().Trim());
//                         }
//                         else
//                         {
//                             btnSave.Enabled = true;
//                             btnDelete.Enabled = true;
//                         }
//                     }
//                     else
//                     {
//                         btnSave.Enabled = true;
//                         btnDelete.Enabled = true;
//                     }
//                 }

//                 if (myFunctions.getIntVAL(drow["N_UserID"].ToString()) > 0)
//                 {
//                     object obj = dba.ExecuteSclar("select D_EntryDate from Pay_MonthlyAddOrDedDetails where N_TransID=" + drow["N_AddorDedID"].ToString() + " and N_EmpID=" + N_EmpID.ToString() + " and N_refID=" + N_TimeSheetID + " and N_FormID=" + N_FormId, "TEXT", new DataTable());
//                     object res = dba.ExecuteSclar("Select X_UserName from Sec_User where N_UserID=" + myFunctions.getIntVAL(drow["N_UserID"].ToString()) + " and N_CompanyID=" + myCompanyID._CompanyID, "TEXT", new DataTable());
//                     if (res != null)
//                     {
//                         //lblmsg.Text = MYG.ReturnMultiLingualVal("-1111", "X_ControlNo", "TimesheetSavedTime").Replace("#", res.ToString())+ Convert.ToDateTime(drow["D_EntryDate"]).ToString("dd/MM/yyyy HH:mm:ss"); 
//                         //lblmsg.Text = MYG.ReturnMultiLingualVal("-1111", "X_ControlNo", "TimesheetSavedTime").Replace("#", res.ToString()) + Convert.ToDateTime(obj).ToString("dd/MM/yyyy HH:mm:ss");
//                         bool B_IsAdditionEntry = false;
//                         N_AddorDedID = myFunctions.getIntVAL(drow["N_AddorDedID"].ToString());
//                         if (N_AddorDedID > 0)
//                         {
//                             object TransDetailID = dba.ExecuteSclar("SELECT isnull(Count(Pay_MonthlyAddOrDedDetails.N_TransDetailsID),0) from Pay_MonthlyAddOrDedDetails where N_TransID=" + drow["N_AddorDedID"].ToString() + " and N_EmpID=" + N_EmpID.ToString() + " and N_refID=" + N_TimeSheetID + " and N_FormID=" + N_FormId, "TEXT", new DataTable());
//                             if (myFunctions.getIntVAL(TransDetailID.ToString()) > 0)
//                             {
//                                 B_IsAdditionEntry = true;
//                             }
//                         }
//                         else if (N_AddorDedID == 0)
//                             B_IsAdditionEntry = true;

//                         lbl_msg.Text = "Approved by " + res.ToString();
//                         if (drow["D_EntryDate"].ToString().Trim() != "")
//                             lbl_msg.Text = lbl_msg.Text + " on " + Convert.ToDateTime(drow["D_EntryDate"]).ToString("dd/MM/yyyy hh:mm:ss tt");
//                         if (!B_IsAdditionEntry)
//                             lbl_msg.Text = lbl_msg.Text + " (Addition or Deduction has been removed from other screen)";
//                         lbl_msg.ForeColor = Color.Green;
//                     }
//                     else
//                         lbl_msg.Text = "";
//                 }

//                 lblTotWrkDayval.Text = myFunctions.getVAL(drow["N_TotalWorkingDays"].ToString()).ToString("0.00");
//                 lblWrkdDaysVal.Text = myFunctions.getVAL(drow["N_TotalWorkedDays"].ToString()).ToString("0.00");

//                 N_BranchID = myFunctions.getIntVAL(drow["N_BranchID"].ToString());

//                 if (dsTimesheetDetails.Tables.Contains("TimeSheetDetails"))
//                     dsTimesheetDetails.Tables.Remove("TimeSheetDetails");
//                 dba.FillDataSet(ref dsTimesheetDetails, "TimeSheetDetails", "Select * from vw_EmpTimeSheetBatch  Where N_CompanyID=" + myCompanyID._CompanyID + " and N_BatchID=" + N_BatchID + " and N_EmpID=" + N_EmpID.ToString() + " and N_FnYearID=" + myCompanyID._FnYearID + "  Order By D_Date ASC", "TEXT", new DataTable());

//                 flxTimesheetVerify.Rows = 1;
//                 int row = 1;
//                 foreach (DataRow var in dsTimesheetDetails.Tables["TimeSheetDetails"].Rows)
//                 {
//                     flxTimesheetVerify.Rows = row + 1;
//                     flxTimesheetVerify.set_TextMatrix(row, mcSlNo, row.ToString());
//                     DateTime Date = Convert.ToDateTime(var["D_Date"].ToString());
//                     flxTimesheetVerify.set_TextMatrix(row, mcDate, Date.ToShortDateString());
//                     flxTimesheetVerify.set_TextMatrix(row, McDis_Date, Date.ToString("dd-MMM-yyy ddd"));

//                     foreach (DataRow Var1 in dsItemCategory.Tables["Pay_OffDays"].Rows)
//                     {
//                         if (((int)Date.DayOfWeek) + 1 == myFunctions.getIntVAL(Var1["N_DayID"].ToString()) || myFunctions.getDateVAL(Date) == myFunctions.getDateVAL(Convert.ToDateTime(Var1["D_Date"].ToString())))
//                         {
//                             flxTimesheetVerify.set_TextMatrix(row, mcRemarks, Var1["X_Remarks"].ToString());
//                             flxTimesheetVerify.set_TextMatrix(row, mcIsVacation, "2");
//                             flxTimesheetVerify.set_TextMatrix(row, mcAttendence, "");
//                         }

//                     }


//                     foreach (DataRow Var2 in dsItemCategory.Tables["Pay_WorkingHours"].Rows)
//                     {
//                         if (((int)Date.DayOfWeek) + 1 == myFunctions.getIntVAL(Var2["N_WHID"].ToString()))
//                         {
//                             flxTimesheetVerify.set_TextMatrix(row, mcWorkingHrs, Var2["N_Workhours"].ToString());
//                         }
//                     }
//                     string defaultTime = "00:00:00";

//                     //if (var["D_In"].ToString() != "")
//                     //{
//                     //    flxTimesheetVerify.set_TextMatrix(row, mcIn, var["D_In"].ToString());
//                     //}
//                     //else
//                     //{
//                     //    flxTimesheetVerify.set_TextMatrix(row, mcIn, defaultTime);
//                     //}
//                     //if (var["D_Out"].ToString() != "")
//                     //{
//                     //    flxTimesheetVerify.set_TextMatrix(row, mcOut, var["D_Out"].ToString());
//                     //}
//                     //else
//                     //{
//                     //    flxTimesheetVerify.set_TextMatrix(row, mcOut, defaultTime);
//                     //}
//                     //if (var["D_Shift2_In"].ToString() != "")
//                     //{
//                     //    flxTimesheetVerify.set_TextMatrix(row, mcIn2, var["D_Shift2_In"].ToString());
//                     //}
//                     //else
//                     //{
//                     //    flxTimesheetVerify.set_TextMatrix(row, mcIn2, defaultTime);
//                     //}
//                     //if (var["D_Shift2_Out"].ToString() != "")
//                     //{
//                     //    flxTimesheetVerify.set_TextMatrix(row, mcOut2, var["D_Shift2_Out"].ToString());
//                     //}
//                     //else
//                     //{
//                     //    flxTimesheetVerify.set_TextMatrix(row, mcOut2, defaultTime);
//                     //}

//                     if (var["D_In"].ToString().Trim() != "")
//                     {
//                         flxTimesheetVerify.Row = row;
//                         flxTimesheetVerify.Col = mcIn;
//                         if (var["D_In"].ToString() == defaultTime && myFunctions.getIntVAL(var["N_Status"].ToString()) == 2)
//                         {

//                             flxTimesheetVerify.set_TextMatrix(row, mcIn, defaultTime);
//                             flxTimesheetVerify.CellForeColor = Color.Red;
//                         }
//                         else
//                         {
//                             flxTimesheetVerify.set_TextMatrix(row, mcIn, var["D_In"].ToString());
//                             flxTimesheetVerify.CellForeColor = Color.FromArgb(51, 51, 51);
//                         }

//                     }
//                     else
//                     {
//                         flxTimesheetVerify.set_TextMatrix(row, mcIn, defaultTime);
//                         flxTimesheetVerify.CellForeColor = Color.FromArgb(51, 51, 51);
//                     }
//                     if (var["D_Out"].ToString().Trim() != "")
//                     {
//                         flxTimesheetVerify.Row = row;
//                         flxTimesheetVerify.Col = mcOut;
//                         if (var["D_Out"].ToString() == defaultTime && myFunctions.getIntVAL(var["N_Status"].ToString()) == 2)
//                         {
//                             flxTimesheetVerify.Row = row;
//                             flxTimesheetVerify.Col = mcOut;
//                             flxTimesheetVerify.set_TextMatrix(row, mcOut, defaultTime);
//                             flxTimesheetVerify.CellForeColor = Color.Red;
//                         }
//                         else
//                         {
//                             flxTimesheetVerify.set_TextMatrix(row, mcOut, var["D_Out"].ToString());
//                             flxTimesheetVerify.CellForeColor = Color.FromArgb(51, 51, 51);
//                         }
//                     }
//                     else
//                     {
//                         flxTimesheetVerify.set_TextMatrix(row, mcOut, defaultTime);
//                         flxTimesheetVerify.CellForeColor = Color.FromArgb(51, 51, 51);
//                     }
//                     if (var["D_Shift2_In"].ToString().Trim() != "")
//                     {
//                         flxTimesheetVerify.Row = row;
//                         flxTimesheetVerify.Col = mcIn2;
//                         if (var["D_Shift2_In"].ToString() == defaultTime && myFunctions.getIntVAL(var["N_Status"].ToString()) == 2)
//                         {
//                             flxTimesheetVerify.set_TextMatrix(row, mcIn2, defaultTime);
//                             flxTimesheetVerify.CellForeColor = Color.Red;
//                         }
//                         else
//                         {
//                             flxTimesheetVerify.set_TextMatrix(row, mcIn2, var["D_Shift2_In"].ToString());
//                             flxTimesheetVerify.CellForeColor = Color.FromArgb(51, 51, 51);
//                         }
//                     }
//                     else
//                     {
//                         flxTimesheetVerify.set_TextMatrix(row, mcIn2, defaultTime);
//                         flxTimesheetVerify.CellForeColor = Color.FromArgb(51, 51, 51);
//                     }
//                     if (var["D_Shift2_Out"].ToString().Trim() != "")
//                     {
//                         flxTimesheetVerify.Row = row;
//                         flxTimesheetVerify.Col = mcOut2;
//                         if (var["D_Shift2_Out"].ToString() == defaultTime && myFunctions.getIntVAL(var["N_Status"].ToString()) == 2)
//                         {
//                             flxTimesheetVerify.set_TextMatrix(row, mcOut2, defaultTime);
//                             flxTimesheetVerify.CellForeColor = Color.Red;
//                         }
//                         else
//                         {
//                             flxTimesheetVerify.set_TextMatrix(row, mcOut2, var["D_Shift2_Out"].ToString());
//                             flxTimesheetVerify.CellForeColor = Color.FromArgb(51, 51, 51);
//                         }

//                     }
//                     else
//                     {
//                         flxTimesheetVerify.set_TextMatrix(row, mcOut2, defaultTime);
//                         flxTimesheetVerify.CellForeColor = Color.FromArgb(51, 51, 51);
//                     }

//                     //flxTimesheetVerify.set_TextMatrix(row, mcTotHrs, myFunctions.getFloatVAL(var["N_TotalWorkHour"].ToString()).ToString());//CMTDON07102020
//                     CalculateTotalWorkedHours(row);
//                     flxTimesheetVerify.set_TextMatrix(row, mcRemarks, var["X_Remarks"].ToString());
//                     flxTimesheetVerify.set_TextMatrix(row, mcSheetID, var["N_SheetID"].ToString());
//                     flxTimesheetVerify.set_TextMatrix(row, mcPayID, var["N_OTPayID"].ToString());
//                     object objPayID = dba.ExecuteSclar("Select X_Description from PAy_PayMaster where N_CompanyID=" + myCompanyID._CompanyID + " and N_FnYearID=" + myCompanyID._FnYearID + " and N_PayID=" + myFunctions.getIntVAL(var["N_OTPayID"].ToString()), "TEXT", new DataTable());
//                     if (objPayID != null)
//                         flxTimesheetVerify.set_TextMatrix(row, mcType, objPayID.ToString());
//                     if (myFunctions.getIntVAL(var["N_Status"].ToString()) == 1)
//                     {
//                         flxTimesheetVerify.Col = mcAttendence;
//                         flxTimesheetVerify.Row = flxTimesheetVerify.Rows - 1;
//                         flxTimesheetVerify.CellForeColor = Color.Green;
//                         flxTimesheetVerify.set_TextMatrix(row, mcAttendence, "P");
//                     }
//                     else if (myFunctions.getIntVAL(var["N_Status"].ToString()) == 2)
//                     {
//                         flxTimesheetVerify.Col = mcAttendence;
//                         flxTimesheetVerify.Row = flxTimesheetVerify.Rows - 1;
//                         flxTimesheetVerify.CellForeColor = Color.Red;
//                         flxTimesheetVerify.set_TextMatrix(row, mcAttendence, "A");
//                         flxTimesheetVerify.set_TextMatrix(row, mcIsVacation, "1");
//                     }
//                     if (myFunctions.getVAL(var["DailyOT"].ToString()) > 0)
//                     {
//                         flxTimesheetVerify.set_TextMatrix(row, mcOt, myFunctions.getVAL(var["DailyOT"].ToString()).ToString());
//                         flxTimesheetVerify.set_TextMatrix(row, mcDeduct, "0");
//                     }
//                     else if (myFunctions.getVAL(var["DailyOT"].ToString()) < 0)
//                     {
//                         flxTimesheetVerify.set_TextMatrix(row, mcDeduct, (-1 * myFunctions.getVAL(var["DailyOT"].ToString())).ToString());
//                         flxTimesheetVerify.set_TextMatrix(row, mcOt, "0");
//                     }
//                     else
//                     {
//                         flxTimesheetVerify.set_TextMatrix(row, mcDeduct, "0");
//                         flxTimesheetVerify.set_TextMatrix(row, mcOt, "0");
//                     }
//                     flxTimesheetVerify.set_TextMatrix(row, mcCompDeduct, myFunctions.getVAL(var["N_Compensate"].ToString()).ToString("0.00"));

//                     flxTimesheetVerify.set_TextMatrix(row, mcDiff, myFunctions.getVAL(var["N_Diff"].ToString()).ToString("0.00"));
//                     flxTimesheetVerify.set_TextMatrix(row, mcDutyHours, myFunctions.getVAL(var["N_DutyHours"].ToString()).ToString("0.00"));
//                     flxTimesheetVerify.set_TextMatrix(row, mcBrkHr, myFunctions.getVAL(var["N_DedHour"].ToString()).ToString("0.00"));


//                     if (myFunctions.getBoolVAL(var["B_IsApproved"].ToString()) == true)
//                         flxTimesheetVerify.set_TextMatrix(row, mcApproved, "P");
//                     else
//                         flxTimesheetVerify.set_TextMatrix(row, mcApproved, "O");
//                     flxTimesheetVerify.Col = mcApproved;
//                     flxTimesheetVerify.Row = flxTimesheetVerify.Rows - 1;
//                     flxTimesheetVerify.CellFontName = "Wingdings 2";
//                     flxTimesheetVerify.CellBackColor = Color.AliceBlue;
//                     flxTimesheetVerify.CellForeColor = Color.Red;

//                     row += 1;


//                 }

//                 CalcAdditionDeduction();
//                 CalculateAbsent();
//                 if (N_AddorDedID > 0)
//                 {
//                     GetApplicableHours(N_AddorDedID, N_EmpID);
//                 }
//             }
//             myFunctions.SetApprovalSystem(ref N_IsApprovalSystem, myFunctions.getIntVAL(MYG.ReturnFormID(this.Text)), N_TimeSheetID, N_EntryUserID, N_ProcStatus, N_ApprovalLevelID, ref N_NextApprovalLevel, btnSave, btnDelete, lblmsg, lblResultDescr, 0, 1, ref IsEditable);
//         }
//         private void GetApplicableHours(int transID, int empId)
//         {
//             if (dsTrans.Tables.Contains("Pay_MonthlyAddOrDedDetails"))
//                 dsTrans.Tables.Remove("Pay_MonthlyAddOrDedDetails");
//             dba.FillDataSet(ref dsTrans, "Pay_MonthlyAddOrDedDetails", "select N_HrsOrDays from Pay_MonthlyAddOrDedDetails where N_TransID=" + transID + " and N_EmpID=" + empId + " and N_CompanyID=" + myCompanyID._CompanyID + " and B_TimeSheetEntry=1 and N_refID=" + N_TimeSheetID + " and N_FormID=" + N_FormId, "TEXT", new DataTable());
//             if (dsTrans != null && dsTrans.Tables != null && dsTrans.Tables["Pay_MonthlyAddOrDedDetails"].Rows.Count > 0)
//             {

//                 foreach (DataRow row in dsTrans.Tables["Pay_MonthlyAddOrDedDetails"].Rows)
//                 {
//                     if (myFunctions.getVAL(row["N_HrsOrDays"].ToString()) >= 0)
//                         txtNetAddApplicable.Text = myFunctions.getVAL(row["N_HrsOrDays"].ToString()).ToString("0.00");
//                     if (myFunctions.getVAL(row["N_HrsOrDays"].ToString()) <= 0)
//                         txtNetDedApplicable.Text = (-1 * myFunctions.getVAL(row["N_HrsOrDays"].ToString())).ToString("0.00");
//                 }
//             }
//         }
//         private void CalculateAbsent()
//         {
//             int absent_count = 0;
//             for (int i = 1; i < flxTimesheetVerify.Rows; i++)
//             {
//                 if (flxTimesheetVerify.get_TextMatrix(i, mcAttendence) == "A")
//                 {
//                     absent_count += 1;
//                 }
//             }
//             lblAbsent.Text = myFunctions.getVAL(absent_count.ToString()).ToString();
//         }


//         private void CalcAdditionDeduction()
//         {
//             double additionTime = 0, deductionTime = 0, CompsateDed = 0, OfficeHours = 0;
//             double N_additionTime = 0, N_deductionTime = 0, N_CompsateDed = 0, N_OfficeHours = 0, N_ExtraHours = 0;
//             double balanc = 0, N_NetDeduction = 0;
//             double N_OTDedu = 0;


//             ///Calculating grid totals in minutes
//             for (int i = 1; i < flxTimesheetVerify.Rows; i++)
//             {
//                 // N_OfficeHours = HoursToMinutes(CalculateOnWorkTime(i));
//                 N_OfficeHours = myFunctions.getVAL(flxTimesheetVerify.get_TextMatrix(i, mcDutyHours));
//                 N_additionTime = myFunctions.getVAL(flxTimesheetVerify.get_TextMatrix(i, mcOt));
//                 N_deductionTime = myFunctions.getVAL(flxTimesheetVerify.get_TextMatrix(i, mcDeduct));
//                 N_CompsateDed = myFunctions.getVAL(flxTimesheetVerify.get_TextMatrix(i, mcCompDeduct));
//                 if (N_additionTime > 0)
//                     additionTime += myFunctions.HoursToMinutes(N_additionTime);
//                 if (N_deductionTime > 0)
//                     deductionTime += myFunctions.HoursToMinutes(N_deductionTime);
//                 if (N_CompsateDed != 0)
//                     CompsateDed += myFunctions.HoursToMinutes(N_CompsateDed);
//                 if (N_OfficeHours != 0)
//                     OfficeHours += myFunctions.HoursToMinutes(N_OfficeHours);
//             }


//             if (CompsateDed < 0)
//             {
//                 double netded = -1 * CompsateDed;
//                 N_NetDeduction = deductionTime + netded;
//                 N_ExtraHours = 0;
//             }
//             else
//             {
//                 N_ExtraHours = CompsateDed;
//                 CompsateDed = 0;
//                 N_NetDeduction = deductionTime;

//             }

//             OfficeHours = myFunctions.MinutesToHours(OfficeHours);

//             additionTime = myFunctions.MinutesToHours(additionTime);


//             deductionTime = myFunctions.MinutesToHours(deductionTime);
//             CompsateDed = myFunctions.MinutesToHours(CompsateDed);
//             N_NetDeduction = myFunctions.MinutesToHours(N_NetDeduction);
//             N_ExtraHours = myFunctions.MinutesToHours(N_ExtraHours);
//             lblOnOfficeTime.Text = OfficeHours.ToString("0.00");

//             ////To solve variation of OT, Taking direct values and calculate
//             //additionTime = myFunctions.getVAL(lblOnOfficeTime.Text) - myFunctions.getVAL(lblTotWrkDayval.Text);


//             lblTotalDeduction.Text = deductionTime.ToString("0.00");

//             lblTotalNonCompMins.Text = (-1 * CompsateDed).ToString("0.00");
//             lblCompnDed.Text = (-1 * CompsateDed).ToString("0.00");

//             lblNetDeduction.Text = N_NetDeduction.ToString("0.00");
//             txtNetDedApplicable.Text = N_NetDeduction.ToString("0.00");
//             lblExtraHrs.Text = N_ExtraHours.ToString("0.00");
//             lblDeductTotal.Text = deductionTime.ToString("0.00");
//             lblTotAddition.Text = additionTime.ToString("0.00");
//             txtNetAddApplicable.Text = additionTime.ToString("0.00");
//             lblNetAddl.Text = additionTime.ToString("0.00");



//             lblTotAddition.Visible = false;
//             lblTotAddName.Visible = false;

//             //Addition Edit field 
//             lblNetAddApplicable.Visible = false;
//             txtNetAddApplicable.Visible = false;
//             //Addition Edit field 
//             if (B_CategoryWiseAddition)
//             {

//                 lblTotAddition.Visible = true;
//                 lblTotAddName.Visible = true;
//                 //Addition Edit field 
//                 lblNetAddApplicable.Visible = true;
//                 txtNetAddApplicable.Visible = true;
//                 //Addition Edit field 

//             }

//             balanc = additionTime + deductionTime;
//             if ((balanc % 60) < 0)
//             {
//                 lblBalance.Text = (((int)balanc / 1) + (-1 * balanc % 1) * .60).ToString();
//                 txtAdjustment.Text = (((int)balanc / 1) + (-1 * balanc % 1) * .60).ToString();
//             }
//             else
//             {
//                 lblBalance.Text = (((int)balanc / 1) + (balanc % 1) * .60).ToString(); ;
//                 txtAdjustment.Text = (((int)balanc / 1) + (balanc % 1) * .60).ToString();
//             }
//         }

//         private void btnFirst_Click(object sender, EventArgs e)
//         {
//             Navigation(1);
//         }

//         private void btnNext_Click(object sender, EventArgs e)
//         {
//             Navigation(2);
//         }

//         private void btnPrev_Click(object sender, EventArgs e)
//         {
//             Navigation(3);
//         }

//         private void btnLast_Click(object sender, EventArgs e)
//         {
//             Navigation(4);
//         }

//         private void ClearScrean()
//         {
//             if (B_ClosedFlag)
//                 return;
//             txtGridTextBox.Text = "";
//             N_PayID = 0;
//             txtEmpName.Text = "";
//             txtEmpCode.Text = "";
//             N_EmpID = 0;
//             N_WorkdHrs = 0;
//             N_WorkHours = 0;
//             N_NonDedApp = 0;

//             lblTotWrkDayval.Text = "0.00";
//             lblWrkdDaysVal.Text = "0.00";

//             lblNetDeduction.Text = "0.00";
//             txtNetDedApplicable.Text = "0.00";
//             lblExtraHrs.Text = "0.00";
//             lblOnOfficeTime.Text = "0.00";

//             lblCompnDed.Text = "0.00";
//             lblTotAddition.Text = "0.00";
//             txtNetAddApplicable.Text = "0.00";
//             lblTotalDeduction.Text = "0.00";
//             lblBalance.Text = "0.00";

//             lblNonAppDed.Text = "0.00";

//             lblNetAddl.Text = "0.00";
//             lblDeductTotal.Text = "0.00";
//             lblTotalNonCompMins.Text = "0.00";
//             N_BranchID = 0;
//             txtAdjustment.Text = "0.00";
//             txtAdjpaycode.Text = "";
//             N_AdjTypeID = 0;
//             N_TimeSheetID = 0;
//             N_BatchID = 0;
//             dtpfromdate.Enabled = true;
//             dtptodate.Enabled = true;
//             btnGridAccount.Visible = false;

//             lbl_msg.Text = "";
//             N_TotalDays = 0;
//             lblAbsent.Text = "0.00";
//             if (B_AutoInvoice)
//                 txtBatchID.Text = "@Auto";
//             else
//                 txtBatchID.Text = "";
//             flxTimesheetVerify.Rows = 1;
//             flxTimesheetVerify.Rows = 2;
//             btnSave.Enabled = true;
//             if (!MYG.HavePermission(this.Text, "Save", lblResult, lblResultDescr))
//                 btnSave.Enabled = false;
//             N_WorkHours = 0;
//             N_WorkdHrs = 0;
//             checkperiod();
//             // ucEmp.Visible = false;
//             //   Get_Attendence();
//             get_Workingdays();
//             DefaultPayCodes();

//             X_Code = "";

//             N_EntryUserID = 0;
//             N_ProcStatus = 0;
//             N_ApprovalLevelID = 0;
//             lblmsg.Visible = false;

//             lblExtraHrs.Visible = true;
//             label12.Visible = true;
//             lblTotalNonCompMins.Visible = true;
//             label7.Visible = true;

//             lblmsg.Text = "";
//             lbl_EmpDetails.Text = "";

//             myFunctions.SetApprovalSystem(ref N_IsApprovalSystem, myFunctions.getIntVAL(MYG.ReturnFormID(this.Text)), N_TimeSheetID, N_EntryUserID, N_ProcStatus, N_ApprovalLevelID, ref N_NextApprovalLevel, btnSave, btnDelete, lblmsg, lblResultDescr, 0, 1, ref IsEditable);

//             txtEmpCode.Focus();
//         }

//         private void btnClear_Click(object sender, EventArgs e)
//         {
//             N_PayID = 0;
//             flxTimesheetVerify.Rows = 1;
//             flxTimesheetVerify.Rows = 2;
//         }

//         private void txtCode_KeyPress(object sender, KeyPressEventArgs e)
//         {
//             MYG.check4SplChar(e);
//         }

//         private void TextBoxEnter(object sender, EventArgs e)
//         {
//             TextBox txt = (TextBox)sender;
//             if (!txt.ReadOnly)
//                 txt.BackColor = Color.AliceBlue;
//             X_GlobalVal = txt.Text;
//             myFunctions.selectText(txt);
//         }

//         private void TextBoxLeave(object sender, EventArgs e)
//         {
//             TextBox txt = (TextBox)sender;
//             txt.BackColor = Color.White;

//             if (txt.Name == "txtCode")
//             {
//                 if (X_GlobalVal != txt.Text.Trim())
//                 {
//                     ViewData();
//                 }
//             }
//             if (txt.Name == "txtAdjustment")
//             {
//                 if (myFunctions.getVAL(txtAdjustment.Text) > 0)
//                     txtAdjpaycode.Text = X_Additions;
//                 else if (myFunctions.getVAL(txtAdjustment.Text) < 0)
//                     txtAdjpaycode.Text = X_Deductions;
//                 else
//                     txtAdjpaycode.Text = "";

//                 txtAdjpaycode.Focus();
//             }
//         }

//         private void TextBoxKeyDown(object sender, KeyEventArgs e)
//         {

//             TextBox txt = (TextBox)sender;
//             if (e.KeyCode == Keys.Escape)
//             {
//                 txt.Text = X_GlobalVal;
//                 SendKeys.Send("{End}");
//                 e.Handled = false;
//             }
//             else if (e.KeyCode == Keys.Down)
//             {
//                 if (e.Alt)
//                 {
//                 }
//             }
//             else if (e.KeyCode == Keys.Enter)
//             {
//                 if (txt.Name == "txtDepartmentCode")
//                     ViewData();
//             }
//         }


//         private void flxFeeDetails_EnterCell(object sender, EventArgs e)
//         {

//             if (flxTimesheetVerify.Row == 0) return;
//             txtGridTextBox.Visible = false;
//             txtGridTextBox.Text = "";
//             if (flxTimesheetVerify.Col == mcType && myFunctions.getVAL(flxTimesheetVerify.get_TextMatrix(flxTimesheetVerify.Row, mcOt).ToString()) != 0)
//                 SetGridTextBox();
//             else if (flxTimesheetVerify.Col == mcType && myFunctions.getVAL(flxTimesheetVerify.get_TextMatrix(flxTimesheetVerify.Row, mcDeduct).ToString()) != 0)
//                 SetGridTextBox();
//             else if (flxTimesheetVerify.Col == mcType && flxTimesheetVerify.get_TextMatrix(flxTimesheetVerify.Row, mcAttendence) == "A")
//                 SetGridTextBox();

//             if (flxTimesheetVerify.Col == mcOt || flxTimesheetVerify.Col == mcDeduct || flxTimesheetVerify.Col == mcRemarks || flxTimesheetVerify.Col == mcTotHrs)
//                 SetGridTextBox();

//             if (flxTimesheetVerify.Col == mcIn || flxTimesheetVerify.Col == mcOut || flxTimesheetVerify.Col == mcIn2 || flxTimesheetVerify.Col == mcOut2)
//             {
//                 //if (B_ManualEntry_InGrid && flxFeeDetails.get_TextMatrix(flxFeeDetails.Row,mcAttendence )!="")
//                 //    SetGridTextBox();
//                 if (B_ManualEntry_InGrid)
//                     SetGridTextBox();
//             }
//         }

//         private void ArrangeSLNo()
//         {
//             int SLNo = 1;
//             for (int i = 1; i < flxTimesheetVerify.Rows; i++)
//             {
//                 if (flxTimesheetVerify.get_RowHeight(i) == 0) continue;
//                 flxTimesheetVerify.set_TextMatrix(i, mcSlNo, SLNo.ToString());
//                 SLNo += 1;
//             }
//         }

//         private void flxFeeDetails_ClickEvent(object sender, EventArgs e)
//         {
//             if (flxTimesheetVerify.Row == 0) return;
//             if (N_PayID == 0) return;
//             //  if (myFunctions.getIntVAL(flxFeeDetails.get_TextMatrix(flxFeeDetails.Row, mc)) == 0) return;

//         }
//         private void flxFeeDetails_DblClick(object sender, EventArgs e)
//         {
//             try
//             {
//                 if (flxTimesheetVerify.RowSel == 1)
//                 {
//                     string GridSize = "";
//                     for (int i = 0; i < flxTimesheetVerify.Cols; i++)
//                     {
//                         GridSize += flxTimesheetVerify.get_ColWidth(i).ToString() + ",";
//                     }
//                     RegistryKey reg;
//                     reg = Registry.CurrentUser.OpenSubKey("SOFTWARE", true);
//                     reg.CreateSubKey(myCompanyID._Project);
//                     reg = reg.OpenSubKey(myCompanyID._Project + dba.RegSubFolder, true);
//                     reg.SetValue("HR_" + this.Text + "_grid", GridSize);
//                     reg.Close();
//                 }
//             }
//             catch (Exception ex)
//             {
//             }
//             //if (flxFeeDetails.Row > 0 && flxFeeDetails.Row != flxFeeDetails.Rows - 1)
//             //{
//             //    if (msg.msgAnswerMe(MYG.ReturnMultiLingualVal("-1111", "X_ControlNo", "DeleteRowConfirm")))
//             //    {
//             //        flxFeeDetails.RemoveItem(flxFeeDetails.Row);
//             //        flxFeeDetails.set_RowHeight(flxFeeDetails.Row, 0);
//             //        if (flxFeeDetails.Row + 1 < flxFeeDetails.Rows)
//             //            flxFeeDetails.Row += 1;
//             //        ArrangeSLNo();
//             //    }
//             //}
//         }
//         private void txtCode_Enter(object sender, EventArgs e)
//         {

//         }
//         private void lnkClass_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
//         {
//             //if (!MYG.HavePermission(MYG.ReturnFormCaption("158"), "Visible"))
//             //{
//             //    msg.msgInformation(MYG.ReturnMultiLingualVal("-1111", "X_ControlNo", "AccessDenied"));
//             //    return;
//             //}
//             //object obj = null;
//             //if (myFunctions.GetOpenedForm(sender.ToString(), out obj))
//             //{
//             //    Sch_ClassMaster frm = (Sch_ClassMaster)obj;
//             //    frm.Activate();

//             //}
//             //else
//             //{
//             //    Sch_ClassMaster frm = new Sch_ClassMaster();
//             //    frm.MdiParent = myCompanyID.MDIForm;
//             //    frm.Text = MYG.ReturnFormCaption("158");
//             //    myFunctions.AddOpenedForms(frm, myCompanyID.mnuToolSideMenu_Click);
//             //    frm.Show();
//             //}
//         }
//         private void Pay_TimeSheetVerify_KeyDown(object sender, KeyEventArgs e)
//         {
//             if (e.KeyCode == Keys.Escape)
//             {
//                 if (ucEmp.Visible)
//                 {
//                     B_EscapePressed = true;
//                     ucEmp.Visible = false;
//                     e.Handled = false;
//                 }
//                 if (ucType.Visible)
//                 {
//                     B_EscapePressed = true;
//                     ucType.Visible = false;
//                     e.Handled = false;
//                 }
//                 if (ucAdjType.Visible)
//                 {
//                     B_EscapePressed = true;
//                     ucAdjType.Visible = false;
//                     e.Handled = false;
//                 }
//             }
//         }
//         private void lnkFees_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
//         {
//             //if (!MYG.HavePermission(MYG.ReturnFormCaption("159"), "Visible"))
//             //{
//             //    msg.msgInformation(MYG.ReturnMultiLingualVal("-1111", "X_ControlNo", "AccessDenied"));
//             //    return;
//             //}
//             //object obj = null;
//             //if (myFunctions.GetOpenedForm(sender.ToString(), out obj))
//             //{
//             //    Sch_FeeTypeMaster frm = (Sch_FeeTypeMaster)obj;
//             //    frm.Activate();
//             //}
//             //else
//             //{
//             //    Sch_FeeTypeMaster frm = new Sch_FeeTypeMaster();
//             //    frm.MdiParent = myCompanyID.MDIForm;
//             //    frm.Text = MYG.ReturnFormCaption("159");
//             //    myFunctions.AddOpenedForms(frm, myCompanyID.mnuToolSideMenu_Click);
//             //    frm.Show();
//             //}
//         }

//         private void flxFeeDetails_Enter(object sender, EventArgs e)
//         {
//             if (flxTimesheetVerify.Row == 0) return;
//             switch (flxTimesheetVerify.Col)
//             {
//                 case mcApproved:
//                     X_PrevValue = flxTimesheetVerify.get_TextMatrix(flxTimesheetVerify.Row, mcApproved);
//                     break;
//                 case mcAttendence:
//                     X_PrevValue = flxTimesheetVerify.get_TextMatrix(flxTimesheetVerify.Row, mcAttendence);
//                     break;

//             }
//             //if (flxFeeDetails.Col == mcSrl)
//             //    flxFeeDetails.Col = mcPayElement;
//             ////else
//             ////    flxFeeDetails_EnterCell(sender, e);
//         }

//         private void btnClose_Click_1(object sender, EventArgs e)
//         {
//             this.Close();
//         }
//         private void ucvisible()
//         {
//             B_EscapePressed = true;
//             ucType.Visible = false;
//             ucAdjType.Visible = false;
//             ucEmp.Visible = false;
//             ucBatch.Visible = false;
//             B_EscapePressed = false;
//         }

//         private void ItemSettings()
//         {
//             string searchfield = "Name";
//             ucEmp.Width = 400;
//             ucEmp.Height = 223;
//             ucEmp.BringToFront();
//             ucEmp.Left = txtEmpCode.Left;
//             ucEmp.Top = btnSearchEmployee.Bottom;
//             ucEmp.X_ProjectName = myCompanyID._Project;
//             ucEmp.X_HideFieldList = "N_CompanyID,N_EmpID,N_BranchID,N_Status,N_FnYearID,N_CatagoryId,X_DefEmpCode";
//             ucEmp.X_OrderByField = "[Employee Code]";
//             ucEmp.X_TableName = "vw_PayEmployee_Disp";
//             ucEmp.X_VisibleFieldList = "[Employee Code],Name";

//             if (myCompanyID._B_AllBranchData == true)
//                 ucEmp.X_Crieteria = "N_CompanyID=" + myCompanyID._CompanyID + " and (N_Status = 0 OR N_Status = 1) and N_FnYearID=" + myCompanyID._FnYearID;
//             else
//                 ucEmp.X_Crieteria = "N_CompanyID=" + myCompanyID._CompanyID + " and N_BranchID= " + myCompanyID._BranchID + " and (N_Status = 0 OR N_Status = 1) and N_FnYearID=" + myCompanyID._FnYearID;

//             ucEmp.X_SearchField = searchfield;
//             ucEmp.B_HeaderVisible = true;
//             ucEmp.X_FieldHeader = MYG.ReturnMultiLingualVal("306", "X_ControlNo", "ucEmp");
//             ucEmp.X_FieldWidths = "30,70";
//             ucEmp.RightToLeft = this.RightToLeft;
//             ucEmp.InitializeControl();
//         }
//         private void BatchSettings()
//         {
//             string searchfield = "BatchCode";
//             ucBatch.Width = 400;
//             ucBatch.Height = 223;
//             ucBatch.BringToFront();
//             ucBatch.Left = txtBatchID.Left;
//             ucBatch.Top = BtnSearchBatch.Bottom;
//             ucBatch.X_ProjectName = myCompanyID._Project;
//             ucBatch.X_HideFieldList = "N_CompanyID,N_BranchID,N_FnYearID,N_TimeSheetID,N_BatchID,N_EmpID";
//             ucBatch.X_OrderByField = "BatchCode";
//             ucBatch.X_TableName = "vw_EmpTimesheetMaster";
//             ucBatch.X_VisibleFieldList = "BatchCode,X_EmpCode";

//             if (myCompanyID._B_AllBranchData == true)
//                 ucBatch.X_Crieteria = "N_CompanyID=" + myCompanyID._CompanyID + " and  N_FnYearID=" + myCompanyID._FnYearID;
//             else
//                 ucBatch.X_Crieteria = "N_CompanyID=" + myCompanyID._CompanyID + " and N_BranchID= " + myCompanyID._BranchID + " and N_FnYearID=" + myCompanyID._FnYearID;

//             ucBatch.X_SearchField = searchfield;
//             ucBatch.B_HeaderVisible = true;
//             ucBatch.X_FieldHeader = MYG.ReturnMultiLingualVal(MYG.ReturnFormID(this.Text), "X_ControlNo", "ucBatch");
//             ucBatch.X_FieldWidths = "30,70";
//             ucBatch.RightToLeft = this.RightToLeft;
//             ucBatch.InitializeControl();
//         }




//         private void btnSearchEmployee_Click(object sender, EventArgs e)
//         {
//             ucvisible();
//             if (ucEmp.Visible)
//             {
//                 B_EscapePressed = true;
//                 ucEmp.Visible = false;
//                 B_EscapePressed = false;
//             }
//             else
//             {
//                 if (B_UcLeaveCalled) { B_UcLeaveCalled = false; return; }
//                 ItemSettings();
//                 ucEmp.ActivateControl();
//                 ucEmp.Focus();
//             }
//         }





//         private void ucEmp_Leave(object sender, EventArgs e)
//         {
//             if (B_EscapePressed) { return; }
//             if (ucEmp.Visible)
//             {
//                 B_EscapePressed = true;
//                 ucEmp.Visible = false;
//                 B_EscapePressed = false;
//                 B_UcLeaveCalled = true;
//             }
//         }
//         private bool ValidateEmployee(int _empid)
//         {

//             if (dsCategory.Tables.Contains("employee"))
//                 dsCategory.Tables.Remove("employee");
//             dba.FillDataSet(ref dsCategory, "employee", "Select * From vw_PayEmployee_Disp Where N_EmpID =" + _empid + " and N_CompanyID=" + myCompanyID._CompanyID, "Text", new DataTable());
//             if (dsCategory.Tables["employee"].Rows.Count > 0)
//             {
//                 DataRow drow = dsCategory.Tables["employee"].Rows[0];
//                 N_EmpID = myFunctions.getIntVAL(drow["N_EmpID"].ToString());

//                 lbl_EmpDetails.Text =  MYG.ReturnMultiLingualVal("-1111", "X_ControlNo", "Department") + ": " + drow["X_Department"] + "  " + MYG.ReturnMultiLingualVal("195", "X_ControlNo", "0") + ": " + drow["X_Position"];
//                 return true;

//             }
//             else
//             {
//                 lbl_EmpDetails.Text = "";
//                 return false;
//             }
//         }

//         private void ucEmp_VisibleChanged(object sender, EventArgs e)
//         {
//             if (!ucEmp.Visible)
//             {
//                 if (B_EscapePressed) { B_EscapePressed = false; return; }
//                 txtEmpCode.Text = ucEmp.ReturnSelectedValue("Employee Code");
//                 txtEmpName.Text = ucEmp.ReturnSelectedValue("Name");
//                 N_EmpID = myFunctions.getIntVAL(ucEmp.ReturnSelectedValue("N_EmpID"));
//                 N_CatagoryId = myFunctions.getIntVAL(ucEmp.ReturnSelectedValue("N_CatagoryId"));
//                 ViewData();
//                 // Get_Attendence();
//                 ValidateEmployee(N_EmpID);
//                 btnGridAccount.Visible = false;
//                 B_UcLeaveCalled = false;

                
//             }
//         }
//         private void Clear()
//         {
//             txtEmpCode.Text = "";
//             txtEmpName.Text = "";
//             flxTimesheetVerify.Rows = 1;
//             //   txtEmpCode.Focus();
//             flxTimesheetVerify.Rows = 2;
//             lbl_EmpDetails.Text = "";
//         }
//         private void Pay_TimeSheetVerify_Click(object sender, EventArgs e)
//         {
//             ucvisible();
//         }

//         private void txtEmpCode_KeyPress(object sender, KeyPressEventArgs e)
//         {
//             if (e.KeyChar == 13)
//             {
//                 if (txtEmpCode.Text.Trim() == "" || !ValidateEmployee())
//                 {
//                     //Clear();
//                     btnSearchEmployee_Click(sender, e);
//                     ucEmp.ValueChanged("[Employee Code]", txtEmpCode.Text.Trim());
//                     return;
//                 }
//                 else
//                 {
//                     ViewData();
//                     dtpPayrunText.Focus();
//                 }
//             }
//         }

//         private void txtEmpCode_KeyDown(object sender, KeyEventArgs e)
//         {
//             if (e.KeyCode == Keys.Down)
//             {
//                 if (e.Alt)
//                 {
//                     btnSearchEmployee_Click(sender, new EventArgs());
//                 }
//             }
//         }

//         private void btnClr_Click(object sender, EventArgs e)
//         {

//         }

//         private bool PaycodeSave()
//         {

//             if (chkSProcessType.Checked)
//                 return true;
//             else
//             {
//                 int val = 0;
//                 for (int i = 1; i < flxTimesheetVerify.Rows; i++)
//                 {
//                     if (myFunctions.getVAL(flxTimesheetVerify.get_TextMatrix(i, mcOt)) != 0 && flxTimesheetVerify.get_TextMatrix(i, mcType) != "")
//                     {
//                         val = 1;
//                         break;
//                     }
//                     else if (myFunctions.getVAL(flxTimesheetVerify.get_TextMatrix(i, mcDeduct)) != 0 && flxTimesheetVerify.get_TextMatrix(i, mcType) != "")
//                     {
//                         val = 1;
//                         break;
//                     }
//                     else if (myFunctions.getVAL(lblNetDeduction.Text) > 0 && N_DeductionPayID > 0 && B_CategoryWiseDeduction)
//                     {
//                         val = 1;
//                         break;
//                     }
//                     else if (myFunctions.getVAL(lblNetAddApplicable.Text) > 0 && N_AdditionPayID > 0 && B_CategoryWiseAddition)
//                     {
//                         val = 1;
//                         break;
//                     }
//                     else
//                         val = 0;

//                 }
//                 if (val == 1)
//                     return true;
//                 else
//                     return false;

//             }
//         }

//         private bool Validate_PayType()
//         {
//             if (chkSProcessType.Checked)
//             {
//                 if (myFunctions.getVAL(txtAdjustment.Text) != 0 && txtAdjpaycode.Text == "")
//                     return false;
//                 else
//                     return true;

//             }
//             int val = 0;
//             for (int i = 1; i < flxTimesheetVerify.Rows; i++)
//             {
//                 // if (myFunctions.getIntVAL(flxFeeDetails.get_TextMatrix(i, mcPayrunID)) <= 0) continue;
//                 if (flxTimesheetVerify.get_TextMatrix(i, mcAttendence) == "P")
//                 {

//                     if (myFunctions.getVAL(flxTimesheetVerify.get_TextMatrix(i, mcOt)) != 0 && flxTimesheetVerify.get_TextMatrix(i, mcType) == "")
//                     {
//                         val = 1;
//                         break;
//                     }
//                 }
//             }
//             if (val == 1)
//                 return false;
//             else
//                 return true;
//         }

//         private bool Validate_Payrun()
//         {
//             object N_Count = dba.ExecuteSclarNoErrorCatch("select count(1) from vw_PayProcessingDetails where N_EmpID= " + N_EmpID + " and X_PayrunText='" + dtpPayrunText.Text.ToString() + "'", "TEXT", new DataTable());
//             if (myFunctions.getIntVAL(N_Count.ToString()) > 0)
//                 return false;
//             else

//                 return true;
//         }

//         private bool SalaryAlreadyProcesd()
//         {
//             String Todate = dtpfromdate.Value.Year.ToString("00##") + dtptodate.Value.Month.ToString("0#");
//             int count = myFunctions.getIntVAL(Convert.ToString(dba.ExecuteSclar("select 1 from Pay_PaymentDetails inner join Pay_PaymentMaster on Pay_PaymentDetails.N_TransID= Pay_PaymentMaster.N_TransID  where Pay_PaymentDetails.N_CompanyID=" + myCompanyID._CompanyID + " and Pay_PaymentMaster.N_FnYearID=" + myCompanyID._FnYearID + " and Pay_PaymentDetails.N_EmpID =" + N_EmpID.ToString() + " and (Pay_PaymentMaster.N_PayRunID = " + Todate + ")", "TEXT", new DataTable())));
//             if (count > 0)
//             {
//                 return false;
//             }
//             return true;

//         }
//         private void btnSave_Click_1(object sender, EventArgs e)
//         {
//             Cursor.Current = Cursors.WaitCursor;
//             int N_AddorDedID = 0, N_AbsentCount = 0, N_Absententry = 0;
//             int N_Aprroved = 0, N_Vaccount = 0;
//             string FieldList = "";
//             string FieldValues = "";
//             string DupCriteria = "";
//             string RefFieldList = "";
//             string RefFileldDescr = "";
//             object Result = 0, TransResult = 0, obj = 0, objdetails = 0;
//             DateTime D_VACFromDate = dtpfromdate.Value.Date, D_VACToDate, D_TempVACToDate = dtpfromdate.Value.Date;
//             bool B_VacFlag = true;
//             int N_OffID = 0;
//             bool B_Completed = true;
//             int N_adddetails = 0;
//             X_Action = btnSave.Text;
//             if (MYG.check4Null(txtEmpCode, MYG.ReturnMultiLingualVal("-1111", "X_ControlNo", "InvalidEntry")))
//                 return;
//             if (!myFunctions.CheckFinanceDate(dtpPayrunText)) return;
//             if (!chkSProcessType.Checked)
//             {
//                 if (!Validate_PayType())
//                 {
//                     msg.msgError(MYG.ReturnMultiLingualVal("-1111", "X_ControlNo", "selpaycode_Att"));
//                     return;
//                 }
//             }
//             else
//             {
//                 if (!ValidatePayCode())
//                 {
//                     msg.msgError(MYG.ReturnMultiLingualVal("-1111", "X_ControlNo", "selpaycode_Att"));
//                     return;
//                 }
//             }
//             if (!Validate_Payrun())
//             {
//                 msg.msgError(MYG.ReturnMultiLingualVal("-1111", "X_ControlNo", "Payrun_Att"));
//                 return;
//             }
//             object obj1 = dba.ExecuteSclarNoErrorCatch("SELECT  [dbo].[SP_TimeSheetCalc_TotalHours](" + myCompanyID._CompanyID + ",'" + myFunctions.getDateVAL(dtpfromdate.Value) + "','" + myFunctions.getDateVAL(dtptodate.Value) + "'," + N_EmpID + ")", "TEXT", new DataTable());
//             if (obj1 != null)
//                 N_TotalDays = myFunctions.getVAL(obj1.ToString());
//             else
//                 N_TotalDays = myFunctions.getVAL(lblTotWrkDayval.Text);
//                // N_TotalDays = myFunctions.getVAL(obj1.ToString());
//             //else
//             //    N_TotalDays = 240;
//             // N_TotalDays = myFunctions.GetSalaryDays(dtpfromdate.Value, dtptodate.Value, N_EmpID);
//             //-- Check Salary pro
//             for (int i = 1; i < flxTimesheetVerify.Rows; i++)
//             {

//                 if (flxTimesheetVerify.get_TextMatrix(i, mcAttendence) == "A")
//                 {
//                     if (!SalaryAlreadyProcesd())
//                     {
//                         msg.msgError(MYG.ReturnMultiLingualVal("-1111", "X_ControlNo", "SalaryProcessed_Att"));
//                         B_VacFlag = false;
//                         break;
//                     }
//                     N_AbsentCount += 1;
//                 }
//             }

//             if (Convert.ToDateTime(dtptodate.Value.Date) > Convert.ToDateTime(myFunctions.GetFormatedDate(myCompanyID._SystemDate)))
//             {
//                 msg.msgError(MYG.ReturnMultiLingualVal("-1111", "X_ControlNo", "Overdate_Attd"));
//                 return;
//             }
//             //if (!SalaryAlreadyProcesd())
//             //{
//             //    msg.msgError(MYG.ReturnMultiLingualVal("-1111", "X_ControlNo", "SalaryProcessed"));
//             //    return;
//             //}
//             dba.SetTransaction();
//             string PayrunID = dtpPayrunText.Value.Year.ToString("00##") + dtpPayrunText.Value.Month.ToString("0#");


//             //if (N_BranchID == 0)
//             //    N_BranchID = myCompanyID._BranchID;

//             try
//             {
//                 if (txtBatchID.Text.Trim() == "@Auto")
//                 {
//                     bool OK = true;
//                     int NewNo = 0, loop = 1;
//                     while (OK)
//                     {
//                         NewNo = myFunctions.getIntVAL(dba.ExecuteSclarNoErrorCatch("Select Isnull(count(1),0) + " + loop + " As Count FRom Pay_TimeSheetMaster Where N_CompanyID=" + myCompanyID._CompanyID + " And N_FnyearID = " + myCompanyID._FnYearID + " And N_BatchID = " + PayrunID, "TEXT", new DataTable()).ToString());
//                         txtBatchID.Text = dtpPayrunText.Value.Year.ToString("00##") + dtpPayrunText.Value.Month.ToString("0#") + NewNo.ToString("0#");
//                         if (myFunctions.getIntVAL(dba.ExecuteSclarNoErrorCatch("Select Isnull(count(1),0) FRom Pay_PaymentMaster Where N_CompanyID=" + myCompanyID._CompanyID + " And N_FnyearID = " + myCompanyID._FnYearID + " And X_Batch = '" + txtBatchID.Text + "'", "TEXT", new DataTable()).ToString()) == 0)
//                         {
//                             OK = false;
//                         }
//                         loop += 1;
//                     }
//                 }

//                 ///////////      Removig Existing Data 
//                 if (N_TimeSheetID > 0)
//                 {                    
//                     if (dba.DeleteData("Pay_TimeSheet", "N_BatchID", N_BatchID.ToString(), "N_CompanyID=" + myCompanyID._CompanyID + " and B_IsApproved=1 and N_FnYearID=" + myCompanyID._FnYearID + " and N_TimeSheetID=" + N_TimeSheetID.ToString()))
//                     {
//                         dba.DeleteData("Pay_TimeSheetMaster", "N_TimeSheetID", N_TimeSheetID.ToString(), "N_CompanyID=" + myCompanyID._CompanyID + " and N_FnYearID=" + myCompanyID._FnYearID);
//                     }
//                 }

//                 //---- Paycode saving
//                 if (PaycodeSave())
//                 {

//                     if (dsItemCategory.Tables.Contains("Pay_Monthlyaddorded"))
//                         dsItemCategory.Tables.Remove("Pay_Monthlyaddorded");
//                     dsItemCategory.Tables.Add("Pay_Monthlyaddorded");
//                     dsItemCategory.Tables["Pay_Monthlyaddorded"].Columns.Add("N_PayID");
//                     dsItemCategory.Tables["Pay_Monthlyaddorded"].Columns.Add("N_OT");
//                     obj = dba.ExecuteSclar(" select N_TransID from Pay_MonthlyAddOrDed where N_CompanyID=" + myCompanyID._CompanyID + " and N_PayrunID=" + dtpPayrunText.Value.Year.ToString("00##") + dtpPayrunText.Value.Month.ToString("0#") + "", "TEXT", new DataTable());
//                     if (obj == null)
//                         N_AddorDedID = 0;
//                     else if (myFunctions.getIntVAL(obj.ToString()) > 0)
//                         N_AddorDedID = myFunctions.getIntVAL(obj.ToString());



//                     if (N_AddorDedID == 0)
//                     {
//                         FieldList = "N_CompanyID,N_PayrunID,X_Batch,D_TransDate,X_PayrunText,B_PostedAccount,D_CreatedDate,D_ModifiedDate";
//                         //FieldValues = myCompanyID._CompanyID + "|" + dtpPayrunText.Value.Year.ToString("00##") + dtpPayrunText.Value.Month.ToString("0#") + "|''|'" + myFunctions.getDateVAL(DateTime.Now.Date) + "'|'" + myFunctions.getDateVAL(dtpPayrunText.Value) + "'|0|'" + myFunctions.getDateVAL(DateTime.Now.Date) + "'|'" + myFunctions.GetFormatedDate_Ret_string(myCompanyID._SystemDate) + "'";
//                         FieldValues = myCompanyID._CompanyID + "|" + dtpPayrunText.Value.Year.ToString("00##") + dtpPayrunText.Value.Month.ToString("0#") + "|'" + dtpPayrunText.Value.Year.ToString("00##") + dtpPayrunText.Value.Month.ToString("0#") + "'|'" + myFunctions.getDateVAL(DateTime.Now.Date) + "'|'" + myFunctions.getDateVAL(dtpPayrunText.Value) + "'|0|'" + myFunctions.getDateVAL(DateTime.Now.Date) + "'|'" + myFunctions.GetFormatedDate_Ret_string(myCompanyID._SystemDate) + "'";//X_Batch added
//                         DupCriteria = "N_CompanyID=" + myCompanyID._CompanyID + " and N_PayrunID=" + dtpPayrunText.Value.Year.ToString("00##") + dtpPayrunText.Value.Month.ToString("0#") + " and X_Batch=''";
//                         dba.SaveData(ref TransResult, "Pay_MonthlyAddOrDed", "N_TransID", N_AddorDedID.ToString(), FieldList, FieldValues, DupCriteria, "");

//                         if (myFunctions.getIntVAL(TransResult.ToString()) > 0)
//                             N_AddorDedID = myFunctions.getIntVAL(TransResult.ToString());
//                         else
//                             B_Completed = false;
//                     }

//                 }
//                 Result = 0;
//                 FieldList = "N_CompanyID,N_FnYearID,X_PayrunText,D_DateFrom,D_DateTo,N_UserID,N_AddorDedID,N_BatchID,X_BatchCode,D_SalaryDate,N_TotalWorkingDays,N_TotalWorkedDays,N_CompMinutes,N_Ded,N_Ot,N_BranchID,N_GridDedTotal,N_TotalDutyHours";
//                 FieldValues = myCompanyID._CompanyID + "|" + myCompanyID._FnYearID + "|'" + dtpPayrunText.Text.ToString() + "'|'" + myFunctions.getDateVAL(dtpfromdate.Value.Date) + "'|'" + myFunctions.getDateVAL(dtptodate.Value.Date) + "'|" + myCompanyID._UserID.ToString() + "|" + N_AddorDedID.ToString() + "|" + PayrunID.ToString() + "|'" + txtBatchID.Text.Trim() + "'|'" + myFunctions.getDateVAL(dtpPayrunText.Value.Date) + "'|" + myFunctions.getVAL(lblTotWrkDayval.Text) + "|" + myFunctions.getVAL(lblWrkdDaysVal.Text) + "|" + myFunctions.getVAL(lblTotalNonCompMins.Text) + "|" + myFunctions.getVAL(txtNetDedApplicable.Text) + "|" + myFunctions.getVAL(txtNetAddApplicable.Text) + "|" + myCompanyID._BranchID + "|" + myFunctions.getVAL(lblDeductTotal.Text) + "|" + myFunctions.getVAL(lblOnOfficeTime.Text);
//                 // FieldValues = myCompanyID._CompanyID + "|" + myCompanyID._FnYearID + "|'" + dtpPayrunText.Text.ToString() + "'|'" + myFunctions.getDateVAL(dtpfromdate.Value.Date) + "'|'" + myFunctions.getDateVAL(dtptodate.Value.Date) + "'|" + myCompanyID._UserID.ToString() + "|" + N_TransID.ToString() + "|" + PayrunID.ToString() + "|'" + txtBatchID.Text.Trim() + "'|'" + myFunctions.getDateVAL(dtpPayrunText.Value.Date) + "'|" + myFunctions.getVAL(lblTotWrkDayval.Text) + "|" + myFunctions.getVAL(lblWrkdDaysVal.Text) + "|" + myFunctions.getVAL(lblTotalNonCompMins.Text) + "|" + myFunctions.getVAL(lblNetDeduction.Text) + "|" + myFunctions.getVAL(lblTotAddition.Text) + "|" + myCompanyID._BranchID + "|" + myFunctions.getVAL(lblDeductTotal.Text) + "|" + myFunctions.getVAL(lblOnOfficeTime.Text);

//                 myFunctions.saveApprovals(ref dba, ref FieldList, ref FieldValues, N_NextApprovalLevel, myFunctions.getIntVAL(btnSave.Tag.ToString()), N_SaveDraft, N_IsApprovalSystem, myFunctions.getIntVAL(MYG.ReturnFormID(this.Text)), 0);

//                 DupCriteria = "N_CompanyID=" + myCompanyID._CompanyID + " and X_BatchCode='" + txtBatchID.Text + "' and N_FnYearID =" + myCompanyID._FnYearID + " and N_EmpID=" + N_EmpID.ToString();
//                 string refField = "N_EmpID";
//                 string refValue = "Pay_Employee|N_EmpID|X_EmpCode='" + txtEmpCode.Text + "' and N_CompanyID=" + myCompanyID._CompanyID + " and N_FnYearID=" + myCompanyID._FnYearID;
//                 dba.SaveData(ref Result, "Pay_TimeSheetMaster", "N_TimeSheetID", N_TimeSheetID.ToString(), FieldList, FieldValues, refField, refValue, DupCriteria, "");
//                 if (myFunctions.getIntVAL(Result.ToString()) > 0)
//                 {
//                     N_TimeSheetID = myFunctions.getIntVAL(Result.ToString());

//                     myFunctions.logApprovals(ref dba, X_TransType, N_TimeSheetID, myFunctions.getIntVAL(MYG.ReturnFormID(this.Text)), X_Action, txtBatchID.Text, DateTime.Now, N_NextApprovalLevel, myCompanyID._UserID, myFunctions.getIntVAL(btnSave.Tag.ToString()), N_IsApprovalSystem, 0, 1, txtEmpName.Text,0,"", btnSave.Top, btnSave.Left);
//                     N_SaveDraft = myFunctions.getIntVAL(dba.ExecuteSclarNoErrorCatch("select CAST(B_IsSaveDraft as INT) from Pay_TimeSheetMaster where N_CompanyID=" + myCompanyID._CompanyID + " and N_TimeSheetID=" + N_TimeSheetID, "TEXT", new DataTable()).ToString());

//                     int N_AddorDedDetailID = 0;
//                     int N_Type = 0;
//                     double days = 0.0;
//                     for (int i = 1; i < flxTimesheetVerify.Rows; i++)
//                     {

//                         if (flxTimesheetVerify.get_TextMatrix(i, mcAttendence) == "P" || myFunctions.getVAL(flxTimesheetVerify.get_TextMatrix(i, mcAttendence)) != 0 || flxTimesheetVerify.get_TextMatrix(i, mcRemarks) != "")
//                         {
//                             days = 0.0;
//                             N_AddorDedDetailID = 0;
//                             N_OffID = 0;
//                             if (myFunctions.getVAL(flxTimesheetVerify.get_TextMatrix(i, mcOt)) != 0 && flxTimesheetVerify.get_TextMatrix(i, mcType) != "" && B_CategoryWiseAddition)
//                             {
//                                 int B_Dataflag = 0;

//                                 if (dsItemCategory.Tables["Pay_Monthlyaddorded"].Rows.Count > 0)
//                                 {
//                                     B_Dataflag = 0;
//                                     for (int k = 0; k < dsItemCategory.Tables["Pay_Monthlyaddorded"].Rows.Count; k++)
//                                     {
//                                         if (myFunctions.getIntVAL(dsItemCategory.Tables["Pay_Monthlyaddorded"].Rows[k]["N_PayID"].ToString()) == myFunctions.getIntVAL(flxTimesheetVerify.get_TextMatrix(i, mcPayID).ToString()))
//                                         {
//                                             B_Dataflag = 0;
//                                             dsItemCategory.Tables["Pay_Monthlyaddorded"].Rows[k]["N_OT"] = myFunctions.getVAL(dsItemCategory.Tables["Pay_Monthlyaddorded"].Rows[k]["N_OT"].ToString()) + myFunctions.getVAL(flxTimesheetVerify.get_TextMatrix(i, mcOt).ToString());
//                                             break;
//                                         }

//                                         B_Dataflag = 1;
//                                     }
//                                     if (B_Dataflag == 1)
//                                     {
//                                         dsItemCategory.Tables["Pay_Monthlyaddorded"].Rows.Add();
//                                         int row1 = dsItemCategory.Tables["Pay_Monthlyaddorded"].Rows.Count - 1;
//                                         dsItemCategory.Tables["Pay_Monthlyaddorded"].Rows[row1]["N_OT"] = myFunctions.getVAL(flxTimesheetVerify.get_TextMatrix(i, mcOt).ToString());
//                                         dsItemCategory.Tables["Pay_Monthlyaddorded"].Rows[row1]["N_PayID"] = myFunctions.getVAL(flxTimesheetVerify.get_TextMatrix(i, mcPayID).ToString());
//                                     }

//                                 }
//                                 else
//                                 {
//                                     dsItemCategory.Tables["Pay_Monthlyaddorded"].Rows.Add();
//                                     dsItemCategory.Tables["Pay_Monthlyaddorded"].Rows[0]["N_OT"] = myFunctions.getVAL(flxTimesheetVerify.get_TextMatrix(i, mcOt).ToString());
//                                     dsItemCategory.Tables["Pay_Monthlyaddorded"].Rows[0]["N_PayID"] = myFunctions.getVAL(flxTimesheetVerify.get_TextMatrix(i, mcPayID).ToString());
//                                 }
//                                 days = myFunctions.getVAL(flxTimesheetVerify.get_TextMatrix(i, mcOt));

//                             }
//                             else if (myFunctions.getVAL(flxTimesheetVerify.get_TextMatrix(i, mcDeduct)) != 0 && flxTimesheetVerify.get_TextMatrix(i, mcType) != "" && B_CategoryWiseDeduction)
//                             {
//                                 int B_Dataflag = 0;
//                                 N_OffID = 0;
//                                 if (dsItemCategory.Tables["Pay_Monthlyaddorded"].Rows.Count > 0)
//                                 {
//                                     B_Dataflag = 0;
//                                     for (int k = 0; k < dsItemCategory.Tables["Pay_Monthlyaddorded"].Rows.Count; k++)
//                                     {
//                                         if (myFunctions.getIntVAL(dsItemCategory.Tables["Pay_Monthlyaddorded"].Rows[k]["N_PayID"].ToString()) == myFunctions.getIntVAL(flxTimesheetVerify.get_TextMatrix(i, mcPayID).ToString()))
//                                         {
//                                             B_Dataflag = 0;
//                                             dsItemCategory.Tables["Pay_Monthlyaddorded"].Rows[k]["N_OT"] = myFunctions.getVAL(dsItemCategory.Tables["Pay_Monthlyaddorded"].Rows[k]["N_OT"].ToString()) + myFunctions.getVAL((-1 * myFunctions.getVAL(flxTimesheetVerify.get_TextMatrix(i, mcDeduct))).ToString());
//                                             break;
//                                         }

//                                         B_Dataflag = 1;
//                                     }
//                                     if (B_Dataflag == 1)
//                                     {
//                                         dsItemCategory.Tables["Pay_Monthlyaddorded"].Rows.Add();
//                                         int row1 = dsItemCategory.Tables["Pay_Monthlyaddorded"].Rows.Count - 1;
//                                         dsItemCategory.Tables["Pay_Monthlyaddorded"].Rows[row1]["N_OT"] = -1 * myFunctions.getVAL(flxTimesheetVerify.get_TextMatrix(i, mcDeduct).ToString());
//                                         dsItemCategory.Tables["Pay_Monthlyaddorded"].Rows[row1]["N_PayID"] = myFunctions.getVAL(flxTimesheetVerify.get_TextMatrix(i, mcPayID).ToString());
//                                     }

//                                 }
//                                 else
//                                 {
//                                     dsItemCategory.Tables["Pay_Monthlyaddorded"].Rows.Add();
//                                     dsItemCategory.Tables["Pay_Monthlyaddorded"].Rows[0]["N_OT"] = -1 * myFunctions.getVAL(flxTimesheetVerify.get_TextMatrix(i, mcDeduct).ToString());
//                                     dsItemCategory.Tables["Pay_Monthlyaddorded"].Rows[0]["N_PayID"] = myFunctions.getVAL(flxTimesheetVerify.get_TextMatrix(i, mcPayID).ToString());
//                                 }
//                                 days = -1 * myFunctions.getVAL(flxTimesheetVerify.get_TextMatrix(i, mcDeduct));

//                             }
//                             else
//                             {

//                                 N_OffID = 1;
//                                 if (myFunctions.getVAL(flxTimesheetVerify.get_TextMatrix(i, mcDeduct)) != 0)
//                                 {
//                                     days = -1 * myFunctions.getVAL(flxTimesheetVerify.get_TextMatrix(i, mcDeduct));
//                                 }
//                                 else if (myFunctions.getVAL(flxTimesheetVerify.get_TextMatrix(i, mcOt)) != 0)
//                                     days = myFunctions.getVAL(flxTimesheetVerify.get_TextMatrix(i, mcOt));
//                             }


//                             if (flxTimesheetVerify.get_TextMatrix(i, mcAttendence) == "P")
//                                 N_Type = 1;
//                             else if (flxTimesheetVerify.get_TextMatrix(i, mcAttendence) == "A")
//                                 N_Type = 2;
//                             else
//                                 N_Type = 0;
//                             DateTime dtIn = DateTime.Parse("00:00:00");
//                             DateTime dtOut = DateTime.Parse("00:00:00");
//                             DateTime dtIn1 = DateTime.Parse("00:00:00");
//                             DateTime dtOut1 = DateTime.Parse("00:00:00");

//                             if (flxTimesheetVerify.get_TextMatrix(i, mcIn) != "")
//                                 dtIn = DateTime.Parse(flxTimesheetVerify.get_TextMatrix(i, mcIn).Replace(".", ":"));
//                             if (flxTimesheetVerify.get_TextMatrix(i, mcOut) != "")
//                                 dtOut = DateTime.Parse(flxTimesheetVerify.get_TextMatrix(i, mcOut).Replace(".", ":"));
//                             if (flxTimesheetVerify.get_TextMatrix(i, mcIn2) != "")
//                                 dtIn1 = DateTime.Parse(flxTimesheetVerify.get_TextMatrix(i, mcIn2).Replace(".", ":"));
//                             if (flxTimesheetVerify.get_TextMatrix(i, mcOut2) != "")

//                                 dtOut1 = DateTime.Parse(flxTimesheetVerify.get_TextMatrix(i, mcOut2).Replace(".", ":"));

//                             FieldList = "N_CompanyID,N_TimeSheetID,N_BatchID,N_OT,N_OTPayID,X_Remarks,N_FnYearID,D_Date,D_In,D_Out,X_PayrunText,B_IsApproved,D_Shift2_In,D_Shift2_Out,N_TotalWorkHour,N_Status,N_Compensate,N_diff,N_DutyHours";
//                             FieldValues = myCompanyID._CompanyID + "|" + N_TimeSheetID.ToString() + "|" + PayrunID.ToString() + "|" + days.ToString() + "|" + myFunctions.getIntVAL(flxTimesheetVerify.get_TextMatrix(i, mcPayID).ToString()) + "|'" + flxTimesheetVerify.get_TextMatrix(i, mcRemarks) + "'|" + myCompanyID._FnYearID + "|'" + myFunctions.GetFormatedDate_Ret_string(flxTimesheetVerify.get_TextMatrix(i, mcDate)) + "'|'" + dtIn.ToString("HH:mm:ss") + "'|'" + dtOut.ToString("HH:mm:ss") + "'|'" + dtpPayrunText.Text.ToString() + "'|1|'" + dtIn1.ToString("HH:mm:ss") + "'|'" + dtOut1.ToString("HH:mm:ss") + "'|" + myFunctions.getVAL(flxTimesheetVerify.get_TextMatrix(i, mcTotHrs).ToString()) + "|" + N_Type + "|" + myFunctions.getVAL(flxTimesheetVerify.get_TextMatrix(i, mcCompDeduct).ToString()) + "|" + myFunctions.getVAL(flxTimesheetVerify.get_TextMatrix(i, mcDiff).ToString()) + "|" + myFunctions.getVAL(flxTimesheetVerify.get_TextMatrix(i, mcDutyHours).ToString());
//                             DupCriteria = "";
//                             RefFieldList = "N_EmpID";
//                             RefFileldDescr = "Pay_Employee|N_EmpID|X_EmpCode='" + txtEmpCode.Text.Trim() + "' and N_CompanyID =" + myCompanyID._CompanyID + " and N_FnYearID=" + myCompanyID._FnYearID;
//                             dba.SaveData(ref Result, "Pay_TimeSheet", "N_SheetID", flxTimesheetVerify.get_TextMatrix(i, mcSheetID), FieldList, FieldValues, RefFieldList, RefFileldDescr, DupCriteria, "");
//                             if (myFunctions.getIntVAL(Result.ToString()) < 0)
//                                 B_Completed = false;
//                         }
//                         //--- VACATION SAVE

//                         else if (flxTimesheetVerify.get_TextMatrix(i, mcAttendence) == "A" && B_VacFlag == true && myFunctions.getIntVAL(flxTimesheetVerify.get_TextMatrix(i, mcIsVacation)) == 0)
//                         {


//                             N_Absententry += 1;

//                             if (N_Vaccount == 0)
//                             {
//                                 D_VACFromDate = myFunctions.GetFormatedDate(flxTimesheetVerify.get_TextMatrix(i, mcDate));
//                                 D_TempVACToDate = myFunctions.GetFormatedDate(flxTimesheetVerify.get_TextMatrix(i, mcDate));

//                             }
//                             else if (N_Vaccount == i - 1)
//                             {
//                                 D_TempVACToDate = myFunctions.GetFormatedDate(flxTimesheetVerify.get_TextMatrix(i, mcDate));
//                             }
//                             else if (N_Vaccount != i - 1)
//                             {
//                                 D_VACToDate = D_TempVACToDate;
//                                 if (!SaveVacation(D_VACFromDate, D_VACToDate, 1))
//                                 {
//                                     dba.Rollback();
//                                     return;
//                                 }
//                                 D_VACFromDate = myFunctions.GetFormatedDate(flxTimesheetVerify.get_TextMatrix(i, mcDate));
//                                 D_TempVACToDate = myFunctions.GetFormatedDate(flxTimesheetVerify.get_TextMatrix(i, mcDate));

//                             }


//                             if (N_Absententry == N_AbsentCount)
//                             {


//                                 D_VACToDate = D_TempVACToDate;

//                                 if (!SaveVacation(D_VACFromDate, D_VACToDate, 1))
//                                 {
//                                     dba.Rollback();
//                                     return;
//                                 }


//                             }

//                             N_Vaccount = i;
//                         }
//                         N_totalDedution += myFunctions.getVAL(flxTimesheetVerify.get_TextMatrix(i, mcDeduct));
//                     }

//                     if (PaycodeSave())
//                     {
//                         obj = dba.ExecuteSclar(" select N_TransID from Pay_MonthlyAddOrDed where N_CompanyID=" + myCompanyID._CompanyID + " and N_PayrunID=" + dtpPayrunText.Value.Year.ToString("00##") + dtpPayrunText.Value.Month.ToString("0#") + "", "TEXT", new DataTable());

//                         //object N_TransDtlID = dba.ExecuteSclar("select N_TransDetailsID from Pay_MonthlyAddOrDedDetails where N_CompanyID=" + myCompanyID._CompanyID + " and N_TransID=" + obj.ToString() + " and N_EmpID=" + N_EmpID + " and N_refID=" + N_TimeSheetID + " and N_FormID=" + N_FormId, "TEXT", new DataTable());

//                         //if (obj != null && N_TransDtlID != null)
//                             //dba.DeleteData("Pay_MonthlyAddOrDedDetails", "N_TransDetailsID", N_TransDtlID.ToString(), "N_CompanyID=" + myCompanyID._CompanyID);
//                         if (!chkSProcessType.Checked)
//                         {
//                             //if (N_totalDedution == 0)
//                             //{
//                             //    CalculateNetDeduct();
//                             //}

                            
//                                 CalculateNetDeduct();
//                                 CalculateNetAddition();
                            

//                             if (dsItemCategory.Tables["Pay_Monthlyaddorded"].Rows.Count > 0)
//                             {
//                                 for (int k = 0; k < dsItemCategory.Tables["Pay_Monthlyaddorded"].Rows.Count; k++)
//                                 {
//                                     objdetails = -1;
//                                     //objdetails = dba.ExecuteSclar(" select B_TimeSheetEntry from Pay_MonthlyAddOrDedDetails where N_CompanyID=" + myCompanyID._CompanyID + " and N_TransID=" + N_AddorDedID + " and N_EmpID=" + N_EmpID + " and N_PayID=" + myFunctions.getIntVAL(dsItemCategory.Tables["Pay_Monthlyaddorded"].Rows[k]["N_PayID"].ToString()) + " and N_refID=" + N_TimeSheetID + " and N_FormID=" + N_FormId, "TEXT", new DataTable());
//                                     //if (objdetails == null)
//                                     //{
//                                     //    N_AddorDedDetailID = 0;

//                                     //}
//                                     //else if (objdetails.ToString() == "True")
//                                     //{
//                                     //    N_AddorDedDetailID = myFunctions.getIntVAL(dba.ExecuteSclar(" select N_TransDetailsID from Pay_MonthlyAddOrDedDetails where N_CompanyID=" + myCompanyID._CompanyID + " and N_TransID=" + N_AddorDedID + " and N_EmpID=" + N_EmpID + " and N_PayID=" + myFunctions.getIntVAL(dsItemCategory.Tables["Pay_Monthlyaddorded"].Rows[k]["N_PayID"].ToString()) + " and N_refID=" + N_TimeSheetID + " and N_FormID=" + N_FormId, "TEXT", new DataTable()).ToString());
//                                     //}
//                                     //else continue;
//                                     objdetails = dba.ExecuteSclar(" select N_TransDetailsID from Pay_MonthlyAddOrDedDetails where N_CompanyID=" + myCompanyID._CompanyID + " and N_TransID=" + N_AddorDedID + " and N_EmpID=" + N_EmpID + " and N_PayID=" + myFunctions.getIntVAL(dsItemCategory.Tables["Pay_Monthlyaddorded"].Rows[k]["N_PayID"].ToString()) + " and N_refID=" + N_TimeSheetID + " and N_FormID=" + N_FormId, "TEXT", new DataTable());
//                                     if (objdetails == null)
//                                     {
//                                         N_AddorDedDetailID = 0;

//                                     }
//                                     else 
//                                     {
//                                         N_AddorDedDetailID = myFunctions.getIntVAL(objdetails.ToString());
//                                     }

//                                     FieldList = "N_CompanyID,N_TransID,N_PayID,N_PayFactor,N_PayRate,N_HrsOrDays,N_ApplytoAmount,B_TimeSheetEntry,N_refID,N_FormID";
//                                     if (B_CompansateTime)
//                                     {
//                                         Netdays = 0;
//                                         if (myFunctions.getVAL(dsItemCategory.Tables["Pay_Monthlyaddorded"].Rows[k]["N_OT"].ToString()) > 0)
//                                         {
//                                             if (myFunctions.getVAL(txtNetAddApplicable.Text) > 0 && txtNetAddApplicable.Visible == true)
//                                             {
//                                                 Netdays = myFunctions.getVAL(txtNetAddApplicable.Text);
//                                             }
//                                             else
//                                             {
//                                                 Netdays = 0;
//                                             }
//                                             //if (myFunctions.getVAL(lblTotAddition.Text) > 0 && lblTotAddition.Visible == true)
//                                             //{
//                                             //    Netdays = myFunctions.getVAL(lblTotAddition.Text);
//                                             //}
//                                             //else
//                                             //{
//                                             //    Netdays = 0;
//                                             //}

//                                         }
//                                         else
//                                         {
//                                             if (myFunctions.getVAL(txtNetDedApplicable.Text) < 0)
//                                             {
//                                                 Netdays = myFunctions.getVAL(txtNetDedApplicable.Text);
//                                             }
//                                             else
//                                             {
//                                                 Netdays = -1 * myFunctions.getVAL(txtNetDedApplicable.Text);
//                                             }
//                                             //if (myFunctions.getVAL(lblNetDeduction.Text) < 0)
//                                             //{
//                                             //    Netdays = myFunctions.getVAL(lblNetDeduction.Text);
//                                             // }
//                                             //else
//                                             //{
//                                             //    Netdays = -1 * myFunctions.getVAL(lblNetDeduction.Text);
//                                             //}

//                                         }
//                                          FieldList = "N_CompanyID                   ,N_TransID,                     N_PayID,                                                                                           N_PayFactor,N_PayRate,                            N_HrsOrDays,N_ApplytoAmount,B_TimeSheetEntry,N_refID,N_FormID";
//                                         FieldValues = myCompanyID._CompanyID + "|" + N_AddorDedID.ToString() + "|" + myFunctions.getIntVAL(dsItemCategory.Tables["Pay_Monthlyaddorded"].Rows[k]["N_PayID"].ToString()) + "|0|" + Calculate_PayAmount(myFunctions.getIntVAL(dsItemCategory.Tables["Pay_Monthlyaddorded"].Rows[k]["N_PayID"].ToString()), Netdays) + "|" + Netdays + "|" + N_Payrate + "|1|" + N_TimeSheetID + "|" + N_FormId;
//                                     }
//                                     else
//                                         FieldValues = myCompanyID._CompanyID + "|" + N_AddorDedID.ToString() + "|" + myFunctions.getIntVAL(dsItemCategory.Tables["Pay_Monthlyaddorded"].Rows[k]["N_PayID"].ToString()) + "|0|" + Calculate_PayAmount(myFunctions.getIntVAL(dsItemCategory.Tables["Pay_Monthlyaddorded"].Rows[k]["N_PayID"].ToString()), myFunctions.getVAL(dsItemCategory.Tables["Pay_Monthlyaddorded"].Rows[k]["N_OT"].ToString())) + "|" + myFunctions.getVAL(dsItemCategory.Tables["Pay_Monthlyaddorded"].Rows[k]["N_OT"].ToString()) + "|" + N_Payrate + "|1|" + N_TimeSheetID + "|" + N_FormId;
                                     

//                                     DupCriteria = "";
//                                     RefFieldList = "N_EmpID";
//                                     RefFileldDescr = "Pay_Employee|N_EmpID|X_EmpCode='" + txtEmpCode.Text.Trim() + "' and N_CompanyID =" + myCompanyID._CompanyID + " and N_FnYearID=" + myCompanyID._FnYearID;
//                                     dba.SaveData(ref TransResult, "Pay_MonthlyAddOrDedDetails", "N_TransDetailsID", N_AddorDedDetailID.ToString(), FieldList, FieldValues, RefFieldList, RefFileldDescr, DupCriteria, "");
//                                     if (myFunctions.getIntVAL(TransResult.ToString()) > 0)
//                                     {
//                                         N_AddorDedDetailID = myFunctions.getIntVAL(TransResult.ToString());
//                                     }
//                                     else
//                                         B_Completed = false;

//                                 }

//                             }

//                         }
//                         else
//                         {
//                             objdetails = -1;
//                             //objdetails = dba.ExecuteSclar(" select B_TimeSheetEntry from Pay_MonthlyAddOrDedDetails where N_CompanyID=" + myCompanyID._CompanyID + " and N_TransID=" + N_AddorDedID + " and N_EmpID=" + N_EmpID + " and N_refID=" + N_TimeSheetID + " and N_FormID=" + N_FormId, "TEXT", new DataTable());
//                             //if (objdetails == null)
//                             //{
//                             //    N_AddorDedDetailID = 0;

//                             //}
//                             //else if (objdetails.ToString() == "True")
//                             //{
//                             //    N_AddorDedDetailID = myFunctions.getIntVAL(dba.ExecuteSclar(" select N_TransDetailsID from Pay_MonthlyAddOrDedDetails where N_CompanyID=" + myCompanyID._CompanyID + " and N_TransID=" + N_AddorDedID + " and N_EmpID=" + N_EmpID + " and N_refID=" + N_TimeSheetID + " and N_FormID=" + N_FormId, "TEXT", new DataTable()).ToString());
//                             //}
//                             objdetails = dba.ExecuteSclar(" select N_TransDetailsID from Pay_MonthlyAddOrDedDetails where N_CompanyID=" + myCompanyID._CompanyID + " and N_TransID=" + N_AddorDedID + " and N_EmpID=" + N_EmpID + " and N_PayID=" + N_AdjTypeID + " and N_refID=" + N_TimeSheetID + " and N_FormID=" + N_FormId, "TEXT", new DataTable());
//                             if (objdetails == null)
//                             {
//                                 N_AddorDedDetailID = 0;

//                             }
//                             else
//                             {
//                                 N_AddorDedDetailID = myFunctions.getIntVAL(objdetails.ToString());
//                             }
//                             FieldList = "N_CompanyID,N_TransID,N_PayID,N_PayFactor,N_PayRate,N_HrsOrDays,N_ApplytoAmount,B_TimeSheetEntry";
//                             FieldValues = myCompanyID._CompanyID + "|" + N_AddorDedID.ToString() + "|" + N_AdjTypeID.ToString() + "|0|" + Calculate_PayAmount(myFunctions.getIntVAL(N_AdjTypeID.ToString()), myFunctions.getVAL(txtAdjustment.Text)) + "|" + myFunctions.getVAL(txtAdjustment.Text) + "|" + N_Payrate + "|1";
//                             DupCriteria = "";
//                             RefFieldList = "N_EmpID";
//                             RefFileldDescr = "Pay_Employee|N_EmpID|X_EmpCode='" + txtEmpCode.Text.Trim() + "' and N_CompanyID =" + myCompanyID._CompanyID + " and N_FnYearID=" + myCompanyID._FnYearID;
//                             dba.SaveData(ref TransResult, "Pay_MonthlyAddOrDedDetails", "N_TransDetailsID", N_AddorDedDetailID.ToString(), FieldList, FieldValues, RefFieldList, RefFileldDescr, DupCriteria, "");
//                             if (myFunctions.getIntVAL(TransResult.ToString()) > 0)
//                             {
//                                 N_AddorDedDetailID = myFunctions.getIntVAL(TransResult.ToString());
//                             }
//                             else
//                                 B_Completed = false;
//                         }

//                     }
//                 }
//                 else
//                     B_Completed = false;
//                 if (B_Completed)
//                 {
//                     dba.Commit();
//                     msg.waitMsg(MYG.ReturnMultiLingualVal("-1111", "X_ControlNo", "Success"));
//                     ClearScrean();
//                 }
//                 else
//                 {
//                     dba.Rollback();
//                 }
//             }

//             catch (Exception ex)
//             {
//                 dba.Rollback();
//                 MYG.ResultMessage(lblResult, lblResultDescr, "Error!", ex.Message);
//             }
//             Cursor.Current = Cursors.Default;

//         }

//         private void CalculateNetDeduct()
//         {
//             //if (myFunctions.getVAL(lblNetDeduction.Text) > 0)
//             //{
//             //    dsItemCategory.Tables["Pay_Monthlyaddorded"].Rows.Add();
//             //    int row1 = dsItemCategory.Tables["Pay_Monthlyaddorded"].Rows.Count - 1;
//             //    dsItemCategory.Tables["Pay_Monthlyaddorded"].Rows[row1]["N_OT"] = -1 * myFunctions.getVAL(lblNetDeduction.Text);
//             //    dsItemCategory.Tables["Pay_Monthlyaddorded"].Rows[row1]["N_PayID"] = N_DeductionPayID;
//             //}
//             bool B_Datalog = false;
//             if (myFunctions.getVAL(txtNetDedApplicable.Text) >= 0 && B_CategoryWiseDeduction)
//             {
//                 if (dsItemCategory.Tables["Pay_Monthlyaddorded"].Rows.Count > 0)
//                     for (int k = 0; k < dsItemCategory.Tables["Pay_Monthlyaddorded"].Rows.Count; k++)
//                     {
//                         if (myFunctions.getIntVAL(dsItemCategory.Tables["Pay_Monthlyaddorded"].Rows[k]["N_PayID"].ToString()) == N_DeductionPayID)
//                         {
//                             dsItemCategory.Tables["Pay_Monthlyaddorded"].Rows[k]["N_OT"] = -1 * myFunctions.getVAL(txtNetDedApplicable.Text);
//                             B_Datalog = true;
//                             break;
//                         }
//                     }
//                 if (!B_Datalog)
//                 {
//                     dsItemCategory.Tables["Pay_Monthlyaddorded"].Rows.Add();
//                     int row1 = dsItemCategory.Tables["Pay_Monthlyaddorded"].Rows.Count - 1;
//                     dsItemCategory.Tables["Pay_Monthlyaddorded"].Rows[row1]["N_OT"] = -1 * myFunctions.getVAL(txtNetDedApplicable.Text);
//                     dsItemCategory.Tables["Pay_Monthlyaddorded"].Rows[row1]["N_PayID"] = N_DeductionPayID;
//                 }
//             }
//         }

//         private void CalculateNetAddition()
//         {
//             //if (myFunctions.getVAL(lblNetDeduction.Text) > 0)
//             //{
//             //    dsItemCategory.Tables["Pay_Monthlyaddorded"].Rows.Add();
//             //    int row1 = dsItemCategory.Tables["Pay_Monthlyaddorded"].Rows.Count - 1;
//             //    dsItemCategory.Tables["Pay_Monthlyaddorded"].Rows[row1]["N_OT"] = -1 * myFunctions.getVAL(lblNetDeduction.Text);
//             //    dsItemCategory.Tables["Pay_Monthlyaddorded"].Rows[row1]["N_PayID"] = N_DeductionPayID;
//             //}
//             bool B_Datalog = false;
//             if (myFunctions.getVAL(txtNetDedApplicable.Text) >= 0 && B_CategoryWiseAddition)
//             {
//                 if (dsItemCategory.Tables["Pay_Monthlyaddorded"].Rows.Count > 0)
//                     for (int k = 0; k < dsItemCategory.Tables["Pay_Monthlyaddorded"].Rows.Count; k++)
//                     {
//                         if (myFunctions.getIntVAL(dsItemCategory.Tables["Pay_Monthlyaddorded"].Rows[k]["N_PayID"].ToString()) == N_AdditionPayID)
//                         {
//                             dsItemCategory.Tables["Pay_Monthlyaddorded"].Rows[k]["N_OT"] = 1 * myFunctions.getVAL(txtNetAddApplicable.Text);
//                             B_Datalog = true;
//                             break;
//                         }
//                     }
//                 if (!B_Datalog)
//                 {
//                     dsItemCategory.Tables["Pay_Monthlyaddorded"].Rows.Add();
//                     int row1 = dsItemCategory.Tables["Pay_Monthlyaddorded"].Rows.Count - 1;
//                     dsItemCategory.Tables["Pay_Monthlyaddorded"].Rows[row1]["N_OT"] = 1 * myFunctions.getVAL(txtNetAddApplicable.Text);
//                     dsItemCategory.Tables["Pay_Monthlyaddorded"].Rows[row1]["N_PayID"] = N_AdditionPayID;
//                 }
//             }
//         }

//         private bool SaveVacation(DateTime FromDate, DateTime ToDate, int isreturn)
//         {

//             string X_VacCode = "";
//             object Vacresult = 0;
//             int N_VacationID = 0;

//             int VacID = 0;
//             object res = dba.ExecuteSclarNoErrorCatch("Select N_Value from Gen_Settings where N_CompanyID =" + myCompanyID._CompanyID + " and X_Group= 'HR' and X_Description='Default AbsentType'", "TEXT", new DataTable()).ToString();
//             if (res != null)
//                 VacID = myFunctions.getIntVAL(res.ToString());

//             if (VacID == 0)
//             {
//                 msg.msgInformation("Please set the deafult Absent Paycode");
//                 return false;
//             }


//             while (true)
//             {
//                 X_VacCode = dba.ExecuteSclarNoErrorCatch("SP_AutoNumberGenerate " + myCompanyID._CompanyID + "," + myCompanyID._FnYearID + ",210", "TEXT", new DataTable()).ToString();
//                 object N_Result = dba.ExecuteSclarNoErrorCatch("Select 1 from Pay_VacationDetails Where X_VacationCode ='" + X_VacCode + "' and N_CompanyID= " + myCompanyID._CompanyID, "TEXT", new DataTable());
//                 if (N_Result == null)
//                     break;
//             }

//             if (X_VacCode == "")
//             {
//                 dba.Rollback();
//                 msg.msgInformation(MYG.ReturnMultiLingualVal("-1111", "X_ControlNo", "AutoGenerateError"));
//                 return false;
//             }

//             TimeSpan day = ToDate.Subtract(FromDate);
//             int days = myFunctions.getIntVAL(Convert.ToString(day.Days + 1));



//             string FieldList = "N_CompanyID,D_VacReqestDate,D_VacSanctionDate,D_VacDateFrom,D_VacDateTo,N_VacDays ,D_VacApprovedDate,N_UserID,N_TransType,N_VacRequestID,N_FnYearID,N_VacationStatus,B_IsAdjustEntry,B_Ticket,B_ReEntry,X_VacationCode,N_FormId";
//             string FieldValues = myCompanyID._CompanyID + "|'" + myFunctions.getDateVAL(FromDate) + "'|'" + myFunctions.GetFormatedDate_Ret_string(myCompanyID._SystemDate) + "'|'" + myFunctions.getDateVAL(FromDate) + "'|'" + myFunctions.getDateVAL(ToDate) + "'|'" + (-1 * myFunctions.getIntVAL(days.ToString())) + "'|'" + myFunctions.getDateVAL(ToDate) + "'|" + myCompanyID._UserID + "|1|0|" + myCompanyID._FnYearID + "|0|0|0|0|'" + X_VacCode + "'|216";
//             string DupCriteria = "";
//             string RefFieldList = "N_VacTypeID,N_EmpID,N_BranchID";
//             string RefFileldDescr = "Gen_Settings|N_Value|N_CompanyID =" + myCompanyID._CompanyID + " and X_Group= 'HR' and X_Description='Default AbsentType'|Pay_Employee|N_EmpID|X_EmpCode='" + txtEmpCode.Text.Trim() + "' and N_CompanyID =" + myCompanyID._CompanyID + " and N_FnYearID=" + myCompanyID._FnYearID + "|Pay_Employee|N_BranchID|X_EmpCode='" + txtEmpCode.Text.Trim() + "' and N_CompanyID =" + myCompanyID._CompanyID + " and N_FnYearID=" + myCompanyID._FnYearID;
//             dba.SaveData(ref Vacresult, "Pay_VacationDetails", "N_VacationID", "0", FieldList, FieldValues, RefFieldList, RefFileldDescr, DupCriteria, "");


//             if (myFunctions.getIntVAL(Vacresult.ToString()) > 0)
//             {


//                 N_VacationID = myFunctions.getIntVAL(Vacresult.ToString());


//                 if (isreturn == 1)
//                 {
//                     string X_VacationReturnCode = dba.ExecuteSclarNoErrorCatch("SP_AutoNumberGenerate " + myCompanyID._CompanyID + "," + myCompanyID._FnYearID + ",463", "TEXT", new DataTable()).ToString();

//                     FieldList = "N_CompanyID,X_VacationReturnCode,N_VacationID,D_ReturnDate,N_FnYearID";
//                     FieldValues = myCompanyID._CompanyID + "|'" + X_VacationReturnCode + "'|" + N_VacationID + "|'" + myFunctions.getDateVAL(ToDate.AddDays(1)) + "'|" + myCompanyID._FnYearID;
//                     DupCriteria = "";
//                     RefFieldList = "N_EmpID,N_BranchID";
//                     RefFileldDescr = "Pay_Employee|N_EmpID|X_EmpCode='" + txtEmpCode.Text.Trim() + "' and N_CompanyID =" + myCompanyID._CompanyID + " and N_FnYearID=" + myCompanyID._FnYearID + "|Pay_Employee|N_BranchID|X_EmpCode='" + txtEmpCode.Text.Trim() + "' and N_CompanyID =" + myCompanyID._CompanyID + " and N_FnYearID=" + myCompanyID._FnYearID;

//                     dba.SaveData(ref Vacresult, "Pay_VacationReturn", "N_VacationReturnID", "0", FieldList, FieldValues, RefFieldList, RefFileldDescr, DupCriteria, "");
//                 }

//             }
//             return true;

//         }
//         private Double Calculate_PayAmount(int N_PayID, double Payhrs)
//         {
//             double amount = 0;
//             int N_Type = 0;
//             for (int j = 0; j < dsItemCategory.Tables["Pay_Payrate"].Rows.Count; j++)
//             {
//                 if (myFunctions.getIntVAL(dsItemCategory.Tables["Pay_Payrate"].Rows[j]["N_PayID"].ToString()) == N_PayID)
//                 {
//                     if (myFunctions.getIntVAL(dsItemCategory.Tables["Pay_Payrate"].Rows[j]["N_PayMethod"].ToString()) == 4)
//                         amount = myFunctions.getVAL(dsItemCategory.Tables["Pay_Payrate"].Rows[j]["N_Value"].ToString()) * Payhrs;
//                     else
//                         amount = (myFunctions.getVAL(dsItemCategory.Tables["Pay_Payrate"].Rows[j]["N_SummaryValue"].ToString()) / N_TotalDays) * myFunctions.getVAL(dsItemCategory.Tables["Pay_Payrate"].Rows[j]["N_Percentage"].ToString()) * Payhrs;
//                     N_Type = myFunctions.getIntVAL(dsItemCategory.Tables["Pay_Payrate"].Rows[j]["N_Type"].ToString());
//                     N_Payrate = myFunctions.getVAL(dsItemCategory.Tables["Pay_Payrate"].Rows[j]["N_SummaryValue"].ToString());
//                     break;
//                 }
//             }
//             if (N_Type == 1)

//                 amount = -1 * amount;

//             return amount;
//         }

//         private void dtpfromdate_ValueChanged(object sender, EventArgs e)
//         {
//             ViewData();
//         }

//         private void dtptodate_ValueChanged(object sender, EventArgs e)
//         {
//             ViewData();
//         }
//         private void SetGridTextBox()
//         {
//             int left = 0;

//             for (int i = 0; i < flxTimesheetVerify.Col; i++)
//             {
//                 left = left + (int)flxTimesheetVerify.get_ColWidth(i) / 15;
//             }

//             if (flxTimesheetVerify.Col == mcType)
//             {

//                 left = (int)(flxTimesheetVerify.Left) + left;
//                 //
//                 txtGridTextBox.Left = left + 2;
//                 txtGridTextBox.Width = (int)flxTimesheetVerify.get_ColWidth(flxTimesheetVerify.Col) / 15 - btnGridAccount.Width - 1;
//                 txtGridTextBox.Top = ((int)flxTimesheetVerify.Top + (flxTimesheetVerify.CellTop / 15)) + 3;
//                 txtGridTextBox.TextAlign = HorizontalAlignment.Left;
//                 //
//                 btnGridAccount.Left = left + (int)flxTimesheetVerify.get_ColWidth(flxTimesheetVerify.Col) / 15 - btnGridAccount.Width + 2;
//                 btnGridAccount.Top = ((int)flxTimesheetVerify.Top + (flxTimesheetVerify.CellTop / 15)) - 1;
//                 btnGridAccount.Height = (flxTimesheetVerify.CellHeight / 15) + 2;
//                 btnGridAccount.Visible = true;
//             }
//             else
//             {
//                 btnGridAccount.Visible = false;
//                 txtGridTextBox.Left = (int)(flxTimesheetVerify.Left) + left + 2;
//                 txtGridTextBox.Width = (int)flxTimesheetVerify.get_ColWidth(flxTimesheetVerify.Col) / 15 - 2;
//                 txtGridTextBox.TextAlign = HorizontalAlignment.Left;
//             }

//             txtGridTextBox.Top = ((int)flxTimesheetVerify.Top + (flxTimesheetVerify.CellTop / 15)) + 3;
//             txtGridTextBox.Height = (flxTimesheetVerify.CellHeight / 15);
//             txtGridTextBox.Text = flxTimesheetVerify.Text;
//             X_GridPrevVal = flxTimesheetVerify.Text;
//             txtGridTextBox.Visible = true;
//             txtGridTextBox.SelectAll();
//             txtGridTextBox.Focus();


//         }

//         private void flxFeeDetails_ClickEvent_1(object sender, EventArgs e)
//         {
//             if (flxTimesheetVerify.Row == 0) return;
//             if (txtEmpCode.Text == "")
//             { //txtEmpCode.Focus();
//                 return;
//             }

//             if (flxTimesheetVerify.Col == mcApproved)
//             {
//                 if (flxTimesheetVerify.Text == "P")
//                 {
//                     flxTimesheetVerify.set_TextMatrix(flxTimesheetVerify.Row, mcApproved, "O");
//                 }
//                 else
//                 {
//                     flxTimesheetVerify.set_TextMatrix(flxTimesheetVerify.Row, mcApproved, "P");
//                 }

//                 if (X_PrevValue != flxTimesheetVerify.get_TextMatrix(flxTimesheetVerify.Row, mcApproved))
//                     B_SaveChanges = true;
//             }

//             if (flxTimesheetVerify.Col == mcAttendence)
//             {
//                 if (flxTimesheetVerify.Text == "P")
//                 {

//                     flxTimesheetVerify.set_TextMatrix(flxTimesheetVerify.Row, mcAttendence, "A");
//                     flxTimesheetVerify.Col = mcAttendence;
//                     flxTimesheetVerify.CellForeColor = Color.Red;
//                     flxTimesheetVerify.set_TextMatrix(flxTimesheetVerify.Row, mcType, X_DefaultAbsentCode);
//                     flxTimesheetVerify.set_TextMatrix(flxTimesheetVerify.Row, mcPayID, N_DefaultAbsentID.ToString());

//                 }

//                 else if (flxTimesheetVerify.get_TextMatrix(flxTimesheetVerify.Row, mcIsVacation).ToString() == "0" || flxTimesheetVerify.get_TextMatrix(flxTimesheetVerify.Row, mcIsVacation).ToString() == "")
//                 {
//                     if (flxTimesheetVerify.get_TextMatrix(flxTimesheetVerify.Row, mcDiff) == "")
//                     {
//                         flxTimesheetVerify.set_TextMatrix(flxTimesheetVerify.Row, mcAttendence, "P");
//                         flxTimesheetVerify.set_TextMatrix(flxTimesheetVerify.Row, mcType, "");
//                         flxTimesheetVerify.set_TextMatrix(flxTimesheetVerify.Row, mcPayID, "0");
//                         flxTimesheetVerify.CellForeColor = Color.Green;
//                     }
//                     else
//                     {
//                         if (myFunctions.getVAL(flxTimesheetVerify.get_TextMatrix(flxTimesheetVerify.Row, mcDiff)) > 0)
//                         {
//                             flxTimesheetVerify.set_TextMatrix(flxTimesheetVerify.Row, mcAttendence, "P");
//                             flxTimesheetVerify.set_TextMatrix(flxTimesheetVerify.Row, mcType, X_Additions);
//                             flxTimesheetVerify.set_TextMatrix(flxTimesheetVerify.Row, mcPayID, N_AdditionPayID.ToString());
//                             flxTimesheetVerify.CellForeColor = Color.Green;
//                         }
//                         else if (myFunctions.getVAL(flxTimesheetVerify.get_TextMatrix(flxTimesheetVerify.Row, mcDiff)) < 0)
//                         {
//                             flxTimesheetVerify.set_TextMatrix(flxTimesheetVerify.Row, mcAttendence, "P");
//                             flxTimesheetVerify.set_TextMatrix(flxTimesheetVerify.Row, mcType, X_Deductions);
//                             flxTimesheetVerify.set_TextMatrix(flxTimesheetVerify.Row, mcPayID, N_DeductionPayID.ToString());
//                             flxTimesheetVerify.CellForeColor = Color.Green;
//                         }
//                         else
//                         {
//                             flxTimesheetVerify.set_TextMatrix(flxTimesheetVerify.Row, mcAttendence, "P");
//                             flxTimesheetVerify.set_TextMatrix(flxTimesheetVerify.Row, mcType, "");
//                             flxTimesheetVerify.set_TextMatrix(flxTimesheetVerify.Row, mcPayID, "0");
//                             flxTimesheetVerify.CellForeColor = Color.Green;
//                         }
//                     }

//                 }
//                 CalculateAbsent();
//                 if (X_PrevValue != flxTimesheetVerify.get_TextMatrix(flxTimesheetVerify.Row, mcAttendence))
//                     B_SaveChanges = true;
//             }

//             switch (flxTimesheetVerify.Col)
//             {
//                 case mcOt:
//                     txtGridTextBox.SelectAll();
//                     break;
//                 case mcRemarks:
//                     txtGridTextBox.SelectAll();
//                     break;
//             }
//         }

//         private void flxFeeDetails_KeyPressEvent(object sender, AxMSFlexGridLib.DMSFlexGridEvents_KeyPressEvent e)
//         {
//             if (e.keyAscii == 32)
//             {
//                 if (flxTimesheetVerify.Row == 0) return;
//                 if (txtEmpCode.Text == "")
//                 { //txtEmpCode.Focus();
//                     return;
//                 }
//                 if (flxTimesheetVerify.Col == mcApproved)
//                     if (flxTimesheetVerify.Text == "P")
//                     {
//                         flxTimesheetVerify.set_TextMatrix(flxTimesheetVerify.Row, mcApproved, "O");
//                     }
//                     else
//                     {
//                         flxTimesheetVerify.set_TextMatrix(flxTimesheetVerify.Row, mcApproved, "P");

//                     }

//                 if (flxTimesheetVerify.Col == mcAttendence)
//                     if (flxTimesheetVerify.Text == "P")
//                     {
//                         flxTimesheetVerify.set_TextMatrix(flxTimesheetVerify.Row, mcAttendence, "A");
//                         flxTimesheetVerify.Col = mcAttendence;
//                         flxTimesheetVerify.CellForeColor = Color.Red;
//                     }
//                     else if (flxTimesheetVerify.get_TextMatrix(flxTimesheetVerify.Row, mcIsVacation).ToString() == "0" || flxTimesheetVerify.get_TextMatrix(flxTimesheetVerify.Row, mcIsVacation).ToString() == "")
//                     {
//                         flxTimesheetVerify.set_TextMatrix(flxTimesheetVerify.Row, mcAttendence, "P");
//                         flxTimesheetVerify.CellForeColor = Color.Green;
//                     }
//             }


//             if (e.keyAscii == 27)
//             {
//                 if (flxTimesheetVerify.Col == mcAttendence)
//                     flxTimesheetVerify.set_TextMatrix(flxTimesheetVerify.Row, mcAttendence, "");

//             }


//         }

//         private void txtGridTextBox_KeyDown(object sender, KeyEventArgs e)
//         {
//             if (e.KeyCode == Keys.Enter)
//             {
//                 switch (flxTimesheetVerify.Col)
//                 {
//                     case mcIn:
//                         if (myCompanyID._RightToLeft)
//                             flxTimesheetVerify.Col = mcOut;
//                         else
//                             flxTimesheetVerify.Col = mcOut;
//                         break;
//                     case mcOut:
//                         if (myCompanyID._RightToLeft)
//                             flxTimesheetVerify.Col = mcIn2;
//                         else
//                             flxTimesheetVerify.Col = mcIn2;
//                         break;
//                     case mcIn2:
//                         if (myCompanyID._RightToLeft)
//                             flxTimesheetVerify.Col = mcOut2;
//                         else
//                             flxTimesheetVerify.Col = mcOut2;
//                         break;
//                     case mcOut2:
//                         if (myCompanyID._RightToLeft)
//                             flxTimesheetVerify.Col = mcTotHrs;
//                         else
//                             flxTimesheetVerify.Col = mcTotHrs;
//                         break;
//                     case mcOt:
//                         flxTimesheetVerify.set_TextMatrix(flxTimesheetVerify.Row, mcOt, txtGridTextBox.Text.Trim());

//                         if (myFunctions.getVAL(flxTimesheetVerify.get_TextMatrix(flxTimesheetVerify.Row, mcOt)) > 0)
//                         {
//                             flxTimesheetVerify.set_TextMatrix(flxTimesheetVerify.Row, mcType, X_Additions);
//                             flxTimesheetVerify.set_TextMatrix(flxTimesheetVerify.Row, mcPayID, N_AdditionPayID.ToString());
//                             flxTimesheetVerify.set_TextMatrix(flxTimesheetVerify.Row, mcAttendence, "P");
//                             flxTimesheetVerify.Col = mcAttendence;
//                             flxTimesheetVerify.CellForeColor = Color.Green;
//                         }
//                         else
//                         {
//                             flxTimesheetVerify.set_TextMatrix(flxTimesheetVerify.Row, mcType, "");
//                             flxTimesheetVerify.set_TextMatrix(flxTimesheetVerify.Row, mcPayID, "0");
//                             flxTimesheetVerify.set_TextMatrix(flxTimesheetVerify.Row, mcAttendence, "P");
//                             flxTimesheetVerify.Col = mcAttendence;
//                             flxTimesheetVerify.CellForeColor = Color.Green;
//                         }
//                         if (myFunctions.getVAL(flxTimesheetVerify.get_TextMatrix(flxTimesheetVerify.Row, mcOt)) != 0)
//                             flxTimesheetVerify.Col = mcType;
//                         else
//                             flxTimesheetVerify.Col = mcRemarks;
//                         break;
//                     case mcDeduct:
//                         flxTimesheetVerify.set_TextMatrix(flxTimesheetVerify.Row, mcDeduct, txtGridTextBox.Text.Trim());
//                         if (myFunctions.getVAL(flxTimesheetVerify.get_TextMatrix(flxTimesheetVerify.Row, mcDeduct)) > 0)
//                         {
//                             flxTimesheetVerify.set_TextMatrix(flxTimesheetVerify.Row, mcType, X_Deductions);
//                             flxTimesheetVerify.set_TextMatrix(flxTimesheetVerify.Row, mcPayID, N_DeductionPayID.ToString());
//                             flxTimesheetVerify.set_TextMatrix(flxTimesheetVerify.Row, mcAttendence, "P");
//                             flxTimesheetVerify.Col = mcAttendence;
//                             flxTimesheetVerify.CellForeColor = Color.Green;
//                         }
//                         else
//                         {
//                             flxTimesheetVerify.set_TextMatrix(flxTimesheetVerify.Row, mcType, "");
//                             flxTimesheetVerify.set_TextMatrix(flxTimesheetVerify.Row, mcPayID, "0");
//                             flxTimesheetVerify.set_TextMatrix(flxTimesheetVerify.Row, mcAttendence, "P");
//                             flxTimesheetVerify.Col = mcAttendence;
//                             flxTimesheetVerify.CellForeColor = Color.Green;
//                         }

//                         if (myFunctions.getVAL(flxTimesheetVerify.get_TextMatrix(flxTimesheetVerify.Row, mcDeduct)) != 0)
//                             flxTimesheetVerify.Col = mcType;
//                         else
//                             flxTimesheetVerify.Col = mcRemarks;
//                         break;
//                     case mcTotHrs:
//                         if (myFunctions.getVAL(flxTimesheetVerify.get_TextMatrix(flxTimesheetVerify.Row, mcDiff)) > 0)
//                             flxTimesheetVerify.Col = mcOt;
//                         else
//                             flxTimesheetVerify.Col = mcDeduct;
//                         break;
//                     case mcType:
//                         flxTimesheetVerify.set_TextMatrix(flxTimesheetVerify.Row, mcType, txtGridTextBox.Text.Trim());
//                         flxTimesheetVerify.Col = mcRemarks;
//                         break;
//                     case mcRemarks:
//                         flxTimesheetVerify.set_TextMatrix(flxTimesheetVerify.Row, mcRemarks, txtGridTextBox.Text.Trim());
//                         if (B_ManualEntry_InGrid)
//                         {
//                             if (flxTimesheetVerify.Row + 1 < flxTimesheetVerify.Rows)
//                             {
//                                 flxTimesheetVerify.Row = flxTimesheetVerify.Row + 1;
//                                 flxTimesheetVerify.Col = mcIn;
//                             }
//                             else
//                                 flxTimesheetVerify.Col = mcIn;
//                         }
//                         else
//                         {
//                             if (flxTimesheetVerify.Row + 1 < flxTimesheetVerify.Rows)
//                             {
//                                 flxTimesheetVerify.Row = flxTimesheetVerify.Row + 1;
//                                 flxTimesheetVerify.Col = mcOt;
//                             }
//                             else
//                                 flxTimesheetVerify.Col = mcOt;
//                         }
//                         // flxFeeDetails.Col = mcApproved ;
//                         break;
//                     case mcApproved:
//                         if (flxTimesheetVerify.Row + 1 < flxTimesheetVerify.Rows)
//                         {
//                             flxTimesheetVerify.Row = flxTimesheetVerify.Row + 1;
//                             flxTimesheetVerify.Col = mcOt;
//                         }
//                         else
//                             flxTimesheetVerify.Col = mcOt;
//                         break;
//                     default:
//                         break;
//                 }
//                 e.Handled = true;
//             }
//             else if (e.KeyCode == Keys.Left)
//             {
//                 switch (flxTimesheetVerify.Col)
//                 {
//                     case mcIn:
//                         if (myCompanyID._RightToLeft)
//                             flxTimesheetVerify.Col = mcOut;
//                         else
//                             flxTimesheetVerify.Col = mcDate;
//                         break;
//                     case mcOut:
//                         if (myCompanyID._RightToLeft)
//                             flxTimesheetVerify.Col = mcIn2;
//                         else
//                             flxTimesheetVerify.Col = mcIn;
//                         break;
//                     case mcIn2:
//                         if (myCompanyID._RightToLeft)
//                             flxTimesheetVerify.Col = mcOut2;
//                         else
//                             flxTimesheetVerify.Col = mcOut;
//                         break;
//                     case mcOut2:
//                         if (myCompanyID._RightToLeft)
//                             flxTimesheetVerify.Col = mcTotHrs;
//                         else
//                             flxTimesheetVerify.Col = mcIn2;
//                         break;
//                     case mcOt:
//                         if (txtGridTextBox.Text.Length == txtGridTextBox.SelectedText.Length)
//                         {
//                             if (myCompanyID._RightToLeft)
//                                 flxTimesheetVerify.Col = mcType;
//                             else
//                                 flxTimesheetVerify.Col = mcTotHrs;
//                         }

//                         break;
//                     case mcTotHrs:
//                         if (txtGridTextBox.Text.Length == txtGridTextBox.SelectedText.Length)
//                             if (myCompanyID._RightToLeft)
//                                 flxTimesheetVerify.Col = mcOt;
//                         break;
//                     case mcType:
//                         if (txtGridTextBox.Text.Length == txtGridTextBox.SelectedText.Length)
//                         {
//                             if (myCompanyID._RightToLeft)
//                                 flxTimesheetVerify.Col = mcRemarks;
//                             else
//                                 flxTimesheetVerify.Col = mcOt;
//                         }
//                         break;
//                     case mcRemarks:
//                         if (txtGridTextBox.Text.Length == txtGridTextBox.SelectedText.Length)
//                         {
//                             if (myCompanyID._RightToLeft)
//                             {

//                                 if (txtGridTextBox.Text.Length == txtGridTextBox.SelectedText.Length)
//                                     if (flxTimesheetVerify.Row + 1 < flxTimesheetVerify.Rows)
//                                     {
//                                         flxTimesheetVerify.Row = flxTimesheetVerify.Row + 1;
//                                         if (B_ManualEntry_InGrid)
//                                             flxTimesheetVerify.Col = mcIn;
//                                         else
//                                             flxTimesheetVerify.Col = mcOt;
//                                     }
//                             }
//                             // flxFeeDetails.Col = mcApproved;
//                             else
//                                 flxTimesheetVerify.Col = mcType;
//                         }
//                         break;
//                     case mcApproved:
//                         if (txtGridTextBox.Text.Length == txtGridTextBox.SelectedText.Length)
//                             if (flxTimesheetVerify.Row + 1 < flxTimesheetVerify.Rows)
//                             {
//                                 flxTimesheetVerify.Row = flxTimesheetVerify.Row + 1;
//                                 flxTimesheetVerify.Col = mcOt;
//                             }
//                         break;
//                     default:
//                         break;
//                 }
//                 if (txtGridTextBox.SelectedText.Length > 0)
//                     e.Handled = true;
//             }
//             else if (e.KeyCode == Keys.Right)
//             {

//                 switch (flxTimesheetVerify.Col)
//                 {
//                     case mcIn:
//                         if (myCompanyID._RightToLeft)
//                             flxTimesheetVerify.Col = mcDate;
//                         else
//                             flxTimesheetVerify.Col = mcOut;
//                         break;
//                     case mcOut:
//                         if (myCompanyID._RightToLeft)
//                             flxTimesheetVerify.Col = mcIn;
//                         else
//                             flxTimesheetVerify.Col = mcIn2;
//                         break;
//                     case mcIn2:
//                         if (myCompanyID._RightToLeft)
//                             flxTimesheetVerify.Col = mcOut;
//                         else
//                             flxTimesheetVerify.Col = mcOut2;
//                         break;
//                     case mcOut2:
//                         if (myCompanyID._RightToLeft)
//                             flxTimesheetVerify.Col = mcIn2;
//                         else
//                             flxTimesheetVerify.Col = mcTotHrs;
//                         break;
//                     case mcTotHrs:
//                         break;
//                     case mcType:
//                         if (txtGridTextBox.Text.Length == txtGridTextBox.SelectedText.Length)
//                         {
//                             if (myCompanyID._RightToLeft)
//                                 flxTimesheetVerify.Col = mcRemarks;
//                             else
//                                 flxTimesheetVerify.Col = mcOt;
//                         }
//                         break;
//                     case mcOt:
//                         if (txtGridTextBox.Text.Length == txtGridTextBox.SelectedText.Length)
//                         {
//                             if (myCompanyID._RightToLeft)
//                                 flxTimesheetVerify.Col = mcType;
//                             else
//                                 flxTimesheetVerify.Col = mcTotHrs;
//                         }
//                         break;
//                     case mcRemarks:
//                         if (txtGridTextBox.Text.Length == txtGridTextBox.SelectedText.Length)
//                         {
//                             if (myCompanyID._RightToLeft)
//                                 flxTimesheetVerify.Col = mcIn;
//                             // flxFeeDetails.Col = mcApproved;
//                             else
//                                 flxTimesheetVerify.Col = mcType;
//                         }
//                         break;
//                     case mcApproved:
//                         if (txtGridTextBox.Text.Length == txtGridTextBox.SelectedText.Length)
//                             flxTimesheetVerify.Col = mcRemarks;
//                         break;
//                     default:
//                         break;
//                 }
//                 if (txtGridTextBox.SelectedText.Length > 0)
//                     e.Handled = true;
//             }
//             else if (e.KeyCode == Keys.Down)
//             {
//                 if (flxTimesheetVerify.Row + 1 < flxTimesheetVerify.Rows)
//                     flxTimesheetVerify.Row += 1;
//                 e.Handled = true;
//             }
//             else if (e.KeyCode == Keys.Up)
//             {
//                 if (flxTimesheetVerify.Row > 1)
//                     flxTimesheetVerify.Row -= 1;
//                 e.Handled = true;
//             }
//             else if (e.KeyCode == Keys.Escape)
//             {
//                 txtGridTextBox.Text = X_GridPrevVal;
//                 txtGridTextBox.SelectAll();
//                 X_GridPrevVal = "";
//                 e.Handled = true;
//             }
//             else if (e.KeyCode == Keys.F2)
//             {
//                 txtGridTextBox.SelectionLength = 0;
//                 e.Handled = true;
//             }

//             if (e.KeyCode == Keys.Down)
//             {
//                 if (e.Alt)
//                 {
//                     switch (flxTimesheetVerify.Col)
//                     {
//                         case mcType:
//                             btnGridAccount_Click(sender, new EventArgs());
//                             break;
//                         default:
//                             break;
//                     }
//                 }
//             }
//         }

//         private void txtGridTextBox_KeyPress(object sender, KeyPressEventArgs e)
//         {
//             if (flxTimesheetVerify.Row <= 0) return;
//             if (!txtGridTextBox.Visible) return;
//             switch (flxTimesheetVerify.Col)
//             {

//                 case mcOt:
//                 case mcTotHrs:
//                     MYG.check4Numeric(e, txtGridTextBox.Text.Trim());
//                     break;
//                 case mcRemarks:
//                 case mcType:
//                     MYG.check4SplChar(e);
//                     break;
//                 default:
//                     break;
//             }
//             if (e.KeyChar == 13 || e.KeyChar == 27)
//                 e.Handled = true;
//         }

//         private void txtGridTextBox_TextChanged(object sender, EventArgs e)
//         {
//             if (flxTimesheetVerify.Row <= 0) return;
//             if (!txtGridTextBox.Visible) return;
//             switch (flxTimesheetVerify.Col)
//             {
//                 case mcIn:
//                     flxTimesheetVerify.Text = txtGridTextBox.Text;
//                     break;
//                 case mcOut:
//                     flxTimesheetVerify.Text = txtGridTextBox.Text;
//                     break;
//                 case mcIn2:
//                     flxTimesheetVerify.Text = txtGridTextBox.Text;
//                     break;
//                 case mcOut2:
//                     flxTimesheetVerify.Text = txtGridTextBox.Text;
//                     break;
//                 case mcOt:
//                     flxTimesheetVerify.Text = txtGridTextBox.Text;
//                     if (myFunctions.getVAL(flxTimesheetVerify.Text) > 0)
//                         flxTimesheetVerify.set_TextMatrix(flxTimesheetVerify.Row, mcDeduct, "");
//                     CalcAdditionDeduction();
//                     break;
//                 case mcDeduct:
//                     flxTimesheetVerify.Text = txtGridTextBox.Text;
//                     if (myFunctions.getVAL(flxTimesheetVerify.Text) > 0)
//                         flxTimesheetVerify.set_TextMatrix(flxTimesheetVerify.Row, mcOt, "");
//                     CalcAdditionDeduction();
//                     break;
//                 case mcTotHrs:
//                     flxTimesheetVerify.Text = myFunctions.getVAL(txtGridTextBox.Text).ToString();
//                     flxTimesheetVerify.set_TextMatrix(flxTimesheetVerify.Row, mcDiff, (myFunctions.getVAL(txtGridTextBox.Text) - myFunctions.getVAL(flxTimesheetVerify.get_TextMatrix(flxTimesheetVerify.Row, mcWorkingHrs))).ToString());
//                     CalcAdditionDeduction();
//                     break;
//                 case mcRemarks:

//                     flxTimesheetVerify.Text = txtGridTextBox.Text;
//                     break;
//             }
//         }
//         //private double CalculateOnWorkTime(int row)
//         //{
//         //    string Tshift1 = "";
//         //    string Tshift2 = "";

//         //    try
//         //    {
//         //        if (flxTimesheetVerify.get_TextMatrix(row, mcIn).Replace('.', ':') != "" && flxTimesheetVerify.get_TextMatrix(row, mcOut).Replace('.', ':') != "")
//         //        {
//         //            TimeSpan ts1 = TimeSpan.Parse(flxTimesheetVerify.get_TextMatrix(row, mcIn).Replace('.', ':')); //"1:35"
//         //            TimeSpan ts2 = TimeSpan.Parse(flxTimesheetVerify.get_TextMatrix(row, mcOut).Replace('.', ':')); //"3:30"

//         //            Tshift1 = (ts2 - ts1).ToString();
//         //        }
//         //        else
//         //        {
//         //            Tshift1 = "0";
//         //        }
//         //        if (flxTimesheetVerify.get_TextMatrix(row, mcIn2).Replace('.', ':') != "" && flxTimesheetVerify.get_TextMatrix(row, mcOut2).Replace('.', ':') != "")
//         //        {
//         //            TimeSpan ts1 = TimeSpan.Parse(flxTimesheetVerify.get_TextMatrix(row, mcIn2).Replace('.', ':')); //"1:35"
//         //            TimeSpan ts2 = TimeSpan.Parse(flxTimesheetVerify.get_TextMatrix(row, mcOut2).Replace('.', ':')); //"3:30"

//         //            Tshift2 = (ts2 - ts1).ToString();
//         //        }
//         //        else
//         //        {
//         //            Tshift2 = "0";
//         //        }

//         //    }
//         //    catch (Exception ex)
//         //    {

//         //    }


//         //    TimeSpan ts3 = TimeSpan.Parse(Tshift1); //"1:35"
//         //    TimeSpan ts4 = TimeSpan.Parse(Tshift2); //"3:30"
//         //    string time = (ts4 + ts3).Hours.ToString() + "." + (ts4 + ts3).Minutes.ToString("00");
//         //    return myFunctions.getVAL(time);
//         //}

//         private void weekoffTime(int row)
//         {
//             string Tshift1 = "";
//             string Tshift2 = "";

//             try
//             {
//                 if (flxTimesheetVerify.get_TextMatrix(row, mcIn).Replace('.', ':') != "" && flxTimesheetVerify.get_TextMatrix(row, mcOut).Replace('.', ':') != "")
//                 {
//                     TimeSpan ts1 = TimeSpan.Parse(flxTimesheetVerify.get_TextMatrix(row, mcIn).Replace('.', ':')); //"1:35"
//                     TimeSpan ts2 = TimeSpan.Parse(flxTimesheetVerify.get_TextMatrix(row, mcOut).Replace('.', ':')); //"3:30"

//                     Tshift1 = (ts2 - ts1).ToString();
//                 }
//                 else
//                 {
//                     Tshift1 = "0";
//                 }
//                 if (flxTimesheetVerify.get_TextMatrix(row, mcIn2).Replace('.', ':') != "" && flxTimesheetVerify.get_TextMatrix(row, mcOut2).Replace('.', ':') != "")
//                 {
//                     TimeSpan ts1 = TimeSpan.Parse(flxTimesheetVerify.get_TextMatrix(row, mcIn2).Replace('.', ':')); //"1:35"
//                     TimeSpan ts2 = TimeSpan.Parse(flxTimesheetVerify.get_TextMatrix(row, mcOut2).Replace('.', ':')); //"3:30"

//                     Tshift2 = (ts2 - ts1).ToString();
//                 }
//                 else
//                 {
//                     Tshift2 = "0";
//                 }

//             }
//             catch (Exception ex)
//             {

//             }


//             TimeSpan ts3 = TimeSpan.Parse(Tshift1); //"1:35"
//             TimeSpan ts4 = TimeSpan.Parse(Tshift2); //"3:30"
//             string time = (ts4 + ts3).Hours.ToString() + "." + (ts4 + ts3).Minutes.ToString("00");
//             flxTimesheetVerify.set_TextMatrix(row, mcTotHrs, time);
//             if (flxTimesheetVerify.get_TextMatrix(row, mcTotHrs) != "0.0")
//             {
//                 flxTimesheetVerify.set_TextMatrix(row, mcDiff, time);
//                 flxTimesheetVerify.set_TextMatrix(row, mcOt, time);
//                 flxTimesheetVerify.set_TextMatrix(row, mcCompDeduct, time);
//             }
//             else
//             {
//                 flxTimesheetVerify.set_TextMatrix(row, mcDiff, "0");
//                 flxTimesheetVerify.set_TextMatrix(row, mcOt, "0");
//                 flxTimesheetVerify.set_TextMatrix(row, mcCompDeduct, "0");
//             }
//         }

//         private void CalculateTime(int row)
//         {
//             string Tshift1 = "";
//             string Tshift2 = "";

//             try
//             {
//                 if (flxTimesheetVerify.get_TextMatrix(row, mcIn).Replace('.', ':') != "" && flxTimesheetVerify.get_TextMatrix(row, mcOut).Replace('.', ':') != "")
//                 {
//                     TimeSpan ts1 = TimeSpan.Parse(flxTimesheetVerify.get_TextMatrix(row, mcIn).Replace('.', ':')); //"1:35"
//                     TimeSpan ts2 = TimeSpan.Parse(flxTimesheetVerify.get_TextMatrix(row, mcOut).Replace('.', ':')); //"3:30"
//                     if (ts1 > ts2)
//                         ts2 = ts2.Add(TimeSpan.FromHours(24));

//                     Tshift1 = (ts2 - ts1).ToString();
//                 }
//                 else
//                 {
//                     Tshift1 = "0";
//                 }
//                 if (flxTimesheetVerify.get_TextMatrix(row, mcIn2).Replace('.', ':') != "" && flxTimesheetVerify.get_TextMatrix(row, mcOut2).Replace('.', ':') != "")
//                 {
//                     TimeSpan ts1 = TimeSpan.Parse(flxTimesheetVerify.get_TextMatrix(row, mcIn2).Replace('.', ':')); //"1:35"
//                     TimeSpan ts2 = TimeSpan.Parse(flxTimesheetVerify.get_TextMatrix(row, mcOut2).Replace('.', ':')); //"3:30"

//                     Tshift2 = (ts2 - ts1).ToString();
//                 }
//                 else
//                 {
//                     Tshift2 = "0";
//                 }

//             }
//             catch (Exception ex)
//             {

//             }


//             TimeSpan ts3 = TimeSpan.Parse(Tshift1); //"1:35"
//             TimeSpan ts4 = TimeSpan.Parse(Tshift2); //"3:30"
//             string time = (ts4 + ts3).Hours.ToString() + "." + (ts4 + ts3).Minutes.ToString();
//             flxTimesheetVerify.set_TextMatrix(row, mcTotHrs, time);
//             if (flxTimesheetVerify.get_TextMatrix(row, mcTotHrs) != "0.0")
//             {
//                 DateTime dt = Convert.ToDateTime(flxTimesheetVerify.get_TextMatrix(row, mcDate));
//                 int Whours = myFunctions.getIntVAL(dba.ExecuteSclar("Select N_Workhours from Pay_WorkingHours where X_Day='" + dt.DayOfWeek + "' and N_CompanyID=" + myCompanyID._CompanyID, "TEXT", new DataTable()).ToString());
//                 flxTimesheetVerify.set_TextMatrix(row, mcWorkingHrs, Whours.ToString());
//                 flxTimesheetVerify.set_TextMatrix(row, mcDiff, (myFunctions.getVAL(flxTimesheetVerify.get_TextMatrix(row, mcTotHrs)) - myFunctions.getVAL(flxTimesheetVerify.get_TextMatrix(row, mcWorkingHrs))).ToString());
//             }
//             else
//                 flxTimesheetVerify.set_TextMatrix(row, mcDiff, "0");
//         }
//         private void btnGridAccount_Click(object sender, EventArgs e)
//         {
//             ucvisible();
//             if (flxTimesheetVerify.Col == mcType)
//             {

//                 if (ucType.Visible)
//                 {
//                     B_EscapePressed = true;
//                     ucType.Visible = false;
//                     B_EscapePressed = false;

//                 }
//                 else
//                 {
//                     if (B_UcLeaveCalled) { B_UcLeaveCalled = false; return; }
//                     TypeSettings();
//                     //ucType.Left = (int)(flxFeeDetails.Left) + (int)flxFeeDetails.get_ColWidth(mcType) / 15;
//                     //ucType.Top = ((int)flxFeeDetails.Top + (flxFeeDetails.CellTop / 15)) + btnGridAccount.Height;
//                     ucType.Left = txtGridTextBox.Left;
//                     ucType.Top = txtGridTextBox.Bottom;
//                     ucType.ActivateControl();
//                     //  ucLocationSearch.ValueChanged("[Location Name]", "");
//                     //  ucLocationSearch.ValueChanged("[Location Name]", txtGridTextBox.Text);
//                     // ucType.ValueChanged("[Location Name]", flxFeeDetails.get_TextMatrix(flxFeeDetails.Row, mcLocation));
//                     ucType.Focus();
//                 }
//             }
//         }



//         private void TypeSettings()
//         {
//             string searchfield = "";
//             ucType.Width = 200;
//             ucType.Height = 200;
//             ucType.X_ProjectName = myCompanyID._Project;
//             if (flxTimesheetVerify.get_TextMatrix(flxTimesheetVerify.Row, mcAttendence) == "A")
//             {
//                 searchfield = "X_VacType";
//                 ucType.X_ProjectName = myCompanyID._Project;
//                 ucType.X_HideFieldList = "N_CompanyID,N_VacTypeID,X_VacCode,X_Type";
//                 ucType.X_OrderByField = "X_VacType";
//                 ucType.X_TableName = "Pay_VacationType";
//                 ucType.X_VisibleFieldList = "X_VacType";
//                 ucType.X_Crieteria = "N_CompanyID=" + myCompanyID._CompanyID + " and X_Type= 'B'";

//             }

//             else
//             {
//                 searchfield = "X_Description";
//                 ucType.X_HideFieldList = "N_CompanyID,N_FnyearID,N_PayID,N_PaymentID,n_paymethod,N_PayTypeID";
//                 ucType.X_OrderByField = "X_Description";
//                 ucType.X_TableName = "Pay_PayMaster";
//                 ucType.X_VisibleFieldList = "X_Description";
//                 ucType.X_Crieteria = "N_CompanyID=" + myCompanyID._CompanyID + " and N_FnyearID= " + myCompanyID._FnYearID + " and N_paymethod <> 0 and (N_PayTypeID <>8 and N_PayTypeID <>13 and N_PayTypeID <>14)";
//             }
//             ucType.X_SearchField = searchfield;
//             ucType.B_HeaderVisible = false;
//             ucType.X_FieldWidths = "100";
//             ucType.RightToLeft = this.RightToLeft;
//             ucType.InitializeControl();

//         }

//         private void ucType_Leave(object sender, EventArgs e)
//         {
//             if (B_EscapePressed) { return; }
//             if (ucType.Visible)
//             {
//                 B_EscapePressed = true;
//                 ucType.Visible = false;
//                 B_EscapePressed = false;
//                 B_UcLeaveCalled = true;
//             }
//         }

//         private void ucType_VisibleChanged(object sender, EventArgs e)
//         {
//             if (!ucType.Visible)
//             {
//                 string InputVal = "";
//                 int row = 0;
//                 if (B_EscapePressed) { B_EscapePressed = false; return; }
//                 if (flxTimesheetVerify.get_TextMatrix(flxTimesheetVerify.Row, mcAttendence) == "A")
//                 {
//                     InputVal = ucType.ReturnSelectedValue("X_VacType");
//                     row = flxTimesheetVerify.Row;
//                 }
//                 else
//                 {
//                     InputVal = ucType.ReturnSelectedValue("X_Description");
//                     row = flxTimesheetVerify.Row;
//                 }


//                 if (txtGridTextBox.Visible)
//                     txtGridTextBox.Focus();
//                 else
//                     flxTimesheetVerify.Focus();
//                 if (flxTimesheetVerify.get_TextMatrix(flxTimesheetVerify.Row, mcAttendence) == "A")
//                 {
//                     flxTimesheetVerify.set_TextMatrix(row, mcType, ucType.ReturnSelectedValue("X_VacType"));
//                     flxTimesheetVerify.set_TextMatrix(row, mcPayID, ucType.ReturnSelectedValue("N_VacTypeID"));

//                 }
//                 else
//                 {
//                     flxTimesheetVerify.set_TextMatrix(row, mcType, ucType.ReturnSelectedValue("X_Description"));
//                     flxTimesheetVerify.set_TextMatrix(row, mcPayID, ucType.ReturnSelectedValue("N_PayID"));
//                 }
//             }
//             btnGridAccount.Visible = false;
//             txtGridTextBox.Visible = false;
//             B_UcLeaveCalled = false;


//         }

//         private bool ValidateEmployee()
//         {
//             if (txtEmpCode.Text.Trim() == "") return true;

//             string X_Condn = " Where N_CompanyID=" + myCompanyID._CompanyID + " and X_EmpCode='" + txtEmpCode.Text.Trim() + "' and N_FnYearID=" + myCompanyID._FnYearID + "";
//             if (myCompanyID._B_AllBranchData == false)
//                 X_Condn += " and N_BranchID=" + myCompanyID._BranchID;

//             if (dsItemCategory.Tables.Contains("Pay_Employee"))
//                 dsItemCategory.Tables.Remove("Pay_Employee");
//             dba.FillDataSet(ref dsItemCategory, "Pay_Employee", "Select X_empcode,X_EmpName,N_EmpID,N_CatagoryId From Pay_Employee " + X_Condn, "TEXT", new DataTable());
//             if (dsItemCategory.Tables["Pay_Employee"].Rows.Count == 0)
//             {
//                 if (dsItemCategory.Tables.Contains("Pay_Employee"))
//                     dsItemCategory.Tables.Remove("Pay_Employee");


//                 X_Condn = " Where N_CompanyID=" + myCompanyID._CompanyID + " and X_EmpCode like '%" + txtEmpCode.Text.Trim() + "%' and N_FnYearID=" + myCompanyID._FnYearID + "";
//                 if (myCompanyID._B_AllBranchData == false)
//                     X_Condn += " and N_BranchID=" + myCompanyID._BranchID;

//                 dba.FillDataSet(ref dsItemCategory, "Pay_Employee", "Select X_empcode,X_EmpName,N_EmpID,N_CatagoryId From Pay_Employee " + X_Condn, "TEXT", new DataTable());
//                 if (dsItemCategory.Tables["Pay_Employee"].Rows.Count != 1)
//                 {
//                     return false;
//                 }
//             }
//             DataRow drow = dsItemCategory.Tables["Pay_Employee"].Rows[0];

//             txtEmpCode.Text = drow["X_EmpCode"].ToString();
//             txtEmpName.Text = drow["X_EmpName"].ToString();
//             N_EmpID = myFunctions.getIntVAL(drow["N_EmpID"].ToString());
//             N_CatagoryId = myFunctions.getIntVAL(drow["N_CatagoryId"].ToString());
//             //txtCustomerAddress.Text = drow["X_Address"].ToString();
//             return true;
//         }

//         private bool ValidatePayCode()
//         {
//             if (txtAdjpaycode.Text.Trim() == "") return true;

//             if (dsItemCategory.Tables.Contains("Pay_PayCode"))
//                 dsItemCategory.Tables.Remove("Pay_PayCode");
//             dba.FillDataSet(ref dsItemCategory, "Pay_PayCode", "Select * From Pay_PayMaster Where N_CompanyID=" + myCompanyID._CompanyID + " and N_FnYearID=" + myCompanyID._FnYearID + " and X_Description='" + txtAdjpaycode.Text.Trim() + "'", "TEXT", new DataTable());
//             if (dsItemCategory.Tables["Pay_PayCode"].Rows.Count > 0)
//             {
//                 DataRow drow = dsItemCategory.Tables["Pay_PayCode"].Rows[0];
//                 N_AdjTypeID = myFunctions.getIntVAL(drow["N_PayID"].ToString());
//                 return true;
//             }
//             else
//                 return false;
//         }


//         private void txtEmpCode_Leave(object sender, EventArgs e)
//         {
//             //TextBox txt = (TextBox)sender;
//             //txt.BackColor = Color.White;

//             //if (txtEmpCode.Text.Trim() == "" ) { clearScrean(); return; }
//             ////if (txt.Name == "txtEmpCode")
//             ////{
//             ////    if (X_GlobalVal != txt.Text.Trim())
//             ////    {
//             ////        if (ValidateCustomer())
//             ////        {
//             ////            Get_Attendence();
//             ////        }
//             ////    }
//             ////}
//         }

//         private void dtpPayrunText_ValueChanged(object sender, EventArgs e)
//         {
//             get_Payrate();
//             checkperiod();
//         }

//         private void dtpfromdate_KeyDown(object sender, KeyEventArgs e)
//         {
//             if (e.KeyCode == Keys.Enter)
//                 dtptodate.Focus();
//         }

//         private void dtptodate_KeyDown(object sender, KeyEventArgs e)
//         {

//             if (e.KeyCode == Keys.Enter)
//                 dtpPayrunText.Focus();
//         }

//         private void dtpPayrunText_KeyDown(object sender, KeyEventArgs e)
//         {

//             if (e.KeyCode == Keys.Enter)
//             {
//                 flxTimesheetVerify.Row = 1;
//                 flxTimesheetVerify.Focus();
//             }
//         }

//         private void dtpDate_Enter(object sender, EventArgs e)
//         {
//             DateTimePicker txt = (DateTimePicker)sender;
//             X_PrevValue = myFunctions.getDateVAL(txt.Value.Date);
//         }

//         private void dtpDate_Leave(object sender, EventArgs e)
//         {
//             DateTimePicker txt = (DateTimePicker)sender;
//             if (X_PrevValue != myFunctions.getDateVAL(txt.Value.Date))
//                 B_SaveChanges = true;
//         }

//         private void txtGridTextBox_Enter(object sender, EventArgs e)
//         {
//             switch (flxTimesheetVerify.Col)
//             {
//                 case mcOt:
//                 case mcRemarks:
//                     X_PrevValue = txtGridTextBox.Text.Trim();
//                     break;
//                 default:
//                     break;
//             }
//         }

//         private void txtGridTextBox_Leave(object sender, EventArgs e)
//         {
//             switch (flxTimesheetVerify.Col)
//             {
//                 case mcIn:
//                 case mcIn2:
//                 case mcOut:
//                 case mcOut2:
//                     ValidateTimeEntry(flxTimesheetVerify.Row);
//                     break;

//                 case mcOt:
//                     if (X_PrevValue != txtGridTextBox.Text.Trim())
//                         B_SaveChanges = true;
//                     break;
//                 case mcRemarks:
//                     if (X_PrevValue != txtGridTextBox.Text.Trim())
//                         B_SaveChanges = true;
//                     break;
//                 default:
//                     break;
//             }
//         }

//         private void Pay_TimeSheetVerify_FormClosing(object sender, FormClosingEventArgs e)
//         {
//             if (B_SaveChanges)
//                 if (!msg.msgAnswerMe(MYG.ReturnMultiLingualVal("-1111", "X_ControlNo", "SaveChanges")))
//                     e.Cancel = true;
//         }
//         private void ValidateTimeEntry(int row)
//         {
//             if (flxTimesheetVerify.Col == mcIn)
//             {
//                 if (flxTimesheetVerify.get_TextMatrix(row, mcIn) == "")
//                     flxTimesheetVerify.set_TextMatrix(row, mcIn, "0.00");
//                 else
//                 {
//                     try
//                     {
//                         var val = flxTimesheetVerify.get_TextMatrix(row, mcIn).Replace(".", ":");
//                         TimeSpan ts = new TimeSpan();
//                         ts = TimeSpan.Parse(val);
//                     }
//                     catch (Exception ex)
//                     {
//                         msg.msgError("Invalid Time,Check Time format is hh:mm:ss");
//                     }
//                 }
//             }
//             if (flxTimesheetVerify.Col == mcOut)
//             {
//                 if (flxTimesheetVerify.get_TextMatrix(row, mcIn) == "")
//                     flxTimesheetVerify.set_TextMatrix(row, mcIn, "0.00");

//                 if (flxTimesheetVerify.get_TextMatrix(row, mcOut) == "")
//                     flxTimesheetVerify.set_TextMatrix(row, mcOut, "0.00");
//                 else
//                 {
//                     try
//                     {
//                         var val = flxTimesheetVerify.get_TextMatrix(row, mcOut).Replace(".", ":");
//                         TimeSpan ts = new TimeSpan();
//                         ts = TimeSpan.Parse(val);
//                     }
//                     catch (Exception ex)
//                     {
//                         msg.msgError("Invalid Time,Check Time format is hh:mm:ss");
//                     }
//                 }
//             }
//             if (flxTimesheetVerify.Col == mcIn2)
//             {
//                 if (flxTimesheetVerify.get_TextMatrix(row, mcIn2) == "")
//                     flxTimesheetVerify.set_TextMatrix(row, mcIn2, "0.00");
//                 else
//                 {
//                     try
//                     {
//                         var val = flxTimesheetVerify.get_TextMatrix(row, mcIn2).Replace(".", ":");
//                         TimeSpan ts = new TimeSpan();
//                         ts = TimeSpan.Parse(val);
//                     }
//                     catch (Exception ex)
//                     {
//                         msg.msgError("Invalid Time,Check Time format is hh:mm:ss");
//                     }
//                 }
//             }
//             if (flxTimesheetVerify.Col == mcOut2)
//             {
//                 if (flxTimesheetVerify.get_TextMatrix(row, mcOut2) == "")
//                     flxTimesheetVerify.set_TextMatrix(row, mcOut2, "0.00");
//                 else
//                 {
//                     try
//                     {
//                         var val = flxTimesheetVerify.get_TextMatrix(row, mcOut2).Replace(".", ":");
//                         TimeSpan ts = new TimeSpan();
//                         ts = TimeSpan.Parse(val);
//                     }
//                     catch (Exception ex)
//                     {
//                         msg.msgError("Invalid Time,Check Time format is hh:mm:ss");
//                     }
//                 }
//             }

//         }
//         private void lnkMail_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
//         {
//             if (!msg.msgAnswerMe(MYG.ReturnMultiLingualVal("-1111", "X_ControlNo", "Sendmail"))) return;
//             SendMail();
//         }

//         private void SendMail()
//         {
//             try
//             {


//                 if (dsItemCategory.Tables.Contains("Pay_Managermails"))
//                     dsItemCategory.Tables.Remove("Pay_Managermails");

//                 dba.FillDataSet(ref dsItemCategory, "Pay_Managermails", "select distinct N_ManagerEmpId,X_ManagerEmpName,X_ManagerEmailID from vw_Pay_EmployeeEmailInformation where N_CompanyId= " + myCompanyID._CompanyID + " and  N_FnyearID=" + myCompanyID._FnYearID, "TEXT", new DataTable());

//                 if (dsItemCategory.Tables["Pay_Managermails"].Rows.Count > 0)
//                 {
//                     //string mailaddress = MYG.ReturnSettings(myCompanyID._CompanyID.ToString(), MYG.ReturnFormID(this.Text), "EmailAddress", "X_Value"); //If email from address different for each form
//                     //string mailaddress = MYG.ReturnSettings(myCompanyID._CompanyID.ToString(), MYG.ReturnFormID(this.Text), "EmailAddress", "X_Value"); //If email from address Password different for each form


//                     string fromemail = MYG.ReturnSettings(myCompanyID._CompanyID.ToString(), "210", "EmailAddress", "X_Value");
//                     string fromemailPassword = MYG.ReturnSettings(myCompanyID._CompanyID.ToString(), "210", "EmailPassword", "X_Value");


//                     string Toemail = "", CCemail = "", Bccemail = "", Toemailnames = "", expression = "", OrderByField = "";

//                     for (int i = 0; i < dsItemCategory.Tables["Pay_Managermails"].Rows.Count; i++)
//                     {

//                         MailMessage mail = new MailMessage();
//                         SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");


//                         //SmtpClient SmtpServer = new SmtpClient("smtp.gov.sa");
//                         //Your Id write here
//                         mail.From = new MailAddress(fromemail);
//                         //Sending Address write here
//                         DataSet dsMail = new DataSet();
//                         DataTable dtMailDetails = new DataTable();
//                         //Filter data from table 
//                         //dba.FillDataSet(ref dsMail, "dtMail", "SELECt * FROm code_StaffEmail Where N_Action=1", "TEXT", new DataTable());
//                         string condition = "";





//                         Toemail = dsItemCategory.Tables["Pay_Managermails"].Rows[i]["X_ManagerEmailID"].ToString().Trim();
//                         expression = "X_Type='TO'";
//                         OrderByField = "";


//                         if (Toemail != "")
//                         {
//                             mail.To.Add(Toemail);
//                             Toemailnames = dsItemCategory.Tables["Pay_Managermails"].Rows[i]["X_ManagerEmpName"].ToString();
//                             // To address                  
//                             mail.Subject = "ATTENDENCE REGISTER(" + dtpfromdate.Text + " to " + dtptodate.Text + ")";
//                             mail.Body = Boody(Toemailnames, myFunctions.getIntVAL(dsItemCategory.Tables["Pay_Managermails"].Rows[i]["N_ManagerEmpId"].ToString()));

//                             if (mail.Body == "") continue;

//                             //mail.Attachments.Add(new Attachment(Application.StartupPath + @"\uploadedfiles\ MyExcelFile.xls"));
//                             mail.Attachments.Add(new Attachment(Application.StartupPath + @"\uploadedfiles\ AttendenceSheet.xls"));


//                             mail.IsBodyHtml = true;
//                             SmtpServer.Port = 587;
//                             //  SmtpServer.Port = 25;
//                             SmtpServer.Credentials = new System.Net.NetworkCredential(fromemail, fromemailPassword);   // From address
//                             SmtpServer.EnableSsl = true;

//                             SmtpServer.Send(mail);
//                             mail.Attachments.Dispose();
//                             File.Delete(Application.StartupPath + @"\uploadedfiles\ AttendenceSheet.xls");
//                             //  return true;
//                         }
//                         //else
//                         //    return false;

//                     }







//                 }
//                 //else
//                 //    return false;

//             }
//             catch (Exception ex)
//             {

//                 MessageBox.Show(ex.ToString());
//                 return;
//                 //return false;
//             }
//         }




//         public string Boody(string Toemailnames, int ManagerID)
//         {
//             StringBuilder message = new StringBuilder();
//             message = message.Append("<body style='font-family:Verdana, Arial, Helvetica, sans-serif; font-size:9pt;'>");
//             if (dsMail.Tables.Contains("AttendenceSheet"))
//                 dsMail.Tables.Remove("AttendenceSheet");

//             dba.FillDataSet(ref dsMail, "AttendenceSheet", "SP_Pay_TimeSheetEmail " + myCompanyID._CompanyID + ",'" + myFunctions.getDateVAL(dtpfromdate.Value) + "','" + myFunctions.getDateVAL(dtptodate.Value) + "'," + ManagerID, "TEXT", new DataTable());

//             if (dsMail.Tables["AttendenceSheet"].Rows.Count > 0)
//             {
//                 message = message.Append("Dear " + Toemailnames + ",<br/><br/><br/><br/><br/>Sincerely,<br/>" + myCompanyID._UserName);

//                 //File.Delete(Application.StartupPath + @"\uploadedfiles\ AttendenceSheet.xls");
//                 ExcelLibrary.DataSetHelper.CreateWorkbook(Application.StartupPath + @"\uploadedfiles\ AttendenceSheet.xls", dsMail);

//             }

//             //if (X_Action == "Save")
//             //{
//             //    if (Toemailnames != "")

//             //else
//             //    message = message.Append("<br/><br/>Site information has been entered into MOH Smart Projects With Following details;<br/><br/>");
//             //}
//             //else
//             //{
//             //    if (Toemailnames != "")
//             //        message = message.Append("Dear " + Toemailnames + ",<br/><br/>Site information has been modified in MOH Smart Projects With Following details;<br/><br/>");
//             //    else
//             //        message = message.Append("<br/><br/>Site information has been modified in MOH Smart Projects With Following details;<br/><br/>");
//             //}



//             return message.ToString();

//         }

//         private void Pay_TimeSheetVerify_Activated(object sender, EventArgs e)
//         {
//             ts.IsBalloon = true;
//             // MYG.MultiLingual(this);
//             get_Offdays();
//             get_Workingdays();
//             // flxFeeDetails.ScrollTrack = true;
//             SetFields();

//             if (!MYG.HavePermission(this.Text, "Save"))
//                 btnSave.Enabled = false;
//             // checkperiod();

//             if (X_Code != "")
//             {
//                 N_EmpID = myFunctions.getIntVAL(dba.ExecuteSclar("select N_EmpID from vw_TimeSheetMaster_Disp where X_BatchCode='" + X_Code + "' and N_CompanyID=" + myCompanyID._CompanyID + " and N_FnYearID=" + myCompanyID._FnYearID, "TEXT", new DataTable()).ToString());
//                 object Res = dba.ExecuteSclar("select D_SalaryDate from vw_TimeSheetMaster_Disp where X_BatchCode='" + X_Code + "' and N_CompanyID=" + myCompanyID._CompanyID + " and N_FnYearID=" + myCompanyID._FnYearID, "TEXT", new DataTable());
//                 string Date = Convert.ToDateTime(Res.ToString()).ToString();
//                 dtpPayrunText.Value = Convert.ToDateTime(Date.ToString());
//             }

//             DefaultPayCodes();
//             setStatus();
//             ViewData();
//             txtEmpCode.Focus();
//         }

//         private void FillSavedData()
//         {

//         }

//         private void lblItemCategory_Click(object sender, EventArgs e)
//         {

//         }

//         private void chkSProcessType_CheckedChanged(object sender, EventArgs e)
//         {
//             //if (chkSProcessType.Checked)
//             //{

//             //    Btnadjpaycode.Visible = false;
//             //    txtAdjpaycode.Visible = false;
//             //    lbladjustment.Visible = false;
//             //    txtAdjustment.Visible = false;
//             //    txtAdjustment.Focus();
//             //}
//             //else
//             //{

//             //    Btnadjpaycode.Visible = false;
//             //    txtAdjpaycode.Visible = false;
//             //    lbladjustment.Visible = false;
//             //    txtAdjustment.Visible = false;
//             //}
//         }

//         private void Btnadjpaycode_Click(object sender, EventArgs e)
//         {
//             ucvisible();
//             if (ucAdjType.Visible)
//             {
//                 B_EscapePressed = true;
//                 ucAdjType.Visible = false;
//                 B_EscapePressed = false;

//             }
//             else
//             {
//                 if (B_UcLeaveCalled) { B_UcLeaveCalled = false; return; }
//                 AdjTypeSettings();
//                 //ucAdjType.Left = Btnadjpaycode.Left;
//                 //ucAdjType.Top = Btnadjpaycode.Top-100;
//                 ucAdjType.ActivateControl();
//                 ucAdjType.Focus();
//             }
//         }
//         private void AdjTypeSettings()
//         {
//             string searchfield = "X_Description";
//             ucAdjType.Width = 200;
//             //ucAdjType.Height = 200;
//             ucAdjType.Height = 100;
//             ucAdjType.Left = Btnadjpaycode.Right - ucAdjType.Width;
//             ucAdjType.Top = Btnadjpaycode.Top - 100;
//             ucAdjType.BringToFront();
//             ucAdjType.X_ProjectName = myCompanyID._Project;
//             ucAdjType.X_HideFieldList = "N_CompanyID,N_FnyearID,N_PayID,N_PaymentID,n_paymethod,N_PayTypeID";
//             ucAdjType.X_OrderByField = "X_Description";
//             ucAdjType.X_TableName = "Pay_PayMaster";
//             ucAdjType.X_VisibleFieldList = "X_Description";
//             ucAdjType.X_Crieteria = "N_CompanyID=" + myCompanyID._CompanyID + " and N_FnyearID= " + myCompanyID._FnYearID + " and N_paymethod <> 0 and (N_PayTypeID <>8 and N_PayTypeID <>13 and N_PayTypeID <>14)";
//             ucAdjType.X_SearchField = searchfield;
//             ucAdjType.B_HeaderVisible = false;
//             ucAdjType.X_FieldWidths = "100";
//             ucAdjType.RightToLeft = this.RightToLeft;
//             ucAdjType.InitializeControl();

//         }

//         private void ucAdjType_Leave(object sender, EventArgs e)
//         {
//             if (B_EscapePressed) { return; }
//             if (ucAdjType.Visible)
//             {
//                 B_EscapePressed = true;
//                 ucAdjType.Visible = false;
//                 B_EscapePressed = false;
//                 B_UcLeaveCalled = true;
//             }
//         }

//         private void ucAdjType_VisibleChanged(object sender, EventArgs e)
//         {
//             if (!ucAdjType.Visible)
//             {
//                 if (B_EscapePressed) { B_EscapePressed = false; return; }
//                 string InputVal = ucAdjType.ReturnSelectedValue("X_Description");
//                 txtAdjpaycode.Text = ucAdjType.ReturnSelectedValue("X_Description");
//                 N_AdjTypeID = myFunctions.getIntVAL(ucAdjType.ReturnSelectedValue("N_PayID"));
//                 txtAdjpaycode.Focus();
//             }
//         }

//         private void txtAdjustment_KeyPress(object sender, KeyPressEventArgs e)
//         {
//             MYG.check4Numeric(e, txtAdjustment.Text.Trim());

//         }

//         private void txtAdjustment_KeyDown(object sender, KeyEventArgs e)
//         {
//             if (e.KeyCode == Keys.Enter)
//                 txtAdjpaycode.Focus();
//         }

//         private void txtAdjpaycode_KeyDown(object sender, KeyEventArgs e)
//         {
//             if (e.KeyCode == Keys.Down)
//             {
//                 if (e.Alt)
//                 {
//                     Btnadjpaycode_Click(sender, new EventArgs());
//                 }
//             }
//         }

//         private void txtAdjpaycode_KeyPress(object sender, KeyPressEventArgs e)
//         {
//             if (e.KeyChar == 13)
//             {
//                 if (myFunctions.getVAL(txtAdjustment.Text) != 0)
//                 {
//                     if (txtAdjpaycode.Text.Trim() == "" || !ValidatePayCode())
//                     {
//                         Btnadjpaycode_Click(sender, e);
//                         ucAdjType.ValueChanged("X_Description", txtAdjpaycode.Text.Trim());
//                         return;
//                     }
//                     else
//                     {
//                         btnSave.Focus();
//                     }
//                 }

//             }
//         }

//         private void lblTotDedutionName_Click(object sender, EventArgs e)
//         {

//         }

//         private void flxFeeDetails_LeaveCell(object sender, EventArgs e)
//         {
//             if (flxTimesheetVerify.Row <= 0) return;
//             if (!txtGridTextBox.Visible) return;
//             switch (flxTimesheetVerify.Col)
//             {
//                 case mcIn:
//                     CalculateTime(flxTimesheetVerify.Row);
//                     break;
//                 case mcOut:
//                     CalculateTime(flxTimesheetVerify.Row);
//                     break;
//                 case mcIn2:
//                     CalculateTime(flxTimesheetVerify.Row);
//                     break;
//                 case mcOut2:
//                     CalculateTime(flxTimesheetVerify.Row);
//                     break;
//             }
//         }

//         private void button1_Click(object sender, EventArgs e)
//         {
//             ClearScrean();
//         }

//         private void BtnSearchBatch_Click(object sender, EventArgs e)
//         {
//             //  ucvisible();
//             if (ucBatch.Visible)
//             {
//                 B_EscapePressed = true;
//                 ucBatch.Visible = false;
//                 B_EscapePressed = false;
//             }
//             else
//             {
//                 if (B_UcLeaveCalled) { B_UcLeaveCalled = false; return; }
//                 BatchSettings();
//                 ucBatch.ActivateControl();
//                 ucBatch.Focus();
//             }
//         }

//         private void ucBatch_Leave(object sender, EventArgs e)
//         {
//             if (B_EscapePressed) { return; }
//             if (ucBatch.Visible)
//             {
//                 B_EscapePressed = true;
//                 ucBatch.Visible = false;
//                 B_EscapePressed = false;
//                 B_UcLeaveCalled = true;
//             }
//         }

//         private void ucBatch_VisibleChanged(object sender, EventArgs e)
//         {
//             if (!ucBatch.Visible)
//             {
//                 if (B_EscapePressed) { B_EscapePressed = false; return; }
//                 txtBatchID.Text = ucBatch.ReturnSelectedValue("BatchCode");
//                 N_BatchID = myFunctions.getIntVAL(ucBatch.ReturnSelectedValue("N_BatchID"));
//                 N_EmpID = myFunctions.getIntVAL(ucBatch.ReturnSelectedValue("N_EmpID"));
//                 N_TimeSheetID = myFunctions.getIntVAL(ucBatch.ReturnSelectedValue("N_TimeSheetID"));
//                 ViewDetails();
//                 B_UcLeaveCalled = false;
//             }
//         }
//         private bool validateBatch()
//         {
//             object res = null;
//             if (txtBatchID.Text.Trim() == "") return true;

//             if (myCompanyID._B_AllBranchData == true)
//                 res = dba.ExecuteSclar("Select 1 From Pay_TimeSheetMaster  Where N_BatchID=" + myFunctions.getIntVAL(txtBatchID.Text) + " and N_CompanyID=" + myCompanyID._CompanyID + " and N_FnYearID = " + myCompanyID._FnYearID, "TEXT", new DataTable());
//             else
//                 res = dba.ExecuteSclar("Select 1 From Pay_TimeSheetMaster Where  N_BatchID=" + myFunctions.getIntVAL(txtBatchID.Text) + " and N_CompanyID=" + myCompanyID._CompanyID + " and N_FnYearID = " + myCompanyID._FnYearID + " and N_BranchID=" + myCompanyID._BranchID, "TEXT", new DataTable());

//             if (res != null)
//             {
//                 return true;
//             }
//             else
//                 return false;
//         }
//         private void txtBatchID_KeyPress(object sender, KeyPressEventArgs e)
//         {
//             MYG.check4SplChar(e);
//             if (e.KeyChar == 13)
//             {
//                 if (txtBatchID.Text.Trim().ToLower() == "@auto")
//                 {
//                     txtEmpCode.Focus();

//                 }
//                 else if (validateBatch())
//                 {
//                     txtEmpCode.Focus();
//                 }
//                 else
//                     ClearScrean();

//             }
//         }

//         private void BtnDelete_Click(object sender, EventArgs e)
//         {
//             bool B_VacFlag = true;
//             int N_AbsentCount = 0;
//             if (MYG.check4Null(txtEmpCode, MYG.ReturnMultiLingualVal("-1111", "X_ControlNo", "InvalidEntry")))
//                 return;
//             if (!myFunctions.CheckFinanceDate(dtpPayrunText)) return;
//             if (!chkSProcessType.Checked)
//             {
//                 if (!Validate_PayType())
//                 {
//                     msg.msgError(MYG.ReturnMultiLingualVal("-1111", "X_ControlNo", "selpaycode_Att"));
//                     return;
//                 }
//             }
//             if (!Validate_Payrun())
//             {
//                 msg.msgError(MYG.ReturnMultiLingualVal("-1111", "X_ControlNo", "Payrun_Att"));
//                 return;
//             }
//             //-- Check Salary pro
//             for (int i = 1; i < flxTimesheetVerify.Rows; i++)
//             {

//                 if (flxTimesheetVerify.get_TextMatrix(i, mcAttendence) == "A")
//                 {
//                     if (!SalaryAlreadyProcesd())
//                     {
//                         msg.msgError(MYG.ReturnMultiLingualVal("-1111", "X_ControlNo", "SalaryProcessed_Att"));
//                         B_VacFlag = false;
//                         break;
//                     }
//                     N_AbsentCount += 1;
//                 }
//             }

//             if (Convert.ToDateTime(dtptodate.Value.Date) > Convert.ToDateTime(myFunctions.GetFormatedDate(myCompanyID._SystemDate)))
//             {
//                 msg.msgError(MYG.ReturnMultiLingualVal("-1111", "X_ControlNo", "Overdate_Attd"));
//                 return;
//             }
//             try
//             {
//                 if (N_BatchID > 0 && N_EmpID > 0)
//                 {
//                     if (msg.msgAnswerMe(MYG.ReturnMultiLingualVal("-1111", "X_ControlNo", "DeleteConfirm")))
//                     {
//                         if (dba.ReturnCon.State == ConnectionState.Closed)
//                             dba.ReturnCon.Open();
//                         dba.SetTransaction();
//                         //dba.SetTransaction();
//                          string ButtonTag = btnDelete.Tag.ToString();
//                          if (ButtonTag == "6" || ButtonTag == "0")
//                          {
//                              object obj = dba.ExecuteSclar("Select  N_TransID from Pay_MonthlyAddOrDed where N_CompanyID=" + myCompanyID._CompanyID + " and N_PayRunID=" + N_BatchID.ToString() + " and B_PostedAccount=0", "TEXT", new DataTable());
//                              if (obj != null)
//                              {
//                                  int transid = myFunctions.getIntVAL(obj.ToString());
//                                  dba.DeleteData("Pay_MonthlyAddOrDedDetails", "N_TransID", transid.ToString(), "N_CompanyID=" + myCompanyID._CompanyID + " and N_EmpID=" + N_EmpID.ToString() + " and B_TimeSheetEntry=1");
//                                  obj = dba.ExecuteSclar("Select  isnull(Count(N_TransDetailsID),0) from Pay_MonthlyAddOrDedDetails where N_CompanyID=" + myCompanyID._CompanyID + " and N_TransID=" + transid.ToString(), "TEXT", new DataTable());
//                                  if (obj != null)
//                                  {
//                                      if (myFunctions.getIntVAL(obj.ToString()) == 0)
//                                          dba.DeleteData("Pay_MonthlyAddOrDed", "N_TransID", transid.ToString(), "N_CompanyID=" + myCompanyID._CompanyID + " and N_PayRunID=" + N_BatchID.ToString() + " and B_PostedAccount=0");
//                                  }
//                              }

//                              dba.ExecuteNonQuery("delete from Pay_VacationDetails where N_EmpID=" + N_EmpID.ToString() + " and N_CompanyID=" + myCompanyID._CompanyID + " and N_FormID = 216 and D_VacDateFrom >= '" + myFunctions.getDateVAL(dtpfromdate.Value.Date) + "' and D_VacDateTo <= '" + myFunctions.getDateVAL(dtptodate.Value.Date) + "'", "TEXT", new DataTable());
//                              dba.DeleteData("Pay_TimeSheet", "N_BatchID", N_BatchID.ToString(), "N_CompanyID=" + myCompanyID._CompanyID + " and B_IsApproved=1 and N_FnYearID=" + myCompanyID._FnYearID + " and N_TimeSheetID=" + N_TimeSheetID.ToString());
//                          }
//                         //if (dba.DeleteData("Pay_TimeSheet", "N_BatchID", N_BatchID.ToString(), "N_CompanyID=" + myCompanyID._CompanyID + " and B_IsApproved=1 and N_FnYearID=" + myCompanyID._FnYearID + " and N_TimeSheetID=" + N_TimeSheetID.ToString()))
//                         //{
//                         //    if (dba.DeleteData("Pay_TimeSheetMaster", "N_TimeSheetID", N_TimeSheetID.ToString(), "N_CompanyID=" + myCompanyID._CompanyID + " and N_FnYearID=" + myCompanyID._FnYearID))
//                         //    {

//                          string X_Criteria = "N_TimeSheetID=" + N_TimeSheetID + " and N_CompanyID=" + myCompanyID._CompanyID;
//                          myFunctions.updateApprovals(ref dba, btnDelete.Tag.ToString(), X_TransType, N_TimeSheetID, MYG.ReturnFormCaption(MYG.ReturnFormID(this.Text).ToString()), txtBatchID.Text, DateTime.Now, N_ApprovalLevelID, myCompanyID._UserID, N_ProcStatus, "Pay_TimeSheetMaster", X_Criteria, myFunctions.getIntVAL(MYG.ReturnFormID(this.Text).ToString()), N_IsApprovalSystem, 0,txtEmpName.Text,btnSave.Top,btnSave.Left);

//                                 dba.Commit();

//                                 ClearScrean();
//                                 txtEmpCode.Focus();
//                                 msg.waitMsg(MYG.ReturnMultiLingualVal("-1111", "X_ControlNo", "Deleted"));
//                         //    }
//                         //    else
//                         //    {
//                         //        dba.Rollback();
//                         //    }
//                         //}
//                         //else
//                         //{
//                         //    dba.Rollback();
//                         //}

//                     }
//                 }
//             }
//             catch (Exception ex)
//             {
//                 msg.msgInformation(MYG.ReturnMultiLingualVal("-1111", "X_ControlNo", "Nodata"));
//                 txtEmpCode.Focus();
//             }



//         }

//         private void txtNetDedApplicable_KeyPress(object sender, KeyPressEventArgs e)
//         {
//             MYG.check4Numeric(e, txtNetDedApplicable.Text);
//         }

//         private void txtNetAddApplicable_KeyPress(object sender, KeyPressEventArgs e)
//         {
//             MYG.check4Numeric(e, txtNetAddApplicable.Text);
//         }

//         private void lblmsg_Click(object sender, EventArgs e)
//         {
//             LoadApprovalHistoryPopup(X_TransType, txtBatchID.Text.Trim());
//         }
//         private void LoadApprovalHistoryPopup(string StrEntryFrom, string StrPKeyCode)
//         {

//             try
//             {
//                 bool B_ShowApprovalHistory = false;
//                 B_ShowApprovalHistory = true;
//                 if (B_ShowApprovalHistory)
//                 {
//                     if (StrPKeyCode != "" && StrPKeyCode.ToLower() != "@auto")
//                     {
//                         Cursor.Current = Cursors.WaitCursor;
//                         Inv_ApprovalHistoryPopup frm = new Inv_ApprovalHistoryPopup();
//                         //frm.Left = lblmsg.Left - 160;
//                         //frm.Top = lblmsg.Bottom + 140;
//                         frm.ActionCode = StrPKeyCode;
//                         frm.EntryFrom = StrEntryFrom;
//                         frm.EntryFormID = myFunctions.getIntVAL(MYG.ReturnFormID(this.Text).ToString());
//                         frm.TransID = N_TimeSheetID;//primary key
//                         //frm.VoucherDetailsID = myFunctions.getIntVAL(flxPurchase.get_TextMatrix(flxPurchase.Row, mcSlNo));
//                         //frm.VendorID = N_CustomerID;
//                         dba.FillDataSet(ref dsMaster, "dsApprovalProcess", "select * from vw_ApprovalHistoryPopup where N_CompanyID=" + myCompanyID._CompanyID + " and X_EntryForm='" + StrEntryFrom + "' and N_TransID=" + N_TimeSheetID + "", "TEXT", new DataTable());
//                         if (dsMaster.Tables["dsApprovalProcess"].Rows.Count > 0)
//                             frm.ShowDialog();
//                         else
//                         {
//                             msg.msgInformation("No transactions!");
//                             //Checking any transaction for the item
//                             //pObjCon.FillDataSet(ref dsSales, "dsApprovalProcess", "select * from Log_ApprovalProcess", "TEXT", new DataTable());
//                             //if (dsSales.Tables["dsApprovalProcess"].Rows.Count > 0)
//                             //{
//                             //    //frm.IsEmpty = 0;
//                             //    //frm.IsEmpty = 1;
//                             //    frm.ShowDialog();
//                             //}
//                         }
//                         Cursor.Current = Cursors.Default;
//                     }
//                 }
//             }
//             catch (Exception ex)
//             {
//                 msg.msgError(ex.Message);
//             }
//         }


//         private void CalculateTotalWorkedHours(int row)
//         {
//             string Tshift1 = "";
//             string Tshift2 = "";

//             try
//             {
//                 if (flxTimesheetVerify.get_TextMatrix(row, mcIn).Replace('.', ':') != "" && flxTimesheetVerify.get_TextMatrix(row, mcOut).Replace('.', ':') != "")
//                 {
//                     try
//                     {
//                         TimeSpan ts1 = TimeSpan.Parse(flxTimesheetVerify.get_TextMatrix(row, mcIn).Replace('.', ':')); //"1:35"
//                         TimeSpan ts2 = TimeSpan.Parse(flxTimesheetVerify.get_TextMatrix(row, mcOut).Replace('.', ':')); //"3:30"
//                         if (ts2 <= ts1)
//                         {
//                             TimeSpan newSpan = new TimeSpan(0, 24, 0, 0);
//                             ts2 = ts2.Add(newSpan);
//                         }
//                         Tshift1 = (ts2 - ts1).ToString();
//                     }
//                     catch (Exception ex)
//                     {
//                         Tshift1 = "0";
//                     }
//                 }
//                 else
//                 {
//                     Tshift1 = "0";
//                 }
//                 if (flxTimesheetVerify.get_TextMatrix(row, mcIn2).Replace('.', ':') != "" && flxTimesheetVerify.get_TextMatrix(row, mcOut2).Replace('.', ':') != "")
//                 {
//                     try
//                     {
//                         TimeSpan ts1 = TimeSpan.Parse(flxTimesheetVerify.get_TextMatrix(row, mcIn2).Replace('.', ':')); //"1:35"
//                         TimeSpan ts2 = TimeSpan.Parse(flxTimesheetVerify.get_TextMatrix(row, mcOut2).Replace('.', ':')); //"3:30"
//                         if (ts2 <= ts1)
//                         {
//                             TimeSpan newSpan = new TimeSpan(0, 24, 0, 0);
//                             ts2 = ts2.Add(newSpan);
//                         }
//                         Tshift2 = (ts2 - ts1).ToString();
//                     }
//                     catch (Exception ex)
//                     {
//                         Tshift2 = "0";
//                     }
//                 }
//                 else
//                 {
//                     Tshift2 = "0";
//                 }

//             }
//             catch (Exception ex)
//             {

//             }


//             TimeSpan ts3 = TimeSpan.Parse(Tshift1); //"1:35"
//             TimeSpan ts4 = TimeSpan.Parse(Tshift2); //"3:30"
//             string time = (ts4 + ts3).Hours.ToString() + "." + (ts4 + ts3).Minutes.ToString();
//             flxTimesheetVerify.set_TextMatrix(row, mcTotHrs, time);
//         }


//     }
// }