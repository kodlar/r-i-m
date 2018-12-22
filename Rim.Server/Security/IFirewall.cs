using System.Collections.Generic;

namespace Rim.Server.Security
{
    public enum FirewallType
    {
        AllowOnlyGrantList,
        BlockOnlyBlockList
    }

    public interface IFirewall
    {
        bool Enabled { get; set; }
        FirewallType Type { get; set; }

        void Block(string ip);
        void Grant(string ip);

        IEnumerable<string> GetBlocks();
        IEnumerable<string> GetGrants();
        
        bool EntryAuthority(string ip);
    }
}
