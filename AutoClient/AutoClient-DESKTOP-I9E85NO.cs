using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoClient.Entities;
using System.Windows.Forms;
using System.Threading;
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

        public int StepbufNMK;
        private PlayerEntity _currentPlayer;
  
        public string MsgScrip = "";


      
        public AutoClient()
        {
            this._currentPlayer = new PlayerEntity(this);
        }

        public int CurrentMapId
        {
            get
            {
                return WinAPI.ReadProcessMemoryInt32(OpenProcessHandle, GameConst.CurrentMappAddress);
            }
        }

        public bool IsShopOpened
        {
            get
            {
                return WinAPI.ReadProcessMemoryWord(OpenProcessHandle, GameConst.IsShopOpenedStaticAddress) == 1;
            }
        }

        public bool IsCurrentUnavailable()
        {
            if (this.CurrentPlayer == null)
                return true;
            if (this.CurrentPlayer.HitPoint == 100)
                return true;
            return false;
        }

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

        public IntPtr UseAddressMessengeHT
        {
            get
            {
                var step1 = WinAPI.ReadProcessMemoryIntPtr(OpenProcessHandle, GameConst.BaseMessgeHeThong);
                step1 = IntPtr.Add(step1, 0x118);
                var step2 = WinAPI.ReadProcessMemoryIntPtr(OpenProcessHandle, step1);
                step2 = IntPtr.Add(step2, 0x0);
                var step3 = WinAPI.ReadProcessMemoryIntPtr(OpenProcessHandle, step2);
                step3 = IntPtr.Add(step3, 0x0);
                return step3;
            }

        }

        public string MessengeHT
        {
            get
            {
                return fontConvert.ConvertTcvn3ToUnicode(WinAPI.ReadProcessMemoryString(OpenProcessHandle, UseAddressMessengeHT, 100));
            }
        }

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

        public int GetCurrentSelectedComposeItem
        {
            get
            {
                return WinAPI.ReadProcessMemoryInt32(OpenProcessHandle, ReadCurrentProcessMemory(GameConst.ComposeItemBaseAddress) + 0x3B10);
            }
        }


    

        public void RefreshEntityList()
        {
            EntityList.Clear();
            var baseAdd = EntityBaseAddress;
            for (int i = 0; i < 256; i++)
            {
                var entity = new PlayerEntity(WinAPI.ReadProcessMemoryIntPtr(OpenProcessHandle, IntPtr.Add(baseAdd, i * 4)), this);
                if (entity.HitPoint > 0)
                {
                    EntityList.Add(entity);
                }
            }
        }

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


        protected IntPtr ReadCurrentProcessMemory(IntPtr address)
        {
            return WinAPI.ReadProcessMemoryIntPtr(OpenProcessHandle, address);
        }
        protected int ReadCurrentProcessMemoryReturnInt(IntPtr address)
        {
            return WinAPI.ReadProcessMemoryInt32(OpenProcessHandle, address);
        }
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



        /// <summary>
        /// 
        /// </summary>
        public PlayerEntity CurrentPlayer
        {
            get
            {
                if (_currentPlayer.Address != UserBaseAddress)
                    _currentPlayer.Address = UserBaseAddress;
                return _currentPlayer;
            }
        }

        public void Attach(IntPtr hwnd)
        {
            this.WindowHwnd = hwnd;
            WinAPI.GetWindowThreadProcessId(WindowHwnd, out ProcessID);
            OpenProcessHandle = WinAPI.OpenProcess(WinAPI.ProcessAccessFlags.All, true, Convert.ToInt32(ProcessID));
        }

        private bool _isInjected = false;

        public bool isInjected
        {
            get { return _isInjected; }
        }

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

        public int DeInject()
        {
            var result = HookGame.UnmapDll(WindowHwnd);
            _isInjected = false;
            return result;
        }

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


        public void MoveTo(int posX, int posY, int mapID)
        {
            
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_start, GameConst.FUNC_MOVE_TO);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push, posX);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push, posY);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push, mapID);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_end, 0);
        }

        public void EntityInteract(int type, int param1, int param2, int param3, int param4, int param5, int param6)
        {
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_start, GameConst.FUNC_ENTITY_INTERACT); //SendNPC
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push,  type);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push,  param1);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push,  param2);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push,  param3);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push,  param4);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push,  param5);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push,  param6);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push,CurrentPlayer.Address.ToInt32());
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_end, 0);
           
        }
        public void DialogInteract(int actionType, int param1, int param2)
        {

            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_start, GameConst.FUNC_DIALOG_INTERACT); //Menu
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push,  actionType);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push,  param1);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push,  param2);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_end, 0);
        }

        public void BuyItem(int itemPos, int param2, int quantity, int param4, int param5) //Mua Item
        {
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_start, GameConst.FUNC_BUYITEM);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push,  itemPos);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push,  param2);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push,  quantity);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push,  param4);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push,  param5);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_end, 0);

        }

        public void UseHorse(bool use) //Su dung ngua
        {
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_start, GameConst.FUNC_USE_HORSE);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push, CurrentPlayer.Address.ToInt32());
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push,  use ? 5 : 6);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_end, 0);
        }

        public void ComposeItem(int itemId) //Che item
        {
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_start, GameConst.FUNC_COMPOSE_ITEM);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push,  itemId);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_end, 0);

        }
        public void SelectMenu(int menuxId) //Che item
        {
            int result = ReadCurrentProcessMemoryReturnInt(GameConst.DialogBaseAddress);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_start, GameConst.FUNC_SELECTMENU);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push,  result);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push,  menuxId);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_end, 0);

        }
        public void MoneyInteract(int money, int param1, int param2) //Tien
        {
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_start, GameConst.FUNC_MONEY_INTERACT);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push,  money);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push,  param1);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push,  param2);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_end, 0);
        }
        //colIndex from 0 -> 4
        //rowIndex from 0 -> 11
        public void UseItem(int colIndex, int rowIndex) //Su dung item
        {
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_start, GameConst.FUNC_USE_ITEM);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push,  colIndex);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push,  rowIndex);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_end, 0);
        }
        public void VeThanh(int param1) //
        {
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_start, GameConst.FUNC_REBIRTH);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push, param1);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push, GameConst.BaseVeThanh.ToInt32());
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_end, 0);

        }
        public void XuongNgua(int use) //Su dung ngua
        {
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_start, GameConst.FUNC_USE_HORSE);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push, CurrentPlayer.Address.ToInt32());
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push, use);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_end, 0);
        }
        public void Chat() //Auto Rao
        {
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_start, GameConst.FUNC_CHAT);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push, CurrentPlayer.Address.ToInt32());
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_end, 0);

        }

        public void LatBai(int bai) //Moi Ty Vo
        {
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_start, GameConst.FUNC_LATBAI);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push, bai);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_end, 0);

        }

        public void Group(int param1, int param2, int param3, int userblock) //To Doi
        {
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_start, GameConst.FUNC_GROUP);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push, param1);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push, param2);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push, param3);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push, userblock);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_end, 0);

        }

        public void Doscrip(string msg)
        {
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_start, GameConst.FUNC_CHAT);
            if (!msg.Equals(this.MsgScrip))
            {
                byte[] arg_29_0 = Encoding.Default.GetBytes(fontConvert.ConvertUnicodeToTcvn3(msg));
                byte[] array = arg_29_0;
                for (int i = 0; i < array.Length; i++)
                {
                    byte lParam = array[i];
                    WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_sendchar, lParam);
                }
                WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_sendchar, 0);
                this.MsgScrip = msg;
            }
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_end, GameConst.FUNC_CHAT);



        }

     
        public void NhanHoiSinh()
        {
            if (_currentPlayer.PlayerStatus == PlayerStatus.Death)
            {
                VeThanh(17);
            }
        }

        public void NhanVeThanh()
        {
            if (_currentPlayer.PlayerStatus == PlayerStatus.Death)
            {
                VeThanh(3);
            }
        }


        public void DrawCash(int money)
        {
            MoneyInteract(money, 2, 3);
        }

        public void SaveCash(int money)
        {
            MoneyInteract(money, 3, 2);
        }

        public void SelfBuffSkill(int skillID)
        {
            var posX = Convert.ToInt32(CurrentPlayer.PositionX);
            var posY = Convert.ToInt32(CurrentPlayer.PositionY);

            EntityInteract(3, posX, posY, skillID, 0, 0, 0);
        }

        public void BuffSkill(int skillID, int posX, int posY)
        {
            EntityInteract(3, posX, posY, skillID, 0, 0, 0);
        }

        public void AttackVictim(int skillID, uint VictimID)
        {
            EntityInteract(3, -1, Convert.ToInt32(VictimID), skillID, 0, 0, 0);
        }

        public void Chosauan(int colIndex, int rowIndex)
        {
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_start, GameConst.FUNC_CHOSAUAN);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push,  colIndex);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push,  rowIndex);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_end, 0);

        }

        public int GetTotalDialogLines()
        {
            return WinAPI.ReadProcessMemoryInt32(OpenProcessHandle, GameConst.DialogBaseTotalLine);
        }

        public bool IsDialogOpened
        {
            get
            {
                var pointer = ReadCurrentProcessMemory(GameConst.DialogBaseAddress);
                return (pointer.ToInt32() != 0);
            }
        }

        public void Chonmenu(string name1)
        {

            var pointer = ReadCurrentProcessMemory(GameConst.DialogBaseAddress);
            if (pointer.ToInt32() != 0)
            {
                pointer += 0x70;//70
                pointer = ReadCurrentProcessMemory(pointer) +0x6C;//6C
                pointer = ReadCurrentProcessMemory(pointer) + 0x6C;//6C
                pointer = ReadCurrentProcessMemory(pointer) + 0x6C;//6C
                pointer = ReadCurrentProcessMemory(pointer) + 0x70;//70
                pointer = ReadCurrentProcessMemory(pointer) + 0xC8;//C8
                pointer = ReadCurrentProcessMemory(pointer);

                var totalLines = GetTotalDialogLines();

                for (var i = 0; i < totalLines; i++)
                {
                    var temp = pointer + i * 0x4;
                    temp = ReadCurrentProcessMemory(temp);
                    temp += 0x38;
                    string name = (fontConvert.TCVN3ToNoMark(WinAPI.ReadProcessMemoryString(OpenProcessHandle, temp, 1000)));
                    if (name == name1)
                    {
                        SelectMenu(i);
                    }
                }
            }

        }

        public int CheckThitNuong
        {
            get
            {
                var ptr = ReadCurrentProcessMemory(GameConst.CheckThit);
                ptr = IntPtr.Add(ptr, 0x1AC);

                return WinAPI.ReadProcessMemoryInt32(OpenProcessHandle, ptr);


            }

        }


        public void ClickNPC(string name)
        {
            arritemNameID = name.Split('-');

            RefreshEntityList();

            PlayerEntity result = null;
            int minDistance = int.MaxValue;
            var posX = CurrentPlayer.PositionX;
            var posY = CurrentPlayer.PositionY;

            foreach (var entity in EntityList)
            {
                if (entity.EntityType != NPCType.Item)
                    continue;
                if (entity.EntityNameNoMark.Contains(arritemNameID[1]))
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

        public void ClickTNC()
        {
            RefreshEntityList();

            var posX = CurrentPlayer.PositionX;
            var posY = CurrentPlayer.PositionY;

            foreach (var entity in EntityList)
            {
                if (entity.EntityType.Equals("6"))
                    continue;

                var d = Convert.ToInt32(GameGeneral.Distance(posX, posY, entity.PositionX, entity.PositionY));

                if (d>0 && d < 200)
                {
                    TalkToEntity(entity.EntityId);
                    break;
                }
                
            }
        }

        public List<String> GetDialogLines()
        {
            var result = new List<String>();
            var pointer = ReadCurrentProcessMemory(GameConst.DialogBaseAddress);
            if (pointer.ToInt32() != 0)
            {
                pointer += 0x70;
                pointer = ReadCurrentProcessMemory(pointer) + 0x6c;
                pointer = ReadCurrentProcessMemory(pointer) + 0x6c;
                pointer = ReadCurrentProcessMemory(pointer) + 0x6c;
                pointer = ReadCurrentProcessMemory(pointer) + 0x70;
                pointer = ReadCurrentProcessMemory(pointer) + 0xc8;
                pointer = ReadCurrentProcessMemory(pointer);

                var totalLines = GetTotalDialogLines();

                for (var i = 0; i < totalLines; i++)
                {
                    var temp = pointer + i * 0x4;
                    temp = ReadCurrentProcessMemory(temp);
                    temp += 0x38;
                    result.Add(fontConvert.TCVN3ToNoMark(WinAPI.ReadProcessMemoryString(OpenProcessHandle, temp, 100)));
                }
            }
            return result;
        }
        public void ShowMenu()
        {
            foreach (String s in GetDialogLines())
            {
                MessageBox.Show(s);
            }
        }

        public IntPtr GetDialogSelectLineParam()
        {
            var result = ReadCurrentProcessMemory(GameConst.DialogBaseAddress);
            if (result.ToInt32() == 0)
                return result;

            result = IntPtr.Add(result, 0x70);
            result = ReadCurrentProcessMemory(result);
            result = IntPtr.Add(result, 0x6c);
            result = ReadCurrentProcessMemory(result);
            result = IntPtr.Add(result, 0x6c);
            result = ReadCurrentProcessMemory(result);
            result = IntPtr.Add(result, 0x6c);
            result = ReadCurrentProcessMemory(result);
            result = IntPtr.Add(result, 0x70);
            result = ReadCurrentProcessMemory(result);
            return result;
        }

        public void DialogSelectLine(int noLine)
        {
            DialogInteract(0x692, GetDialogSelectLineParam().ToInt32(), noLine);
        }

        public void SelectMenuLine(int noLine)
        {            
            SelectMenu( noLine);
        }

        public void ShortMove(int posX, int poxY)
        {
            EntityInteract(2, posX, poxY, 0, 0, 0, 0);
        }
        public void LeftClick(int posX, int posY)
        {
            EntityInteract(2,posX,posY, 0, 0, 0, 0);
        }

        public void TalkToEntity(int entityID)
        {
            EntityInteract(4, entityID, 0, 0, 0, 0, 0);
        }

        public void TalkToEntity(UInt32 entityID)
        {
            EntityInteract(4, Convert.ToInt32(entityID), 0, 0, 0, 0, 0);
        }

        public void FindAndUseItem(string itemName)
        {
            itemName = fontConvert.convertUnicodeToNomark(itemName);
            RefreshStorageItems();
            foreach (var gameItem in ItemList)
            {
                if (gameItem.Location == 2 && gameItem.ItemNameNoMark.ToLower().Trim() == itemName.ToLower().Trim())
                {
                    UseItem(gameItem.PositionColumn, gameItem.PositionRow);
                    return;
                }
            }
        }

        public void FindAndUseItemID(string itemNameID)
        {
            arritemNameID = itemNameID.Split('-');
            RefreshStorageItems();
            foreach (var gameItem in ItemList)
            {
                if (gameItem.Location == 2 && gameItem.ItemId.ToString().Trim() == arritemNameID[0].ToLower().Trim())
                {
                    UseItem(gameItem.PositionColumn, gameItem.PositionRow);
                    return;
                }
            }
        }

        public bool FindItem(string itemName)
        {
            itemName = fontConvert.convertUnicodeToNomark(itemName);
            RefreshStorageItems();
            foreach (var gameItem in ItemList)
            {
                if (gameItem.Location == 2 && gameItem.ItemNameNoMark.ToLower().Trim() == itemName.ToLower().Trim())
                {
                    return true;
                }
            }
            return false;
        }

        public bool FindNPC(string itemName)
        {
            itemName = fontConvert.convertUnicodeToNomark(itemName);
            RefreshEntityList();
            foreach (var entity in EntityList)
            {
                if (entity.EntityNameNoMark == itemName && entity.EntityType != NPCType.Beast)
                {
                    return true;
                }
            }
            return false;
        }

        public bool FindPlayer(string itemName)
        {
            itemName = fontConvert.convertUnicodeToNomark(itemName);
            RefreshEntityList();
            foreach (var entity in EntityList)
            {
                if (entity.EntityNameNoMark == itemName && entity.EntityType != NPCType.Player)
                {
                    return true;
                }
            }
            return false;
        }



        public void HoiSinh(string name)
        {

            RefreshEntityList();

            PlayerEntity result = null;
            int minDistance = int.MaxValue;
            var posX = CurrentPlayer.PositionX;
            var posY = CurrentPlayer.PositionY;

            foreach (var entity in EntityList)
            {
                if (entity.EntityType != NPCType.Player)
                    continue;
                if (entity.EntityNameUnicode.Contains(name))
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
            if (result != null && result.PlayerStatus == PlayerStatus.Death)//YeuCo 128  
            {
                AttackVictim(458840, result.EntityId);
            }

        }


        public void DeSau(string itemName)
        {
            itemName = fontConvert.convertUnicodeToNomark(itemName);
            RefreshStorageItems();
            foreach (var gameItem in ItemList)
            {
                if (gameItem.Location == 2 && gameItem.ItemNameNoMark.ToLower().Trim() == itemName.ToLower().Trim())
                {
                    Chosauan(gameItem.PositionRow,gameItem.PositionColumn);
                    return;
                }
            }
        }
        public void ShowAllItem()
        {
            RefreshStorageItems();
            foreach (var gameItem in ItemList)
            {
                if (gameItem.Location == 2 )
                {
                    MessageBox.Show(gameItem.ItemNameNoMark.ToLower() + " " + gameItem.PositionColumn.ToString() + " " + gameItem.PositionRow.ToString());
                }
            }
        }


  
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

      
    
        public void PickNPutItem(int locationPick, int colPick, int rowPick, int locationPut, int colPut, int rowPut)
        {
            var itemLocationAddress = WinAPI.ReadProcessMemoryIntPtr(OpenProcessHandle, GameConst.BaseAddress);
            itemLocationAddress = IntPtr.Add(itemLocationAddress, 0x0026E558);
            itemLocationAddress = WinAPI.ReadProcessMemoryIntPtr(OpenProcessHandle, itemLocationAddress);
            itemLocationAddress = WinAPI.ReadProcessMemoryIntPtr(OpenProcessHandle, itemLocationAddress);
            itemLocationAddress = IntPtr.Add(itemLocationAddress, 0x4);
            itemLocationAddress = WinAPI.ReadProcessMemoryIntPtr(OpenProcessHandle, itemLocationAddress);

            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_start, GameConst.FUNC_PICKPUT);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push,  locationPick);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push,  colPick);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push,  rowPick);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push,  locationPut);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push,  colPut);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push,  rowPut);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push, itemLocationAddress.ToInt32());
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_end, 0);
        }

        public void sendphim(int A)
        {
           WinAPI.PostMessage(WindowHwnd,(uint)WinAPI.Keyvisual.WM_KEYDOWN,A,1);
        }
    }

}
