using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [SerializeField] GameObject menuUI;
    [SerializeField] GameObject settingUI;
    [SerializeField] GameObject saveUI;
    [SerializeField] GameObject loadUI;

    [SerializeField] Image MenuBackScreen;
    
    private GameManager gameManager;
    private Settings settings;

    public static UnityAction<bool> OnMenuOpened; // 직접 이벤트를 정의

    private bool isMenuOpen = false;
    private bool isSettingMenuOpen = false;

    void Start()
    {
        gameManager = GameManager.Instance;
        settings = SettingsManager.GetSettings;
        CloseMenu();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (isMenuOpen && isSettingMenuOpen) {
                ToggleSettingMenu();
            }
            else {
                ToggleMenu();
            }
        }
    }

    public void ToggleMenu() {
        isMenuOpen = !isMenuOpen;

        if (isMenuOpen) {
            OpenMenu();
        }
        else {
            CloseMenu();
        }
    }

    public void ToggleSettingMenu() {
        isSettingMenuOpen = !isSettingMenuOpen;

        if (isSettingMenuOpen) {
            OpenSettingMenu();
        }
        else {
            CloseSettingMenu();
        }
    }

    private void OpenMenu() {
        OnMenuOpened?.Invoke(true);
        Time.timeScale = 0;
        menuUI.SetActive(true);
        MenuBackScreen.enabled = true;
    }

    private void CloseMenu() {
        Time.timeScale = 1; // 일시 정지 해제
        OnMenuOpened?.Invoke(false);
        menuUI.SetActive(false);
        MenuBackScreen.enabled = false;
    }

    private void OpenSettingMenu() {
        settingUI.SetActive(true);
        settingUI.GetComponent<SettingsUIManager>().ShowSettingMenu();
        menuUI.SetActive(false);
    }

    private void CloseSettingMenu() {

        Debug.Log("메뉴 닫기(저장):" + settings);
        SettingsManager.Instance.ApplySetting(settings);
        settingUI.SetActive(false);
        menuUI.SetActive(true);
    }

    public void GotoTitle() {
        Time.timeScale = 1;
        SceneManager.LoadScene("GameTitle");
    }

    public void QuitGame() {
        gameManager.QuitGame();
    }
}