using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

        //Screen.SetResolution(1152, 864, false);
        Screen.SetResolution(1280, 960, false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void MoveGame()
    {
        SceneManager.LoadScene("Game");
    }

}
