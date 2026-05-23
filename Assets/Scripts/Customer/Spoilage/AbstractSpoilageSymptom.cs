using System;
using UnityEngine;

public abstract class AbstractSpoilageSymptom : ScriptableObject
{
    public static readonly Type[] symptomTypes =
    {
        typeof(BlinkingRapidly),
        typeof(Soilage),
        typeof(Sweating),
        typeof(BugSkittering),
        typeof(Gurgling),
        typeof(Tentacles),
        typeof(Ourgh),
        typeof(Ewww),
        typeof(Threat),
        typeof(OozePus),
    };

    public GameObject customer;

    public SpoilageCategory category;
    private CustomerAnimation _animation;
    private bool _hasApplied;
    private bool _isRegistered;

    public abstract void ApplySpoilage();

    protected CustomerAnimation Animation
    {
        get
        {
            return _animation;
        }
    }

    public void AssignCustomer(GameObject newCustomer)
    {
        if (customer == newCustomer)
        {
            return;
        }

        customer = newCustomer;
        _animation = customer.GetComponent<CustomerAnimation>();
        _hasApplied = false;
    }

    public void Register()
    {
        if (_isRegistered)
        {
            return;
        }

        SpoilageTriggerManager.Instance.AddSymptom(this);
        _isRegistered = true;
    }

    public void Unregister()
    {
        if (!_isRegistered || SpoilageTriggerManager.Instance == null)
        {
            _isRegistered = false;
            return;
        }

        SpoilageTriggerManager.Instance.RemoveSymptom(this);
        _isRegistered = false;
    }

    public void ApplySpoilageOnce()
    {
        if (_hasApplied)
        {
            return;
        }

        _hasApplied = true;
        ApplySpoilage();
    }

    public void DeleteSymptom()
    {
        Unregister();
        Destroy(this);
    }

    protected void SetBlinkMultiplier(float multiplier)
    {
        CustomerAnimation animation = Animation;
        animation.currentBlinkMultiplier = multiplier;
    }

    protected void SetMood(CustomerAnimation.Mood mood)
    {
        Animation.SetMood(mood);
    }

    protected void PlayDialogue(string suffix)
    {
        SpoilageTriggerManager.PlayDialogue(suffix);
    }

    protected void ApplyFrontSpriteSheet(string[] spritePaths)
    {
        Sprite[] sprites = LoadRandomSpriteSheet(spritePaths);
        SetFrontSprites(sprites);
        Animation.StartSpoilageAnim();
    }

    protected void ApplyFrontStaticSprite(string[] spritePaths)
    {
        Sprite sprite = LoadRandomSprite(spritePaths);
        SetFrontSprite(sprite);
        Animation.SetSpoilageStatus(CustomerAnimation.SpoilageStatus.FRAME_1);
    }

    protected Sprite[] LoadRandomSpriteSheet(string[] spritePaths)
    {
        return LoadSpriteSheet(PickRandomPath(spritePaths));
    }

    protected Sprite[] LoadSpriteSheet(string spritePath)
    {
        return Resources.LoadAll<Sprite>(spritePath);
    }

    protected Sprite LoadRandomSprite(string[] spritePaths)
    {
        return Resources.Load<Sprite>(PickRandomPath(spritePaths));
    }

    protected void SetFrontSprites(Sprite[] sprites)
    {
        SetSprite("SPOILAGE_FRONT_1", sprites[0]);
        SetSprite("SPOILAGE_FRONT_2", sprites[1]);
    }

    protected void SetBackSprites(Sprite[] sprites)
    {
        SetSprite("SPOILAGE_BACK_1", sprites[0]);
        SetSprite("SPOILAGE_BACK_2", sprites[1]);
    }

    protected void StartSpoilageAnimation()
    {
        Animation.StartSpoilageAnim();
    }

    private void SetFrontSprite(Sprite sprite)
    {
        SetSprite("SPOILAGE_FRONT_1", sprite);
        SetSprite("SPOILAGE_FRONT_2", sprite);
    }

    private void SetSprite(string rendererName, Sprite sprite)
    {
        Customer.SetSprite(customer, "Sprites/SPOILAGE/" + rendererName, sprite);
    }

    private static string PickRandomPath(string[] spritePaths)
    {
        return spritePaths[UnityEngine.Random.Range(0, spritePaths.Length)];
    }
}
