using System.Collections.Generic;

namespace Rim.Server.Security
{
    /// <summary>
    /// Firewall working type to block and grant types
    /// </summary>
    public enum FirewallType
    {
        /// <summary>
        /// Blocks all connections and grants IP Addresses only in grant list
        /// </summary>
        AllowOnlyGrantList,

        /// <summary>
        /// Grants all connections and blocks IP Addresses only in block list
        /// </summary>
        BlockOnlyBlockList
    }

    /// <summary>
    /// Firewall implementation for HttpServer object
    /// </summary>
    public interface IFirewall
    {
        /// <summary>
        /// If true, firewall checks ip address if granted of blocked.
        /// If false, accepts all connections.
        /// </summary>
        bool Enabled { get; set; }

        /// <summary>
        /// Firewall operation type
        /// </summary>
        FirewallType Type { get; set; }

        /// <summary>
        /// Blocks the IP address.
        /// If the IP address in grant list it will be removed from the grant list.
        /// </summary>
        void Block(string ip);

        /// <summary>
        /// Grants the IP address.
        /// If the IP address in block list it will be removed from the block list.
        /// </summary>
        void Grant(string ip);

        /// <summary>
        /// Gets all blocked IP Addresses
        /// </summary>
        IEnumerable<string> GetBlocks();

        /// <summary>
        /// Gets all granted IP Addresses
        /// </summary>
        IEnumerable<string> GetGrants();
     
        /// <summary>
        /// Checks if the IP address is granted.
        /// If granted returns true, otherwise returns false.
        /// </summary>
        bool EntryAuthority(string ip);

    }
}
