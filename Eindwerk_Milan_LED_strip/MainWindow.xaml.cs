using System;
using System.Collections.Generic;
using System.IO.Ports;
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



namespace Eindwerk_Milan_LED_strip
{
    public partial class MainWindow : Window
    {
        SerialPort _serialPort;

        public MainWindow()
        {
            InitializeComponent();

            _serialPort = new SerialPort();

            cbxComPorts.Items.Add("None");
            foreach (string s in SerialPort.GetPortNames())
                cbxComPorts.Items.Add(s);
        }

        private void cbxComPorts_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (_serialPort != null)
            {
                if (_serialPort.IsOpen)
                {
                    _serialPort.Close();
                }

                if (cbxComPorts.SelectedItem.ToString() != "None")
                {
                    _serialPort.PortName = cbxComPorts.SelectedItem.ToString();
                    _serialPort.Open();
                    sldrBirghtness.IsEnabled = true;
                }
                else
                    sldrBirghtness.IsEnabled = false;
            }
        }

        private void sldrBirghtness_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            byte data = 0;

            for (int i = 0; i < sldrBirghtness.Value; i++)
            {
                data <<= 1;
                data++;
            }

            if ((_serialPort != null) && (_serialPort.IsOpen))
            {
                _serialPort.Write(new byte[] { data }, 0, 1);
            }
        }

        private void btnSetWhiteColor_Click(object sender, RoutedEventArgs e)
        {
            SendCommand("W"); // Replace "W" with the command you want to send for setWhiteColor
        }

        private void btnRainbowCycle_Click(object sender, RoutedEventArgs e)
        {
            SendCommand("R"); // Replace "R" with the command you want to send for rainbowCycle
        }

        private void SendCommand(string command)
        {
            if ((_serialPort != null) && (_serialPort.IsOpen))
            {
                _serialPort.Write(command);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if ((_serialPort != null) && _serialPort.IsOpen)
            {
                _serialPort.Write(new byte[] { 0 }, 0, 1);
                _serialPort.Dispose();
            }
        }

        private void btnOnOff_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}

