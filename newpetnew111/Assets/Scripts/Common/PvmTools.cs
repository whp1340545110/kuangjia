using PFramework;
using TMPro;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace PVM
{
    public static class PvmTools
    {
        
        public static void CopyText2Clipboard(string text)
        {
            GUIUtility.systemCopyBuffer = text;
        }

        
        /// <summary>
        /// 转换成多少 天:小时:分
        /// </summary>
        /// <param name="ticks"></param>
        /// <returns></returns>
        public static string ConvertTimeShowStr(long ticks)
        {
            var sMin = 10000000L * 60;
            var sHour = sMin * 60;
            var sDay = sHour * 24;
            if (ticks < sMin)
            {
                return "小于1分钟";
            }

            if (ticks < sHour)
            {
                return $"{ticks / sMin}分钟";
            }

            long hour;
            if (ticks < sDay)
            {
                hour = ticks / sHour;
                return $"{hour}小时{(ticks - hour * sHour) / sMin}分钟";
            }

            var day = ticks / sDay;
            hour = (ticks - sDay * day) / sHour;
            var sec = (ticks % sHour) / sMin;
            return $"{day}天{hour}小时{sec}分钟";
        }

        public static async void LoadNetImage(this Image image, string url)
        {
            if (string.IsNullOrEmpty(url)) return;
            var userAvatarTex = await Peach.NetResMgr.LoadTexture(url);
            if (image == null || userAvatarTex == null) return;
            image.sprite = Sprite.Create(userAvatarTex,
                new Rect(0, 0, userAvatarTex.width, userAvatarTex.height), new Vector2(0.5f, 0.5f));
        }

        public static async void LoadSprite(this Image image, string atlas, string name)
        {
            var spAltas = await Peach.LoadAsync<SpriteAtlas>(atlas);
            if (spAltas is null ) return;
            var sprite = spAltas.GetSprite(name);
            if (image != null)
            {
                image.sprite = sprite;
                image.SetNativeSize();
            }
        }
        public static async void LoadSprite1(this Image image, string atlas, string name)
        {
            var spAltas = await Peach.LoadAsync<SpriteAtlas>(atlas);
            if (spAltas is null) return;
            var sprite = spAltas.GetSprite(name);
            if (image != null)
            {
                image.sprite = sprite;
               
            }
        }

        public static async void LoadGameObject(this GameObject game, string atlas)
        {
            var go = await Peach.LoadAsync<GameObject>(atlas);
            if (go is null) return;
            game = go;
        }

        public static async void LoadFontMaterial(this TextMeshProUGUI text, string name, string parentPath = "Common/Font")
        {
            text.fontMaterial = await Peach.LoadAsync<Material>($"{parentPath}/{name}");
        }

        public static void TextString(this Text game, string atlas,string color)
        {
           
            string tip = "";
            if (atlas.Length>=5&&atlas.Length<8)
            {
                tip= atlas.Insert(atlas.Length - 3, ",");
            }
          
            game.text = $"<color=#{color}>{tip}</color>" ;
        }

        public static string NumString(string atlas) {
            string tip = atlas;
            if (atlas.Length >= 5 && atlas.Length < 8)
            {
                tip = atlas.Insert(atlas.Length - 3, ",");

            }
            if (tip.Length>=8)
            {
                tip = tip.Insert(atlas.Length - 6, ",");
            }
            return tip;
        }

        public static void TextString(this Text game, string atlas) {
            string tip = atlas;
            if (atlas.Length >= 5 && atlas.Length < 8)
            {
                tip = atlas.Insert(atlas.Length - 3, ",");

            }
            game.text = $"{tip}";
        }

        public static void SetColor(this Image color, string name) {
            Color nowColor;
            ColorUtility.TryParseHtmlString($"#{name}", out nowColor);
            color.color= nowColor;
        }

        public static RectTransform TipTrType(this RectTransform transform, bool isTr, int type)
        {
            if (isTr)
            {
                transform.SetActive(true);
                if (type == 1)
                {
                    transform.GetComponent<UnityEngine.UI.Image>().enabled = false;
                    transform.GetChild(0).SetActive(true);
                }
                else
                {
                    transform.GetComponent<UnityEngine.UI.Image>().enabled = true;
                    transform.GetChild(0).SetActive(false);
                }

            }
            else
            {
                transform.SetActive(false);
            }
            return transform;

        }
    }
    
    
}