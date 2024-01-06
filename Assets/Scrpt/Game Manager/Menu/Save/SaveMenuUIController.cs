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

    private GameDataManager gameDataManager;
    private List<SaveSlotComponent> saveSlots;
    private List<SaveData> saveDatas;

    void Awake()
    {
        saveSlots = new List<SaveSlotComponent>();
        gameDataManager = GameDataManager.Instance;
        for (int i = 0; i < slotCount; i++) {
            GameObject slotObj = Instantiate(saveSlotPrefep, slotTransform.position, Quaternion.identity);
            SaveSlotComponent saveSlotController = slotObj.GetComponent<SaveSlotComponent>();
            slotObj.transform.SetParent(slotTransform);
            saveSlots.Add(saveSlotController);

            saveSlotController.OnClick += DoSaveEventHandler;
        }
    }

    void Start()
    {
        //gameDataManager = GameDataManager.Instance; 
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
        saveDatas = gameDataManager.GetSaveDataList();

        for (int i = 0; i < slotCount; i++) {
            SaveSlotComponent slot = saveSlots[i];

            int index = saveDatas.FindIndex(save => save.saveNumber == (i+1));

            if (index != -1) {
                slot.SetSaveSlot(saveDatas[index]);
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
        GameDataManager.Instance.SaveGameData(slotIndex);
        OpenMenu();
    }

    private void DeleteCurrentGameData(int slotIndex)
    {

    }

    private void ShowOverwriteWarning()
    {
        // 세이브를 덮어씌울 때의 경고 메시지를 표시하는 로직을 여기에 구현
        // 예를 들어, 다이얼로그를 띄우거나 UI를 업데이트하는 등의 동작
        Debug.Log("Show Overwrite Warning");
    }


}
