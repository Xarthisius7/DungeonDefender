using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Required to manipulate UI elements
using TMPro; // Required to manipulate UI elements

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance { get; private set; }

    public Tuple<ItemScriptableObject, int>[] inventory = new Tuple<ItemScriptableObject, int>[10];
    public WeaponItemScriptableObj equippedWeapon = null;

    [SerializeField] public GameObject InventorySlotPrefab;
    [SerializeField] public GameObject ItemPrefab;
    private GameObject[] inventorySlotsGO = new GameObject[11];
    private RectTransform inventoryPanel;

    // Offset for the equipped weapon slot
    [SerializeField] private float equippedSlotOffset = 100f;

    [SerializeField] ItemScriptableObject[] triesItemsAdd = new ItemScriptableObject[0];

    [SerializeField] Sprite slotSprite;
    [SerializeField] Sprite slotSelectedSprite;

    // Variable to keep track of the currently selected slot
    private int selectedSlot = -1;  // -1 means no slot is selected initially

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Subscribe to the sceneLoaded event
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Unsubscribe from the sceneLoaded event to avoid memory leaks
    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Try to find the InventoryPanel in the Canvas
        FindInventoryPanel();

        // Instantiate the inventory slots if the panel was found
        if (inventoryPanel != null)
        {
            InstantiateInventorySlots();
        }

        //Sample Item add for debug
        foreach(var item in triesItemsAdd)
        {
            getItem(item);
        }
    }

    private void FindInventoryPanel()
    {
        // Find the Canvas in the scene
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas != null)
        {
            // Find the InventoryPanel within the Canvas
            Transform panelTransform = canvas.transform.Find("InventoryPanel");
            if (panelTransform != null)
            {
                inventoryPanel = panelTransform.GetComponent<RectTransform>();
            }
            else
            {
                Debug.LogError("InventoryPanel not found in Canvas!");
            }
        }
        else
        {
            Debug.LogError("Canvas not found in the scene!");
        }
    }

    void InstantiateInventorySlots()
    {
        // Get the width of the panel (inventoryPanel)
        float panelWidth = inventoryPanel.rect.width;

        // Calculate slot width based on prefab's RectTransform size
        RectTransform prefabRect = InventorySlotPrefab.GetComponent<RectTransform>();
        float slotWidth = prefabRect.rect.width;

        // Calculate total available space for spacing between slots
        float totalSlots = inventory.Length;
        float totalSpacing = panelWidth - (totalSlots * slotWidth);

        // Calculate the spacing between each slot to distribute them evenly
        float spacingBetweenSlots = totalSpacing / (totalSlots - 1);

        // Define the initial position for the first inventory slot
        float startX = -(panelWidth / 2) + (slotWidth / 2);

        // Loop to instantiate 10 inventory slots within the panel
        for (int i = 0; i < totalSlots; i++)
        {
            // Instantiate the slot prefab and position it in the panel
            GameObject slotGO = Instantiate(InventorySlotPrefab, inventoryPanel);
            inventorySlotsGO[i] = slotGO;

            // Calculate the x position for this slot
            float xPos = startX + i * (slotWidth + spacingBetweenSlots);

            // Set the anchored position of the slot
            RectTransform slotRectTransform = slotGO.GetComponent<RectTransform>();
            slotRectTransform.anchoredPosition = new Vector2(xPos, 0);
        }

        // Instantiate and position the equipped weapon slot outside the panel (to the far left)
        GameObject equippedSlotGO = Instantiate(InventorySlotPrefab, inventoryPanel); // Parent it to the same canvas, not the inventory panel
        inventorySlotsGO[10] = equippedSlotGO;

        // Position the equipped weapon slot to the left of the inventory slots
        RectTransform equippedSlotRectTransform = equippedSlotGO.GetComponent<RectTransform>();
        equippedSlotRectTransform.anchoredPosition = new Vector2(startX - 1 * (slotWidth + spacingBetweenSlots), 0); // Offset to the left

        updateSlots();
    }

    // Update inventory UI slots with item data
    private void updateSlots()
    {
        // First pass to clear any existing "Item" instances
        for (int i = 0; i < inventorySlotsGO.Length; i++)
        {
            GameObject slotGO = inventorySlotsGO[i];
            RectTransform slotRect = slotGO.GetComponent<RectTransform>();

            // Remove all existing "Item" children to avoid duplicates
            foreach (Transform child in slotRect)
            {
                if (child.name.Contains("Item"))
                {
                    Destroy(child.gameObject);
                }
            }
        }

        // Second pass to update slots with the current inventory data
        for (int i = 0; i < inventorySlotsGO.Length; i++)
        {
            GameObject slotGO = inventorySlotsGO[i];
            RectTransform slotRect = slotGO.GetComponent<RectTransform>();

            // Check if there's an item in the inventory at this index
            ItemScriptableObject item = (i < inventory.Length) ? inventory[i]?.Item1 : equippedWeapon;

            if (item != null)
            {
                // Instantiate a new child RectTransform for the item using a prefab
                GameObject itemGO = Instantiate(ItemPrefab, slotRect);

                // Configure the RectTransform
                RectTransform itemRect = itemGO.GetComponent<RectTransform>();
                itemRect.sizeDelta = new Vector2(80, 80);
                itemRect.anchoredPosition = Vector2.zero; // Center it in the parent slot

                // Set the sprite of the item
                Image itemImage = itemGO.GetComponent<Image>();
                itemImage.sprite = item.sprite; // Assuming the ScriptableObject has a 'sprite' field
                itemImage.preserveAspect = true;
            }

            // Update the item count if applicable
            TextMeshProUGUI counter = slotRect.Find("Count")?.GetComponent<TextMeshProUGUI>();
            int itemCount = (i < inventory.Length && inventory[i] != null) ? inventory[i].Item2 : (equippedWeapon != null ? 1 : 0);

            if (counter != null && itemCount != -1)
            {
                counter.text = (itemCount > 1) ? itemCount.ToString() : "";
            }
            else if (counter != null)
            {
                counter.text = "";
            }
        }

        // Update slot appearance based on selection
        for (int i = 0; i < inventorySlotsGO.Length; i++)
        {
            GameObject slotGO = inventorySlotsGO[i];
            Image slotImage = slotGO.GetComponent<Image>();

            // Set the appropriate sprite for the slot based on selection
            slotImage.sprite = (i == selectedSlot) ? slotSelectedSprite : slotSprite;
        }
    }


    // Method to set the selected slot and update UI
    public void SetSelectedSlot(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < inventorySlotsGO.Length)
        {
            selectedSlot = slotIndex;
            updateSlots(); // Refresh the UI to show the selection change
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Here you can add input handling to change the selected slot, for example:
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            SetSelectedSlot(Mathf.Max(0, selectedSlot - 1)); // Move selection to the left
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            SetSelectedSlot(Mathf.Min(inventorySlotsGO.Length - 1, selectedSlot + 1)); // Move selection to the right
        }
        else if (Input.GetKeyDown(KeyCode.P))
        {
            useItem(selectedSlot);
            Debug.Log("enter");
        }
    }

    public void getItem(ItemScriptableObject item)
    {
        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i]?.Item1 == item && inventory[i].Item2 < inventory[i].Item1.maxStack)
            {
                inventory[i] = new Tuple<ItemScriptableObject, int>(item, inventory[i].Item2 + 1);
                Debug.Log($"{item} added successfully");
                updateSlots(); // Refresh the UI when an item is added
                return;
            }
        }

        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i] == null)
            {
                inventory[i] = new Tuple<ItemScriptableObject, int>(item, 1);
                Debug.Log($"{item} added successfully");
                updateSlots(); // Refresh the UI when an item is added
                return;
            }
        }

        Debug.Log($"Inventory is full");
    }

    public void useItem(int slotNumber)
    {
        if (inventory[slotNumber]?.Item1.use() == true)
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
            else
            {
                inventory[slotNumber] = new Tuple<ItemScriptableObject, int>(inventory[slotNumber].Item1, inventory[slotNumber].Item2 - 1);

                if (inventory[slotNumber]?.Item2 <= 0)
                {
                    inventory[slotNumber] = null;
                }
            }

            Debug.Log("Item used");
            updateSlots(); // Refresh the UI when an item is used
        }
        else
        {
            Debug.Log("Item could not be used");
        }
    }
}
