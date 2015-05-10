using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ControllerMimic
{
    public class RecMessage
    {
        public enum State
        {
            Idle,
            InDataReq,
            InCmd
        };

        private State eCurState;
        private byte[] Bytes = new byte[10];
        private byte[] RetBytes = new byte[3];
        private int Idx = 0;
        private int nSpeed = 0;
        private int nPower = 0x08;
        private int NumRemain = 0;

        public RecMessage()
        {
            eCurState = State.Idle;
        }

        public int Speed
        {
            get
            {
                return nSpeed;
            }
            set
            {
                nSpeed = value;
                //if (nSpeed < 32)
                 //   nSpeed = 32;
            }
        }

        public int Power
        {
            get
            {
                return nPower;
            }
            set
            {
                nPower = value;
            }
        }


        public void AddFirstByte(byte data)
        {
            //Valid first char?
            Bytes[0] = data;
            switch(data)
            {
                case 0x16:
                    Idx = 1;
                    eCurState = State.InCmd;
                    break;
                case 0x11:
                    Idx = 1;
                    eCurState = State.InDataReq;
                    break;
                default:
                    Idx = 0;
                    eCurState = State.Idle;
                    break;
            }
        }

        public void AddByte(byte data)
        {
            if (eCurState == State.Idle)
                AddFirstByte(data);
            else
            {
                switch(eCurState)
                {
                    case State.InDataReq:
                        DoDataReq(data);
                        eCurState = State.Idle;
                        Idx = 0;
                        break;
                    case State.InCmd:
                        DoCommand(data);
                        break;
                    default:
                        eCurState = State.Idle;
                        Idx = 0;
                        break;
                }
            }             
        }

        private void DoCommand(byte data)
        {
            Bytes[Idx] = data;
            if(Idx == 1)
            {
                switch(data)
                {
                    case 0x0b://PAS level
                        NumRemain = 2;
                        break;
                    case 0x1a: //?
                        NumRemain = 1;
                        break;
                    case 0x1f://?
                        NumRemain = 3;
                        break;
                }
                Idx++;
            }
            else
            {
                if(--NumRemain == 0)
                {
                    Idx = 0;
                    if(Bytes[1] == 0x0b)
                    {
                        //Display the level
                       MainWindow.theComms.Wnd.Dispatcher.Invoke(MainWindow.ShowPAS, Bytes[2]);
                       
                    }
                    eCurState = State.Idle;
                }
                else
                {
                    Idx++;
                }
            }
        }
        private void CalcSpeedMsg()
        {
            int nT = nSpeed;
            RetBytes[0] = (byte)((nSpeed >> 8) & 0xff);
            RetBytes[1] = (byte)(nSpeed & 0xff);
            RetBytes[2] = (byte)((RetBytes[0] + RetBytes[1] + 0x20) & 0xff);

        }

        private void DoDataReq(byte data)
        {
            switch(data)
            {
                case 0x20://Wheel speed
                    CalcSpeedMsg();
                    //RetBytes[0] = (byte)((nSpeed >> 16) & 0xff);
                    //RetBytes[1] = (byte)((nSpeed>>8) & 0xff);
                    //RetBytes[2] = (byte)(nSpeed & 0xff);
                    MainWindow.theComms.SendData(RetBytes, 3);
                    Debug.WriteLine("Speed {0:X} {1:X} {2:X}",RetBytes[0],RetBytes[1],RetBytes[2]);
                    break;
                case  0x08: //?
                    RetBytes[0] = 1;
                    MainWindow.theComms.SendData(RetBytes, 1);
                    break;
                case 0x0a://Power
                    RetBytes[0] = (byte)((nPower>>8) & 0xff);;
                    RetBytes[1] = (byte)((nPower>>8) & 0xff);;
                    //RetBytes[1] = (byte)(nPower & 0xff);;
                    MainWindow.theComms.SendData(RetBytes, 2);
                    break;
                case 0x31://?
                    RetBytes[0] = 0x31;
                    RetBytes[1] = 0x31;
                    MainWindow.theComms.SendData(RetBytes, 2);
                    break;
                case 0x11://?
                    RetBytes[0] = 0x50;
                    RetBytes[1] = 0x50;
                    MainWindow.theComms.SendData(RetBytes, 2);
                    break;
                default:
                    break;
            }
        }

        public bool InMessage
        {
            get
            {
                return eCurState != State.Idle;
            }
         }

    }
}
