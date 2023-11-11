using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using NAudio;
using NAudio.Wave;

namespace SharpLoops
{
    /// <summary>
    /// Interaktionslogik für WaveOutput.xaml
    /// </summary>
    public partial class WaveOutput : Window
    {
        private const string PATH_KICK1 = @"C:\Recording\Samples\Samples2019\LoopLords 80s Drums Vol.1\BOSS DR-220E\DR220A_KICK.wav";
        public WaveOutput()
        {
            InitializeComponent();
        }


        private void button_Click(object sender, RoutedEventArgs e)
        {
            using (var audioFile = new AudioFileReader(PATH_KICK1))

            {
                byte[] data = new byte[5000];

                audioFile.Read(data, 0, data.Length);

                WpfPlot1.Plot.AddSignal(data);
                WpfPlot1.Refresh();
            }
        }
    }
}
