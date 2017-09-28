using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.GoAhead
{
    public class GASelectButton : MonoBehaviour {
        // The button to select
        public GameObject selectedButton;

        /// <summary>
        /// Runs when this object is enabled
        /// </summary>
        void OnEnable()
        {
            if (selectedButton)
            {
                // Select the button
                if (EventSystem.current) EventSystem.current.SetSelectedGameObject(selectedButton);
            }
        }
    }
}