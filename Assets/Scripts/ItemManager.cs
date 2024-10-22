using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance { get; private set; }

    public Tuple<ItemScriptableObject, int>[] inventory = new Tuple<ItemScriptableObject, int>[10];
    public WeaponItemScriptableObj equippedWeapon = null;

    private RectTransform rectTransform;
    [SerializeField] public GameObject InventorySlotPrefab;
    private GameObject[] inventorySlotsGO = new GameObject[11];

    // Spacing between inventory slots and offset for the equipped weapon slot
    [SerializeField] private float slotSpacing = 50f;
    [SerializeField] private float equippedSlotOffset = 100f;

    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            rectTransform = GetComponent<RectTransform>();

            // Instantiate the inventory slots
            InstantiateInventorySlots();
        }
        else
        {
            Destroy(gameObject);
        }

    }

    void InstantiateInventorySlots()
    {
        // Get the width of the panel (rectTransform)
        float panelWidth = rectTransform.rect.width;

        // Calculate slot width based on prefab's RectTransform size
        RectTransform prefabRect = InventorySlotPrefab.GetComponent<RectTransform>();
        float slotWidth = prefabRect.rect.width;

        // Calculate total available space for spacing between slots
        float totalSlots = 10;
        float totalSpacing = panelWidth - (totalSlots * slotWidth);

        // Calculate the spacing between each slot to distribute them evenly
        float spacingBetweenSlots = totalSpacing / (totalSlots - 1);

        // Define the initial position for the first inventory slot
        float startX = -(panelWidth / 2) + (slotWidth / 2);

        // Loop to instantiate 10 inventory slots within the panel
        for (int i = 0; i < totalSlots; i++)
        {
            // Instantiate the slot prefab and position it in the panel
            GameObject slotGO = Instantiate(InventorySlotPrefab, rectTransform);
            inventorySlotsGO[i] = slotGO;

            // Calculate the x position for this slot
            float xPos = startX + i * (slotWidth + spacingBetweenSlots);

            // Set the anchored position of the slot
            RectTransform slotRectTransform = slotGO.GetComponent<RectTransform>();
            slotRectTransform.anchoredPosition = new Vector2(xPos, 0);
        }

        // Instantiate and position the equipped weapon slot outside the panel (to the far left)
        GameObject equippedSlotGO = Instantiate(InventorySlotPrefab, rectTransform); // Parent it to the same canvas, not the inventory panel
        inventorySlotsGO[10] = equippedSlotGO;

        // Position the equipped weapon slot to the left of the inventory slots
        RectTransform equippedSlotRectTransform = equippedSlotGO.GetComponent<RectTransform>();
        equippedSlotRectTransform.anchoredPosition = new Vector2(startX - 1 * (slotWidth + spacingBetweenSlots), 0); // Offset to the left
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void getItem(ItemScriptableObject item)
    {
        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i].Item1 == item && inventory[i].Item2 < inventory[i].Item1.maxStack - 1)
            {
                inventory[i] = new Tuple<ItemScriptableObject, int>(item, inventory[i].Item2 + 1);
                Debug.Log($"{item} added successfully");
                return;
            }
        }

        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i] == null)
            {
                inventory[i] = new Tuple<ItemScriptableObject, int>(item, 1);
                Debug.Log($"{item} added successfully");
                return;
            }
        }
        Debug.Log($"Inventory is full");
    }

    public void useItem(int slotNumber)
    {
        if (inventory[slotNumber].Item1.use())
        {
            if (inventory[slotNumber].Item1 is WeaponItemScriptableObj)
            {
                if (equippedWeapon != null)
                {
                    WeaponItemScriptableObj equipped = equippedWeapon;
                    equippedWeapon = (WeaponItemScriptableObj)inventory[slotNumber].Item1;
                    inventory[slotNumber] = new Tuple<ItemScriptableObject, int>(equipped, 1);
                }

                else
                {
                    equippedWeapon = (WeaponItemScriptableObj)inventory[slotNumber].Item1;
                    inventory[slotNumber] = null;
                }
            }

            inventory[slotNumber] = new Tuple<ItemScriptableObject, int>(inventory[slotNumber].Item1, inventory[slotNumber].Item2 - 1);

            if(inventory[slotNumber].Item2 <= 0)
            {
                inventory[slotNumber] = null;
            }

            Debug.Log("Item used");
        }

        else
        {
            Debug.Log("Item could not be used");
        }
    }

    private void updateSlots()
    {

    }
}
