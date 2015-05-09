using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace ControllerMimic
{
    public class Comms
    {
        //Class should be a singleton
        public static SerialPort m_Port;
        private static RecMessage Msg;
        private MainWindow parent;

        public Comms(string sPort, MainWindow  p)
        {
            if(m_Port == null)
                m_Port = new SerialPort(sPort, 1200);
            m_Port.Parity = Parity.None;
            m_Port.DataReceived += new SerialDataReceivedEventHandler(DataReceived);
            Msg = new RecMessage();
            parent = p;
        }

        ~Comms()
        {
            if (m_Port.IsOpen)
                m_Port.Close();
        }
        
        public MainWindow Wnd
        {
            get { return parent; }
        }

        public RecMessage Message 
        {
            get
            {
                return Msg;
            }
        }

        public bool Open()
        {
            if(!m_Port.IsOpen)
                m_Port.Open();
            return m_Port.IsOpen;
        }

        public void SendData(byte [] data, int nLen)
        {
            m_Port.Write(data, 0, nLen);
        }

        private static void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            while(sp.BytesToRead > 0)
            {
                Msg.AddByte((byte)sp.ReadByte());
            }
        }
    }
}
