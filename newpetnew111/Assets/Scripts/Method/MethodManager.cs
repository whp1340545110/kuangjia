using PFramework;
using Singleton;
using UnityEngine;
using UnityEngine.U2D;

namespace PVM
{
    public class MethodManager : CSharpSingleton<MethodManager>
    {
        public SpriteAtlas achievement = new SpriteAtlas();
        public MethodManager()
        {
        }
        #region 初始化,加载图集

        public async void InitSprite( )
        {
            achievement = await Peach.LoadAsync<SpriteAtlas>("Achievement/achieve");
        }
        public Sprite SetSprite(SpriteAtlas sprite,string name)
        {
           return sprite.GetSprite(name);
        }
        #endregion
    }
}

