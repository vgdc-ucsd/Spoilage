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
    public SaveOverview(int id, int day)
    {
        ID = id;
        Day = day;
    }

    public int ID;
    public int Day; 
}
