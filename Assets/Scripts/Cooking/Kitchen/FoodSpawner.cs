using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _foodPrefab;
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private bool _spawnFoodOnStart = true;
    private FoodGrab _currentFood;

    void Start()
    {
        if (_spawnFoodOnStart)
        {
            SpawnFood();
        }
    }

    public void SpawnFood()
    {
        if (_currentFood != null) return;
        GameObject newFood = Instantiate(_foodPrefab, _spawnPoint.position, Quaternion.identity, _spawnPoint);

        _currentFood = newFood.GetComponent<FoodGrab>();

        if (_currentFood != null)
        {
            _currentFood.SetSpawner(this);
            _currentFood.SetCameFromFridge(true);
            _currentFood.SetHomePosition(_spawnPoint.position);
        }
    }

    public void ClearCurrentFood(FoodGrab food)
    {
        if (_currentFood == food)
        {
            _currentFood = null;
        }
    }
}
