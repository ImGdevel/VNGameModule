using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueSystem.GlobalValue;

namespace DialogueSystem.Event
{
    [CreateAssetMenu(menuName = "Dialogue/Event/Global Value")]
    public class GlobalValueEvent : DialogueEvent
    {
        #region Variables
        [HideInInspector] GlobalValueManager manager;
        public GlobalValueOperationClass Operation;
        #endregion

        /// <summary>.
        /// The RunDialogue function is called by the Event Node
        /// It can also be called manually
        /// </summary>.
        public override void RunEvent()
        {
            // Load Global Value Manager
            manager = Resources.Load<GlobalValueManager>("GlobalValue");
            manager.LoadFile();

            // Calling the Global Value Change Function
            manager.Set(Operation.ValueName, Operation.Operation, Operation.OperationValue);
        }
    }
}
