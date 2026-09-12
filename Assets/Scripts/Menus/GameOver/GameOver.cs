using TMPro;
using UnityEngine;

public enum GameOverCondition
{
    Bankruptcy,
    Reputation
}

public class GameOver : MonoBehaviour
{
    [SerializeField] private GameObject _reputationScreen;
    [SerializeField] private GameObject _bankruptcyScreen;
    [SerializeField] private TextMeshProUGUI _reputationText;
    [SerializeField] private TextMeshProUGUI _bankruptcyText;

    void Start()
    {
        SaveManager.OnPlayerLoad(SetGameOverScreen);        
    }

    public void SetGameOverScreen()
    {
        int day = SaveManager.Instance.Player.Day;
        _reputationScreen.SetActive(false);
        _bankruptcyScreen.SetActive(false);

        switch (SaveManager.Instance.Player.GameOverCondition)
        {
            case GameOverCondition.Bankruptcy:
                _bankruptcyScreen.SetActive(true);
                _bankruptcyText.text = $"You went bankrupt on Day {day}.";
                break;
            case GameOverCondition.Reputation:
                _reputationScreen.SetActive(true);
                _reputationText.text = $"As of Day {day}, this establishment is";
                break;
        }
    }
}
