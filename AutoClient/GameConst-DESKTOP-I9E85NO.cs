using System;

namespace AutoClient
{
    public static class GameConst
    {
        public static IntPtr CheckBaseLatThe = new IntPtr(0x0A4013C); //A3EFDC
        public static IntPtr BaseAddress = new IntPtr(0x00AD7518); // Updat 10.1.32
        public static IntPtr CurrentMappAddress = new IntPtr(0x00AD7D88); // Check 00AD7D88//00B0FAB4//00B0FAB8
        public static IntPtr ComposeItemBaseAddress = new IntPtr(0x00AC8AC0);
        public static IntPtr IsShopOpenedStaticAddress = new IntPtr(0x008E73F4);
        public static IntPtr BaseMessgeHeThong = new IntPtr(0x00AF0A98);

        // Skill NMK
        public static int SenIDKill = Convert.ToInt32(0x40051); // thành 4
        public static int BDIDKill = Convert.ToInt32(0x00050055); // thành 5
        public static int PhoteIDKill = Convert.ToInt32(0x0060054); // thành 6
        public static int LTIDKill = Convert.ToInt32(0x0090053); // thành 9
        public static int NoiIDKill = Convert.ToInt32(0x00060057); // thành 6
        public static int NgoaiIDKill = Convert.ToInt32(0x0060056); // thành 6
        public static int ManaIDKill = Convert.ToInt32(0x006004F); // thành 6
        public static int YCIDKill = Convert.ToInt32(0x0050167); // thành 5
        public static int HaCoIDKill = Convert.ToInt32(0x004015C); // thành 4
        // Skill TLQ
        public static int CDTSKILL = 196644; // Thành 3
        public static int CDTSKILL5 = 327734; // Thành 5
        public static int VVSKILL = 196647; // Thành 3
        public static int VACSKILL = 589866; // Thành 9
        public static int VACSKILL8 = 524354; // Thành 8
        public static int VDQSKILL = 196646; // Thành 3
        public static int DtuSKILL = 458793; // Thành 7
        // Base
        public static IntPtr BaseVeThanh = new IntPtr(0xA4E82C);
        public static IntPtr DialogBaseAddress = new IntPtr(0x00AF2900);
        public static IntPtr DialogBaseTotalLine = new IntPtr(0x00A33768);
        public static IntPtr BASEKILLPHAI = new IntPtr(0x00B0DE88);
        public static IntPtr HuongCo = new IntPtr(0x5016A);
        public static IntPtr CheckThit = new IntPtr(0xA583C4);
        public static IntPtr CheckSoLuongThit = new IntPtr(0xA531B0);

        public static int cmd_start = 1000;
        public static int cmd_end = 1001;
        public static int cmd_push = 1002;
        public const int cmd_sendchar = 1003;

        public static int FUNC_MOVE_TO = 8000;
        public static int FUNC_ENTITY_INTERACT = 8001;
        public static int FUNC_DIALOG_INTERACT = 8002;
        public static int FUNC_BUYITEM = 8003;
        public static int FUNC_USE_HORSE = 8004;
        public static int FUNC_MONEY_INTERACT = 8005;
        public static int FUNC_COMPOSE_ITEM = 8006;
        public static int FUNC_USE_ITEM = 8007;
        public static int FUNC_REBIRTH = 8008;
        public static int FUNC_PICKPUT = 8009;
        public static int FUNC_CHOSAUAN = 8012;
        public static int FUNC_SELECTMENU = 8010;
        public static int FUNC_CHAT = 8011;
        public static int FUNC_BAYBAN = 8014;
        public static int FUNC_MOITV = 8015;
        public static int FUNC_GROUP = 8016;
        public static int FUNC_LEVELUP = 8017;
        public static int FUNC_VUTITEM = 8018;
        public static int FUNC_BANITEM = 8019;
        public static int FUNC_LATBAI = 8020;
        public static int FUNC_NHATITEM = 8021;
        public static int FUNC_LATBAI1 = 8022;
        public static int FUNC_CHONSV = 8024;
        public static int FUNC_CLEAR = 8026;
        public static int FUNC_PHUTHANH = 8027;
        public static int FUNC_MORUONG = 8028;
        public static int FUNC_ENTITY_INTERACT1 = 8029;

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
            Mgb = 26,
            Tyvt = 29,
            Tyln = 30,
        }
    }
}
