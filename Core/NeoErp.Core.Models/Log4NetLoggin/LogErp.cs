using log4net;
namespace NeoErp.Core.Models.Log4NetLoggin
{
    public class LogErp : ILogErp
    {
        private readonly ILog log;


        public LogErp(object value)
        {
            log = LogManager.GetLogger(value.GetType().ToString());
        }
        public LogErp(object value,string userCode , string companyCode,string branchCode,string typeCode,string module,string formCode)
        {
            log = LogManager.GetLogger(value.GetType().FullName);


            LogicalThreadContext.Properties["log_user"] = userCode;
            LogicalThreadContext.Properties["log_company"] = companyCode;
            LogicalThreadContext.Properties["log_branch"] = branchCode;
            LogicalThreadContext.Properties["log_typecode"] = typeCode;
            LogicalThreadContext.Properties["log_module"] = module;
            LogicalThreadContext.Properties["form_code"] = formCode;

        }

        public void DebugInFile(string message)
        {
            log.Debug(message);
        }

        public void ErrorInDB(string message)
        {
            log.Error(message);
        }

        public void Fatal(string message)
        {
            log.Fatal(message);
        }

        public void InfoInFile(string message)
        {
            log.Info(message);
        }

        public void WarnInDB(string message)
        {
            log.Warn(message);
        }
    }
}
