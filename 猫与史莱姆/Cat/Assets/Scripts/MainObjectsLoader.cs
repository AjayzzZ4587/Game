using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainObjectsLoader : MonoBehaviour
{
   
    public GameObject UIScreen;
    //public GameObject player;
    public GameObject gameMan;

    void Start()
    {
        if (ScreenFade.instance == null)
        {
            ScreenFade.instance = Instantiate(UIScreen).GetComponent<ScreenFade>();
        }

        if (GameManager.instance == null)
        {
            GameManager.instance = Instantiate(gameMan).GetComponent<GameManager>();
        }
    }

   
    void Update()
    {
        
    }
}
