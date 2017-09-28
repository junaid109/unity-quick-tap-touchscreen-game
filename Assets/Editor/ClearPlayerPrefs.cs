using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ClearPlayerPrefs : MonoBehaviour {

    [MenuItem("Go Ahead Game/Delete Saved Data")]
    static void DeleteMyPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("Data has been deleted");
        Debug.Log("Pound count is now: " + PlayerPrefs.GetString("PoundCount").ToString());
    }

    [MenuItem("Go Ahead Game/Set Pound Count 0")]
    static void SetPoundToZero()
    {
        PlayerPrefs.SetString("PoundCount", "0");
        Debug.Log("Pound count is now: " + PlayerPrefs.GetString("PoundCount").ToString());
    }

    [MenuItem("Go Ahead Game/Set Pound Count -1")]
    static void SetPoundToNegative()
    {
        PlayerPrefs.SetString("PoundCount", "-1");
        Debug.Log("Pound count is now: " + PlayerPrefs.GetString("PoundCount").ToString());
    }
}
