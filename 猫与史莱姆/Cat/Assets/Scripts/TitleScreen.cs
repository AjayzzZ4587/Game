using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TitleScreen : MonoBehaviour
{
    public string newGameScene;




    // Start is called before the first frame update
    void Start()
    {
        Screen.SetResolution(1280, 720, true);
        ScreenFade.instance.fadeScreenObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NewGame(int difficulty)
    {
        

        ScreenFade.instance.fadeScreenObject.SetActive(true);

        
        //PlayerController.instance.canMove = true;
        //GameManager.instance.cutSceneActive = true;
        //SceneManager.LoadScene(newGameScene);
    }




    public void Exit()
    {
        Application.Quit();
    }
}
