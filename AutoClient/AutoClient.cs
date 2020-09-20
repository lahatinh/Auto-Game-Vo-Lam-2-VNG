using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoClient.Entities;
using System.Windows.Forms;
using System.Threading;
using System.Reflection;

namespace AutoClient
{
    public class AutoClient
    {
        public List<PlayerEntity> EntityList = new List<PlayerEntity>();
        public List<GameItem> ItemList = new List<GameItem>();

        public IntPtr OpenProcessHandle;
        public IntPtr WindowHwnd;
        public uint ProcessID;
        public uint HookMsg;
        private string[] arritemNameID;
        private string[] arritemNameItemOnPlayer;
        private string[] arritemNamequaiID;


        private PlayerEntity _currentPlayer;

        public string MsgScrip = "";


        // Thủ tục nạp các Player vào Client
        public AutoClient()
        {
            this._currentPlayer = new PlayerEntity(this);
        }

        // Thủ tục nạp ID map game hiện tại
        public int CurrentMapId
        {
            get
            {
                return WinAPI.ReadProcessMemoryInt32(OpenProcessHandle, GameConst.CurrentMappAddress);
            }
        }

        // Thủ tục nhận dạng có bảng menu kích mở thẻ
        public int Menumothe
        {
            get
            {
                return WinAPI.ReadProcessMemoryInt32(OpenProcessHandle, GameConst.BaseMenuMothe);
            }
        }

        // Thủ tục sử dụng Base Address
        public IntPtr UserBaseAddress
        {
            get
            {
                var step1 = WinAPI.ReadProcessMemoryIntPtr(OpenProcessHandle, GameConst.BaseAddress);
                step1 = IntPtr.Add(step1, 0x331AC8);
                var step2 = WinAPI.ReadProcessMemoryIntPtr(OpenProcessHandle, step1);
                var step3 = WinAPI.ReadProcessMemoryIntPtr(OpenProcessHandle, step2);
                step3 = IntPtr.Add(step3, 0x4);
                var step4 = WinAPI.ReadProcessMemoryIntPtr(OpenProcessHandle, step3);
                step4 = IntPtr.Add(step4, 0xF8);
                var step5 = WinAPI.ReadProcessMemoryIntPtr(OpenProcessHandle, step4);

                return step5;
            }

        }

        // Thủ tục sử dụng Base address
        public IntPtr EntityBaseAddress
        {
            get
            {
                var step1 = WinAPI.ReadProcessMemoryIntPtr(OpenProcessHandle, GameConst.BaseAddress);
                step1 = IntPtr.Add(step1, 0x331AC8 + 0x4);
                var step2 = WinAPI.ReadProcessMemoryIntPtr(OpenProcessHandle, step1);
                var step3 = WinAPI.ReadProcessMemoryIntPtr(OpenProcessHandle, step2);
                step3 = IntPtr.Add(step3, 0x4);
                return step3;
            }
        }

        // Thủ tục get số lần đào TNC
        public int SolandaoTNC
        {
            get
            {
                var step1 = WinAPI.ReadProcessMemoryIntPtr(OpenProcessHandle, GameConst.BaseAddress);
                step1 = IntPtr.Add(step1, 0x20448C);// Offset So lan dao TNC

                return WinAPI.ReadProcessMemoryInt32(OpenProcessHandle, step1);
            }
        }

        // Thủ tục get Base menu
        public IntPtr UseBaseAddressMenu
        {
            get
            {
                var step1 = WinAPI.ReadProcessMemoryIntPtr(OpenProcessHandle, GameConst.DialogBaseAddress);
                step1 = IntPtr.Add(step1, 0x70);
                var step2 = WinAPI.ReadProcessMemoryIntPtr(OpenProcessHandle, step1);
                step2 = IntPtr.Add(step2, 0x6C);
                var step3 = WinAPI.ReadProcessMemoryIntPtr(OpenProcessHandle, step2);
                step3 = IntPtr.Add(step3, 0x6C);
                var step4 = WinAPI.ReadProcessMemoryIntPtr(OpenProcessHandle, step3);
                step4 = IntPtr.Add(step4, 0x6C);
                var step5 = WinAPI.ReadProcessMemoryIntPtr(OpenProcessHandle, step4);
                step5 = IntPtr.Add(step5, 0x70);
                var step6 = WinAPI.ReadProcessMemoryIntPtr(OpenProcessHandle, step5);
                step6 = IntPtr.Add(step6, 0xC8);
                var step7 = WinAPI.ReadProcessMemoryIntPtr(OpenProcessHandle, step6);
                return step7;
            }
        }

        // Thủ tục làm mới danh sách Game
        public void RefreshEntityList()
        {
            EntityList.Clear();

            var baseAdd = EntityBaseAddress;

            for (int i = 0; i < 256; i++)
            {
                var entity = new PlayerEntity(WinAPI.ReadProcessMemoryIntPtr(OpenProcessHandle, IntPtr.Add(baseAdd, i * 4)), this);

                if (entity.HitPoint > 0 && !EntityList.Contains(entity))
                {
                    EntityList.Add(entity);
                }
            }

        }

        // Thủ tục bộ nhớ Base Address
        public IntPtr StorageAddress
        {
            get
            {
                var step1 = WinAPI.ReadProcessMemoryIntPtr(OpenProcessHandle, GameConst.BaseAddress);
                var step2 = IntPtr.Add(step1, 0x331AF4);
                var step3 = WinAPI.ReadProcessMemoryIntPtr(OpenProcessHandle, step2);
                var step4 = WinAPI.ReadProcessMemoryIntPtr(OpenProcessHandle, step3);
                return step4;
            }
        }

        // Thủ tục sử dụng bộ nhớ địa chỉ
        public IntPtr UserStorageAddress
        {
            get
            {
                var step1 = WinAPI.ReadProcessMemoryIntPtr(OpenProcessHandle, GameConst.BaseAddress);
                var step2i = IntPtr.Add(step1, 0x331AC8);
                var step3i = WinAPI.ReadProcessMemoryIntPtr(OpenProcessHandle, step2i);
                var step4i = WinAPI.ReadProcessMemoryIntPtr(OpenProcessHandle, step3i);
                var step5i = IntPtr.Add(step4i, 0x4);
                var step6i = WinAPI.ReadProcessMemoryIntPtr(OpenProcessHandle, step5i);
                var step7i = IntPtr.Add(step6i, 0x04E4);
                return step7i;
            }
        }

        // Thủ tục đọc memory của process hiện tại
        protected IntPtr ReadCurrentProcessMemory(IntPtr address)
        {
            return WinAPI.ReadProcessMemoryIntPtr(OpenProcessHandle, address);
        }

        // Thủ tục đọc memory của process hiện tại dạng Integer
        protected int ReadCurrentProcessMemoryReturnInt(IntPtr address)
        {
            return WinAPI.ReadProcessMemoryInt32(OpenProcessHandle, address);
        }

        // Thủ tục làm mới danh sách Items
        public void RefreshStorageItems()
        {
            ItemList.Clear();

            var storageAddress = StorageAddress;
            var userStorageAddress = UserStorageAddress;

            for (int i = 0; i < 0x012C; i++)
            {

                var ptr1 = IntPtr.Add(userStorageAddress, i * 0x0014);
                var x = WinAPI.ReadProcessMemoryInt32(OpenProcessHandle, ptr1);
                ptr1 = IntPtr.Add(storageAddress, x * 4);
                ptr1 = WinAPI.ReadProcessMemoryIntPtr(OpenProcessHandle, ptr1);

                var item = new GameItem(ptr1, this);
                item.PositionAddress = IntPtr.Add(userStorageAddress, i * 0x0014);
                if (item.ItemName.Trim() != "")
                {
                    ItemList.Add(item);
                }
            }
        }

        // Thủ tục nhận dạng Player hiện tại
        public PlayerEntity CurrentPlayer
        {
            get
            {
                if (_currentPlayer.Address != UserBaseAddress)
                    _currentPlayer.Address = UserBaseAddress;
                return _currentPlayer;
            }
        }

        // Thủ tục nạp handle và client
        public void Attach(IntPtr hwnd)
        {
            this.WindowHwnd = hwnd;
            WinAPI.GetWindowThreadProcessId(WindowHwnd, out ProcessID);
            OpenProcessHandle = WinAPI.OpenProcess(WinAPI.ProcessAccessFlags.All, true, Convert.ToInt32(ProcessID));
        }

        // Thủ tục định nghĩa chưa Inject hook
        private bool _isInjected = false;

        // Thủ tục định nghĩa chưa Inject hook
        public bool isInjected
        {
            get { return _isInjected; }
        }

        // Thủ tục Inject hook
        public int Inject()
        {
            var result = HookGame.InjectDll(WindowHwnd);
            if (result == 1)
            {
                _isInjected = true;
                this.HookMsg = HookGame.GetMsg();
            }
            return result;
        }

        // Thủ tục DeInject hook
        public int DeInject()
        {
            var result = HookGame.UnmapDll(WindowHwnd);
            _isInjected = false;
            return result;
        }

        // Thủ tục lấy vàng của nhân vật hiện tại
        public int CurrentPlayerMoney
        {
            get
            {
                var ptr = ReadCurrentProcessMemory(GameConst.BaseAddress);
                ptr = IntPtr.Add(ptr, 0x26e558);
                ptr = ReadCurrentProcessMemory(ptr);
                ptr = ReadCurrentProcessMemory(ptr);
                ptr = IntPtr.Add(ptr, 0x4);
                ptr = ReadCurrentProcessMemory(ptr);
                ptr = IntPtr.Add(ptr, 0x1d4);

                return WinAPI.ReadProcessMemoryInt32(OpenProcessHandle, ptr);
            }
        }

        // Gửi thông điệp đến cho hook xử lý
        public void MoveTo(int posX, int posY, int mapID)
        {

            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_start, GameConst.FUNC_MOVE_TO);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push, posX);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push, posY);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push, mapID);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_end, 0);
        }

        // Gửi thông điệp đến cho hook xử lý
        public void EntityInteract(int type, int param1, int param2, int param3, int param4, int param5, int param6)
        {
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_start, GameConst.FUNC_ENTITY_INTERACT); //SendNPC
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push, type);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push, param1);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push, param2);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push, param3);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push, param4);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push, param5);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push, param6);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push, CurrentPlayer.Address.ToInt32());
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_end, 0);

        }

        // Gửi thông điệp đến cho hook xử lý
        public void DialogInteract(int actionType, int param1, int param2)
        {

            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_start, GameConst.FUNC_DIALOG_INTERACT); //Menu
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push, actionType);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push, param1);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push, param2);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_end, 0);
        }

        // Gửi thông điệp đến cho hook xử lý
        public void BuyItem(int itemPos, int param2, int quantity, int param4, int param5) //Mua Item
        {
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_start, GameConst.FUNC_BUYITEM);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push, itemPos);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push, param2);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push, quantity);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push, param4);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push, param5);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_end, 0);

        }

        // Gửi thông điệp đến cho hook xử lý
        public void UseHorse(bool use) //Su dung ngua
        {
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_start, GameConst.FUNC_USE_HORSE);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push, CurrentPlayer.Address.ToInt32());
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push, use ? 5 : 6);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_end, 0);
        }

        // Gửi thông điệp đến cho hook xử lý
        public void ComposeItem(int itemId) //Che item
        {
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_start, GameConst.FUNC_COMPOSE_ITEM);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push, itemId);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_end, 0);

        }

        // Gửi thông điệp đến cho hook xử lý
        public void SelectMenu(int menuxId) //Che item
        {
            int result = ReadCurrentProcessMemoryReturnInt(GameConst.DialogBaseAddress);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_start, GameConst.FUNC_SELECTMENU);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push, result);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push, menuxId);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_end, 0);

        }

        // Gửi thông điệp đến cho hook xử lý
        public void MoneyInteract(int money, int param1, int param2) //Tien
        {
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_start, GameConst.FUNC_MONEY_INTERACT);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push, money);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push, param1);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push, param2);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_end, 0);
        }

        // Gửi thông điệp đến cho hook xử lý
        public void UseItem(int colIndex, int rowIndex) //Su dung item
        {
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_start, GameConst.FUNC_USE_ITEM);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push, colIndex);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push, rowIndex);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_end, 0);
        }

        // Gửi thông điệp đến cho hook xử lý
        public void VeThanh(int param1) //
        {
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_start, GameConst.FUNC_REBIRTH);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push, param1);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push, GameConst.BaseVeThanh.ToInt32());
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_end, 0);

        }

        // Gửi thông điệp đến cho hook xử lý
        public void XuongNgua(int use) //Su dung ngua
        {
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_start, GameConst.FUNC_USE_HORSE);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push, CurrentPlayer.Address.ToInt32());
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push, use);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_end, 0);
        }

        // Gửi thông điệp đến cho hook xử lý
        public void Chat() //Auto Rao
        {
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_start, GameConst.FUNC_CHAT);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push, CurrentPlayer.Address.ToInt32());
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_end, 0);

        }

        // Gửi thông điệp đến cho hook xử lý
        public void LatBai(int bai) //Moi Ty Vo
        {
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_start, GameConst.FUNC_LATBAI);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push, bai);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_end, 0);

        }

        // Gửi thông điệp đến cho hook xử lý
        public void chonsv(int bai) //Moi Ty Vo
        {
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_start, GameConst.FUNC_CHONSV);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push, bai);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_end, 0);

        }

        // Gửi thông điệp đến cho hook xử lý
        public void Group(int param1, int param2, int param3, int userblock) //To Doi
        {
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_start, GameConst.FUNC_GROUP);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push, param1);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push, param2);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push, param3);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push, userblock);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_end, 0);

        }

        // Base sử dụng Item
        public IntPtr UseBaseDungItem
        {
            get
            {
                var step1 = WinAPI.ReadProcessMemoryIntPtr(OpenProcessHandle, GameConst.BaseAddress);
                step1 = IntPtr.Add(step1, 0x331AC8);
                var step2 = WinAPI.ReadProcessMemoryIntPtr(OpenProcessHandle, step1);
                var step3 = WinAPI.ReadProcessMemoryIntPtr(OpenProcessHandle, step2);
                step3 = IntPtr.Add(step3, 0x4);
                var step4 = WinAPI.ReadProcessMemoryIntPtr(OpenProcessHandle, step3);
                return step4;
            }
        }

        // Lập tổ đội
        public void laptodoi()
        {
            Group(0, 0, 0, CurrentPlayer.Idplayer);
        }
        // Giải tán tổ đội
        public void giaitantodoi()
        {
            Group(0, 0, 2,CurrentPlayer.Idplayer);
        }
        // Nhận lời mời tổ đội
        public void nhanloimoitodoi(int IDplayer)
        {
            Group(1, IDplayer, 6, CurrentPlayer.Idplayer);
        }
        // Xin gia nhập tổ đội
        public void xinnhaptodoi(int IDplayer)
        {
            Group(0, IDplayer, 4, CurrentPlayer.Idplayer);
        }
        // Mời tổ đội
        public void moitodoi(int IDplayer)
        {
            Group(0, IDplayer, 3, CurrentPlayer.Idplayer);
        }
        // chấp nhận gia nhập tổ đội
        public void chapnhanchonhaptodoi(int IDplayer)
        {
            Group(1, IDplayer, 7, CurrentPlayer.Idplayer);
        }

        /* Bỏ không dùng nữa
        // Gửi thông điệp chat đến game
        public void Doscrip(string msg)
        {
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_start, GameConst.FUNC_CHAT);

            if (!msg.Equals(this.MsgScrip))
            {
                this.MsgScrip = msg;

                byte[] array = Encoding.Default.GetBytes(fontConvert.ConvertUnicodeToTcvn3(msg));

                for (int i = 0; i < array.Length; i++)
                {
                    byte lParam = array[i];
                    WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_sendchar, lParam);
                }
                WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_sendchar, 0);
            }

            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_end, GameConst.FUNC_CHAT);
        }
        */

        // Thủ tục nhận hồi sinh
        public void NhanHoiSinh()
        {
            if (_currentPlayer.PlayerStatus == PlayerStatus.Death)
            {
                VeThanh(17);
            }
        }

        // Thủ tục về thành
        public void NhanVeThanh()
        {
            if (_currentPlayer.PlayerStatus == PlayerStatus.Death)
            {
                VeThanh(3);
            }
        }

        // Thủ tục lấy vàng nhân vật
        public void DrawCash(int money)
        {
            MoneyInteract(money, 2, 3);
        }

        // Thủ tục lưu vàng nhân vật
        public void SaveCash(int money)
        {
            MoneyInteract(money, 3, 2);
        }

        // Thủ tục buff skin cho bản thân nhân vật
        public void SelfBuffSkill(int skillID)
        {
            var posX = Convert.ToInt32(CurrentPlayer.PositionX);
            var posY = Convert.ToInt32(CurrentPlayer.PositionY);

            EntityInteract(3, posX, posY, skillID, 0, 0, 0);
        }

        // Thủ tục buff skin cho các nhân vật
        public void BuffSkill(int skillID, int posX, int posY)
        {
            EntityInteract(3, posX, posY, skillID, 0, 0, 0);
        }

        // Thủ tục buff skin cho các nhân vật
        public void AttackVictim(int skillID, uint VictimID)
        {
            EntityInteract(3, -1, Convert.ToInt32(VictimID), skillID, 0, 0, 0);
        }

        // Gửi thông điệp đến Hook
        public void Chosauan(int colIndex, int rowIndex)
        {
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_start, GameConst.FUNC_CHOSAUAN);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push,  colIndex);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push,  rowIndex);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_end, 0);

        }

        // Gửi thông điệp đến Hook
        public void PickNPutItem(int locationPick, int colPick, int rowPick, int locationPut, int colPut, int rowPut)
        {
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_start, GameConst.FUNC_PICKPUT);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push, locationPick);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push, colPick);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push, rowPick);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push, locationPut);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push, colPut);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push, rowPut);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push, UseBaseDungItem.ToInt32());
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_end, 0);
        }

        // Gửi lấy tổng số dòng menu
        public int GetTotalDialogLines()
        {
            return WinAPI.ReadProcessMemoryInt32(OpenProcessHandle, GameConst.DialogBaseTotalLine);
        }

        // thủ tục nhận biết có menu mở
        public bool IsDialogOpened
        {
            get
            {
                var pointer = ReadCurrentProcessMemory(GameConst.DialogBaseAddress);
                return (pointer.ToInt32() != 0);
            }
        }

        // thủ tục chọn dòng menu
        public void Chonmenu(string name1)
        {
            var totalLines = GetTotalDialogLines();

            for (var i = 0; i < totalLines; i++)
            {
                var temp = UseBaseAddressMenu + i * 0x4;
                temp = ReadCurrentProcessMemory(temp);
                temp += 0x38;
                string name = (fontConvert.TCVN3ToNoMark(WinAPI.ReadProcessMemoryString(OpenProcessHandle, temp, 1000)));
                if (name == name1)
                {
                    SelectMenu(i);
                }
            }
        }

        // thủ tục click NPC Game
        public void ClickNPC(string name)
        {
            if (name != "" && name != null)
            {
                RefreshEntityList();

                var EntityListClickNPC = new List<PlayerEntity>(EntityList);

                arritemNameID = name.Split('/');

                PlayerEntity result = null;
                int minDistance = int.MaxValue;
                var posX = CurrentPlayer.PositionX;
                var posY = CurrentPlayer.PositionY;

                foreach (var entity in EntityListClickNPC)
                {
                    if (entity.EntityNameNoMark.Contains(arritemNameID[1]) && entity.EntityType == NPCType.Item && entity.EntityNameNoMark == arritemNameID[1])
                    {
                        int d = Convert.ToInt32(GameGeneral.Distance(posX, posY, entity.PositionX, entity.PositionY));
                        if (d < minDistance)
                        {
                            minDistance = d;
                            result = entity;
                            break;
                        }
                    }
                }

                if (result != null)
                {
                    TalkToEntity(result.EntityId);
                }
                
            }
            
        }

        // thủ tục click NPC Game trong các ải
        public void ClickNPCvuotai(string name)
        {
            if (name != "" && name != null)
            {
                RefreshEntityList();

                var EntityListClickNPCvuotai = new List<PlayerEntity>(EntityList);

                PlayerEntity result = null;
                int minDistance = int.MaxValue;

                var posX = CurrentPlayer.PositionX;
                var posY = CurrentPlayer.PositionY;

                foreach (var entity in EntityListClickNPCvuotai)
                {
                    if (entity.EntityNameNoMark.Contains(name) && entity.EntityType == NPCType.Item && entity.EntityNameNoMark == name)
                    {
                        int d = Convert.ToInt32(GameGeneral.Distance(posX, posY, entity.PositionX, entity.PositionY));
                        if (d < minDistance)
                        {
                            minDistance = d;
                            result = entity;
                            break;
                        }
                    }
                }

                if (result != null)
                {
                    TalkToEntity(result.EntityId);
                }
            }
        }

        // thủ tục Buff cho các nhân vật game
        public void BuffPlayer(string NpcName)
        {
            if (NpcName != "" && NpcName != null)
            {

                RefreshEntityList();

                if (EntityList.Count < 1)
                    return;

                var EntityListBuffPlayer = new List<PlayerEntity>(EntityList);

                PlayerEntity result = null;
                int minDistance = int.MaxValue;

                var posX = CurrentPlayer.PositionX;
                var posY = CurrentPlayer.PositionY;

                foreach (var entity in EntityListBuffPlayer)
                {
                    if (entity.EntityType == NPCType.Player && entity.EntityNameNoMark == NpcName)
                    {
                        int d = Convert.ToInt32(GameGeneral.Distance(posX, posY, entity.PositionX, entity.PositionY));
                        if (d < minDistance)
                        {
                            minDistance = d;
                            result = entity;
                            break;
                        }
                    }
                }

                if (result != null)
                {
                    AttackVictim(GameConst.SenIDKill, result.EntityId);
                }
            }
        }

        // thủ tục đào TNC
        public void ClickTNC()
        {
            RefreshEntityList();

            if (EntityList.Count < 1)
                return;

            var EntityListClickTNC = new List<PlayerEntity>(EntityList);

            int minDistance = 200;

            var posX = CurrentPlayer.PositionX;
            var posY = CurrentPlayer.PositionY;

            foreach (var entity in EntityListClickTNC)
            {
                if (entity.EntityType == NPCType.Item)
                {
                    int d = Convert.ToInt32(GameGeneral.Distance(posX, posY, entity.PositionX, entity.PositionY));
                    if (d > 0 && d < minDistance)
                    {
                        minDistance = d;

                        TalkToEntity(entity.EntityId);

                        break;
                    }
                }
            }
        }
        // thủ tục đánh quái vuot ai
        public void Trainquaivuotai(int IDskin, string name)
        {
            if (name != "" && name != null && IDskin.ToString() != "" && IDskin.ToString() != null)
            {
                RefreshEntityList();

                if (EntityList.Count < 1)
                    return;

                var EntityListTrainvuotai = new List<PlayerEntity>(EntityList);

                    var posX = CurrentPlayer.PositionX;
                    var posY = CurrentPlayer.PositionY;

                    foreach (var entity in EntityListTrainvuotai)
                    {
                        if (entity.EntityNameNoMark.Contains(name) && entity.EntityType == NPCType.Beast && entity.EntityNameNoMark == name.ToString())
                        {
                            int d = Convert.ToInt32(GameGeneral.Distance(posX, posY, entity.PositionX, entity.PositionY));

                            if (d < 800 && (CurrentPlayer.Hephai == (int)HePhai.Nmk || CurrentPlayer.Hephai == (int)HePhai.Dgc || CurrentPlayer.Hephai == (int)HePhai.Tyvt || CurrentPlayer.Hephai == (int)HePhai.Tyln || CurrentPlayer.Hephai == (int)HePhai.Nmd || CurrentPlayer.Hephai == (int)HePhai.Mgb || CurrentPlayer.Hephai == (int)HePhai.Vdk || CurrentPlayer.Hephai == (int)HePhai.Vdb || CurrentPlayer.Hephai == (int)HePhai.Tlt || CurrentPlayer.Hephai == (int)HePhai.Clts || CurrentPlayer.Hephai == (int)HePhai.Dm || CurrentPlayer.Hephai == (int)HePhai.Hd))
                            {
                                BuffSkill(IDskin, entity.PositionX, entity.PositionY);
                                break;
                            }
                            else if (d > 800 && (CurrentPlayer.Hephai == (int)HePhai.Nmk || CurrentPlayer.Hephai == (int)HePhai.Dgc || CurrentPlayer.Hephai == (int)HePhai.Tyvt || CurrentPlayer.Hephai == (int)HePhai.Tyln || CurrentPlayer.Hephai == (int)HePhai.Nmd || CurrentPlayer.Hephai == (int)HePhai.Mgb || CurrentPlayer.Hephai == (int)HePhai.Vdk || CurrentPlayer.Hephai == (int)HePhai.Vdb || CurrentPlayer.Hephai == (int)HePhai.Tlt || CurrentPlayer.Hephai == (int)HePhai.Clts || CurrentPlayer.Hephai == (int)HePhai.Dm || CurrentPlayer.Hephai == (int)HePhai.Hd))
                            {
                                ShortMove(entity.PositionX, entity.PositionY);

                                if (Convert.ToInt32(GameGeneral.Distance(posX, posY, entity.PositionX, entity.PositionY)) < 800)
                                {
                                    BuffSkill(IDskin, entity.PositionX, entity.PositionY);
                                    break;
                                }
                            }
                            else if (CurrentPlayer.Hephai == (int)HePhai.Tld || CurrentPlayer.Hephai == (int)HePhai.Tlq || CurrentPlayer.Hephai == (int)HePhai.Cbb || CurrentPlayer.Hephai == (int)HePhai.Cs)
                            {
                                ShortMove(entity.PositionX, entity.PositionY);

                                if (Convert.ToInt32(GameGeneral.Distance(posX, posY, entity.PositionX, entity.PositionY)) < 100)
                                {
                                    BuffSkill(IDskin, entity.PositionX, entity.PositionY);
                                    break;
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
            }
        }

        // thủ tục đánh quái
        public void Trainquai(int IDskin, string name)
        {
            if (name != "" && name != null && IDskin.ToString() != "" && IDskin.ToString() != null)
            {
                RefreshEntityList();

                arritemNamequaiID = name.Split('/');

                if (EntityList.Count < 1)
                    return;

                var EntityListTrainquai = new List<PlayerEntity>(EntityList);

                var posX = CurrentPlayer.PositionX;
                var posY = CurrentPlayer.PositionY;


                foreach (var entity in EntityListTrainquai)
                {
                    if (entity.EntityType == NPCType.Beast && entity.EntityId.ToString() == arritemNamequaiID[0].ToLower().Trim())
                    {
                        int d = Convert.ToInt32(GameGeneral.Distance(posX, posY, entity.PositionX, entity.PositionY));

                        if (d < 1000 && (CurrentPlayer.Hephai == (int)HePhai.Nmk || CurrentPlayer.Hephai == (int)HePhai.Dgc || CurrentPlayer.Hephai == (int)HePhai.Tyvt || CurrentPlayer.Hephai == (int)HePhai.Tyln || CurrentPlayer.Hephai == (int)HePhai.Nmd || CurrentPlayer.Hephai == (int)HePhai.Mgb || CurrentPlayer.Hephai == (int)HePhai.Vdk || CurrentPlayer.Hephai == (int)HePhai.Vdb || CurrentPlayer.Hephai == (int)HePhai.Tlt || CurrentPlayer.Hephai == (int)HePhai.Clts || CurrentPlayer.Hephai == (int)HePhai.Dm || CurrentPlayer.Hephai == (int)HePhai.Hd))
                        {
                            BuffSkill(IDskin, entity.PositionX, entity.PositionY);
                            break;
                        }
                        else if (d > 1000 && (CurrentPlayer.Hephai == (int)HePhai.Nmk || CurrentPlayer.Hephai == (int)HePhai.Dgc || CurrentPlayer.Hephai == (int)HePhai.Tyvt || CurrentPlayer.Hephai == (int)HePhai.Tyln || CurrentPlayer.Hephai == (int)HePhai.Nmd || CurrentPlayer.Hephai == (int)HePhai.Mgb || CurrentPlayer.Hephai == (int)HePhai.Vdk || CurrentPlayer.Hephai == (int)HePhai.Vdb || CurrentPlayer.Hephai == (int)HePhai.Tlt || CurrentPlayer.Hephai == (int)HePhai.Clts || CurrentPlayer.Hephai == (int)HePhai.Dm || CurrentPlayer.Hephai == (int)HePhai.Hd))
                        {
                            ShortMove(entity.PositionX, entity.PositionY);

                            if (Convert.ToInt32(GameGeneral.Distance(posX, posY, entity.PositionX, entity.PositionY)) < 1000)
                            {
                                BuffSkill(IDskin, entity.PositionX, entity.PositionY);
                                break;
                            }
                        }
                        else if (CurrentPlayer.Hephai == (int)HePhai.Tld || CurrentPlayer.Hephai == (int)HePhai.Tlq || CurrentPlayer.Hephai == (int)HePhai.Cbb || CurrentPlayer.Hephai == (int)HePhai.Cs)
                        {
                            ShortMove(entity.PositionX, entity.PositionY);

                            if (Convert.ToInt32(GameGeneral.Distance(posX, posY, entity.PositionX, entity.PositionY)) < 100)
                            {
                                BuffSkill(IDskin, entity.PositionX, entity.PositionY);
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }

            }
        }

        // Lấy danh sách các menu
        public List<String> GetDialogLines()
        {
            var result = new List<String>();         
            var totalLines = GetTotalDialogLines();

            for (var i = 0; i < totalLines; i++)
            {
                var temp = UseBaseAddressMenu + i * 0x4;
                temp = ReadCurrentProcessMemory(temp);
                temp += 0x38;
                result.Add(fontConvert.TCVN3ToNoMark(WinAPI.ReadProcessMemoryString(OpenProcessHandle, temp, 100)));
            }
            
            return result;
        }

        // Hiển thị các menu
        public void ShowMenu()
        {
            foreach (String s in GetDialogLines())
            {
                MessageBox.Show(s);
            }
        }

        // thủ tục di chuyển ngắn dùng cho những map không sử dụng được chức năng tìm đường
        public void ShortMove(int posX, int poxY)
        {
            EntityInteract(2, posX, poxY, 0, 0, 0, 0);
        }
        // thủ tục ngắt skill
        public void LeftClick(int posX, int posY)
        {
            EntityInteract(2,posX,posY, 0, 0, 0, 0);
        }

        // thủ tục Buff Skill
        public void TalkToEntity(int entityID)
        {
            EntityInteract(4, entityID, 0, 0, 0, 0, 0);
        }

        // thủ tục Buff Skill
        public void TalkToEntity(UInt32 entityID)
        {
            EntityInteract(4, Convert.ToInt32(entityID), 0, 0, 0, 0, 0);
        }

        // thủ tục tìm và sử dụng Item theo tên
        public void FindAndUseItem(string itemName)
        {
            if (itemName != "" && itemName != null)
            {
                RefreshStorageItems();

                if (ItemList.Count < 1)
                    return;

                var ItemListFindItemName = new List<GameItem>(ItemList);

                foreach (var gameItem in ItemListFindItemName)
                {
                    if (gameItem.Location == 2 && gameItem.ItemNameNoMark.ToLower() == itemName.ToLower())
                    {
                        UseItem(gameItem.PositionColumn, gameItem.PositionRow);
                    }
                }
            }
        }

        // thủ tục tìm và sử dụng thay đồ
        public void Thayngoc2(string itemName)
        {
            if (itemName != "" && itemName != null)
            {
                arritemNameID = itemName.Split('/');

                RefreshStorageItems();

                if (ItemList.Count < 1)
                    return;

                var ItemListthaydo = new List<GameItem>(ItemList);

                foreach (var gameItem in ItemListthaydo)
                {
                    if (gameItem.Location == 2 && gameItem.ItemOPT.ToString() == arritemNameID[0].ToString())
                    {
                        PickNPutItem(2,gameItem.PositionRow, gameItem.PositionColumn,1,0,5);
                    }
                }
            }
        }

        // thủ tục tìm và sử dụng Item theo ID
        public void FindAndUseItemID(string itemName)
        {
            if (itemName != "" && itemName != null)
            {
                arritemNameID = itemName.Split('/');

                RefreshStorageItems();

                if (ItemList.Count < 1)
                    return;

                var ItemListFindItemNameID = new List<GameItem>(ItemList);

                foreach (var gameItem in ItemListFindItemNameID)
                {
                    if (gameItem.Location == 2 && gameItem.ItemOPT.ToString() == arritemNameID[0].ToString())
                    {
                        UseItem(gameItem.PositionColumn, gameItem.PositionRow);
                        return;
                    }
                }
            }
        }
        // thủ tục tìm kiếm còn hay không Item trong hành trang
        public bool FindItem(string itemName)
        {
            if (itemName != "" && itemName != null)
            {
                RefreshStorageItems();

                if (ItemList.Count < 1)
                {
                    return false;
                }

                var ItemListtimItem = new List<GameItem>(ItemList);

                foreach (var gameItem in ItemListtimItem)
                {
                    if (gameItem.Location == 2 && gameItem.ItemNameNoMark.ToLower() == itemName.ToLower())
                    {
                        return true;
                    }
                }
            }
            return false;

        }

        // thủ tục tìm kiếm còn hay không Item trên người
        public bool FindItemOnPlayer(string itemName)
        {
            if (itemName != "" && itemName != null)
            {
                arritemNameItemOnPlayer = itemName.Split('/');

                RefreshStorageItems();

                var ItemListtimItemOnPlayer = new List<GameItem>(ItemList);

                foreach (var gameItem in ItemListtimItemOnPlayer)
                {
                    if (gameItem.Location == 1 && gameItem.ItemOPT.ToString() == arritemNameItemOnPlayer[0].ToString())
                    {
                        return true;
                    }
                }
            }
            return false;

        }

        // thủ tục tìm kiếm còn hay không NPC
        public bool FindNPC(string itemName)
        {
            if (itemName != "" && itemName != null)
            {

                RefreshEntityList();

                var EntityListFindNPC = new List<PlayerEntity>(EntityList);

                itemName = fontConvert.convertUnicodeToNomark(itemName);

                foreach (var entity in EntityListFindNPC)
                {
                    if (entity.EntityType == NPCType.Item && entity.EntityNameNoMark.ToLower() == itemName.ToLower())
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        // thủ tục tìm kiếm còn hay không Boss
        public bool FindBoss(string itemName)
        {
            if (itemName != "" && itemName != null)
            {
                RefreshEntityList();

                var EntityListFindBoss = new List<PlayerEntity>(EntityList);


                itemName = fontConvert.convertUnicodeToNomark(itemName);

                foreach (var entity in EntityListFindBoss)
                {
                    if (entity.EntityType == NPCType.Beast && entity.EntityNameNoMark.ToLower() == itemName.ToLower())
                    {
                        return true;
                    }
                }

            }
            return false;
        }

        // thủ tục tìm kiếm còn hay không Boss train
        public bool FindBossTrain(string itemName)
        {
            if (itemName != "" && itemName != null)
            {
                RefreshEntityList();

                var EntityListFindBossTrain = new List<PlayerEntity>(EntityList);

                var arritemNamequaiID = itemName.Split('/');

                foreach (var entity in EntityListFindBossTrain)
                {
                    if (entity.EntityType == NPCType.Beast && entity.EntityId.ToString() == arritemNamequaiID[0].ToString())
                    {
                        return true;
                    }
                }

            }
            return false;
        }
        // thủ tục tìm kiếm còn hay không menu
        public bool FindMenu(string itemName)
        {
            itemName = fontConvert.convertUnicodeToNomark(itemName);
            var totalLines = GetTotalDialogLines();
            var Basemenu1 = WinAPI.ReadProcessMemoryIntPtr(OpenProcessHandle, UseBaseAddressMenu);
            Basemenu1 += 0x38;

            string name = (fontConvert.TCVN3ToNoMark(WinAPI.ReadProcessMemoryString(OpenProcessHandle,Basemenu1, 1000)));

            if (name.ToLower().Trim() == itemName.ToLower().Trim())
            {
                return true;
            }
            return false;
        }

        // thủ tục tìm kiếm còn hay không người chơi xung quanh
        public bool FindPlayer(string itemName)
        {
            if (itemName != "" && itemName != null)
            {
                RefreshEntityList();

                var EntityListFindPlayer = new List<PlayerEntity>(EntityList);

                itemName = fontConvert.convertUnicodeToNomark(itemName);

                foreach (var entity in EntityListFindPlayer)
                {
                    if (entity.EntityType == NPCType.Player && entity.EntityNameNoMark.ToLower() == itemName.ToLower())
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        // thủ tục lấy lever skill tay phải
        public string LVSKILLPHAI
        {
            get
            {
                var ptr = ReadCurrentProcessMemory(GameConst.BASEKILLPHAI);
                ptr = IntPtr.Add(ptr, 0x70);
                ptr = ReadCurrentProcessMemory(ptr);
                ptr = IntPtr.Add(ptr, 0x6C);
                ptr = ReadCurrentProcessMemory(ptr);
                ptr = IntPtr.Add(ptr, 0x6C);
                ptr = ReadCurrentProcessMemory(ptr);
                ptr = IntPtr.Add(ptr, 0xEC);
                int ptr1 = WinAPI.ReadProcessMemoryInt32(OpenProcessHandle, ptr);
                string x = (String.Format("{0:X}", ptr1));
                return x;


            }
        }

        // thủ tục lấy ID skill tay phải dạng Integer
        public int IDSKILLPHAI
        {
            get
            {
                var ptr = ReadCurrentProcessMemory(GameConst.BASEKILLPHAI);
                ptr = IntPtr.Add(ptr, 0x70);
                ptr = ReadCurrentProcessMemory(ptr);
                ptr = IntPtr.Add(ptr, 0x6C);
                ptr = ReadCurrentProcessMemory(ptr);
                ptr = IntPtr.Add(ptr, 0x6C);
                ptr = ReadCurrentProcessMemory(ptr);
                ptr = IntPtr.Add(ptr, 0xCC);
                int ptr1 = WinAPI.ReadProcessMemoryInt32(OpenProcessHandle, ptr);
               
                return ptr1;

            }
        }

        // thủ tục lấy ID skill tay phải dạng string
        public string IDSKILLPHAI2
        {
            get
            {
                var ptr = ReadCurrentProcessMemory(GameConst.BASEKILLPHAI);
                ptr = IntPtr.Add(ptr, 0x70);
                ptr = ReadCurrentProcessMemory(ptr);
                ptr = IntPtr.Add(ptr, 0x6C);
                ptr = ReadCurrentProcessMemory(ptr);
                ptr = IntPtr.Add(ptr, 0x6C);
                ptr = ReadCurrentProcessMemory(ptr);
                ptr = IntPtr.Add(ptr, 0xCC);
                int ptr1 = WinAPI.ReadProcessMemoryInt32(OpenProcessHandle, ptr);
                //int myHex = int.Parse(ptr1.ToString("X"));  // Gives you hexadecimal
                string x = (String.Format("{0:X}", ptr1));
                // int x = Convert.ToInt32(ptr1);
                return x;

            }
        }

        // thủ tục lấy lever skill tay trái dạng String
        public string LVSKILLTRAI
        {
            get
            {
                var ptr = ReadCurrentProcessMemory(GameConst.BASEKILLPHAI);
                ptr = IntPtr.Add(ptr, 0x70);
                ptr = ReadCurrentProcessMemory(ptr);
                ptr = IntPtr.Add(ptr, 0xEC);
                int ptr1 = WinAPI.ReadProcessMemoryInt32(OpenProcessHandle, ptr);
                string x = (String.Format("{0:X}", ptr1));
                return x;


            }
        }


        // thủ tục lấy ID skill tay trái dạng String
        public string IDSKILLTRAI
        {
            get
            {
                var ptr = ReadCurrentProcessMemory(GameConst.BASEKILLPHAI);
                ptr = IntPtr.Add(ptr, 0x70);
                ptr = ReadCurrentProcessMemory(ptr);
                ptr = IntPtr.Add(ptr, 0xCC);
                int ptr1 = WinAPI.ReadProcessMemoryInt32(OpenProcessHandle, ptr);
                string x = (String.Format("{0:X}", ptr1));
                return x;

            }
        }

        // Send phím đến cho các Handle
        public void sendphim(int A)
        {
           WinAPI.PostMessage(WindowHwnd,(uint)WinAPI.Keyvisual.WM_KEYDOWN,A,A);
        }

        // Send string đến cho các Handle
        public void sendstring(string A)
        {
            byte[] array = Encoding.Default.GetBytes(fontConvert.ConvertUnicodeToTcvn3(A));

            for (int i = 0;i<array.Length; i++)
            {
                WinAPI.PostMessage(WindowHwnd, (uint)WinAPI.Keyvisual.WM_CHAR,(int)(array[i]), 1);
            }
        }

        private static int MAKEPARAM(int l, int h)
        {
            return ((l & 0xffff) | (h << 0x10));
        }

        // Send Click API
        public void SendClick(int x, int y)
        {
            int Lparam = MAKEPARAM(x,y);

            WinAPI.PostMessage(WindowHwnd, (uint)WinAPI.WMessenges.WM_LBUTTONDOWN, (int)WinAPI.WMessenges.MK_LBUTTON,Lparam);
            WinAPI.PostMessage(WindowHwnd, (uint)WinAPI.WMessenges.WM_LBUTTONUP, (int)0,Lparam);

            WinAPI.PostMessage(WindowHwnd, (uint)WinAPI.WMessenges.WM_LBUTTONDBCLICK, (int)WinAPI.WMessenges.MK_LBUTTON,Lparam);
            WinAPI.PostMessage(WindowHwnd, (uint)WinAPI.WMessenges.WM_LBUTTONUP, (int)0,Lparam);
        }
    }

}
