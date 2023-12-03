using System;
using Un4seen.Bass;

namespace SharpLoops.Audio.BASS
{
    public static class BassTest
    {
        public static void PlayTest()
        {
            // init BASS using the default output device
            if (Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero))
            {

                int sample = Bass.BASS_SampleLoad(@"C:\\clap.wav", 0, 0, 1, BASSFlag.BASS_DEFAULT);
                int sample2 = Bass.BASS_SampleLoad(@"C:\\hat.wav", 0, 0, 1, BASSFlag.BASS_DEFAULT);

                // create a stream channel from a file
                if (sample != 0)
                {
                    int channel = Bass.BASS_SampleGetChannel(sample, 0);
                    int channel2 = Bass.BASS_SampleGetChannel(sample2, 0);
                    // play the stream channel
                    Bass.BASS_ChannelPlay(sample, false);
                    Bass.BASS_ChannelPlay(sample2, false);
                    
                }
                else
                {
                    // error creating the stream
                    Console.WriteLine("Stream error: {0}", Bass.BASS_ErrorGetCode());
                }

                // wait for a key
                Console.WriteLine("Press any key to exit");

                Thread.Sleep(1000);

                // free the stream
                Bass.BASS_StreamFree(sample);
                Bass.BASS_StreamFree(sample2);
                // free BASS
                Bass.BASS_Free();
            }
        }
    }
}
