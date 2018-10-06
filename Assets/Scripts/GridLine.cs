using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GridLine : PoolObject
{
    [SerializeField] private Image Image;
    [SerializeField] private RectTransform Trans;
    public override void PoolRecycle()
    {
        base.PoolRecycle();
        Trans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0.05f);
        Trans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);
        Trans.anchoredPosition = new Vector2(0, 0);
    }

    public enum Orient
    {
        Horizontal,
        Vertical,
    }

    public void Initialize(Orient orient, Vector2 startPoint, float width, float length)
    {
        Trans.anchoredPosition = startPoint;
        if (orient == Orient.Horizontal)
        {
            Trans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, width);
            Trans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, length);
        }
        else
        {
            Trans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            Trans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, length);
        }
    }
}
