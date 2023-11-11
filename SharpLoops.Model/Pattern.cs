namespace SharpLoops.Model
{
    public class Pattern
    {
        public Pattern(int tracks, int steps) 
        { 
            Tracks = tracks;
            Steps = steps;
            Grid = new int[Tracks, Steps];
        }

        public int Tracks { get; private set; }
        public int Steps { get; private set; }

        public int[,] Grid
        {
            get;
            private set;
        }

        public bool PatternHasChanged { get; private set; }

        public void UpdateNote(int track, int step, int volume)
        {
            Grid[track, step] = volume;
        }
    }
}