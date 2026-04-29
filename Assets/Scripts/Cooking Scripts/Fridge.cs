using UnityEngine;

public class Fridge : MonoBehaviour
{
    [SerializeField] private GameObject _foodPrefab;
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private bool _spawnFoodOnStart = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (_spawnFoodOnStart)
        {
            SpawnFood();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnFood()
    {
         if (_foodPrefab == null || _spawnPoint == null)
        {
            Debug.LogWarning("Fridge is missing food prefab or spawn point.");
            return;
        }

        GameObject foodObject = Instantiate(_foodPrefab, _spawnPoint.position, Quaternion.identity);

        FoodGrab foodGrab = foodObject.GetComponent<FoodGrab>();

        if (foodGrab != null)
        {
            foodGrab.SetHomePosition(_spawnPoint.position);
            foodGrab.SetCameFromFridge(true);
        }
    }
}
