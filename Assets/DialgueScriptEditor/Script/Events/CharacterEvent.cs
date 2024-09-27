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

        public CharacterType characterType;
        public CharacterPosition characterPosition;
        #endregion


        public override void RunEvent()
        {
            
        }
    }
}
