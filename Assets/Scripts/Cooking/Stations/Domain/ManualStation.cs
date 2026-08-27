using UnityEngine;

public class ManualStation : Station
{
    public override PlaceableUI UI => throw new System.NotImplementedException();

    public override void Place(Placeable placeable) => throw new System.NotImplementedException();
    public override void Process(float dt) => throw new System.NotImplementedException();
    public override Placeable Produces() => throw new System.NotImplementedException();
    public override void Remove() => throw new System.NotImplementedException();
}
