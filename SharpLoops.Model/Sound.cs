using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpLoops.Model
{
    public class Sound
    {
        public Sound(int channelId, string samplePath, string sampleName)
        {
            ChannelId = channelId;
            SamplePath = samplePath;
            SampleName = sampleName;
        }

        public int ChannelId { get; set; }
        public string SamplePath { get; set; }
        public string SampleName { get; set; }


    }
}
