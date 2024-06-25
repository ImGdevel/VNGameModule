using UnityEditor;
using UnityEngine;

namespace VisualNovelGame
{
    public class DialogueEditorWindow : EditorWindow
    {
        private string dialogueText = "This is a dialogue text.";

        [MenuItem("Window/Dialogue Editor")]
        public static void ShowWindow()
        {
            GetWindow<DialogueEditorWindow>("Dialogue Editor");
        }

        private void OnGUI()
        {
            GUILayout.Label("Dialogue Preview", EditorStyles.boldLabel);

            dialogueText = EditorGUILayout.TextField("Dialogue Text", dialogueText);

            if (GUILayout.Button("Show Dialogue")) {
                DialogueUI dialogueUI = FindObjectOfType<DialogueUI>();
                if (dialogueUI != null) {
                    dialogueUI.ShowDialogue(dialogueText);
                }
            }

            if (GUILayout.Button("Hide Dialogue")) {
                DialogueUI dialogueUI = FindObjectOfType<DialogueUI>();
                if (dialogueUI != null) {
                    dialogueUI.HideDialogue();
                }
            }
        }
    }
}
