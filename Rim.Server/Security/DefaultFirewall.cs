using System.Collections.Generic;
using System.Linq;

namespace Rim.Server.Security
{
    /// <summary>
    /// Default Firewall class for HttpServer.
    /// This class keeps block and grant ip lists and checks if the client's ip address in these lists or not.
    /// </summary>
    public class DefaultFirewall : IFirewall
    {

        #region Properties

        /// <summary>
        /// Firewall's server
        /// </summary>
        public HttpServer Server { get; private set; }

        /// <summary>
        /// If true, firewall checks ip address if granted of blocked.
        /// If false, accepts all connections.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Firewall operation type
        /// </summary>
        public FirewallType Type { get; set; }

        /// <summary>
        /// Granted IP List
        /// </summary>
        private List<string> GrantList { get; set; }

        /// <summary>
        /// Blocked IP List
        /// </summary>
        private List<string> BlockList { get; set; }

        #endregion

        #region Init - Authority

        public DefaultFirewall(HttpServer server)
        {
            Server = server;
            GrantList = new List<string>();
            BlockList = new List<string>();
            Type = FirewallType.BlockOnlyBlockList;
        }

        /// <summary>
        /// Checks if the IP address is granted.
        /// If granted returns true, otherwise returns false.
        /// </summary>
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

        #endregion

        #region Grant - Block

        /// <summary>
        /// Blocks the IP address.
        /// If the IP address in grant list it will be removed from the grant list.
        /// </summary>
        public void Block(string ip)
        {
            lock (GrantList)
                GrantList.Remove(ip);

            lock (BlockList)
                BlockList.Add(ip);
        }

        /// <summary>
        /// Grants the IP address.
        /// If the IP address in block list it will be removed from the block list.
        /// </summary>
        public void Grant(string ip)
        {
            lock (BlockList)
                BlockList.Remove(ip);

            lock (GrantList)
                GrantList.Add(ip);
        }

        /// <summary>
        /// Gets all blocked IP Addresses
        /// </summary>
        public IEnumerable<string> GetBlocks()
        {
            return BlockList;
        }

        /// <summary>
        /// Gets all granted IP Addresses
        /// </summary>
        public IEnumerable<string> GetGrants()
        {
            return GrantList;
        }

        #endregion

    }
}
