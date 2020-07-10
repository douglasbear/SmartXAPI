using System;
using System.Collections;

namespace SmartxAPI.GeneralFunctions
{
    public class MyFunctions:IMyFunctions
    {
         public MyFunctions()
        {
        }

    public bool CheckPermission(int N_CompanyID,int N_MenuID, string admin,IDataAccessLayer dLayer)
        {
            SortedList Params=new SortedList();
            Params.Add("@p1",N_CompanyID);
            Params.Add("@p2",N_MenuID);
            Params.Add("@p3",admin);
            bool Result = Convert.ToBoolean(dLayer.ExecuteScalar("Select ISNULL(B_Visible,0) From vw_userPrevileges Where N_CompanyID=@p1 and N_MenuID = @p2 and X_UserCategory=@p3",Params));
            return Result;
        }
    public int getIntVAL(string val)
        {
            if (val.Trim() == "")
                return 0;
            else
                return Convert.ToInt32(val);
        }


    }
public interface IMyFunctions
    {
        public bool CheckPermission(int N_CompanyID,int N_MenuID, string admin,IDataAccessLayer dLayer);
        public int getIntVAL(string val);
    }    
}