using UnityEngine;
using System.Runtime.Serialization;
using UnityEngine.SceneManagement;

public class LoadSaves : MonoBehaviour
{
    public void CollectionOfSaves()
    {
        SceneManager.LoadSceneAsync("Saves");
    }

}
