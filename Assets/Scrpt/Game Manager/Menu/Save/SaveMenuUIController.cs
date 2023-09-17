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
    

    void Awake() {
        saveSlots = new List<SaveSlotComponent>();
        for (int i=0; i<slotCount; i++) {
            GameObject slotObj = Instantiate(saveSlotPrefep, slotTransform.position, Quaternion.identity);
            SaveSlotComponent saveSlotController = slotObj.AddComponent<SaveSlotComponent>();
            slotObj.transform.SetParent(slotTransform);
            saveSlots.Add(saveSlotController);
        }
    }

    void Start() {
        
    }

    


    private void OnEnable() {
        OpenMenu();
    }

    public override void OpenMenu() {
        List<SaveData> saveDatas = GameManager.userData.saveDatas;

        for (int i = 0; i < slotCount; i++) {
            SaveSlotComponent slot = saveSlots[i];
            if (saveDatas.Count > i) {
                SaveData data = saveDatas[i];
                slot.SetSaveSlot(null, "", data.chapter, data.dialogId, 0);
            }
            else {
                slot.SetEmptySaveSlot();
            }
        }
    }

    private void OnDisable() {
        CloseMenu();
    }

    public override void CloseMenu() {

    }

}
