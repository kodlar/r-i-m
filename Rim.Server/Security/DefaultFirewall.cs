using System.Collections.Generic;
using System.Linq;

namespace Rim.Server.Security
{
    public class DefaultFirewall : IFirewall
    {

        #region Properties

        public HttpServer Server { get; private set; }
        public bool Enabled { get; set; }
        public FirewallType Type { get; set; }

        private List<string> GrantList { get; set; }
        private List<string> BlockList { get; set; }

        #endregion

        public DefaultFirewall(HttpServer server)
        {
            Server = server;
            GrantList = new List<string>();
            BlockList = new List<string>();
            Type = FirewallType.BlockOnlyBlockList;
        }

        public bool EntryAuthority(string ip)
        {
            if (!Enabled)
                return true;

            switch(Type)
            {
                case FirewallType.AllowOnlyGrantList:
                    return GrantList.Any(x => x == ip);

                case FirewallType.BlockOnlyBlockList:
                    if (BlockList.Any(x => x == ip))
                        return false;

                    return true;

                default:
                    return false;
            }

        }

        public void Block(string ip)
        {
            lock (GrantList)
                GrantList.Remove(ip);

            lock (BlockList)
                BlockList.Add(ip);
        }

        public void Grant(string ip)
        {
            lock (BlockList)
                BlockList.Remove(ip);

            lock (GrantList)
                GrantList.Add(ip);
        }

        public IEnumerable<string> GetBlocks()
        {
            return BlockList;
        }

        public IEnumerable<string> GetGrants()
        {
            return GrantList;
        }

    }
}
