using UnityEngine;
using UnityEngine.UI;

public static class UIExtend
{
    //Image置灰
    static public void setGray(this Image image, bool isGray)
    {
        Material mat;
        if (isGray)
        {
            mat = new Material(Shader.Find("Sprites/UIGray"));
            image.material = mat;
        }
        else
        {
            //mat = new Material(Shader.Find("Sprites/Default"));
            mat = null;
        }
        image.material = mat;
    }
    //text置灰
    static public void setGray(this Text text, bool isGray)
    {
        Material mat;
        if (isGray)
        {
            mat = new Material(Shader.Find("Sprites/UIGray"));
        }
        else
        {
            //mat = new Material(Shader.Find("Sprites/Default"));
            mat = null;
        }
        text.material = mat;
    }
}
