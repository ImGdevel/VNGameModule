using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SaveSlotController : MonoBehaviour
{
    [SerializeField] Image saveImage;
    [SerializeField] TMP_Text saveTitleText;
    [SerializeField] TMP_Text playerNameText;
    [SerializeField] TMP_Text playChapterText;
    [SerializeField] TMP_Text playTimeText;

    [SerializeField] private Sprite defaultImage;

    void Start() {
        //saveImage.sprite = defaultImage;
    }

    public void SetSaveSlot(Sprite sprite, string title, string name, string chapter, float playtime) {
        saveImage.sprite = sprite;
        saveTitleText.text = title;
        playerNameText.text = name;
        playChapterText.text = chapter;
        playTimeText.text = playTimeText.ToString();
    }
}


