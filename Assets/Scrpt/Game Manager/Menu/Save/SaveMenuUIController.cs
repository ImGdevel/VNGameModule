using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveMenuUIController : MenuModal
{
    [SerializeField] private GameObject saveSlotPrefep;
    [SerializeField] private Transform slotTransform;
    [SerializeField] private int slotCount;

    private List<SaveSlotController> saveSlots;

    void Awake() {
        saveSlots = new List<SaveSlotController>();
        for (int i=0; i<slotCount; i++) {
            GameObject slotObj = Instantiate(saveSlotPrefep, slotTransform.position, Quaternion.identity);
            SaveSlotController saveSlotController = slotObj.AddComponent<SaveSlotController>();
            slotObj.transform.SetParent(slotTransform);
            saveSlots.Add(saveSlotController);
        }
    }

    void Start() {
        List<SaveData> saveDatas = GameManager.userData.saveDatas;
        
        for(int i=0; i<slotCount; i++) {
            SaveSlotController slot = saveSlots[i];
            if (saveDatas.Count > i) {
                SaveData data = saveDatas[i];
                //slot.SetSaveSlot(null,"","","",0);
            }
        }
    }

    private void OnEnable() {
        OpenMenu();
    }

    public override void OpenMenu() {

        throw new System.NotImplementedException();
    }

    private void OnDisable() {
        CloseMenu();
    }

    public override void CloseMenu() {

        throw new System.NotImplementedException();
    }

}
