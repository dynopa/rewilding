using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class ShopItem
{
    public string itemName;
    public Sprite icon;
    public float price = 1f;
    public bool isJunk;
}

public class PrinterScrollList : MonoBehaviour
{
    public bool isPlayer;
    public List<ShopItem> shopItemList;
    public Transform contentPanel;
    public PrinterScrollList otherShop;
    public Text myResourceDisplay;
    public SimpleObjectPool buttonObjectPool;
    public float resource;

    // Start is called before the first frame update
    void Start()
    {
        if (isPlayer)
        {
            
        }
        RefreshDisplay();
    }

    public void RefreshDisplay()
    {
        myResourceDisplay.text = resource.ToString() + "r";
        RemoveButtons();
        AddButtons();
    }

    private void AddButtons()
    {
        for (int i = 0; i < shopItemList.Count; i++)
        {
            ShopItem shopItem = shopItemList[i];
            GameObject newButton = buttonObjectPool.GetObject();
            newButton.transform.SetParent(contentPanel,false);
            StoreButton storeButton = newButton.GetComponent<StoreButton>();
            storeButton.Setup(shopItem, this);
        }
    }

    private void RemoveButtons()
    {
        Debug.Log("bout to remove");
        //while (contentPanel.childCount > 0)
        for (int i = 0; i < contentPanel.childCount; i++)
        {
            Debug.Log("REMOVE");
            GameObject toRemove = transform.GetChild(0).gameObject;
            buttonObjectPool.ReturnObject(toRemove);
        }
    }

    public void TrySellItem(ShopItem shopItem)
    {
        if (!isPlayer && otherShop.resource >= shopItem.price)
        {
            resource += shopItem.price;
            otherShop.resource -= shopItem.price;
            RemoveItem(shopItem, this);
            if (!shopItem.isJunk)
            {
                AddItem(shopItem, otherShop);
            }
            RefreshDisplay();
            otherShop.RefreshDisplay();
        }
        else if (isPlayer)
        {
            resource += shopItem.price;
            otherShop.resource -= shopItem.price;
            RemoveItem(shopItem, this);
            if (!shopItem.isJunk)
            {
                AddItem(shopItem, otherShop);
            }
            RefreshDisplay();
            otherShop.RefreshDisplay();
        }
    }

    private void AddItem(ShopItem itemToAdd, PrinterScrollList shopList)
    {
        List<int> numItems = new List<int>() { 0, 0, 0, 0, 0 };
        for (int i = 0; i < shopItemList.Count; i++)
        {
            //numItems[value]++;
        }
        shopList.shopItemList.Add(itemToAdd);
    }

    private void RemoveItem(ShopItem itemToRemove, PrinterScrollList shopList)
    {
        for (int i = shopList.shopItemList.Count - 1; i >= 0; i--)
        {
            if (shopList.shopItemList[i] == itemToRemove)
            {
                shopList.shopItemList.RemoveAt(i);
            }
        }
    }
}
