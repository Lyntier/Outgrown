using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayButton : MonoBehaviour
{
    [SerializeField] Object scene;

    public void PlayGame()
    {
        print("Loading scene");
        SceneManager.LoadScene(scene.name);
    }
}
