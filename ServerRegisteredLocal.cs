namespace PEPatch
{
    public class ServerRegisteredLocal
    {
        public long ServerUID;
        public int ServerID;
        public int LimitedConn;
        public int CurConn;
        public int ServerStatus;
        public int GameType;
        public int GameMode;
        public int PasswordStatus;
        public int Ping;
        public int Port;
        public string ServerVersion;
        public string ServerName;
        public string ServerMasterAccount;
        public string ServerMasterName;
        public string IPAddress;
        public string UID;
        public string MapName;
        public bool UseProxy;
        public bool IsLan;

        public ServerRegistered ToServerRegistered()
        {
            var rez = new ServerRegistered();

            foreach (var field in typeof(ServerRegisteredLocal).GetFields())
            {
                typeof(ServerRegistered).GetField(field.Name, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)?.SetValue(rez, field.GetValue(this));
            }

            return rez;
        }
    }
}