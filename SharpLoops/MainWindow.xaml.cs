using System;
using System.ComponentModel;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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

        private System.Timers.Timer Clock;

        public MainWindow()
        {
            // init values
            PatternPosition = 0;

            // start the clock
            Clock = new System.Timers.Timer(500);
            Clock.Elapsed += MoveLocatorToNextPos!;
            Clock.AutoReset = true;
            Clock.Enabled = true;

            // clear the pattern
            Pattern = new int[,]
            { 
                { 0,0,0,0,0,0,0,0},
                { 0,0,0,0,0,0,0,0},
                { 0,0,0,0,0,0,0,0},
                { 0,0,0,0,0,0,0,0} 
            };

            // cache the drum samples
            _sound1 = new CachedSound(PATH_KICK1);
            _sound2 = new CachedSound(PATH_SNARE1);
            _sound3 = new CachedSound(PATH_HAT1);
            _sound4 = new CachedSound(PATH_CRASH1);

            InitializeComponent();
        }

        public int[,] Pattern 
        { 
            get; 
            set; 
        }
        public int PatternPosition 
        { 
            get; 
            private set; 
        }

        /// <summary>
        /// Total number of sample tracks.
        /// </summary>
        public int TotalChannels
        {
            get { return Pattern.GetLength(0); }
        }

        /// <summary>
        /// Total number of track steps.
        /// </summary>
        public int TotalSteps
        {
            get { return Pattern.GetLength(1); }
        }


        private void MoveLocatorToNextPos(Object source, ElapsedEventArgs e)
        {
            if (PatternPosition < 7)
            {
                PatternPosition++;
            }
            else
            {
                PatternPosition = 0;
            }

            //string n = "buttonB0" + PatternPosition;


            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += worker_DoWork;
            worker.RunWorkerAsync(PatternPosition);




            MarkLocator(PatternPosition);
        }


        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            int pos = (int)e.Argument;

            if (Pattern[0, pos] != 0) AudioPlaybackEngine.Instance.PlaySound(_sound1);
            if (Pattern[1, pos] != 0) AudioPlaybackEngine.Instance.PlaySound(_sound2);
            if (Pattern[2, pos] != 0) AudioPlaybackEngine.Instance.PlaySound(_sound3);
            if (Pattern[3, pos] != 0) AudioPlaybackEngine.Instance.PlaySound(_sound4);
        }




        private bool MarkLocator(int pos)
        {
            // source: https://mycsharp.de/forum/threads/33113/faq-controls-von-thread-aktualisieren-lassen-control-invoke-dispatcher-invoke
            if (!this.Dispatcher.CheckAccess())
            { // Wenn Invoke nötig ist, ...
              // dann rufen wir die Methode selbst per Invoke auf

                Console.WriteLine("bla");

                return (bool)this.Dispatcher.Invoke((Func<int, bool>)MarkLocator, pos);
                // hier ist immer ein return (oder alternativ ein else) erforderlich.
                // Es verhindert, dass der folgende Code im Worker-Thread ausgeführt wird.
            }
            // eigentliche Zugriffe; laufen jetzt auf jeden Fall im GUI-Thread
            //progressBar.Value = percent; // schreibender Zugriff

            for (int i = 0; i < 8; i++)
            {
                Label lbl = (Label)this.FindName("label_00_0"+i);

                if (i == pos)
                {
                    lbl.Background = new SolidColorBrush(Colors.White);
                }
                else
                {
                    lbl.Background = new SolidColorBrush(Colors.Blue);
                }
            }
            return true; // lesender Zugriff
        }

        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;

            if (btn != null)
            {
                string[] split = btn.Name.Split('_');

                int row = Convert.ToInt32(split[1]);
                int col = Convert.ToInt32(split[2]);

                // toggle the value
                if (Pattern[row, col] == 0)
                {
                    Pattern[row, col] = 127;
                    btn.Background = new SolidColorBrush(Colors.Red);
                }
                else
                {
                    Pattern[row, col] = 0;
                    btn.Background = new SolidColorBrush(Colors.White);
                }
            }
            #region stuff
            //Button c = (Button)this.FindName("buttonStop");

            //c.Background = new SolidColorBrush(Colors.White);

            //Thread thread = new Thread(new ThreadStart(PlayMultiSounds));
            //thread.Start();
            #endregion
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
