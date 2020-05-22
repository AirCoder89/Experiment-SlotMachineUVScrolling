using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public enum BlurLevel
{
    None, Low, Medium, High
}

public enum SlotType
{
    Red,Blue,Green,Yellow,Heart,Diamond,Energy
}

[System.Serializable]
public struct BlurryResource
{
    public BlurLevel level;
    public Sprite[] resource;
}

[System.Serializable]
public struct SlotResource
{
    public SlotType type;
    [PreviewField] public Sprite background;
    [PreviewField] public Sprite icon;
}

public class SlotMachine : MonoBehaviour
{
    [TabGroup("Elements")] public List<SlotColumn> columns;
    [TabGroup("Elements")] public List<UvAnimation> uvColumns;
    [TabGroup("Elements")] public HorizontalLayoutGroup slotLayout;
    [TabGroup("Elements")] public SpinBtn spinBtn;
    
    [TabGroup("Resources")][TableList] public List<SlotResource> resources;
    [TabGroup("Resources")] public List<BlurryResource> blurryResources;

    [TabGroup("Spin Settings")] [Title("Visual Options")]
    [SerializeField][EnumToggleButtons] private BlurLevel blurLevel;

    [TabGroup("Spin Settings")] [Range(0.8f, 1)] [SerializeField]
    private float shaderBlurAmount;
    [TabGroup("Spin Settings")] [SerializeField]
    private Vector2 shaderSlotTiling;
    
    [TabGroup("Spin Settings")] [Range(0, 10)] [SerializeField]
    private float spinSpeed;

    [TabGroup("Spin Settings")]
    public Ease easeIn;
    [TabGroup("Spin Settings")] [Range(0, 5)]
    public float speedIn;
    [TabGroup("Spin Settings")] 
    public float startYPosIn;
    [TabGroup("Spin Settings")]
    public float targetYPosIn;
    
    [TabGroup("Spin Settings")]
    public Ease easeOut;
    [TabGroup("Spin Settings")] [Range(0, 5)]
    public float speedOut;
    [TabGroup("Spin Settings")]
    public float startYPosOut;
    [TabGroup("Spin Settings")]
    public float targetYPosOut;
    
    [TabGroup("Spin Settings")] [Title("Behaviours Options")][MinMaxSlider(0,10,true)]
    public Vector2 spinDuration;

    [TabGroup("Spin Settings")] [SerializeField]
    private float delayAmongSlots;

    [HideInInspector] public float randomSpinDuration;
    
    
    void Start()
    {
        spinBtn.Initialize(this);
        
        SlotColumn.OnSpinComplete += OnColumnSpinComplete;
        
        for (var i = 0; i < columns.Count; i++)
        {
            var index = i;
            columns[i].Initialize(this, index);
            columns[i].SetRandom();
        }
        
        for (var i = 0; i < uvColumns.Count; i++)
        {
            var index = i;
            uvColumns[i].Initialize(this,index);
            uvColumns[i].SetLevel(this.blurLevel);
            uvColumns[i].SetShaderSettings(this.shaderBlurAmount, this.spinSpeed, this.shaderSlotTiling);
        }
    }

    private void OnDestroy()
    {
        SlotColumn.OnSpinComplete -= OnColumnSpinComplete;
    }

    private void OnColumnSpinComplete(int index)
    {
        print($"Column [{index}] Complete");
        if (index == columns.Count - 1)
        {
            spinBtn.SetViewAsSpin();
            slotLayout.enabled = true;
            print("Matching Time !!");
        }
    }

    public void StartSpin()
    {
        slotLayout.enabled = false;
        randomSpinDuration = Random.Range(this.spinDuration.x, this.spinDuration.y);
        SpinColumn(0);
    }

    public void StopSpin()
    {
        StopAllCoroutines();
        foreach (var col in columns)
        {
            col.StopSpin();
        }
    }
    

    [Button("Show UV", ButtonSizes.Medium)]
    private void ShowUV()
    {
        slotLayout.enabled = false;
        for (var i = 0; i < columns.Count; i++)
        {
            columns[i].SetVisibility(false);
            uvColumns[i].SetVisibility(true);
        }
    }
    
    [Button("Hide UV", ButtonSizes.Medium)]
    private void HideUV()
    {
        for (var i = 0; i < columns.Count; i++)
        {
            columns[i].SetVisibility(true);
            uvColumns[i].SetVisibility(false);
        }
        slotLayout.enabled = true;
    }
    private void SpinColumn(int index)
    {
        if (index < columns.Count)
        {
            this.columns[index].StartSpin();
            index++;
            StartCoroutine(WaitAndSpinNext(index));
        }
        else
        {
            print("on Spin");
        }
    }

    private IEnumerator WaitAndSpinNext(int nextIndex)
    {
        yield return new WaitForSeconds(delayAmongSlots);
        SpinColumn(nextIndex);
    }
    
    public void UVAnimationVisibility(int index, bool status)
    {
        uvColumns[index].SetVisibility(status);
    }
    
    [TabGroup("Spin Settings")][Button("Apply Changes",ButtonSizes.Medium)]
    private void UpdateSpinSettings()
    {
        foreach (var uv in uvColumns)
        {
            uv.SetLevel(this.blurLevel);
            uv.SetShaderSettings(this.shaderBlurAmount, this.spinSpeed, this.shaderSlotTiling);
        }
    }
    
    public Sprite GetBlurrySprite(BlurLevel level, int index)
    {
        return this.blurryResources.FirstOrDefault(r => r.level == level).resource[index];
    }

    public SlotResource GetSlotByType(SlotType type)
    {
        return this.resources.FirstOrDefault(r => r.type == type);
    }
}
