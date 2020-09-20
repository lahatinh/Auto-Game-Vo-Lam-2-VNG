using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoClient.Entities
{
    public class PlayerEntity
    {
        public AutoClient Client;
        public IntPtr Address;
        public PlayerEntity(IntPtr Address, AutoClient client)
        {
            this.Address = Address;
            this.Client = client;
        }
        public PlayerEntity(AutoClient client)
        {
            this.Address = client.EntityBaseAddress; //Address
            this.Client = client;
        }
      
        public PlayerStatus PlayerStatus
        {
            get
            {
                var result = WinAPI.ReadProcessMemoryInt32(Client.OpenProcessHandle, IntPtr.Add(Address, 0xFC));
                switch (result)
                {
                    case 0:
                        return PlayerStatus.DoNothing;
                    case 1:
                        return PlayerStatus.Running;
                    case 7:
                        return PlayerStatus.Collecting;
                    case 25:
                        return PlayerStatus.PrepareHorse;
                    case 21:
                        return PlayerStatus.UsingTDP;
                    case 5:
                        return PlayerStatus.Death;
                    default:
                        return PlayerStatus.Other;
                }
            }
        }


        public String TrangThai
        {
            get
            {
                if (WinAPI.ReadProcessMemoryString(Client.OpenProcessHandle, IntPtr.Add(Address, 0x84), 32) == "")
                    return "Offline";
                return "Online";
            }
        }
        public uint EntityId
        {
            get
            {
                return WinAPI.ReadProcessMemoryUInt32(Client.OpenProcessHandle, IntPtr.Add(Address, 0x4));

            }
        }

        public int PositionX
        {
            get
            {
                return WinAPI.ReadProcessMemoryInt32(Client.OpenProcessHandle, IntPtr.Add(Address, 0x1FD0));
            }
        }

        public int SoThitNuong
        {
            get
            {
                return WinAPI.ReadProcessMemoryInt32(Client.OpenProcessHandle, IntPtr.Add(Address, 0x1D24AB0));
            }
        }


        public int Idplayer
        {
            get
            {
                return WinAPI.ReadProcessMemoryInt32(Client.OpenProcessHandle, IntPtr.Add(Address, 0x4));

            }
        }

        public bool CheckHc
        {
            get
            {
                return WinAPI.ReadProcessMemoryInt32(this.Client.OpenProcessHandle, IntPtr.Add(this.Address, 0xFB0)) == 0;
            }
        }


        public bool CheckChat
        {
            get
            {
                return WinAPI.ReadProcessMemoryInt32(this.Client.OpenProcessHandle, IntPtr.Add(this.Address, 0x2D54)) == 0;
            }
        }

        public int PositionY
        {
            get
            {
                return WinAPI.ReadProcessMemoryInt32(Client.OpenProcessHandle, IntPtr.Add(Address, 0x1FD0+0x4));
            }
        }
        public int HitPoint
        {
            get
            {
                return WinAPI.ReadProcessMemoryInt32(Client.OpenProcessHandle, IntPtr.Add(Address, 0x2FC));
            }
        }
        public int MaxHitPoint
        {
            get
            {
                return WinAPI.ReadProcessMemoryInt32(Client.OpenProcessHandle, IntPtr.Add(Address, 0x2F8));
            }
        }

        public int Mana
        {
            get
            {
                return WinAPI.ReadProcessMemoryInt32(Client.OpenProcessHandle, IntPtr.Add(Address, 0x330));
            }
        }

        public int MaxMana
        {
            get
            {
                return WinAPI.ReadProcessMemoryInt32(Client.OpenProcessHandle, IntPtr.Add(Address, 0x32C));
            }
        }

        public int CurrentEnergyConsume //thể lực max
        {
            get
            {
                return WinAPI.ReadProcessMemoryInt32(Client.OpenProcessHandle, IntPtr.Add(Address, 0x334));
            }
        }

        public int TheLuc //Thể lực hiện tại
        {
            get
            {
                return WinAPI.ReadProcessMemoryInt32(Client.OpenProcessHandle, IntPtr.Add(Address, 0x338));
            }
        }

        public bool IsOnHorse //đang sử dụng ngựa
        {
            get
            {
                if(WinAPI.ReadProcessMemoryInt32(Client.OpenProcessHandle, IntPtr.Add(Address, 0x3178)) == 1)
                {
                    return true;
                }
                return false;
            }
        }

        public NPCType EntityType
        {
            get
            {
                var type = WinAPI.ReadProcessMemoryInt32(Client.OpenProcessHandle, IntPtr.Add(Address, 0x108));
                switch (type)
                {
                    case 0:
                        return NPCType.Beast;
                    case 5:
                        return NPCType.Player;
                    case 6:
                        return NPCType.Item;
                    case 7:
                        return NPCType.Pet;
                    case 13:
                        return NPCType.Ghost;
                    default:
                        return NPCType.Unknown;
                }
            }
        }

        public int typep
        {
            get
            {
              return  WinAPI.ReadProcessMemoryInt32(Client.OpenProcessHandle, IntPtr.Add(Address, 0x108));
            }
        }

        public String TrangThaiOnline
        {
            get
            {
                if (WinAPI.ReadProcessMemoryString(Client.OpenProcessHandle, IntPtr.Add(Address, 0x84), 32) == "")
                    return "Offline";
                return "Online";
            }
        }

        public String EntityName
        {
            get
            {
                if (WinAPI.ReadProcessMemoryString(Client.OpenProcessHandle, IntPtr.Add(Address, 0x84), 32) == "")
                    return "Chưa đăng nhập";
                return WinAPI.ReadProcessMemoryString(Client.OpenProcessHandle, IntPtr.Add(Address, 0x84), 32);
            }
        }
        public int EntityMapID
        {
            get
            {
                return WinAPI.ReadProcessMemoryInt32(Client.OpenProcessHandle, IntPtr.Add(Address, 0x4C));
            }
        }
        public String EntityNameUnicode
        {
            get { return fontConvert.TCVN3ToUnicode(EntityName); }
        }

        public String EntityNameTCVN3
        {
            get { return EntityName; }
        }

        public String EntityNameNoMark
        {
            get { return fontConvert.TCVN3ToNoMark(EntityName); }
        }

        public int Hephai
        {
            get
            {
                return (int)WinAPI.ReadProcessMemoryByteArray(Client.OpenProcessHandle, (IntPtr)((long)(Address + (int)170U)), 1)[0];
            }
        }

        public int TukhiTLQ
        {
            get
            {
                return WinAPI.ReadProcessMemoryInt32(Client.OpenProcessHandle, IntPtr.Add(Address, 0x5E8));
            }
        }

        public int TukhiTLQMax
        {
            get
            {
                return WinAPI.ReadProcessMemoryInt32(Client.OpenProcessHandle, IntPtr.Add(Address, 0x5E8 - 0x4));
            }
        }

    }
}
