using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SaveSlotComponent : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] Image saveImage;
    [SerializeField] TMP_Text saveTitleText;
    [SerializeField] TMP_Text playerNameText;
    [SerializeField] TMP_Text playChapterText;
    [SerializeField] TMP_Text playTimeText;

    [SerializeField] private Sprite emptySlotImage;

    public delegate void ClickEventHandler(SaveSlotComponent clickedSlot);
    public event ClickEventHandler OnClick;

    public void SetSaveSlot(Sprite sprite, string title, string name, string chapter, float playtime)
    {
        //Debug.Log("tmffht");
        saveImage.sprite = sprite;
        saveTitleText.text = title;
        playerNameText.text = name;
        playChapterText.text = chapter;
        playTimeText.text = playtime.ToString();
    }

    public void SetEmptySaveSlot()
    {
        //Debug.Log("tmffht");
        saveImage.sprite = emptySlotImage;
        saveTitleText.text = "Empty Slot";
        playerNameText.text = "";
        playChapterText.text = "";
        playTimeText.text = "";
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick?.Invoke(this);
    }
}


