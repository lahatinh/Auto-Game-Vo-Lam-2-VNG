using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoPK
{
    class UsingHookKey
    {
        KeyboardListener KListener;

        public UsingHookKey()
        {
            KListener = new KeyboardListener();

            KListener.KeyDown += KListener_KeyDown;
        }

        void KListener_KeyDown(object sender, RawKeyEventArgs args)
        {
            switch (args.VKCode)
            {
                case VKCodeConst.N: //Key N
                    break;
                case VKCodeConst.P: //Key P
                    break;
                case VKCodeConst.V: //Key V
                    break;
            }
        }
    }
}
