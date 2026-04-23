using UnityEngine;

public class PlayerData
{
    //gonna save the variables after we get more info on what to be saved here
    public int progress;
    public int money;

    //public String[] kitchenUpgrades;
    //public String[] ingredients;
    public float volume;
    public int resolutionWidth;
    public int resolutionHeight;

    //right now I'm assuming we have a player script that will be passed
    //into the constructor, which will then parse and take in the specified vars
    //for now I'm hardcoding in values
    public PlayerData()
    {
        progress = 2;
        money = 4;
    }
}

