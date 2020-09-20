using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoClient
{
    public enum NPCType
    {
        Beast = 0,
        Player = 5,
        Pet = 7,
        Item = 6,
        Ghost = 13,
        Unknown = -1,
    }

    public enum ItemLocation
    {
        wearing = 1,
        onPersonalPack = 2,
        onStore = 7,
        inStorage = 3,
    }

    public enum PlayerStatus
    {
        DoNothing = 0,
        Running = 1,
        Collecting = 7,
        PrepareHorse = 25,
        UsingTDP = 21,
        Other = 22,
        Death = 5
    }

    public enum HePhai : byte
    {
        Tld = 2,
        Tlt = 3,
        Tlq = 4,
        Dm = 6,
        Nmk = 8,
        Nmd = 9,
        Cbb = 12,
        Vdk = 14,
        Vdb = 15,
        Dgc = 18,
        Hd = 20,
        Cs = 21,
        Clts = 23,
        mgd = 25,
        Mgb = 26,
        Tyvt = 29,
        Tyln = 30,
    }
}
