using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    [SerializeField] Vector2 enablePos;
    [SerializeField] Vector2 disablePos;
    [SerializeField] RectTransform rectTransform;
    [SerializeField] Transition[] hoverTransitions;
    [SerializeField] Transition[] selectTransitions;
    public bool IsEnable
    {
        get => rectTransform.gameObject.activeSelf;
    }

    public void Enable()
    {
        rectTransform.gameObject.SetActive(true);
        rectTransform.DOAnchorPos(enablePos, 0.25f).SetEase(Ease.OutQuad);
    }

    public void Disable()
    {
        rectTransform.DOAnchorPos(disablePos, 0.25f).SetEase(Ease.InOutQuad).OnComplete(() =>
        {
            rectTransform.gameObject.SetActive(false);
        });
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        foreach (Transition item in hoverTransitions)
        {
            if (item.highLighted != null)
            {
                item.target.sprite = item.highLighted;
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        foreach (Transition item in hoverTransitions)
        {
            if (item.normal != null)
            {
                item.target.sprite = item.normal;
            }
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        foreach (Transition item in selectTransitions)
        {
            if (item.highLighted != null)
            {
                item.target.sprite = item.highLighted;
            }
        }
    }

    public void OnDeselect(BaseEventData eventData)
    {
        foreach (Transition item in selectTransitions)
        {
            if (item.normal != null)
            {
                item.target.sprite = item.normal;
            }
        }
    }
}

[Serializable]
public class Transition
{
    public Image target;
    public Sprite normal;
    public Sprite highLighted;
}
