﻿using NAudio.Wave;
using SharpLoops.Audio;

namespace SharpLoops.AudioPlayground
{
    internal class Program
    {
        private const string PATH_KICK1 = @"C:\Recording\Samples\Samples2019\LoopLords 80s Drums Vol.1\BOSS DR-220E\DR220A_KICK.wav";
        private const string PATH_SNARE1 = @"C:\Recording\Samples\Samples2019\LoopLords 80s Drums Vol.1\BOSS DR-220E\DR220A_SNARE.wav";
        private const string PATH_HAT1 = @"C:\Recording\Samples\Samples2019\LoopLords 80s Drums Vol.1\BOSS DR-220E\DR220A_CL HAT.wav";
        private const string PATH_HAT2 = @"C:\Recording\Samples\Samples2019\LoopLords 80s Drums Vol.1\BOSS DR-220E\DR220A_KICK.wav";
        private const string PATH_TOM1 = @"C:\Recording\Samples\Samples2019\LoopLords 80s Drums Vol.1\BOSS DR-220E\DR220A_LOW TOM.wav";
        private const string PATH_TOM2 = @"C:\Recording\Samples\Samples2019\LoopLords 80s Drums Vol.1\BOSS DR-220E\DR220A_KICK.wav";
        private const string PATH_CRASH1 = @"C:\Recording\Samples\Samples2019\LoopLords 80s Drums Vol.1\BOSS DR-220E\DR220A_CRASH.wav";

        static void Main(string[] args)
        {


            while (true)
            {

                


                using (var audioFile = new AudioFileReader(PATH_KICK1))
                using (var outputDevice = new WaveOutEvent())
                {
                    outputDevice.Init(audioFile);
                    outputDevice.Play();
                    while (outputDevice.PlaybackState == PlaybackState.Playing)
                    
                    {
 
                        Thread.Sleep(200);
                    }
                } 
            }

            // on startup:
            var zap = new CachedSound(PATH_KICK1);
            //var boom = new CachedSound(PATH_SNARE1);

            // later in the app...

            while (true)
            {
                AudioPlaybackEngine.Instance.PlaySound(zap);
                Thread.Sleep(200);
            }


            





            //AudioPlaybackEngine.Instance.PlaySound(boom);
            AudioPlaybackEngine.Instance.PlaySound(PATH_HAT1);

            Thread.Sleep(1000);

            // later in the app...
            AudioPlaybackEngine.Instance.PlaySound(zap);
            //AudioPlaybackEngine.Instance.PlaySound(boom);
            AudioPlaybackEngine.Instance.PlaySound(PATH_HAT1);

            Thread.Sleep(1000);

            // later in the app...
            AudioPlaybackEngine.Instance.PlaySound(zap);
            //AudioPlaybackEngine.Instance.PlaySound(boom);
            AudioPlaybackEngine.Instance.PlaySound(PATH_HAT1);

            Thread.Sleep(1000);

            // on shutdown
            AudioPlaybackEngine.Instance.Dispose();
        }
    }
}