using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CursorManager : MonoBehaviour
{
    public RectTransform hand;
    private Vector3 mouseWorldPos => Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));

    private bool canClick;

    private bool holdItem;

    private ItemName currentItem;

    private void OnEnable() {
        EventHandler.ItemSelectedEvent += OnItemSelectedEvent;
        EventHandler.ItemUsedEvent += OnItemUsedEvent;
    }

    private void OnDisable() {
        EventHandler.ItemSelectedEvent -= OnItemSelectedEvent;
        EventHandler.ItemUsedEvent -= OnItemUsedEvent;
    }





    private void Update()
    {
        canClick = ObjectAtMousePosition();

        if (hand.gameObject.activeInHierarchy)
            hand.position = Input.mousePosition;

        if (InteractWithUI()) return;

        if (canClick && Input.GetMouseButtonDown(0))
        {
            // 检测鼠标互动情况
            ClickAction(ObjectAtMousePosition().gameObject);
        }
    }

    private void OnItemUsedEvent(ItemName obj)
    {
        currentItem = ItemName.None;
        holdItem = false;
        hand.gameObject.SetActive(false);
    }

    private void OnItemSelectedEvent(ItemDetails itemDetails, bool isSelected)
    {
        holdItem = isSelected;
        if (isSelected)
        {
            currentItem = itemDetails.itemName;
        }
        hand.gameObject.SetActive(holdItem);
    }


    private void ClickAction(GameObject clickObject)
    {
        // Debug.Log("你按下了a键.");
        // Debug.Log(clickObject.tag);
        // if (clickObject.tag == "Teleport ") {
        //     Debug.Log(clickObject.tag);
        // }
        switch (clickObject.tag)
        {       
            case "Teleport ":
                var teleport = clickObject.GetComponent<Teleport>();
                teleport?.TeleportToScene();
                break;
            case "Item":
                var item = clickObject.GetComponent<Item>();
                item?.ItemClicked();
                break;
            case "Interactive":
                Debug.Log("在这里测试");
                var interactive = clickObject.GetComponent<Interactive>();
                if (holdItem)
                    interactive?.CheckItem(currentItem);
                else
                    interactive?.EmptyClicked();
                break;
        }
    }


    /// <summary>
    /// 检测鼠标点击范围碰撞体
    /// </summary>
    /// <return></returns>
    private Collider2D ObjectAtMousePosition()
    {
         return Physics2D.OverlapPoint(mouseWorldPos);
    }

    private bool InteractWithUI()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return true;

        return false;
    }
}
