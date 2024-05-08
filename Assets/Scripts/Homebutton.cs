using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Homebutton : MonoBehaviour
{
    [SerializeField] private GameObject Game, Menu;
    bool finished;
    public void Home()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

}
