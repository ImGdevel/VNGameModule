using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameTitle : MonoBehaviour
{


    private void Awake() {
        
    }


    private void Start() {
        
    }


    public void ShowOpeningCredits() {

    }

    public void ShowGameTitle() {

    }



    public void NewGame() {
        SceneManager.LoadScene("Prologue");
    }
}
