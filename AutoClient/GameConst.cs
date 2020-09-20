using System;

namespace AutoClient
{
    public static class GameConst
    {
        public static IntPtr BaseAddress = (IntPtr)0x00AD8518; // Updat 10.1.38
        public static IntPtr CurrentMappAddress = (IntPtr)0x00B10AC8; // Updat 10.1.38
        public static IntPtr BaseMenuMothe = (IntPtr)0x00AEFC90; // Updat 10.1.38
        // Base
        public static IntPtr BaseVeThanh = (IntPtr)0x00B10AA8; // Updat 10.1.38
        public static IntPtr DialogBaseAddress = (IntPtr)0x00B2DBD4; // Update 10.1.38
        public static IntPtr DialogBaseTotalLine = (IntPtr)0x00A3476C;//Update 10.1.38
        public static IntPtr BASEKILLPHAI = (IntPtr)0x00B0EE98;//Update 10.1.38
        public static IntPtr Bangnhanvat = (IntPtr)0x00B05778;//Update 10.1.38


        // Skill NMK
        public static int SenIDKill = (Int32)0x40051; // thành 4
        public static int BDIDKill = (Int32)0x00050055; // thành 5
        public static int PhoteIDKill = (Int32)0x0060054; // thành 6
        public static int LTIDKill = (Int32)0x0090053; // thành 9
        public static int NoiIDKill = (Int32)0x00060057; // thành 6
        public static int NgoaiIDKill = (Int32)0x0060056; // thành 6
        public static int ManaIDKill = (Int32)0x006004F; // thành 6
        public static int YCIDKill = (Int32)0x0050167; // thành 5
        public static int HaCoIDKill = (Int32)0x004015C; // thành 4
        // Skill TLQ
        public static int CDTSKILL = 196644; // Thành 3
        public static int CDTSKILL5 = 327734; // Thành 5
        public static int VVSKILL = 196647; // Thành 3
        public static int VACSKILL = 589866; // Thành 9
        public static int VACSKILL8 = 524354; // Thành 8
        public static int VDQSKILL = 196646; // Thành 3
        public static int DtuSKILL = 458793; // Thành 7
        public static int ThatbaoSKILL = 655394; // Thành 7

        // Base chon SV
        public static int BaseSV = (Int32)0x00AEDEF0; // Base Name server

        public static int[] Thebaimienphi = {0x000000B, 0x001000B, 0x002000B, 0x003000B, 0x004000B, 0x005000B, 0x006000B, 0x007000B};
        public static int[] ThebaiTKL = {0x008000B, 0x009000B, 0x010000B, 0x011000B, 0x012000B, 0x013000B, 0x014000B, 0x015000B};

        public static int[] Toadobaithietphu12T = {40338,95129};
        public static int[] Toadobaichebidao12T = {43606,92067};
        public static int[] Toadobaicuocchim5T = {42516,93931};
        public static int[] Toadobaicuocchim12L = {43530,98500};
        public static int[] Toadobaichebidao5L = {44738,96317};
        public static int[] Toadobaicuocthuoc12L = {46928,95219};
        public static int[] Toadobaithietphu6Tren = {48160,90248};
        public static int[] Toadobaithietphu5Duoi = {39528,97468};
        public static int[] Toadobaicuocchim6Duoi = {41708,99569};
        public static int[] Toadobaicuocthuoc5Duoi = {39933,101589};
        public static int[] ToadotranvTong = {39609,91314};
        public static int[] ToadotrunggianTong = {40031,92216};
        public static int[] ToadocongTong = {41445,92716};
        public static int[] ToadotranvLieu = {47852,99466};
        public static int[] ToadotrunggianLieu = {47001,99408};
        public static int[] ToadocongLieu = {46467,97550};
        public static int[] Toadomuadungcu = {43272,95796};

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
        public static int FUNC_BAT_DAU = 8030;
        public static int FUNC_CHON_SEVER = 8031;
        public static int FUNC_CHON_NV = 8032;
        public static int FUNC_BAT_DAU_NV = 8033;
    }
}
