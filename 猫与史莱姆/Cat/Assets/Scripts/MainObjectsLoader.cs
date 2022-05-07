using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainObjectsLoader : MonoBehaviour
{
   
    public GameObject UIScreen;
    //public GameObject player;


    void Start()
    {
        if (ScreenFade.instance == null)
        {
            ScreenFade.instance = Instantiate(UIScreen).GetComponent<ScreenFade>();
        }


    }

   
    void Update()
    {
        
    }
}
