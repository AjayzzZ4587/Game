using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraController : MonoBehaviour
{

    [HideInInspector]
    public Transform player;

    private GameObject lightSource;


    public Tilemap tilemap;
    private Vector3 boundary1;
    private Vector3 boundary2;

    private float halfHeight;
    private float halfWidth;


    private void Awake()
    {
        lightSource = GameObject.Find("Light Source");
    }


    // Start is called before the first frame update
    void Start()
    {
        lightSource.SetActive(false);

        float targetaspect = 16.0f / 9.0f;

        float windowaspect = (float)Screen.width / (float)Screen.height;

        float scaleheight = windowaspect / targetaspect;

        Camera camera = GetComponent<Camera>();


        if (scaleheight < 1.0f)
        {
            Rect rect = camera.rect;

            rect.width = 1.0f;
            rect.height = scaleheight;
            rect.x = 0;
            rect.y = (1.0f - scaleheight) / 2.0f;

            camera.rect = rect;
        }
        else // add pillarbox
        {
            float scalewidth = 1.0f / scaleheight;

            Rect rect = camera.rect;

            rect.width = scalewidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scalewidth) / 2.0f;
            rect.y = 0;

            camera.rect = rect;
        }

        //player = FindObjectOfType<PlayerController>().transform;

        halfHeight = Camera.main.orthographicSize;
        halfWidth = halfHeight * Camera.main.aspect;

        if (tilemap != null)
        {
            tilemap.CompressBounds();

            boundary1 = tilemap.localBounds.min + new Vector3(halfWidth, halfHeight, 0f);
            boundary2 = tilemap.localBounds.max + new Vector3(-halfWidth, -halfHeight, 0f);

            //PlayerController.instance.SetBounds(tilemap.localBounds.min, tilemap.localBounds.max);
        }



    }

    void LateUpdate () {
        // transform.position = new Vector3(player.position.x, player.position.y, transform.position.z);

        // //keep the camera inside the bounds
        // transform.position = new Vector3(Mathf.Clamp(transform.position.x, boundary1.x, boundary2.x), Mathf.Clamp(transform.position.y, boundary1.y, boundary2.y), transform.position.z);

        // if(!musicStarted)
        // {
        //     musicStarted = true;
        //     AudioManager.instance.PlayBGM(musicToPlay);
        // }
	}
}
