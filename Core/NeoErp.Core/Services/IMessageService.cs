using NeoErp.Core.Models.CustomModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Core.Services
{
    public interface IMessageService
    {
        int CreateMessageQueue(MessageQueueModel model);
        int CreateMessageQueue(MessageQueueModel model, string userName);
        IEnumerable<MessageQueueModel> GetMessageQueue(MessageProcessStatus processStatus);
        IEnumerable<MessageQueueModel> GetMessageQueue(char status);
        int UpdateMessageStatus(List<string> mailGuids, MessageProcessStatus processStatus, string userName);
        void SendMail();
        void SendSchedularMailDaily();

        void SendSchedularMail();
        //while sending manually  from  Database 
        void SendMail(MailListModel model);
        int SendMailDerect(MailListModel model);
        List<MailListModel> AllMailList();
        string UpdateMailList(MailListModel modal);


        IEnumerable<MessageQueueModel> GetSchedularMessageQueue(MessageProcessStatus processStatus);
        IEnumerable<MessageQueueModel> GetSchedularMessageQueue(char status);
        int UpdateSchedularMessageStatus(List<string> mailGuids, MessageProcessStatus processStatus, string userName);
        // List<MailListModel> AllMailList();
    }
}
