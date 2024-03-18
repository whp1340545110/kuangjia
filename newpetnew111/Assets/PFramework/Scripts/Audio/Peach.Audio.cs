namespace PFramework
{
    using PFramework.Audio;
    using UnityEngine;
    public static partial class Peach
    {
        public static AudioManager AudioMgr => AudioManager.Instance;

        public static void PlayMusic(string path, bool loop = true)
        {
            AudioMgr.PlayMusic(path, loop);
        }

        public static void StopMusic()
        {
            AudioMgr.StopMusic();
        }

        public static void PlayAmbience(string path)
        {
            AudioMgr.PlayAmbience(path);
        }

        public static void PlaySound(string path)
        {
            AudioMgr.PlaySound(path);
        }
    }
}
