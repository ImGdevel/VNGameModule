using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DialogueSystem.Event
{
    [CreateAssetMenu(menuName = "Dialogue/BackgroundEvent")]
    public class BackgroundEvent : DialogueEvent
    {
        #region Variables
        public Sprite Background;
        #endregion

        /// <summary>
        /// RunEvent 함수는 Event Node에 의해 호출됩니다.
        /// 수동으로도 호출할 수 있습니다.
        /// </summary>
        public override void RunEvent()
        {
            
        }
    }
}