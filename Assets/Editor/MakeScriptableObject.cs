using UnityEngine;
using System.Collections;
using Assets.Scripts.GoAhead.Data;
using UnityEditor;

public class MakeScriptableObject
{
    [MenuItem("Assets/Create/Prize Data Object")]
    public static void CreateMyAsset()
    {
        PrizeData asset = ScriptableObject.CreateInstance<PrizeData>();

        AssetDatabase.CreateAsset(asset, "Assets/NewScripableObject.asset");
        EditorUtility.SetDirty(asset);

        EditorUtility.FocusProjectWindow();

        AssetDatabase.Refresh();

        AssetDatabase.SaveAssets();

        Selection.activeObject = asset;    
    }
}