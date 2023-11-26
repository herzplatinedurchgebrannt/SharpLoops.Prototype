using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SharpLoops.Audio;
using System.Windows.Media;
using SharpLoops.Model;
using System.Media;

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

        private float[] _buf = new float[1000];
        private ScottPlot.Plottable.DataStreamer? _streamer;
        private CachedSound _sound1;
        private CachedSound _sound2;
        private CachedSound _sound3;
        private CachedSound _sound4;
        private Stopwatch _stopwatch;
        private BackgroundWorker _worker;
        private System.Timers.Timer _timer;
        private PlayerState _state;
        private bool _newDataAvailable;
        private int _dynamicValue;
        private int _tempoBPM;
        private int _activePattern;
        private SharpLoops.Model.Preset _preset;

        AudioPlaybackEngine engine = new AudioPlaybackEngine();

        public MainWindow()
        {
            // init preset & pattern stuff
            _preset = new Model.Preset();

            _preset.Pattern.Add( new Pattern(4, 16) { Sounds = new Sound[4] { new Sound(1, PATH_KICK1, "Kick"), new Sound(2, PATH_SNARE1, "Snare"), new Sound(3, PATH_HAT1, "Hihat"), new Sound(4, PATH_CRASH1, "Crash") } });
            _preset.Pattern.Add( new Pattern(4, 16) { Sounds = new Sound[4] { new Sound(1, PATH_KICK1, "Kick"), new Sound(2, PATH_SNARE1, "Snare"), new Sound(3, PATH_HAT1, "Hihat"), new Sound(4, PATH_CRASH1, "Crash") } });
            _preset.Pattern.Add( new Pattern(4, 16) { Sounds = new Sound[4] { new Sound(1, PATH_KICK1, "Kick"), new Sound(2, PATH_SNARE1, "Snare"), new Sound(3, PATH_HAT1, "Hihat"), new Sound(4, PATH_CRASH1, "Crash") } });
            _preset.Pattern.Add( new Pattern(4, 16) { Sounds = new Sound[4] { new Sound(1, PATH_KICK1, "Kick"), new Sound(2, PATH_SNARE1, "Snare"), new Sound(3, PATH_HAT1, "Hihat"), new Sound(4, PATH_CRASH1, "Crash") } });
            _preset.Pattern.Add( new Pattern(4, 16) { Sounds = new Sound[4] { new Sound(1, PATH_KICK1, "Kick"), new Sound(2, PATH_SNARE1, "Snare"), new Sound(3, PATH_HAT1, "Hihat"), new Sound(4, PATH_CRASH1, "Crash") } });
            _preset.Pattern.Add( new Pattern(4, 16) { Sounds = new Sound[4] { new Sound(1, PATH_KICK1, "Kick"), new Sound(2, PATH_SNARE1, "Snare"), new Sound(3, PATH_HAT1, "Hihat"), new Sound(4, PATH_CRASH1, "Crash") } });
            _preset.Pattern.Add( new Pattern(4, 16) { Sounds = new Sound[4] { new Sound(1, PATH_KICK1, "Kick"), new Sound(2, PATH_SNARE1, "Snare"), new Sound(3, PATH_HAT1, "Hihat"), new Sound(4, PATH_CRASH1, "Crash") } });
            _preset.Pattern.Add( new Pattern(4, 16) { Sounds = new Sound[4] { new Sound(1, PATH_KICK1, "Kick"), new Sound(2, PATH_SNARE1, "Snare"), new Sound(3, PATH_HAT1, "Hihat"), new Sound(4, PATH_CRASH1, "Crash") } });
            _preset.Pattern.Add( new Pattern(4, 16) { Sounds = new Sound[4] { new Sound(1, PATH_KICK1, "Kick"), new Sound(2, PATH_SNARE1, "Snare"), new Sound(3, PATH_HAT1, "Hihat"), new Sound(4, PATH_CRASH1, "Crash") } });

            // init values
            _activePattern = 0;
            PatternPosition = 0;
            _dynamicValue = 127;
            TempoBPM = 120;

            _sound1 = new CachedSound(_preset.Pattern[_activePattern].Sounds[0].SamplePath);
            _sound2 = new CachedSound(_preset.Pattern[_activePattern].Sounds[1].SamplePath);
            _sound3 = new CachedSound(_preset.Pattern[_activePattern].Sounds[2].SamplePath);
            _sound4 = new CachedSound(_preset.Pattern[_activePattern].Sounds[3].SamplePath);

            _stopwatch = new Stopwatch();

            _state = PlayerState.Stop;

            // cache the drum samples
            _sound1 = new CachedSound(PATH_KICK1);
            _sound2 = new CachedSound(PATH_SNARE1);
            _sound3 = new CachedSound(PATH_HAT1);
            _sound4 = new CachedSound(PATH_CRASH1);

            _worker = new BackgroundWorker();

            TempoBPM = 120;

            _timer = new System.Timers.Timer(50);
            _timer.Elapsed += RefreshGraph!;
            _timer.AutoReset = true;
            _timer.Enabled = true;
            _newDataAvailable = false;

            AudioPlaybackEngine.Instance.PlottDataAvailable += UpdateGraphData!;

            InitializeComponent();
            InitWaveWindow();
        }

        public void InitWaveWindow()
        {
            Dispatcher.Invoke(new Action(() =>
            {
                // output wave plot
                var plt = OutputPlot.Plot;

                plt.SetAxisLimitsY(-2, 2);
                _streamer = plt.AddDataStreamer(1000);
                plt.Style(
                    figureBackground: System.Drawing.Color.Black,
                    dataBackground: System.Drawing.Color.Black);
                plt.Title("");
                plt.XLabel("");
                plt.YLabel("");
                plt.Render();

                // init button states
                dynamicBox.Text = _dynamicValue.ToString();
                tempoBox.Text = TempoBPM.ToString();
                patternBox.Text = _activePattern.ToString();

                btn_pattern_00.Background = new SolidColorBrush(Colors.DarkRed);
            }));
        }


        public void RefreshGraph(Object source, ElapsedEventArgs e)
        {
            if (_streamer == null) return;

            Dispatcher.Invoke(new Action(() =>
            {
                foreach (var item in _buf)
                {
                    _streamer.Add(item);
                }
                OutputPlot.Refresh();
            }));

            Array.Clear(_buf);
        }

        public void UpdateGraphData(Object sender, float[] f)
        {
            _buf = f;

            //_newDataAvailable = true;
        }

        public int PatternPosition
        {
            get;
            private set;
        }

        public int TempoBPM
        {
            get => _tempoBPM;
            private set => _tempoBPM = value;
        }

        /// <summary>
        /// Total number of sample tracks.
        /// </summary>
        public int TotalTracks
        {
            get { return _preset.Pattern[_activePattern].Grid.GetLength(0); }
        }

        /// <summary>
        /// Total number of track steps.
        /// </summary>
        public int TotalSteps
        {
            get { return _preset.Pattern[_activePattern].Grid.GetLength(1); }
        }

        private void AudioWorker(object sender, DoWorkEventArgs e)
        {
            // source: https://wpf-tutorial.com/de/97/sonstiges-miscellaneous/multithreading-mit-dem-backgroundworker/
            _stopwatch.Start();

            while (_state == PlayerState.Playing)
            {
                if (_stopwatch.ElapsedMilliseconds >= 60000 / _tempoBPM / 2) // 1/8 notes
                {
                    //Debug.WriteLine(60000 / _tempoBPM / 2);     
                    Debug.WriteLine("AW: " + _activePattern);

                    if (_preset.Pattern[_activePattern].Grid[0, PatternPosition] != 0) AudioPlaybackEngine.Instance.PlaySound(_sound1);
                    if (_preset.Pattern[_activePattern].Grid[1, PatternPosition] != 0) AudioPlaybackEngine.Instance.PlaySound(_sound2);
                    if (_preset.Pattern[_activePattern].Grid[2, PatternPosition] != 0) AudioPlaybackEngine.Instance.PlaySound(_sound3);
                    if (_preset.Pattern[_activePattern].Grid[3, PatternPosition] != 0) AudioPlaybackEngine.Instance.PlaySound(_sound4);

                    MarkLocator(PatternPosition); // this must be done somewhere else. 

                    if (PatternPosition < TotalSteps - 1)
                    {
                        PatternPosition++;
                    }
                    else
                    {
                        PatternPosition = 0;
                    }
                    _stopwatch.Restart();
                }
            }
        }

        private bool MarkLocator(int pos)
        {
            // source: https://mycsharp.de/forum/threads/33113/faq-controls-von-thread-aktualisieren-lassen-control-invoke-dispatcher-invoke
            if (!this.Dispatcher.CheckAccess())
            { // Wenn Invoke nötig ist, ...
              // dann rufen wir die Methode selbst per Invoke auf

                return (bool)this.Dispatcher.Invoke((Func<int, bool>)MarkLocator, pos);
                // hier ist immer ein return (oder alternativ ein else) erforderlich.
                // Es verhindert, dass der folgende Code im Worker-Thread ausgeführt wird.
            }
            // eigentliche Zugriffe; laufen jetzt auf jeden Fall im GUI-Thread
            //progressBar.Value = percent; // schreibender Zugriff

            for (int i = 0; i < TotalSteps; i++)
            {
                Label lbl = i >= 10 ? (Label)this.FindName("label_00_" + i) : (Label)this.FindName("label_00_0" + i);

                if (i == pos)
                {
                    lbl.Background = new SolidColorBrush(Colors.DarkGray);
                }
                else
                {
                    lbl.Background = new SolidColorBrush(Colors.DarkRed);
                }
            }
            return true; // lesender Zugriff
        }

        private bool ClearButtonState(int pos)
        {
            // source: https://mycsharp.de/forum/threads/33113/faq-controls-von-thread-aktualisieren-lassen-control-invoke-dispatcher-invoke
            if (!this.Dispatcher.CheckAccess())
            { // Wenn Invoke nötig ist, ...
              // dann rufen wir die Methode selbst per Invoke auf

                return (bool)this.Dispatcher.Invoke((Func<int, bool>)MarkLocator, pos);
                // hier ist immer ein return (oder alternativ ein else) erforderlich.
                // Es verhindert, dass der folgende Code im Worker-Thread ausgeführt wird.
            }
            // eigentliche Zugriffe; laufen jetzt auf jeden Fall im GUI-Thread
            //progressBar.Value = percent; // schreibender Zugriff

            for (int i = 0; i < TotalTracks; i++)
            {
                for (int j = 0; j < TotalSteps; j++)
                {
                    Button lbl = j >= 10 ? (Button)this.FindName("button_0" + i + "_" + j) : (Button)this.FindName("button_0" + i + "_0" + j);

                    lbl.Background = new SolidColorBrush(Colors.White);
                }
            }

            return true; // lesender Zugriff
        }

        private bool DrawPattern(int pos)
        {
            // source: https://mycsharp.de/forum/threads/33113/faq-controls-von-thread-aktualisieren-lassen-control-invoke-dispatcher-invoke
            if (!this.Dispatcher.CheckAccess())
            { // Wenn Invoke nötig ist, ...
              // dann rufen wir die Methode selbst per Invoke auf

                return (bool)this.Dispatcher.Invoke((Func<int, bool>)MarkLocator, pos);
                // hier ist immer ein return (oder alternativ ein else) erforderlich.
                // Es verhindert, dass der folgende Code im Worker-Thread ausgeführt wird.
            }
            // eigentliche Zugriffe; laufen jetzt auf jeden Fall im GUI-Thread
            //progressBar.Value = percent; // schreibender Zugriff

            for (int i = 0; i < TotalTracks; i++)
            {
                for (int j = 0; j < TotalSteps; j++)
                {
                    Button lbl = j >= 10 ? (Button)this.FindName("button_0" + i + "_" + j) : (Button)this.FindName("button_0" + i + "_0" + j);

                    if (_preset.Pattern[_activePattern].Grid[i, j] > 0)
                    {
                        lbl.Background = new SolidColorBrush(Colors.DarkRed);
                    }
                    else
                    {
                        lbl.Background = new SolidColorBrush(Colors.LightGray);
                    }
                }
            }

            return true; // lesender Zugriff
        }

        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            double[] dataX = new double[] { 1, 2, 3, 4, 5 };
            double[] dataY = new double[] { 1, 4, 9, 16, 25 };

            Button btn = (Button)sender;

            if (btn != null)
            {
                string[] split = btn.Name.Split('_');

                int row = Convert.ToInt32(split[1]);
                int col = Convert.ToInt32(split[2]);

                // toggle the value
                if (_preset.Pattern[_activePattern].Grid[row, col] == 0)
                {
                    _preset.Pattern[_activePattern].Grid[row, col] = _dynamicValue;
                    btn.Background = new SolidColorBrush(Colors.Red);
                }
                else
                {
                    _preset.Pattern[_activePattern].Grid[row, col] = 0;
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

        private void PlayButtonClick(object sender, RoutedEventArgs e)
        {
            if (_state != PlayerState.Playing)
            {
                _state = PlayerState.Playing;

                _worker.WorkerReportsProgress = true;
                _worker.DoWork += AudioWorker!;
                _worker.RunWorkerAsync(PatternPosition);
            }
            else
            {
                _state = PlayerState.Pause;
            }
        }

        private void StopButtonClick(object sender, RoutedEventArgs e)
        {
            _state = PlayerState.Stop;
            PatternPosition = 0;
        }

        private void ClearButtonClick(object sender, RoutedEventArgs e)
        {
            // issue: reset button color

            _preset.Pattern[_activePattern] = new Pattern(4, 16);
            ClearButtonState(0);
        }

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TempoBPM = Convert.ToInt32(tempoBox.Text);
            }
        }

        private void ChooseSampleClick(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.FileName = "Sample"; // Default file name
            dialog.DefaultExt = ".wav"; // Default file extension
            dialog.Filter = "Audio samples (.wav)|*.wav"; // Filter files by extension

            // Show open file dialog box
            bool? result = dialog.ShowDialog();

            // Process open file dialog box results
            if (result.HasValue)
            {
                // Open document
                string filename = dialog.FileName;

                var btn = (Button)sender;

                if (btn != null)
                {
                    string[] split = btn.Name.Split('_');

                    int col = Convert.ToInt32(split[2]);

                    switch (col)
                    {
                        case 0:
                            _preset.Pattern[_activePattern].Sounds[0].SamplePath = @filename;
                            _preset.Pattern[_activePattern].Sounds[0].SampleName = dialog.SafeFileName;
                            _sound1 = new CachedSound(@filename);
                            break;
                        case 1:
                            _preset.Pattern[_activePattern].Sounds[1].SamplePath = @filename;
                            _preset.Pattern[_activePattern].Sounds[1].SampleName = dialog.SafeFileName;
                            _sound2 = new CachedSound(@filename);
                            break;
                        case 2:
                            _preset.Pattern[_activePattern].Sounds[2].SamplePath = @filename;
                            _preset.Pattern[_activePattern].Sounds[2].SampleName = dialog.SafeFileName;
                            _sound3 = new CachedSound(@filename);
                            break;
                        case 3:
                            _preset.Pattern[_activePattern].Sounds[3].SamplePath = @filename;
                            _preset.Pattern[_activePattern].Sounds[3].SampleName = dialog.SafeFileName;
                            _sound4 = new CachedSound(@filename);
                            break;

                        default:
                            throw new ArgumentException();
                    }

                    btn.Content = dialog.SafeFileName;
                }
            }  
        }

        private void dynamicBox_KeyDown(object sender, KeyEventArgs e)
        {
            _dynamicValue = Convert.ToInt32(dynamicBox.Text);
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void buttonChangeValue_Click(object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;

            if (btn != null)
            {
                string[] split = btn.Name.Split('_');

                string prop = split[1];
                string val = split[2];

                switch (prop)
                {
                    case "pattern":

                        int oldPattern = _activePattern;

                        if (val == "inc")
                        {
                            if (_activePattern != 8) _activePattern++;
                            else _activePattern = 0;
                        }
                        else
                        {
                            if (_activePattern != 0) _activePattern--;
                            else _activePattern = 8;
                        }

                        _sound1 = new CachedSound(_preset.Pattern[_activePattern].Sounds[0].SamplePath);
                        _sound2 = new CachedSound(_preset.Pattern[_activePattern].Sounds[1].SamplePath);
                        _sound3 = new CachedSound(_preset.Pattern[_activePattern].Sounds[2].SamplePath);
                        _sound4 = new CachedSound(_preset.Pattern[_activePattern].Sounds[3].SamplePath);

                        btn_SampleDialog_00.Content = _preset.Pattern[_activePattern].Sounds[0].SamplePath;
                        btn_SampleDialog_01.Content = _preset.Pattern[_activePattern].Sounds[1].SamplePath;
                        btn_SampleDialog_02.Content = _preset.Pattern[_activePattern].Sounds[2].SamplePath;
                        btn_SampleDialog_03.Content = _preset.Pattern[_activePattern].Sounds[3].SamplePath;

                        Button released = (Button)this.FindName("btn_pattern_0" + oldPattern);
                        released.Background = new SolidColorBrush(Colors.Black);

                        Button target = (Button)this.FindName("btn_pattern_0" + _activePattern);
                        target.Background = new SolidColorBrush(Colors.DarkRed);

                        DrawPattern(0);

                        patternBox.Text = _activePattern.ToString();
                        break;

                    case "dynamic":

                        if (val == "inc")
                        {
                            if (_dynamicValue < 127) _dynamicValue++;
                        }
                        else
                        {
                            if (_dynamicValue > 0) _dynamicValue--;
                        }

                        dynamicBox.Text = _dynamicValue.ToString();
                        break;

                    case "tempo":

                        if (val == "inc")
                        {
                            if (TempoBPM < 200) TempoBPM++;
                        }
                        else
                        {
                            if (TempoBPM > 0) TempoBPM--;
                        }

                        tempoBox.Text = TempoBPM.ToString();
                        break;

                    default:
                        throw new ArgumentException();
                }
            }
        }

        private void changePattern_Click(object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;

            if (btn != null)
            {
                int oldPattern = _activePattern;

                string[] split = btn.Name.Split('_');

                int num = Convert.ToInt32(split[2]);

                Button released = (Button)this.FindName("btn_pattern_0" + _activePattern);
                released.Background = new SolidColorBrush(Colors.Black);

                Button target = (Button)this.FindName("btn_pattern_0" + num);
                target.Background = new SolidColorBrush(Colors.DarkRed);

                patternBox.Text = num.ToString();
                _activePattern = Convert.ToInt32(num);

                _sound1 = new CachedSound(_preset.Pattern[_activePattern].Sounds[0].SamplePath);
                _sound2 = new CachedSound(_preset.Pattern[_activePattern].Sounds[1].SamplePath);
                _sound3 = new CachedSound(_preset.Pattern[_activePattern].Sounds[2].SamplePath);
                _sound4 = new CachedSound(_preset.Pattern[_activePattern].Sounds[3].SamplePath);

                btn_SampleDialog_00.Content = _preset.Pattern[_activePattern].Sounds[0].SamplePath;
                btn_SampleDialog_01.Content = _preset.Pattern[_activePattern].Sounds[1].SamplePath;
                btn_SampleDialog_02.Content = _preset.Pattern[_activePattern].Sounds[2].SamplePath;
                btn_SampleDialog_03.Content = _preset.Pattern[_activePattern].Sounds[3].SamplePath;

                DrawPattern(0);

                Debug.WriteLine("active pattern: " + _activePattern);

            }
        }
    }
}
