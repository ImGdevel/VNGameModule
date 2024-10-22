using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [SerializeField] GameObject menuUI;
    [SerializeField] Image MenuBackScreen;

    [SerializeField] List<ModalData> ModalWindows;

    private GameManager gameManager;

    public static UnityAction<bool> OnMenuOpened;

    private bool isMenuOpen = false;
    private bool isModalWindowOpen = false;
    private int currentOpenModalWindowIndex;

    void Start()
    {
        gameManager = GameManager.Instance;
        InitializeModalWindow();
        CloseMenu();
    }

    private void InitializeModalWindow()
    {
        foreach (ModalData modal in ModalWindows) {
            foreach (Button button in modal.button) {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => ToggleModalWindows(ModalWindows.IndexOf(modal)));
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (isMenuOpen && isModalWindowOpen) {
                ToggleModalWindows(currentOpenModalWindowIndex);
            }
            else {
                ToggleMenu();
            }
        }
    }

    public void ToggleMenu()
    {
        isMenuOpen = !isMenuOpen;

        if (isMenuOpen) {
            OpenMenu();
        }
        else {
            CloseMenu();
        }
    }

    private void OpenMenu()
    {
        OnMenuOpened?.Invoke(true);
        Time.timeScale = 0;
        menuUI.SetActive(true);
        MenuBackScreen.enabled = true;
    }

    private void CloseMenu()
    {
        Time.timeScale = 1;
        OnMenuOpened?.Invoke(false);
        menuUI.SetActive(false);
        MenuBackScreen.enabled = false;
    }

    public void ToggleModalWindows(int index)
    {
        ModalData modal = ModalWindows[index];
        modal.isOpen = !modal.isOpen;

        if (modal.isOpen) {
            currentOpenModalWindowIndex = index;
            OpenModalWindow(modal);
        }
        else {
            CloseModalWindow(modal);
        }
    }

    private void OpenModalWindow(ModalData modal)
    {
        modal.window.SetActive(true);
        menuUI?.SetActive(false);
        isModalWindowOpen = true;
    }

    private void CloseModalWindow(ModalData modal)
    {
        modal.window.SetActive(false);
        menuUI?.SetActive(true);
        isModalWindowOpen = false;
    }

    public void GotoTitle()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("GameTitle");
    }

    public void QuitGame()
    {
        gameManager.QuitGame();
    }
}

[System.Serializable]
class ModalData
{
    [Header("Button Settings")]
    public Button[] button;

    [Header("Window Settings")]
    public GameObject window;
    public bool isOpenByDefault = false;

    [HideInInspector]
    public bool isOpen;
}