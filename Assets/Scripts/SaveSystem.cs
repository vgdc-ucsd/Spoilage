using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Json;
using System;


public class PlayerData
{
    //gonna save the variables after we get more info on what to be saved here
    public int progress;
    public int money;
    //public String[] kitchenUpgrades;
    //public String[] ingredients;
    public int volume;
    public Resolution[] resolution;

    //right now I'm assuming we have a player script that will be passed
    //into the constructor, which will then parse and take in the specified vars
    //for now I'm hardcoding in values
    public PlayerData()
    {
        progress = 2;
        money = 4;
    }
}


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
