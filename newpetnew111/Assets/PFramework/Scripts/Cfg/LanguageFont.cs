namespace PFramework.Cfg
{
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;

    [RequireComponent(typeof(TMP_Text))]
    public class LanguageFont : LanguageObserver
    {
        private const string FontMaterialSplit = " - ";

        private static SystemLanguage _fontLanguage;
        private static TMP_FontAsset _font;

        private static Dictionary<string, Material> _fontMaterials = new Dictionary<string, Material>();

        private static TMP_FontAsset GetLanguageFont(SystemLanguage language)
        {
            if (_fontLanguage != language || !_font)
            {
                _fontLanguage = language;
                var fontPath = Peach.CfgMgr.Settings.localFontsLoadPath + "/" + language.ToString();
                var fonts = Peach.LoadAllSync<TMP_FontAsset>(fontPath);
                if (fonts.Length > 0)
                {
                    _font = fonts[0];
                }
                _fontMaterials.Clear();
                var materials = Peach.LoadAllSync<Material>(fontPath);
                foreach (var mat in materials)
                {
                    var splits = mat.name.Split(new string[] { FontMaterialSplit }, System.StringSplitOptions.RemoveEmptyEntries);
                    if (splits.Length > 1)
                    {
                        _fontMaterials.Add(splits[1], mat);
                    }
                    else
                    {
                        _fontMaterials.Add(string.Empty, mat);
                    }
                }
            }
            if (!_font)
            {
                Debug.LogError("No Such Language Font");
            }
            return _font;
        }

        private static Material GetLanguageMaterial(string materialName)
        {
            if (_fontMaterials.ContainsKey(materialName))
            {
                return _fontMaterials[materialName];
            }
            return _fontMaterials[string.Empty];
        }

        protected TMP_Text tmpText;

        public FontWeight fontWeight = FontWeight.Regular;


        private string _fontMaterialName = string.Empty;

        protected override void OnObserverAwake()
        {
            tmpText = GetComponent<TMP_Text>();
            var fontMat = tmpText.fontSharedMaterial;
            var splits = fontMat.name.Split(new string[] { FontMaterialSplit }, System.StringSplitOptions.RemoveEmptyEntries);
            if (splits.Length > 1)
            {
                _fontMaterialName = splits[1];
            }
        }

        protected override void OnLanguageChanged(SystemLanguage language)
        {

            var font = GetLanguageFont(language);
            if (font)
            {
                tmpText.font = font;
                tmpText.fontSharedMaterial = GetLanguageMaterial(_fontMaterialName);
            }
            tmpText.fontWeight = fontWeight;

        }

#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            if (isActiveAndEnabled == true)
            {
                GetComponent<TMP_Text>().fontWeight = fontWeight;
            }
        }
#endif
    }
}
