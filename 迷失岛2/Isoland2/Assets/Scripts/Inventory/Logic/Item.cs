using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public ItemName itemName;

    public void ItemClicked()
    {
        // 添加到背包后隐藏物体
        InventoryManager.Instance.AddItem(itemName);
        Debug.Log("还在");
        this.gameObject.SetActive(false);
        Debug.Log("没了");

    }
}
