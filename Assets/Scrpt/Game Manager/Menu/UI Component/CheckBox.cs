using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckBox : UIComponent
{
    Button checkBox;
    private bool isCheckd;

    private void Start() {
        checkBox = transform.GetComponent<Button>();
        checkBox.onClick.AddListener(Toggle);
    }

    public void InitializeUI(bool check) {
        if (check) {

        }
        Toggle();
    }

    private void Toggle() {
        isCheckd = !isCheckd;
        if (isCheckd) {
            
        }
        else {

        }
    }
}
