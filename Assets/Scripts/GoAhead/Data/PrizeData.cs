using UnityEngine;
using System;
using Assets.Scripts.GoAhead.Data;

namespace Assets.Scripts.GoAhead.Data
{
    [CreateAssetMenu(fileName = "Data", menuName = "Inventory/List", order = 1)]
    [Serializable]
    public class PrizeData : ScriptableObject {

        public string objectName = "New MyScriptableObject";

       // public DateTime[] activityLiveDates;

        [Tooltip("This is the bags to be won for each day")]
        [Range(0,500)] [Space(5)]
        public uint bagCount;

        [Tooltip("This is how many pounds can be won per day")]
        [Range(0,10)] [Space(5)]
        public uint hundredPoundCount;

        [SerializeField]
        public uint BagCount
        {
            get { return bagCount; }
            set { if (value < 0) { bagCount = value;} }
        }

        [SerializeField]
        public uint PoundCount
        {
            get { return hundredPoundCount; }
            set { if (value < 0)
                {
                    value = 0;
                }
                hundredPoundCount = value; }
        }
    }
}