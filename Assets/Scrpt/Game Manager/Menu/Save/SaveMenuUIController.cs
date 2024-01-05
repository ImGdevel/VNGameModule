using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveMenuUIController : MenuModal
{
    [SerializeField] private GameObject saveSlotPrefep;
    [SerializeField] private Transform slotTransform;
    [SerializeField] private int slotCount;

    private List<SaveSlotComponent> saveSlots;
    private List<GameData> saveDatas;

    void Awake()
    {
        saveSlots = new List<SaveSlotComponent>();
        for (int i = 0; i < slotCount; i++) {
            GameObject slotObj = Instantiate(saveSlotPrefep, slotTransform.position, Quaternion.identity);
            SaveSlotComponent saveSlotController = slotObj.GetComponent<SaveSlotComponent>();
            slotObj.transform.SetParent(slotTransform);
            saveSlots.Add(saveSlotController);

            saveSlotController.OnClick += DoSaveEventHandler;
        }
    }

    private void OnEnable()
    {
        OpenMenu();
    }

    private void OnDisable()
    {
        CloseMenu();
    }

    public override void OpenMenu()
    {
        saveDatas = new List<GameData> {
            
        };
        

        for (int i = 0; i < slotCount; i++) {
            SaveSlotComponent slot = saveSlots[i];
            if (saveDatas.Count > i) {


            }
            else {
                slot.SetEmptySaveSlot();
            }
        }
    }

    public override void CloseMenu()
    {
        //todo : close event
    }

    // Handle the click event
    private void DoSaveEventHandler(SaveSlotComponent clickedSlot)
    {
        int index = saveSlots.IndexOf(clickedSlot);
        if (index == -1) {
            Debug.Log("Clicked on save slot at index: " + index);
            return;
        }
        Debug.Log(index);
        if (index < slotCount) {
            SaveCurrentGameData(index);
        }
        else {
            ShowOverwriteWarning();
        }
    }


    private void SaveCurrentGameData(int slotIndex)
    {
        // 현재 게임 데이터를 저장하는 로직을 여기에 구현
        // 예를 들어, GameManager에 있는 SaveGame 메서드 호출 등
        //GameDataManager.Instance.SaveData(slotIndex);
        // 저장 후 메뉴를 다시 열어 갱신
        OpenMenu();
    }

    private void ShowOverwriteWarning()
    {
        // 세이브를 덮어씌울 때의 경고 메시지를 표시하는 로직을 여기에 구현
        // 예를 들어, 다이얼로그를 띄우거나 UI를 업데이트하는 등의 동작
        Debug.Log("Show Overwrite Warning");
    }


}
