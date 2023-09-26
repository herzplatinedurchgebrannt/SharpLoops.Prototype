using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SharpLoops
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private System.Timers.Timer aTimer;
        private int pos = 0;
        private bool[] state = new bool[8];

             
        public MainWindow()
        {
            SetTimer();
            InitializeComponent();
        }


        private void SetTimer()        
        {
            aTimer = new System.Timers.Timer(100);
            aTimer.Elapsed += OnTimedEvent!;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
        }

        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            if (pos < 8)
            {
                pos++;
            }
            else
            {
                pos = 1;
            }

            string n = "buttonB0" + pos;

            PaintButton(pos);
        }

        private bool PaintButton(int pos)
        {
            if (!this.Dispatcher.CheckAccess())
            { // Wenn Invoke nötig ist, ...
              // dann rufen wir die Methode selbst per Invoke auf
                return (bool)this.Dispatcher.Invoke((Func<int, bool>)PaintButton, pos);
                // hier ist immer ein return (oder alternativ ein else) erforderlich.
                // Es verhindert, dass der folgende Code im Worker-Thread ausgeführt wird.
            }
            // eigentliche Zugriffe; laufen jetzt auf jeden Fall im GUI-Thread
            //progressBar.Value = percent; // schreibender Zugriff

            for (int i = 1; i < 9; i++)
            {
                ToggleButton d = (ToggleButton)this.FindName("buttonB0"+i);

                if (i == pos)
                {
                    d.Background = new SolidColorBrush(Colors.White);
                }
                else
                {
                    d.Background = new SolidColorBrush(Colors.Blue);
                }
            }

            return true; // lesender Zugriff
        }

        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            Button c = (Button)this.FindName("buttonStop");

            c.Background = new SolidColorBrush(Colors.White);





            //ToggleButton btn = (ToggleButton)sender;

            //if (btn.IsChecked == true)
            //{
            //    string b = btn.Name;
            //    MessageBox.Show(b);
            //    btn.Background = Brushes.Red;
            //}
            //else
            //{
            //    btn.Background = Brushes.LightGreen;
            //}
        }

        private void PlayButtonClick(object sender, RoutedEventArgs e)
        {
            Button b = (Button)this.FindName("buttonStop");

            b.Background = new SolidColorBrush(Colors.White);
        }
    }
}
