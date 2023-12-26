using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameTitle : MonoBehaviour
{

    private void Start()
    {
        Invoke("ShowGameTitle", 1.3f);
    }

    public void ShowGameTitle()
    {
        BackgroundMusicManager.Instance.PlayMusic("GameTitle");
    }

    public void NewGame()
    {
        BackgroundMusicManager.Instance.StopMusic();
        VNScreenController.Instance.FadeOut(2f);
        Invoke("LoadPrologueScene", 2f);
    }

    public void LoadPrologueScene()
    {
        GameManager.Instance.CreateNewSaveSlot();
        SceneManager.LoadScene("Prologue");
    }
}
