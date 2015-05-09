using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ControllerMimic
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static Comms theComms;
        public delegate void ShowInt(int val);
        public static ShowInt ShowPAS;

        public MainWindow()
        {
            InitializeComponent();
            ShowPAS = new ShowInt(DisplayPAS);
        }

        private void butConnect_Click(object sender, RoutedEventArgs e)
        {
            string sPort = txtPort.Text;
            theComms = new Comms(sPort, this);
            if(theComms.Open())
            {
                theComms.Message.Speed = (int)sldSpeed.Value;
            }
        }

        private void sldSpeed_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            sldSpeed = (Slider)sender;
            if (theComms != null)
            {
                theComms.Message.Speed = (int)sldSpeed.Value;
            }
            if(txtSpeed != null)
                txtSpeed.Text = ((int)sldSpeed.Value).ToString();
        }

        public void DisplayPAS(int n)
        {
            txtPAS.Text = n.ToString();
        }
    }
}
