using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Data
{

    public struct ConCredential
    {
        public ConnectionType ConType;
        public string Server;
        public string UserName;
        public string Password;
        public string Database;
        public bool IsLocalDB;
        public string Datapath;
    }

    public enum ConnectionType
    {
        Access = 1,
        SqlServer,
        Oracle
    }
}
