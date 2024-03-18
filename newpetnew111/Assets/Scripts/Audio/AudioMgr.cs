using PFramework;
using Singleton;

namespace PVM
{
    public class AudioMgr : CSharpSingleton<AudioMgr>
    {
        private string mPrePath = "Audio/";
        public void PlaySound(int _value)
        {
            Peach.AudioMgr.PlaySound(mPrePath + _value);
        }
        public void PlayMusic(int _value)
        {
            Peach.AudioMgr.PlayMusic(mPrePath + _value);
        }
    }    
}

