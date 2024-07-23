using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DialogueSystem.Event
{
    [CreateAssetMenu(menuName = "Dialogue/Event/Character Event")]
    public class CharacterEvent : DialogueEvent
    {
        #region Variables
        public DialogueCharacter Character;
        #endregion

        /// <summary>.
        /// The RunDialogue function is called by the Event Node
        /// It can also be called manually
        /// </summary>.
        public override void RunEvent()
        {
            DialogueEventManager.Instance.CharacterEvent(Character);
        }
    }
}
