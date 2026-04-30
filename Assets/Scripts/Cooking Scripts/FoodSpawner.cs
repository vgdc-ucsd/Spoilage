using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _foodPrefab;
    [SerializeField] private Transform _spawnPoint;

    private void Start()
    {
        SpawnFood();
    }
    public void SpawnFood()
    {
        GameObject newFood = Instantiate(_foodPrefab, _spawnPoint.position, Quaternion.identity);

        FoodGrab foodGrab = newFood.GetComponent<FoodGrab>();
        if (foodGrab != null)
        {
            foodGrab.SetSpawner(this);
        }
    }
}
