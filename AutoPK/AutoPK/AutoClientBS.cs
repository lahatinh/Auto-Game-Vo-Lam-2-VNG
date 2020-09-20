using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoPK
{
    public class AutoClientBS : AutoClient.AutoClient
    {
        public bool ischecked = false;
        public string Featurebuff = "";

        public List<String> _Danhsachbuff = new List<string>();
        public List<String> _Danhsachquai = new List<string>();
        public List<String> _DanhsachPlayerGroup = new List<string>();

        public bool Featurenmk = false;
        public bool ComboPKTLQ = false;
        public bool FeatureHoiPhuc = false;
        public bool buffdanhsach = false;
        public bool Bufftheosau = false;

        public int Aiphoban;
        public int[] ToadovitridaoTNC = {0,0};


        public string[] arraytoadonv;
        public string[] arraytoadoTrain01;
        public string[] arraytoadoTrain02;
        public string[] arraytoadoTrain03;


        public bool Autobuffnmk = true;
        public string PlayerTheoSau = string.Empty;
        public int StepbuffNMK;
        public int StepTrain;
        public int StepTrain1;
        public int StepKillBoss;
        public int StepKillBossvuotai;
        public int Tranhiemvu;



        // Mặc định giá trị
        public int timevac = 0;
        public int timedt = 0;
        public int timecdt = 0;
        public int timerun = 0;

        public int HP = 0;
        public int HPuse = 0;
        public int CCuse = 0;
        public int Delay = 0;
        public int ProsX = 0;
        public int ProsY = 0;
        public int ProsXnew = 0;
        public int ProsYnew = 0;
        public int SinhnoiUse = 0;
        public int ManaUse = 0;
        public int LakCTUse = 0;
        public int ThelucUse = 0;


        //Cài đặt phím
        public string phimdung = "";
        public string phimvac = "";
        public string phimvacdt = "";
        public string phimvaccdt = "";
        public string phimdt1o = "";

        public string phimthaydo1 = "";
        public string phimthaydo2 = "";
        public string phimthaydo3 = "";
        public string phimthaydo4 = "";

        // Lưu Vật phẩm phục hồi
        public string ItemSinhLuc = "";
        public string ItemSinhNoi = "";
        public string ItemNoiLuc = "";
        public string ItemCuuChuyen = "";
        public string ItemLakCtr = "";
        public string ItemTheLuc = "";

        // Lưu giữ giá trị combobox Click NPC
        public string ValueClickNPC = "";
        public string ValueClickMenu1 = "";
        public string ValueClickMenu2 = "";
        public string ValueClickMenu3 = "";
        public string ValueClickMenu4 = "";
        public int ValueGiancach = 0;

        //Lưu giá trị textbox AutoTrain
        public string ValueToadoNV = "";
        public string ValueToadoTrain01 = "";
        public string ValueToadoTrain02 = "";
        public string ValueToadoTrain03 = "";
        public string ValueSkindanhquai = "";
        public string ValueSkinBuff01 = "";
        public string ValueSkinBuff02 = "";
        public string ValueBossKills = "";
        public string ValueBossKillsvuotai = "";


        // Auto rao vặt và auto Tài nguyên chiến
        public string ValueLoirao = "";
        public string ValueQuancong = "";
        public bool ckautoTNC = false;
        public bool cktutranv = false;
        public bool cktumuadungcu = false;
        public bool ckraovat = false;
        public bool ckphetong = false;
        public int BaidaoTNC;


        //Tham số các checkbox
        public bool ckxdamevac = false;
        public bool cksutngatvac = false;
        public bool ckcombodt = false;
        public bool ckvaccdt = false;
        public bool cksutkngatvac = false;
        public bool ckdt1o = false;

        public bool ckthaydo = false;
        public bool ckthaydobo1 = false;
        public bool ckthaydobo2 = false;
        public bool ckthaydobo3 = false;
        public bool ckthaydobo4 = false;

        public bool ckbommau = false;
        public bool ckbomsinhnoi = false;
        public bool ckbommana = false;
        public bool ckancc = false;
        public bool ckanlakctr = false;
        public bool ckantheluc = false;

        public bool ckClickNPC = false;
        public bool ckClickMenu1 = false;
        public bool ckClickMenu2 = false;
        public bool ckClickMenu3 = false;
        public bool ckClickMenu4 = false;

        public bool ckautotrain = false;
        public bool CkToadoTrain01 = false;
        public bool CkToadoTrain02 = false;
        public bool CkToadoTrain03 = false;
        public bool CkSkindanhquai = false;
        public bool CkSkinBuff01 = false;
        public bool CkSkinBuff02 = false;
        public bool CkKillBoss = false;
        public bool CkKillBossvuotai = false;

        public bool CkThainhatthap = false;
        public bool CkChukey = false;
        public bool CkVuotaiTKL = false;
        public bool CkmoruongTKL = false;
        public bool CklattheTKL = false;
        public bool Ckacctreo = false;

        // Auto tổ đội
        public bool Ckautotodoi = false;
        public bool Ckmoitatca = false;
        public bool Cknhaptatca = false;
        public bool Cknhanloimoitodoi = false;
        public bool Ckchapnhanxinnhapdoi = false;


        // Auto sử dụng bảo hạp + ngôi sao may mắn
        public bool Ckdotphao = false;
        public bool Cknuoabaohap = false;
        public bool Ckngoisaomayman = false;


        //Lưu giữ các List
        public List<string> Listbuff = new List<string>(7); // List buff NMK
        public List<string> Listthaydo1 = new List<string>(25); // List thay do
        public List<string> Listthaydo2 = new List<string>(25); // List thay do
        public List<string> Listthaydo3 = new List<string>(25); // List thay do
        public List<string> Listthaydo4 = new List<string>(25); // List thay do
    }
}
