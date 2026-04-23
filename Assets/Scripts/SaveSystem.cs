using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Json;
using System;

[Serializable]
public static class SaveSystem
{
    //any button or autosave functionality can just call SaveSystem.SavePlayer()
    public static void SavePlayer()
    {
        //can change it later on so that it will pass in a player into the
        //constructor
        PlayerData playerData = new PlayerData();
        string json = JsonUtility.ToJson( playerData );
        string path = Application.persistentDataPath + "/playerData.json";
        File.WriteAllText( path, json );
        Debug.Log("Save path is " + path);
    }

}
