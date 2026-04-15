using UnityEngine;

public abstract class Ingredient
{
    private string _name;
    private bool _needsCooking;
    private int _cookTime;
    private bool _isSpoiled;

    public Ingredient(string name, bool needsCooking, int cookTime, bool isSpoiled)
    {
        _name = name;
        _needsCooking = needsCooking;
        _cookTime = cookTime;
        _isSpoiled = isSpoiled;
    }

    public string Name => _name;
    public bool NeedsCooking => _needsCooking;
    public int CookTime => _cookTime;
    public bool isSpoiled => _isSpoiled;
}

public class Dough : Ingredient
{
    public Dough(string name, bool needsCooking, int cookTime, bool isSpoiled)
        : base(name, needsCooking, cookTime, isSpoiled)
    {
    }
}
