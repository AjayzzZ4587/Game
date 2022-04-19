# 迷失岛2
M_Studio《迷失岛2》游戏框架开发跟随学习笔记

## 场景切换

Teleport

```C#
public class Teleport : MonoBehaviour
{
    [SceneName] public string sceneFrom;

    [SceneName] public string sceneToGO;

    public void TeleportToScene()
    {
        TransitionManager.Instance.Transition(sceneFrom, sceneToGO);
    }
}
```

[SceneName] 这个需要作者写的一个插件，可以实现参加自动选择

```c#
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionManager : Singleton<TransitionManager>, ISaveable
{

  [SceneName] public string startScene;
  
  public CanvasGroup fadeCanvasGroup;

  public float fadeDuration;

  private bool isFade;

  private bool canTransition;

  private void OnEnable()
  {
      EventHandler.GameStateChangeEvent += OnGameStateChangeEvent;
      EventHandler.StartNewGameEvent += OnStartNewGameEvent;
  }

  private void OnDisable()
  {
      EventHandler.GameStateChangeEvent -= OnGameStateChangeEvent;
      EventHandler.StartNewGameEvent -= OnStartNewGameEvent;
                   //StartNewGameEvent
  }

  private void OnStartNewGameEvent(int obj)
  {
        StartCoroutine(TransitionToScene("Menu", startScene));
  }

  private void OnGameStateChangeEvent(GameState gameState)
  {
        canTransition = gameState == GameState.GamePlay;
  }

  private void Start()
  {
      ISaveable saveable = this;
      saveable.SaveableRegister();
  }

  public void Transition(string from, string to)
  {
      if (!isFade && canTransition)
        StartCoroutine(TransitionToScene(from, to));
  }

  private IEnumerator TransitionToScene(string from, string to)
  {
      yield return Fade(1);

      if (from != string.Empty)
      {
          EventHandler.CallBeforeSceneUnloadEvent();
          yield return SceneManager.UnloadSceneAsync(from);
      }
      yield return SceneManager.LoadSceneAsync(to, LoadSceneMode.Additive);

      // 设置新场景为激活
      Scene newScene = SceneManager.GetSceneAt(SceneManager.sceneCount -  1);
      SceneManager.SetActiveScene(newScene);
      
      EventHandler.CallAfterSceneLoadedEvent();
      yield return Fade(0);
  }

  // 写一个协程来负责渐变过程
  private IEnumerator Fade(float targetAlpha)
  {
      isFade = true;

      fadeCanvasGroup.blocksRaycasts = true;

      float speed = Mathf.Abs(fadeCanvasGroup.alpha - targetAlpha) / fadeDuration;

      while (!Mathf.Approximately(fadeCanvasGroup.alpha, targetAlpha))
      {
          fadeCanvasGroup.alpha = Mathf.MoveTowards(fadeCanvasGroup.alpha, targetAlpha, speed * Time.deltaTime);
          yield return null;
      }

      fadeCanvasGroup.blocksRaycasts = false;

      isFade = false;

  }

    public GameSaveData GenerateSaveData()
    {
        GameSaveData saveData = new GameSaveData();
        saveData.currentScene = SceneManager.GetActiveScene().name;
        return saveData;
    }

    public void RestoreGameData(GameSaveData saveData)
    {
        Transition("Menu", saveData.currentScene);
    }

}
```

鼠标的类

```c#
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
        switch (clickObject.tag)
        {       
            case "Teleport ":
                var teleport = clickObject.GetComponent<Teleport>();
                teleport?.TeleportToScene();
                break;
            case "Item":
                var item = clickObject.GetComponent<Item>();
                item?.ItemClicked(); // 就是调用里面的方法
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

```

场景切换特细

添加一个Canvas，然后加上panel，中间添加上Canvas Group

## 背包基本逻辑

将物品直接拖进去，设置好层级，让其能够显示。

作者贯彻了MVC设计模式，创建一个Inventory的文件夹，当中有Data， logic, UI文件夹，

写一个Item的代码挂在物品上，记得tag要更新上。

```c#
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
        this.gameObject.SetActive(false);
    }
}
```

作者还写了一个工具文件夹，当中放了枚举工具类

```c#
public enum ItemName
{
    None, Key, Ticket
}

public enum GameState
{
    Pause, GamePlay
}

public enum BallName
{
    None, B1, B2, B3, B4, B5, B6
}
```

logic上的逻辑是由InventoryManager负责调用的

```c#
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : Singleton<InventoryManager>,ISaveable
{

    public ItemDataList_SO itemData;

    [SerializeField] private List<ItemName> itemList = new List<ItemName>();

    private void OnEnable()
    {
        EventHandler.ItemUsedEvent += OnItemUsedEvent;
        EventHandler.ChangeItemEvent += OnChangeItemEvent;
        EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
        EventHandler.StartNewGameEvent += OnStartNewGameEvent;
    }

    private void OnDisable()
    {
        EventHandler.ItemUsedEvent -= OnItemUsedEvent;
        EventHandler.ChangeItemEvent -= OnChangeItemEvent;
        EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
        EventHandler.StartNewGameEvent -= OnStartNewGameEvent;
    }

    private void Start()
    {
        ISaveable saveable = this;
        saveable.SaveableRegister();
    }

    private void OnStartNewGameEvent(int obj)
    {
        itemList.Clear();
    }

    private void OnAfterSceneLoadedEvent()
    {
        if (itemList.Count == 0)
        {
            EventHandler.CallUpdateUIEvent(null, -1);
        }
        else
        {
            for (int i = 0; i < itemList.Count; i++)
            {
                EventHandler.CallUpdateUIEvent(itemData.GetItemDetails(itemList[i]), i);
            }
        }
    }

    private void OnChangeItemEvent(int index)
    {
        if (index >= 0 && index < itemList.Count)
        {
            ItemDetails item = itemData.GetItemDetails(itemList[index]);
            EventHandler.CallUpdateUIEvent(item, index);
        }
    }

    private void OnItemUsedEvent(ItemName itemName)
    {
        var index = GetItemIndex(itemName);
        itemList.RemoveAt(index);

        if (itemList.Count == 0) 
        {
            EventHandler.CallUpdateUIEvent(null, -1);
        }
    }

   

   public void AddItem(ItemName itemName)
   {
       if (!itemList.Contains(itemName))
       {
           
           itemList.Add(itemName);
           
           EventHandler.CallUpdateUIEvent(itemData.GetItemDetails(itemName), itemList.Count - 1);
       }
   }

   private int GetItemIndex(ItemName itemName)
   {
       for (int i = 0; i < itemList.Count; i++)
       {
           if (itemList[i] == itemName)
                return i;
       }
       return -1;
   }


   public GameSaveData GenerateSaveData()
    {
        GameSaveData saveData = new GameSaveData();
        saveData.itemList = this.itemList;
      
        return saveData;
    }

    public void RestoreGameData(GameSaveData saveData)
    {
        this.itemList = saveData.itemList;
    }
}

```

Manager这种类还需要设置好Persistent中的空物体，然后将其挂载上去

记住了上面用了List保存了物品的名字

所有的物品的用一个带SO后缀的名字

```c#
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDataList_SO", menuName = "Inventory/ItemDataList_SO")]

public class ItemDataList_SO : ScriptableObject
{
   public List<ItemDetails> itemDetailsList;

   public ItemDetails GetItemDetails(ItemName itemName)
   {
       return itemDetailsList.Find(i => i.itemName == itemName);
   }
}

[System.Serializable]
public class ItemDetails
{
    public ItemName itemName;

    public Sprite itemSprite;
}

```

要创建一个专门的文件夹来放SO文件，中的数据，然后叫做Game Data，里面有一个SO文件，当中放着对应的信息啦。这种文件要右键Create能找到之前设置的SO文件，这里是具体的信息存在位置。要记得对这里的信息进行更新。

## C#

#### 事件

#### 协程

### 单例模式

上述xxxManager一般只有一个实例，所以作者都用了单例模式。

