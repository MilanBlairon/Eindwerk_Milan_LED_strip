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
        private SerialPort _serialPort;
        bool ledStripActief = false;
        public MainWindow()
        {
            InitializeComponent();

            _serialPort = new SerialPort();

            // Voeg "None" toe aan de combobox
            cbxComPorts.Items.Add("None");

            // Voeg beschikbare poorten toe aan de combobox
            foreach (string portName in SerialPort.GetPortNames())
            {
                cbxComPorts.Items.Add(portName);
            }
        }

        private void cbxComPorts_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            try
            {
                if (_serialPort != null)
                {
                    if (_serialPort.IsOpen)
                    {
                        _serialPort.Close();
                    }

                    // Controleer op null voordat ToString() wordt aangeroepen
                    if (cbxComPorts.SelectedItem != null && cbxComPorts.SelectedItem.ToString() != "None")
                    {
                        _serialPort.PortName = cbxComPorts.SelectedItem.ToString();
                        _serialPort.Open();
                        sldrBrightness.IsEnabled = true;
                    }
                    else
                    {
                        sldrBrightness.IsEnabled = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fout: " + ex.Message, "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void sldrBrightness_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                if (ledStripActief == true)
                {
                    // Stuur de schuifregelaarwaarde naar Arduino wanneer deze verandert
                    int value = (int)sldrBrightness.Value;

                    if (value == 0)
                    {
                        SendCommand("B" + value.ToString());

                    }

                    if ((value > 0) && (value < 6)) // Ledstrip problemen omzeilen (minimum brightness instellen)
                    {
                        SendCommand("B5");
                    }

                    else
                    {
                        SendCommand("B" + value.ToString());
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Fout: " + ex.Message, "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnLichtRood_Click(object sender, RoutedEventArgs e)
        {
            SendCommand("C");
            KnoppenTransparant();
            RoodAchtergrond();
        }

        private void RoodAchtergrond()
        {
            LinearGradientBrush gradientBrush = new LinearGradientBrush();
            gradientBrush.StartPoint = new Point(0, 0);
            gradientBrush.EndPoint = new Point(1, 1);

            gradientBrush.GradientStops.Add(new GradientStop(Color.FromRgb(200, 0, 0), 0));
            gradientBrush.GradientStops.Add(new GradientStop(Color.FromRgb(128, 0, 0), 1));

            btnLichtRood.Background = gradientBrush;
        }

        private void btnSetWhiteColor_Click(object sender, RoutedEventArgs e)
        {
            SendCommand("W");
            KnoppenTransparant();
            WitAchtergrond();
        }

        private void WitAchtergrond()
        {
            LinearGradientBrush gradientBrush = new LinearGradientBrush();
            gradientBrush.StartPoint = new Point(0, 0);
            gradientBrush.EndPoint = new Point(1, 1);

            gradientBrush.GradientStops.Add(new GradientStop(Color.FromRgb(200, 200, 200), 0)); // Donkerder
            gradientBrush.GradientStops.Add(new GradientStop(Color.FromRgb(255, 255, 255), 1)); // Witter

            btnWit.Background = gradientBrush;
        }

        private void btnRainbowCycle_Click(object sender, RoutedEventArgs e)
        {
            SendCommand("R");
            KnoppenTransparant();
            RegenboogAchtergrond();
        }

        private void RegenboogAchtergrond()
        {
            LinearGradientBrush gradientBrush = new LinearGradientBrush();
            gradientBrush.StartPoint = new Point(0, 0);
            gradientBrush.EndPoint = new Point(1, 1);

            gradientBrush.GradientStops.Add(new GradientStop(Colors.Red, 0));
            gradientBrush.GradientStops.Add(new GradientStop(Colors.Orange, 0.17));
            gradientBrush.GradientStops.Add(new GradientStop(Colors.Yellow, 0.33));
            gradientBrush.GradientStops.Add(new GradientStop(Colors.Green, 0.5));
            gradientBrush.GradientStops.Add(new GradientStop(Colors.Blue, 0.67));
            gradientBrush.GradientStops.Add(new GradientStop(Colors.Indigo, 0.83));
            gradientBrush.GradientStops.Add(new GradientStop(Colors.Violet, 1));
            gradientBrush.Opacity = 0.75;
            btnRegenboog.Background = gradientBrush;
        }

        private void KnoppenTransparant()
        {
            btnRegenboog.Background = new SolidColorBrush(Colors.Transparent);
            btnWit.Background = new SolidColorBrush(Colors.Transparent);
            btnLichtRood.Background = new SolidColorBrush(Colors.Transparent);
            btnLichtRood.Background = new SolidColorBrush(Colors.Transparent);
        }

        private void SendCommand(string command)
        {
            try
            {
                if (_serialPort != null && _serialPort.IsOpen)
                {
                    _serialPort.Write(command + "\n\r");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fout: " + ex.Message, "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                if (_serialPort != null && _serialPort.IsOpen)
                {
                    SendCommand("B0");
                    SendCommand("U");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fout bij het sluiten: " + ex.Message, "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                try
                {
                    if (_serialPort != null && _serialPort.IsOpen)
                    {
                        _serialPort.Close();
                        _serialPort.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Fout bij het sluiten van de poort: " + ex.Message, "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnOnOff_Click(object sender, RoutedEventArgs e)
        {
            // Toggle de kleur van de knop en de tekst
            AanUitKnop();
        }

        private void AanUitKnop()
        {
            // Toggle de kleur van de knop
            if (ledStripActief == false)
            {
                LinearGradientBrush gradientBrush = new LinearGradientBrush();
                gradientBrush.StartPoint = new Point(0, 0);
                gradientBrush.EndPoint = new Point(1, 1);

                gradientBrush.GradientStops.Add(new GradientStop(Color.FromRgb(0, 128, 0), 0)); // Donkergroen
                gradientBrush.GradientStops.Add(new GradientStop(Color.FromRgb(0, 255, 0), 1)); // Helder groen

                btnOnOff.Background = gradientBrush;
                btnOnOff.Content = "Ingeschakeld";
                SendCommand("I"); // Ingeschakeld sturen
                SendCommand("B" + sldrBrightness.Value.ToString());
                ledStripActief = true;
            }
            else
            {
                LinearGradientBrush gradientBrush = new LinearGradientBrush();
                gradientBrush.StartPoint = new Point(0, 0);
                gradientBrush.EndPoint = new Point(1, 1);

                gradientBrush.GradientStops.Add(new GradientStop(Color.FromRgb(255, 0, 0), 0)); // Donkergroen
                gradientBrush.GradientStops.Add(new GradientStop(Color.FromRgb(128, 0, 0), 1)); // Helder groen

                btnOnOff.Background = gradientBrush;
                btnOnOff.Content = "Uitgeschakeld";
                SendCommand("U"); // Uitgeschakeld sturen
                ledStripActief = false;
            }

        }

        private void btnDonkerRood_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnLichtOranje_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnLichtGeel_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnLichtGroen_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnLichtBlauw_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnLichtPaars_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnLichtRoze_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnLichtStatischeRegenboog_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnDonkerOranje_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnDonkerGeel_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnDonkerGroen_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnDonkerBlauw_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnDonkerPaars_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnDonkerRoze_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnGrijs_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnDonkerStatischeRegenboog_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}