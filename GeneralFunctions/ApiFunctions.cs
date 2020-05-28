namespace SmartxAPI.GeneralFunctions
{
    public class ApiFunctions:IApiFunctions
    {
          public object Response(int Code,string Response)
                {
                        return (new { StatusCode = Code , Message= Response });
                }
    }
public interface IApiFunctions
    {
        public object Response(int Code,string Response);
    }    
}