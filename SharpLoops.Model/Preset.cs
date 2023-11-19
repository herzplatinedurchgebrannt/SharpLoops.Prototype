using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpLoops.Model
{
    public class Preset
    {
        private int _activePattern;

        public Preset() 
        {
            Pattern = new List<Pattern>();
            Path = "";
        }
        public List<Pattern> Pattern { get; private set; }
        public void AddPattern(Pattern pattern)
        {
            if (Pattern.Count < 9) Pattern.Add(pattern);
        }
        public string Path { get; set; }

    }
}
