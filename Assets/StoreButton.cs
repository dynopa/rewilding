using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreButton : MonoBehaviour
{
    public Button button;
    public Image iconImage;
    public Text nameLabel;
    public Text priceLabel;

    private ShopItem shopItem;
    private PrinterScrollList scrollList; 

    // Start is called before the first frame update
    void Start()
    {
        button.onClick.AddListener(HandleClick); 
    }

    public void Setup(ShopItem currentItem, PrinterScrollList currentScrollList)
    {
        shopItem = currentItem;
        nameLabel.text = shopItem.itemName;
        priceLabel.text = shopItem.price.ToString();
        iconImage.sprite = shopItem.icon;

        scrollList = currentScrollList;
    }

    public void HandleClick()
    {
        scrollList.TrySellItem(shopItem);
    }
}
