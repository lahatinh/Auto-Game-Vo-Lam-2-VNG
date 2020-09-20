using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoClient.Entities
{
    public class GameItem
    {
        public IntPtr Address;
        public IntPtr PositionAddress;
        public IntPtr AddressOPT;
        public AutoClient Client;
        public GameItem(IntPtr address, AutoClient client)
        {
            this.Address = address;
            this.Client = client;
        }

        public GameItem(AutoClient client)
        {
            this.Client = client;
        }

        public string ItemName
        {
            get
            {
                return WinAPI.ReadProcessMemoryString(Client.OpenProcessHandle, IntPtr.Add(Address, 0x0BE0), 32);
            }
        }

        public uint ItemId
        {
            get
            {
                return WinAPI.ReadProcessMemoryUInt32(Client.OpenProcessHandle, IntPtr.Add(Address, 0x07));
            }
        }

        public uint ItemOPT
        {
            get
            {
                return WinAPI.ReadProcessMemoryUInt32(Client.OpenProcessHandle, IntPtr.Add(Address, 0x0C10));
            }
        }

        public String ItemNameUnicode
        {
            get { return fontConvert.TCVN3ToUnicode(ItemName); }
        }

        public String ItemNameNoMark
        {
            get { return fontConvert.TCVN3ToNoMark(ItemName); }
        }


        public int Quantity
        {
            get
            {
                return WinAPI.ReadProcessMemoryInt32(Client.OpenProcessHandle, IntPtr.Add(Address, 0x0030));
            }
        }

        public int Location
        {
            get
            {
                return WinAPI.ReadProcessMemoryInt32(Client.OpenProcessHandle, IntPtr.Add(PositionAddress, 0x4));
            }
        }

        public int PositionRow
        {
            get
            {
                return WinAPI.ReadProcessMemoryInt32(Client.OpenProcessHandle, IntPtr.Add(PositionAddress, 0x8));
            }
        }

        public int PositionColumn
        {
            get
            {
                return WinAPI.ReadProcessMemoryInt32(Client.OpenProcessHandle, IntPtr.Add(PositionAddress, 0x8 + 0x4));
            }
        }
    }
}
