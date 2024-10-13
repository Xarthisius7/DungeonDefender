using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance { get; private set; }
    void Start()
    {

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void getItem(int itemID, int amount)
    {
        //player gets a certain amount of item with that ID
    }

    public void useItem(int slotNumber)
    {
        // will be called when player press a certain key refers to a certain slot.
        // will use the item. need to trigger its effect & update the ui
    }


}
