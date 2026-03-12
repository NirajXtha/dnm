
namespace NeoErp.Core.Integration
{
    public class IntegrationHelpers
    {

        #region Initializations

        public void ReadSettings()
        {
            if (_Settings.FiscalYear == "")
            {
                _Settings.FiscalYear = "2072-2073";
            }                    
          

           
            _Settings.LeaveFiscalYear = "21/72";
        }

       

        #endregion
        
    }







}
