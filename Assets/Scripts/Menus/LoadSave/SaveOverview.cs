using System;
using System.Collections.Generic;

[Serializable]
public class SaveOverviewCollection
{
    public SaveOverviewCollection(List<SaveOverview> overviews)
    {
        SaveOverviews = overviews;
    }

    public List<SaveOverview> SaveOverviews;
}

[Serializable]
public class SaveOverview
{
    public SaveOverview(int id, int day, int wealth, int ingredients, int stations)
    {
        ID = id;
        Day = day;
        Wealth = wealth;
        IngredientsUnlocked = ingredients;
        StationsUnlocked = stations;
    }

    public int ID;
    public int Day; 
    public int Wealth;
    public int IngredientsUnlocked;
    public int StationsUnlocked;
}
