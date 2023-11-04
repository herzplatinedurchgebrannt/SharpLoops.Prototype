using NAudio.Mixer;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
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
using SharpLoops.Audio;

namespace SharpLoops
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string PATH_KICK1 = @"C:\Recording\Samples\Samples2019\LoopLords 80s Drums Vol.1\BOSS DR-220E\DR220A_KICK.wav";
        private const string PATH_SNARE1 = @"C:\Recording\Samples\Samples2019\LoopLords 80s Drums Vol.1\BOSS DR-220E\DR220A_SNARE.wav";
        private const string PATH_HAT1 = @"C:\Recording\Samples\Samples2019\LoopLords 80s Drums Vol.1\BOSS DR-220E\DR220A_CL HAT.wav";
        private const string PATH_HAT2 = @"C:\Recording\Samples\Samples2019\LoopLords 80s Drums Vol.1\BOSS DR-220E\DR220A_KICK.wav";
        private const string PATH_TOM1 = @"C:\Recording\Samples\Samples2019\LoopLords 80s Drums Vol.1\BOSS DR-220E\DR220A_LOW TOM.wav";
        private const string PATH_TOM2 = @"C:\Recording\Samples\Samples2019\LoopLords 80s Drums Vol.1\BOSS DR-220E\DR220A_KICK.wav";
        private const string PATH_CRASH1 = @"C:\Recording\Samples\Samples2019\LoopLords 80s Drums Vol.1\BOSS DR-220E\DR220A_CRASH.wav";

        private CachedSound _sound1;
        private CachedSound _sound2;
        private CachedSound _sound3;
        private CachedSound _sound4;

        MixingWaveProvider32 _mixer;
        DirectSoundOut _soundOutput;

        WaveOutEvent _waveOutEvent;


        private int[,] Pattern { get; set; }


        private System.Timers.Timer aTimer;
        private int pos = 0;
        private bool[] state = new bool[8];



        public MainWindow()
        {
            Pattern = new int[,]
            { 
                { 0,0,0,0,0,0,0,0},
                { 0,0,0,0,0,0,0,0},
                { 0,0,0,0,0,0,0,0},
                { 0,0,0,0,0,0,0,0} 
            };

            _sound1 = new CachedSound(PATH_KICK1);
            _sound2 = new CachedSound(PATH_SNARE1);
            _sound3 = new CachedSound(PATH_HAT1);
            _sound4 = new CachedSound(PATH_CRASH1);

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


            Thread thread = new Thread(new ThreadStart(PlayMultiSounds));
            thread.Start();
        }



        private void PlayMultiSounds()
        {
            AudioPlaybackEngine.Instance.PlaySound(_sound1);
            AudioPlaybackEngine.Instance.PlaySound(_sound2);
            AudioPlaybackEngine.Instance.PlaySound(_sound3);
            AudioPlaybackEngine.Instance.PlaySound(_sound4);
        }

      

        private void PlayButtonClick(object sender, RoutedEventArgs e)
        {
            Button b = (Button)this.FindName("buttonStop");

            b.Background = new SolidColorBrush(Colors.White);
        }
    }
}
