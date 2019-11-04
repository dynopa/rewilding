using System.Collections.Generic;
using UnityEngine;

public class ItemHandler : MonoBehaviour
{
    //situational data
    public bool holdingItem;
    public float grabRange;
    public Item itemHeld;
    float whenDropped;//so you don't pick up the same thing instantly after

    //testing data
    public float upHelp;

    //private data
    Vector3 itemHeldOffset;

    //private values
    float vPos = 0.5f;
    float hPos = 1;
    public float throwForce = 30f;
    // Start is called before the first frame update
    void Awake()
    {
    }

    // Update is called once per frame
    void Update()
    {
       /* if(holdingItem)
        {
            if (Input.GetMouseButton(1) || Input.GetKey(KeyCode.Q))
            {
                Vector3 forceDirection = (cam.transform.forward + cam.transform.up * upHelp).normalized;
                Debug.DrawLine(itemHeld.transform.position, itemHeld.transform.position + forceDirection, Color.yellow);
                itemHeld.transform.position = transform.position + (cam.transform.up * vPos + cam.transform.right * 0f + cam.transform.forward).normalized;
                itemHeld.transform.forward = cam.transform.forward;
                if (Input.GetMouseButtonDown(0))//throw
                {
                    itemHeld.Throw();
                    holdingItem = false;
                    //Vector3 forceDirection = (cam.transform.forward + cam.transform.up * 0.5f).normalized;

                    itemHeld.rb.AddForce(forceDirection* throwForce, ForceMode.Impulse);
                    whenDropped = Time.time;
                }
            }
            else
            {
                itemHeld.transform.position = transform.position + (transform.up * vPos + transform.right * hPos + transform.forward).normalized;
                itemHeld.transform.forward = transform.forward;
                /*if (Input.GetMouseButtonDown(0))//drop
                {
                    DropItem();
                }
            }
        }
        else
        {
            if(itemHeld != null && Time.time > whenDropped+1f)
            {
                itemHeld = null;
            }
            if (Input.GetMouseButton(0) && itemHeld == null)
            {
                CheckInteraction();
            }
        }*/
    }
    public void DropItem()
    {
        itemHeld.PutDown();
        whenDropped = Time.time;
        holdingItem = false;
    }
    public void PickUpItem(Item item)
    {
        itemHeld = item;
        item.PickUp();
        holdingItem = true;
    }
    public void GiveItem(ItemHandler reciever)
    {
        holdingItem = false;
        reciever.PickUpItem(itemHeld);
    }
    public Item CheckNearby(Creature creature,ItemTrait trait = ItemTrait.Default)//for puffballs to pick stuff up
    {
        List<Item> items = new List<Item>();
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 8f);//LAYER MASK LATER
        foreach (Collider c in hitColliders)
        {
            if (c.transform == transform)
            {
                continue;
            }
            if (c.CompareTag("Item") || c.CompareTag("Food"))
            {
                Item item = c.GetComponent<Item>();
                if (!item.held)
                {
                    if(trait == ItemTrait.Default)
                    {
                        if (creature.mind.likes.Contains(item.trait))
                        {
                            items.Add(item);
                        }
                    }
                    else
                    {
                        if (item.trait == trait)
                        {
                            items.Clear();
                            items.Add(item);
                            break;
                        }
                    }
                }
            }
        }
        //for now just pick up the first one
        if(items.Count > 0)
        {
            return items[0];
        }
        else
        {
            return null;
        }
    }
    public void HoldItem(bool holdAbove = false)//physically keep the item on your person
    {
        itemHeld.transform.position = transform.position + (transform.up * vPos + transform.right * hPos + transform.forward).normalized;
        if (holdAbove)
        {
            itemHeld.transform.position = transform.position + (Vector3.up * vPos);
        }
        itemHeld.transform.forward = transform.forward;
    }
}
