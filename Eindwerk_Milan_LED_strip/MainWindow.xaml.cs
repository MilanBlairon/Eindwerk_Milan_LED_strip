using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;



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
                        btnOnOff.IsEnabled = true;
                    }
                    else
                    {
                        btnOnOff.IsEnabled = false;
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
                    sldrBrightness.IsEnabled = true;
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

                else
                {
                    sldrBrightness.IsEnabled = false;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Fout: " + ex.Message, "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
                MessageBox.Show("Fout bij het versturen van een commando: " + ex.Message, "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                if (_serialPort != null && _serialPort.IsOpen)
                {
                    SendCommand("B0");
                    SendCommand("9");
                    SendCommand("C");
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
                SendCommand("A"); // Ingeschakeld sturen
                SendCommand("B" + sldrBrightness.Value.ToString());
                sldrBrightness.IsEnabled = true;
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
                SendCommand("C"); // Uitgeschakeld sturen
                sldrBrightness.IsEnabled = false;
                ledStripActief = false;
            }

        }

        private void KnoppenTransparant()
        {

            List<Button> knoppen = new List<Button>
            {
                btnLichtRood, btnLichtOranje, btnLichtGeel, btnLichtGroen,
                btnLichtBlauw, btnLichtPaars, btnLichtRoze, btnWarmWit,
                btnWit, btnDonkerRood, btnDonkerOranje, btnDonkerGeel, btnDonkerGroen,
                btnDonkerBlauw, btnDonkerPaars, btnDonkerRoze, btnPastel,
                btnDonkerStatischeRegenboog, btnRegenboog, btnBlauwRood, btnGroenBlauw,
                btnSmoothPastel
            };

            foreach (var knop in knoppen)
            {
                knop.Background = new SolidColorBrush(Colors.Transparent);
            }

        }
        private void AchtergrondKleurKnoppenVeranderen(object sender, RoutedEventArgs e, string command, Button button, Color startColor, Color endColor)
        {
            SendCommand(command);
            KnoppenTransparant();

            LinearGradientBrush gradientBrush = new LinearGradientBrush();
            gradientBrush.StartPoint = new Point(0, 0);
            gradientBrush.EndPoint = new Point(1, 1);

            gradientBrush.GradientStops.Add(new GradientStop(startColor, 0));
            gradientBrush.GradientStops.Add(new GradientStop(endColor, 1));

            ((Button)sender).Background = gradientBrush;
        }

        private void AchtergrondRegenboogKnoppenVeranderen(object sender, RoutedEventArgs e, string command, Button button, double helderheid)
        {
            SendCommand(command);
            KnoppenTransparant();
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
            gradientBrush.Opacity = helderheid;

            ((Button)sender).Background = gradientBrush;
        }

        private void btnSnelleStart_Click(object sender, RoutedEventArgs e)
        {
            // Toggle de kleur van de knop
            if (ledStripActief == false)
            {
                if (cbxComPorts.SelectedItem != null && cbxComPorts.SelectedItem.ToString() != "None")
                {
                    LinearGradientBrush gradientBrush = new LinearGradientBrush();
                    gradientBrush.StartPoint = new Point(0, 0);
                    gradientBrush.EndPoint = new Point(1, 1);

                    gradientBrush.GradientStops.Add(new GradientStop(Color.FromRgb(0, 128, 0), 0)); // Donkergroen
                    gradientBrush.GradientStops.Add(new GradientStop(Color.FromRgb(0, 255, 0), 1)); // Helder groen

                    btnOnOff.Background = gradientBrush;
                    btnOnOff.Content = "Ingeschakeld";

                    SendCommand("I"); // Ingeschakeld sturen
                    SendCommand("B50"); // 50% lichtsterkte
                    sldrBrightness.Value = 50;

                    KnoppenTransparant();
                    SendCommand("K");
                    witteKnopVeranderen();
                    sldrBrightness.IsEnabled = true;
                    ledStripActief = true;
                }
                else
                    MessageBox.Show("Selecteer eerst een COM poort!", "Geen COM poort!", MessageBoxButton.OK, MessageBoxImage.Error);

            }
            if (ledStripActief == true)
            {
                LinearGradientBrush gradientBrush = new LinearGradientBrush();
                gradientBrush.StartPoint = new Point(0, 0);
                gradientBrush.EndPoint = new Point(1, 1);

                gradientBrush.GradientStops.Add(new GradientStop(Color.FromRgb(0, 128, 0), 0)); // Donkergroen
                gradientBrush.GradientStops.Add(new GradientStop(Color.FromRgb(0, 255, 0), 1)); // Helder groen

                btnOnOff.Background = gradientBrush;
                btnOnOff.Content = "Ingeschakeld";

                SendCommand("I"); // Ingeschakeld sturen
                SendCommand("B50"); // 50% lichtsterkte
                sldrBrightness.Value = 50;

                KnoppenTransparant();
                SendCommand("K");
                witteKnopVeranderen();
                sldrBrightness.IsEnabled = true;
            }
        }

        private void witteKnopVeranderen()
        {
            LinearGradientBrush gradientBrush2 = new LinearGradientBrush();
            gradientBrush2.StartPoint = new Point(0, 0);
            gradientBrush2.EndPoint = new Point(1, 1);

            gradientBrush2.GradientStops.Add(new GradientStop(Color.FromRgb(200, 200, 200), 0));
            gradientBrush2.GradientStops.Add(new GradientStop(Color.FromRgb(255, 255, 255), 1));

            btnWit.Background = gradientBrush2;
        }
        private void btnOnOff_Click(object sender, RoutedEventArgs e)
            => AanUitKnop();

        private void btnLichtRood_Click(object sender, RoutedEventArgs e)
            => AchtergrondKleurKnoppenVeranderen(sender, e, "D", btnLichtRood, Color.FromRgb(200, 0, 0), Color.FromRgb(128, 0, 0));

        private void btnLichtOranje_Click(object sender, RoutedEventArgs e)
            => AchtergrondKleurKnoppenVeranderen(sender, e, "E", btnLichtOranje, Color.FromRgb(255, 100, 0), Color.FromRgb(255, 130, 0));

        private void btnLichtGeel_Click(object sender, RoutedEventArgs e)
            => AchtergrondKleurKnoppenVeranderen(sender, e, "F", btnLichtGeel, Color.FromRgb(255, 255, 0), Color.FromRgb(255, 255, 0));

        private void btnLichtGroen_Click(object sender, RoutedEventArgs e)
            => AchtergrondKleurKnoppenVeranderen(sender, e, "G", btnLichtGroen, Color.FromRgb(0, 255, 0), Color.FromRgb(0, 255, 0));

        private void btnLichtBlauw_Click(object sender, RoutedEventArgs e)
            => AchtergrondKleurKnoppenVeranderen(sender, e, "H", btnLichtBlauw, Color.FromRgb(0, 0, 255), Color.FromRgb(0, 0, 255));

        private void btnLichtPaars_Click(object sender, RoutedEventArgs e)
            => AchtergrondKleurKnoppenVeranderen(sender, e, "I", btnLichtPaars, Color.FromRgb(128, 0, 128), Color.FromRgb(128, 0, 128));

        private void btnLichtRoze_Click(object sender, RoutedEventArgs e)
            => AchtergrondKleurKnoppenVeranderen(sender, e, "J", btnLichtRoze, Color.FromRgb(255, 182, 193), Color.FromRgb(255, 192, 203));

        private void btnWit_Click(object sender, RoutedEventArgs e)
            => AchtergrondKleurKnoppenVeranderen(sender, e, "K", btnWit, Color.FromRgb(200, 200, 200), Color.FromRgb(255, 255, 255));

        private void btnWarmWit_Click(object sender, RoutedEventArgs e)
            => AchtergrondKleurKnoppenVeranderen(sender, e, "L", btnWarmWit, Color.FromRgb(200, 200, 200), Color.FromRgb(200, 200, 100));

        private void btnDonkerRood_Click(object sender, RoutedEventArgs e)
            => AchtergrondKleurKnoppenVeranderen(sender, e, "M", btnDonkerRood, Color.FromRgb(200, 0, 0), Color.FromRgb(128, 0, 0));

        private void btnDonkerOranje_Click(object sender, RoutedEventArgs e)
            => AchtergrondKleurKnoppenVeranderen(sender, e, "N", btnDonkerOranje, Color.FromRgb(255, 100, 0), Color.FromRgb(255, 130, 0));

        private void btnDonkerGeel_Click(object sender, RoutedEventArgs e)
            => AchtergrondKleurKnoppenVeranderen(sender, e, "O", btnDonkerGeel, Color.FromRgb(255, 255, 0), Color.FromRgb(255, 255, 0));

        private void btnDonkerGroen_Click(object sender, RoutedEventArgs e)
            => AchtergrondKleurKnoppenVeranderen(sender, e, "P", btnDonkerGroen, Color.FromRgb(0, 255, 0), Color.FromRgb(0, 255, 0));

        private void btnDonkerBlauw_Click(object sender, RoutedEventArgs e)
            => AchtergrondKleurKnoppenVeranderen(sender, e, "Q", btnDonkerBlauw, Color.FromRgb(0, 0, 255), Color.FromRgb(0, 0, 255));

        private void btnDonkerPaars_Click(object sender, RoutedEventArgs e)
            => AchtergrondKleurKnoppenVeranderen(sender, e, "R", btnDonkerPaars, Color.FromRgb(128, 0, 128), Color.FromRgb(128, 0, 128));

        private void btnDonkerRoze_Click(object sender, RoutedEventArgs e)
            => AchtergrondKleurKnoppenVeranderen(sender, e, "S", btnDonkerRoze, Color.FromRgb(255, 182, 193), Color.FromRgb(255, 192, 203));

        private void btnPastel_Click(object sender, RoutedEventArgs e)
        {
            SendCommand("T");
            KnoppenTransparant();
            LinearGradientBrush gradientBrush = new LinearGradientBrush();
            gradientBrush.StartPoint = new Point(0, 0);
            gradientBrush.EndPoint = new Point(1, 1);

            gradientBrush.GradientStops.Add(new GradientStop(Color.FromRgb(210, 150, 210), 0));    // Pastel Paars
            gradientBrush.GradientStops.Add(new GradientStop(Color.FromRgb(150, 200, 255), 0.2)); // Pastel Blauw
            gradientBrush.GradientStops.Add(new GradientStop(Color.FromRgb(180, 255, 180), 0.4)); // Pastel Groen
            gradientBrush.GradientStops.Add(new GradientStop(Color.FromRgb(255, 255, 180), 0.6)); // Pastel Geel
            gradientBrush.GradientStops.Add(new GradientStop(Color.FromRgb(255, 180, 120), 0.8));  // Pastel Oranje
            gradientBrush.GradientStops.Add(new GradientStop(Color.FromRgb(255, 150, 150), 1));    // Pastel Roze
            gradientBrush.Opacity = 0.8;

            ((Button)sender).Background = gradientBrush;
        }

        private void btnDonkerStatischeRegenboog_Click(object sender, RoutedEventArgs e)
            => AchtergrondRegenboogKnoppenVeranderen(sender, e, "U", btnDonkerStatischeRegenboog, 0.80);

        private void btnRainbowCycle_Click(object sender, RoutedEventArgs e)
            => AchtergrondRegenboogKnoppenVeranderen(sender, e, "V", btnRegenboog, 0.9);

        private void btnBlauwRood_Click(object sender, RoutedEventArgs e)
            => AchtergrondKleurKnoppenVeranderen(sender, e, "W", btnBlauwRood, Color.FromRgb(0, 0, 255), Color.FromRgb(255, 0, 0));

        private void btnGroenBlauw_Click(object sender, RoutedEventArgs e)
            => AchtergrondKleurKnoppenVeranderen(sender, e, "X", btnGroenBlauw, Color.FromRgb(0, 255, 0), Color.FromRgb(0, 0, 255));

        private void btnSmoothPastel_Click(object sender, RoutedEventArgs e)
            => AchtergrondKleurKnoppenVeranderen(sender, e, "Y", btnSmoothPastel, Color.FromRgb(0, 200, 255), Color.FromRgb(255, 180, 0));

        private void btnSoon1_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnSoon2_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnSoon3_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnSoon4_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnSoon5_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}