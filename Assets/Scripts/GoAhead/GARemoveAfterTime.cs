using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.GoAhead
{
    /// <summary>
    /// Author: Junaid Malik   
    /// Description: Removes object after a peroid of time and loads a level
    /// </summary>
    public class GARemoveAfterTime : MonoBehaviour
    {
        [Tooltip("How many seconds to wait before removing this object")]
        public float removeAfterTime = 0.1f;

        [Tooltip("What level to load after interstitial is over")]
        public string levelToLoad;

        [Tooltip("Decide if level needs to be loaded")]
        public bool shouldLevelBeloaded;

        /// <summary>
        /// Start is only called once in the lifetime of the behaviour.
        /// The difference between Awake and Start is that Start is only called if the script instance is enabled.
        /// This allows you to delay any initialization code, until it is really needed.
        /// Awake is always called before any Start functions.
        /// This allows you to order initialization of scripts
        /// </summary>
        void Start()
        {
            if (shouldLevelBeloaded) { 
            Invoke("LoadLevel", removeAfterTime);
            }
            // Remove this object after a delay
            Destroy(gameObject, removeAfterTime);
        }

        void LoadLevel()
        {
            try
            {
                SceneManager.LoadScene(levelToLoad);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unable to load level on gameobject: " 
                    + gameObject.name.ToString() 
                    + "\nWith Error :" + e);
            }
        }
    }
}