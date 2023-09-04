using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AddSoundToUI : MonoBehaviour
{
    private AudioSource soundPlayer;

    [SerializeField]
    private List<GameObject> uiElements;

    public AudioClip clickSound;
    public AudioClip highlightSound;

    [Range(0f, 1f)]
    public float volume = 1.0f;

    private void Start() {
        soundPlayer = GetComponent<AudioSource>();
        if (soundPlayer == null) {
            soundPlayer = gameObject.AddComponent<AudioSource>();
        }

        soundPlayer.volume = volume;

        foreach (var uiElement in uiElements) {
            if (clickSound != null) {
                Button button = uiElement.GetComponent<Button>();
                button.onClick.AddListener(PlayClickSound);
            }

            if (highlightSound != null) {
                // EventTrigger 컴포넌트를 가져오고, 하이라이트와 언하이라이트 이벤트를 추가
                EventTrigger eventTrigger = uiElement.GetComponent<EventTrigger>();
                if (eventTrigger == null) {
                    eventTrigger = uiElement.AddComponent<EventTrigger>();
                }

                // 하이라이트 이벤트 추가
                EventTrigger.Entry highlightEntry = new EventTrigger.Entry();
                highlightEntry.eventID = EventTriggerType.PointerEnter;
                highlightEntry.callback.AddListener((data) => { PlayHighlightSound(); });
                eventTrigger.triggers.Add(highlightEntry);

                // 언하이라이트 이벤트 추가
                EventTrigger.Entry unhighlightEntry = new EventTrigger.Entry();
                unhighlightEntry.eventID = EventTriggerType.PointerExit;
                unhighlightEntry.callback.AddListener((data) => { StopHighlightSound(); });
                eventTrigger.triggers.Add(unhighlightEntry);
            }
        }
    }

    private void PlayClickSound() {
        soundPlayer.clip = clickSound;
        soundPlayer.PlayOneShot(clickSound);
    }

    private void PlayHighlightSound() {
        soundPlayer.clip = highlightSound;
        soundPlayer.PlayOneShot(highlightSound);
    }

    private void StopHighlightSound() {
        soundPlayer.Stop();
    }
}
