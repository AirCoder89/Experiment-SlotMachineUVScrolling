using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[System.Serializable]
public class SlotData
{
   public SlotType type;
   public Image background;
   public Image icon;
   public int index;
}

public class SlotColumn : MonoBehaviour
{
   public delegate void SlotEvents(int index);
   public static event SlotEvents OnSpinComplete;
   
   [SerializeField] private List<SlotData> slots;
   
   private SlotMachine _parent;
   private int _index;
   private RectTransform _rt;
   private bool _isComplete;
   
   public void Initialize(SlotMachine parent, int index)
   {
      _rt = GetComponent<RectTransform>();
      this._parent = parent;
      this._index = index;
   }

   public void SetRandom()
   {
      var allTypes = Enum.GetValues(typeof(SlotType));
      
      foreach (var slot in slots)
      {
         var randomType = (SlotType)(Random.Range(0, allTypes.Length));
         var slotResource = _parent.GetSlotByType(randomType);
         slot.background.sprite = slotResource.background;
         slot.icon.sprite = slotResource.icon;
         slot.type = slotResource.type;
      }
   }

   public void StartSpin()
   {
      _isComplete = false;
      this._rt.anchoredPosition = new Vector2(this._rt.anchoredPosition.x , _parent.startYPosOut);
      _parent.UVAnimationVisibility(this._index, true);
      this._rt.DOAnchorPosY(_parent.targetYPosOut, _parent.speedOut).SetEase(_parent.easeOut).OnComplete(() =>
      {
         SetRandom();
         StopCoroutine("WaitToEndSpin");
         StartCoroutine(WaitToEndSpin(_parent.randomSpinDuration));
      });
   }

   public void StopSpin()
   {
      if(_isComplete) return;
      StopCoroutine("WaitToEndSpin");
      this._rt.anchoredPosition = new Vector2(this._rt.anchoredPosition.x , _parent.startYPosIn);
      this._rt.DOAnchorPosY(_parent.targetYPosIn, _parent.speedIn).SetEase(_parent.easeIn).OnComplete(() =>
      {
         _isComplete = true;
         _parent.UVAnimationVisibility(this._index, false);
         OnSpinComplete?.Invoke(this._index);
      });
   }
   
   private IEnumerator WaitToEndSpin(float duration)
   {
      yield return new WaitForSeconds(duration);
      StopSpin();
   }

   public void SetVisibility(bool status)
   {
      this.gameObject.SetActive(status);
   }
   
}
