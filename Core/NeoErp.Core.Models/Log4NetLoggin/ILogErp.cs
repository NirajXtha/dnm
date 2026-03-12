namespace NeoErp.Core.Models.Log4NetLoggin
{
    public interface ILogErp
    {
        void DebugInFile(string message);
        void InfoInFile(string message);
        void WarnInDB(string message);
        void ErrorInDB(string message); 
        void Fatal(string message);

    }
}
