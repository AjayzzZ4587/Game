using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drag : MonoBehaviour
{
        Vector3 dist;
        float posX;
        float posY;

        Vector3 Tmp_curPos;
        Vector3 Tmp_worldPos;


        private Vector3 curPos;
        private Vector3 worldPos;

        private bool CanDraw = true;
        private bool OnDrawing = false;


        void OnMouseDown()
        {
            dist = Camera.main.WorldToScreenPoint(transform.position);
            posX = Input.mousePosition.x - dist.x;
            posY = Input.mousePosition.y - dist.y;
           
            CanDraw = true;
           
            
        }

        void OnMouseDrag()
        {
            if (CanDraw)
            {
                curPos = new Vector3(Input.mousePosition.x - posX, Input.mousePosition.y - posY, dist.z);
                worldPos = Camera.main.ScreenToWorldPoint(curPos);
                transform.position = worldPos;
                OnDrawing = true;
            }
            
        }


        private  void OnCollisionEnter2D(Collision2D other) {
            //Debug.Log("鼠标就不该动了");
            CanDraw = false;
            // Tmp_worldPos = worldPos;
            // Tmp_curPos = curPos;
            // Cursor.lockState = CursorLockMode.Locked;
            // transform.position = Tmp_worldPos;
            if (OnDrawing) 
            {   
                if (this.gameObject.tag == "KingSlime") 
                {
                    if (this.gameObject.tag == other.gameObject.tag)
                    {
                        Vector3 centerPos = (this.transform.position + other.transform.position) /2;
                        Instantiate(this.gameObject, centerPos, this.gameObject.transform.rotation);
                        Destroy(other.gameObject);
                        Destroy(this.gameObject);      
                    }  
                }
                else
                {
                    if (this.gameObject.tag == other.gameObject.tag)
                    {
                    Destroy(other.gameObject);
                    Destroy(this.gameObject);      
                    }  
                }
                
            }
        }

        private void OnCollisionExit2D(Collision2D other) {
            // Cursor.lockState = CursorLockMode.None;
            // transform.position = Tmp_worldPos;
            
            CanDraw = true;
            
        }

        private void OnCollisionStay2D(Collision2D other)
        {   
            Debug.Log("还在碰撞");
            Start();
        }

        private void Start()
        {
            StartCoroutine("DelayFunc");
        }
        IEnumerator DelayFunc()
        {
            //double delay = 0.1;7yuuuuuuu
            yield return new WaitForSeconds(0.05f);
            CanDraw = false;
            //yield return new WaitForSeconds(delay);
        }
}
