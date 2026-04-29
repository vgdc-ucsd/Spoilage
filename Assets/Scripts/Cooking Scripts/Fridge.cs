using UnityEngine;

public class Fridge : MonoBehaviour
{
    [SerializeField] private GameObject _foodPrefab;
    [SerializeField] private Transform _spawnPoint;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnFood()
    {
        Instantiate(_foodPrefab, _spawnPoint.position, Quaternion.identity);
    }
}
