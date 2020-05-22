using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UvAnimation : MonoBehaviour
{
    private Material _material;
    private Image _image;
    private SlotMachine _parent;
    private int _index;
    
    public void Initialize(SlotMachine parent, int index)
    {
        this._parent = parent;
        this._index = index;
        this._image = GetComponent<Image>();
        this._material = this._image.material;
        SetVisibility(false);
    }

    public void SetLevel(BlurLevel level)
    {
        this._image.sprite = this._parent.GetBlurrySprite(level, this._index);
    }

    public void SetShaderSettings(float blurAmount, float speed, Vector2 tiling)
    {
        if(!_material) return;
        _material.SetVector("_Speed",new Vector2(0,speed));
        _material.SetFloat("_BlurAmount", blurAmount);
        _material.SetVector("_Tiling",tiling);
    }

    public void SetVisibility(bool status)
    {
        this.gameObject.SetActive(status);
    }
    
}
