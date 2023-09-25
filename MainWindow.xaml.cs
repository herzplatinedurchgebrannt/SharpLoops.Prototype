using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public MainWindow()
        {
            InitializeComponent();
        }

        private void clickButton(object sender, RoutedEventArgs e)
        {
            ToggleButton btn = (ToggleButton)sender;

            if (btn.IsChecked == true)
            {
                string b = btn.Name;
                MessageBox.Show(b);
                btn.Background = Brushes.Red;
            }
            else
            {
                btn.Background = Brushes.LightGreen;
            }
        }
    }
}
