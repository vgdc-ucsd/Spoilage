using System.Collections;
using UnityEngine;

public enum GameEndScenario
{
    Resistance,
    Neutral,
    Warlord
}

public enum GameEndItemServed
{
    None,
    Poisoned,
    Unpoisoned
}

public class GameEnd : MonoBehaviour
{
    [SerializeField] private GameObject _svetkaDiesScreen;
    [SerializeField] private GameObject _shutDownScreen;
    [SerializeField] private GameObject _childhoodDishScreen;
    [SerializeField] private GameObject _loyalistScreen;

    private float _creditsTimer = 5f;

    void Start()
    {
        SaveManager.OnPlayerLoad(SetGameEndScreen);
    }

    public void SetGameEndScreen()
    {
        GameEndScenario scenario = SaveManager.Instance.Player.GameEndScenario;
        GameEndItemServed item = SaveManager.Instance.Player.GameEndItemServed;

        Debug.Log($"GameEnd: scenario={scenario}, item={item}");

        _svetkaDiesScreen.SetActive(false);
        _shutDownScreen.SetActive(false);
        _childhoodDishScreen.SetActive(false);
        _loyalistScreen.SetActive(false);

        GameObject screen = (scenario, item) switch
        {
            (GameEndScenario.Resistance, GameEndItemServed.None) => _shutDownScreen,            // SVE2.1B
            (GameEndScenario.Resistance, GameEndItemServed.Poisoned) => _shutDownScreen,        // SVE2.1C
            (GameEndScenario.Resistance, GameEndItemServed.Unpoisoned) => _childhoodDishScreen, // SVE2.1D
            (GameEndScenario.Neutral, _) => _shutDownScreen,                                    // SVE2.2B
            (GameEndScenario.Warlord, GameEndItemServed.None) => _loyalistScreen,               // SVE2.3B
            (GameEndScenario.Warlord, GameEndItemServed.Poisoned) => _svetkaDiesScreen,         // SVE2.3C
            _ => _shutDownScreen
        };

        screen.SetActive(true);

        StartCoroutine(LoadCreditsTimer());
    }

    private IEnumerator LoadCreditsTimer()
    {
        yield return new WaitForSeconds(_creditsTimer);
        // GameManager.Instance.Load(GameScene.CREDITS);
    }
}
