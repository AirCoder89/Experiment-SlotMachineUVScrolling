using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpinBtn : MonoBehaviour
{
    [SerializeField] private Text label;
    [Header("Spin View")]
    [SerializeField] private Sprite spinSp;
    [SerializeField] private Color spinColor;
    [Header("Stop View")]
    [SerializeField] private Sprite stopSp;
    [SerializeField] private Color stopColor;

    private SlotMachine _parent;
    private bool _isSpin;
    private Image _background;
    
    public void Initialize(SlotMachine parent)
    {
        this._parent = parent;
        _background = GetComponent<Image>();
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        if (_isSpin)
        {
            _parent.StopSpin();
            SetViewAsSpin();
        }
        else
        {
            _parent.StartSpin();
            SetViewAsStop();
        }
    }

    public void SetViewAsSpin()
    {
        _isSpin = false;
        _background.sprite = this.spinSp;
        label.color = this.spinColor;
        label.text = "Spin";
    }

    private void SetViewAsStop()
    {
        _isSpin = true;
        _background.sprite = this.stopSp;
        label.color = this.stopColor;
        label.text = "Stop";
    }
    
}
