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

    public void SetSaveSlot(SaveData saveData)
    {
        saveImage.sprite = emptySlotImage;
        saveTitleText.text = saveData.saveName;
        playerNameText.text = saveData.saveNumber.ToString();
        playChapterText.text = "0";
        playTimeText.text = saveData.gamePlayTime.ToString();
    }

    public void SetEmptySaveSlot()
    {
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


