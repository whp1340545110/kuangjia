using UnityEngine;
using UnityEngine.Sprites;
using UnityEngine.UI;
 
//自定义圆形图片
[RequireComponent(typeof(Image))]
public class ImgRound : BaseMeshEffect
{
    //圆行由多少个三角形组成
    public int segements = 50;

    private RectTransform _rectTransform;
    private Image _image;

    protected override void Awake()
    {
        
        base.Awake();
        _rectTransform = GetComponent<RectTransform>();
        _image = GetComponent<Image>();
    }

    //这个方法，会将图片的顶点，三角信息，发送到GPU。这里重写自己构建一个圆
    public override void ModifyMesh(VertexHelper vh)
    {
        vh.Clear();
        //得到图片的宽高
        float width = _rectTransform.rect.width;
        float height = _rectTransform.rect.height;
        //得到UV
        Vector4 uv = (_image != null && _image.overrideSprite != null) ? DataUtility.GetOuterUV(_image.overrideSprite) : Vector4.zero;
        
        float uvWidth = uv.z - uv.x;
        float uvHeight = uv.w - uv.y;
 
        //UV中心点
        Vector2 uvCenter = new Vector2(uvWidth * 0.5f + uv.x, uvHeight * 0.5f + uv.y);
        //UV和长度之间的比值
        Vector2 convertRatio = new Vector2(uvWidth / width, uvHeight / height);
        //三角形的弧度
        float radian = (2 * Mathf.PI) / segements;
        //半径
        float radius = width * 0.5f;
 
        //这里用一个中间值保存，再赋值，是因为下面位置和UV的赋值是同时的，不能用position对UV进行赋值
        Vector2 tempVec = Vector2.zero;
        //圆的中心点
        UIVertex origin = new UIVertex();
        origin.color = _image != null ? _image.color : Color.white;
        origin.position = tempVec;
        origin.uv0 = new Vector2(tempVec.x * convertRatio.x + uvCenter.x, tempVec.y * convertRatio.y + uvCenter.y);
        vh.AddVert(origin);
 
        //计算其他的点
        float vertCount = segements + 1;  //点的个数是三角个数+1
        float angle = 0;
        for (int i = 0; i < vertCount; i++)
        {
            float xTemp = Mathf.Cos(angle) * radius;
            float yTemp = Mathf.Sin(angle) * radius;
            angle += radian;
 
            UIVertex originTemp = new UIVertex();
            originTemp.color = _image != null ? _image.color : Color.white;
            tempVec = new Vector2(xTemp, yTemp);
            originTemp.position = tempVec;
            originTemp.uv0 = new Vector2(tempVec.x * convertRatio.x + uvCenter.x, tempVec.y * convertRatio.y + uvCenter.y);
            vh.AddVert(originTemp);
        }
 
        //设置三角形,点由顺时针方向组成三角形，组成的图形才能看见
        int id = 1;
        for (int i = 0; i < segements; i++)
        {
            vh.AddTriangle(id, 0, id + 1);
            id++;
        }
    }
}