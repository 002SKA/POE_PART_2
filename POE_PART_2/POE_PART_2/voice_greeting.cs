using System;
using System.IO;
using System.Media;
using System.Windows.Shapes;

namespace POE_PART_2
{
    public class voice_greeting
    {
        public voice_greeting()
        {
            greet();
        }
        
        private void greet()
        {
            //Get base directory of app
            string paths = AppDomain.CurrentDomain.BaseDirectory;

            //Locate wav file in output folder
            string fullpath = paths.Replace(@"bin\Debug\", "voice.wav");

            //Play audio
            SoundPlayer voice_play = new SoundPlayer(fullpath);
            voice_play.Load();
            voice_play.Play();
        }
    }
}