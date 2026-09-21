using System.Collections.Generic;
using UnityEngine;

public class LoadSaveManager : Singleton<LoadSaveManager>
{
    [SerializeField] private SaveCard _saveCardTemplate;
    [SerializeField] private Transform _cardContainer;

    void Start()
    {
        List<SaveOverview> overviews = SaveManager.Instance.LoadSaveOverviews()?.SaveOverviews;

        foreach (Transform child in _cardContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (SaveOverview overview in overviews)
        {
            SaveCard card = Instantiate(_saveCardTemplate, _cardContainer);
            card.SetData(overview);
        }
    }

    public void Load(int id)
    {
        SaveManager.Instance.LoadPlayer(id);
        SaveManager.OnPlayerLoad(() => GameManager.Instance.Load(GameScene.COOKING));
    }
}
