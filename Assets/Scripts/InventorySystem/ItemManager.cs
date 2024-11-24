using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Required to manipulate UI elements
using TMPro; // Required to manipulate UI elements
using UnityEditor;
using System.Linq;
using System.IO;

public class ItemSlot
{
    public ItemScriptableObject item; // The item in the slot
    public int quantity; // The quantity of the item

    public ItemSlot(ItemScriptableObject item, int quantity)
    {
        this.item = item;
        this.quantity = quantity;
    }
}


public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance { get; private set; }


    // Variable to keep track of the currently selected slot
    private int selectedSlot = 0;  // -1 means no slot is selected initially..  .. But WHY? just let player select slot 0 at the start of the game! (Message From Dash.)
    [SerializeField] private float scrollSensitivity = 1.0f;  // how many scroll input per 1 item slot change.
    private float scrollAccumulated = 0f;
    private int totalSlots = 10;

    public List<ItemScriptableObject> allItems = new List<ItemScriptableObject>(); 
    public List<ItemSlot> inventory = new List<ItemSlot>();
    public ItemSlot EqupimentSlot = new ItemSlot(null, 0);
    [SerializeField] private GameObject SlotDisplayerPrefab;
    [SerializeField] private Transform SlotDisplayInitialTransform;
    private float slotDisplayDistance  = 72f;
    private List<GameObject> slotDisplayers = new List<GameObject>();

    [SerializeField] public GameObject InventoryHighLightedFrame;
    [SerializeField] public Transform IHFInitialPosition;

    private float cooldownTime = 0.5f; // Cooldown duration in seconds
    private float lastUseTime = -0.5f; // Timestamp of the last use, initialized to allow immediate use


    [SerializeField] public Image EquipmentSlotDisplaySprite;
    [SerializeField] private ItemUseFunctions itemFunctions;

    [SerializeField] private GameObject droppedItemPrefab;
    [SerializeField] private Transform playerTransform;

    private List<int> equipmentPool;
    private List<int> consumeablePool;




    void Awake()
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

        // setup the slots for display. it's not the inventory, just the ui.
        GenerateSlotDisplayers();
        InventoryHighLightedFrame.transform.position = IHFInitialPosition.position;

        for (int i = 0; i < totalSlots; i++)
        {
            inventory.Add(new ItemSlot(null,0)); // ini every slot to null.
        }

        for (int i = 0; i < totalSlots; i++)
        {
            SetSlotDisplay(i,5, null);
        }

        LoadAllItems();

        equipmentPool =  new List<int> {30,31,32,33};
        consumeablePool =  new List<int> { 10,11,12,13,14,15 };

        UpdateInventoryDisplay();

        //CreateDroppedItem(2, 3);

    }
    public bool ConsumeItemById(int targetId)
    {
        // check if player have certain id's item. if he does, return true and consume it.
        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i].item == null) continue;
            if (inventory[i].item.id == targetId)
            {
                inventory[i].item = null;
                UpdateInventoryDisplay();
                return true;
            }
        }
        return false;
    }


    public int GetRandomEquipmentID()
    {
        if (equipmentPool == null || equipmentPool.Count == 0)
        {
            Debug.LogWarning("The equipment pool is empty or not initialized!");
            return -1; 
        }

        int randomIndex = UnityEngine.Random.Range(0, equipmentPool.Count);
        return equipmentPool[randomIndex];
    }

    public int GetRandomConsumeableID()
    {
        if (consumeablePool == null || consumeablePool.Count == 0)
        {
            Debug.LogWarning("The consumeable pool is empty or not initialized!");
            return -1;
        }

        int randomIndex = UnityEngine.Random.Range(0, consumeablePool.Count);
        return consumeablePool[randomIndex];
    }




    // Loading All the items from Prefabs/Items 
    private void LoadAllItems()
    {
        string path = "Assets/Resources/Prefabs/Items";
        string[] assetGUIDs = AssetDatabase.FindAssets("t:ItemScriptableObject", new[] { path });

        allItems.Clear();
        foreach (string guid in assetGUIDs)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            ItemScriptableObject item = AssetDatabase.LoadAssetAtPath<ItemScriptableObject>(assetPath);
            if (item != null)
            {
                allItems.Add(item);
            }
        }
        Debug.Log($"[Item system]Loaded {allItems.Count} items from {path}.");

    }


    // Adding items to player base on id.
    public bool AddItemsById(int id, int quantity)
    {
        ItemScriptableObject item = allItems.FirstOrDefault(i => i.id == id);
        if (item == null)
        {
            Debug.LogError("Item with specified ID not found.");
            return false;
        }

        while (quantity > 0)
        {
            bool added = false;

            // Try stacking item
            if (item.maxStack > 1)
            {
                for (int i = 0; i < inventory.Count; i++)
                {
                    if (inventory[i].item != null && inventory[i].item == item && inventory[i].quantity < item.maxStack)
                    {
                        int spaceLeft = item.maxStack - inventory[i].quantity;
                        int addQuantity = Mathf.Min(spaceLeft, quantity);
                        inventory[i].quantity += addQuantity;
                        if (inventory[i].item.itemName == null) inventory[i].item.itemName = "";
                        UIManager.Instance.ShowMessage("+ " + addQuantity + " " + inventory[i].item.itemName);
                        quantity -= addQuantity;
                        added = true;
                        break;
                    }
                }
            }

            // If it cann't be stacked or no space, add it to a new slot.
            if (!added)
            {
                for (int i = 0; i < inventory.Count; i++)
                {
                    if (inventory[i].item == null)
                    {
                        int addQuantity = Mathf.Min(item.maxStack, quantity);
                        inventory[i] = new ItemSlot(item, addQuantity);
                        UIManager.Instance.ShowMessage(("+ " + addQuantity + " " + inventory[i].item.itemName));
                        quantity -= addQuantity;
                        added = true;
                        break;
                    }
                }
            }

            if (!added)
            {
                //if still didnt add this round - all the slot is full; no avaliable new slot.

                UIManager.Instance.ShowMessage("Inventory full! item has dropped to the ground...");

                CreateDroppedItem(id,quantity);
                return false;
            }
        }

        UpdateInventoryDisplay();

        return true;
    }


    private void UpdateInventoryDisplay()
    {
        if(EqupimentSlot.item == null)
        {
            EquipmentSlotDisplaySprite.gameObject.SetActive(false);
        } else
        {
            EquipmentSlotDisplaySprite.gameObject.SetActive(true);
            EquipmentSlotDisplaySprite.sprite = EqupimentSlot.item.sprite;
        }

        for (int i = 0; i < totalSlots;  i++)
        {
            if (inventory[i].item != null)
            {
                SetSlotDisplay(i, inventory[i].quantity, inventory[i].item.sprite);
            }else
            {
                SetSlotDisplay(i, 0, null);
            }
       
        }
    }

    private void UpdateSelectedSlot(int selectedSlot)
    {
        Vector3 position = InventoryHighLightedFrame.transform.position;
        position.x = IHFInitialPosition.position.x + 72 * selectedSlot;

        InventoryHighLightedFrame.transform.position = position;    }


    private void GenerateSlotDisplayers()
    {
        for (int i = 0; i < totalSlots; i++)
        {
            Vector3 spawnPosition = SlotDisplayInitialTransform.position + new Vector3(i * slotDisplayDistance, 0, 0);

            GameObject slotDisplayer = Instantiate(SlotDisplayerPrefab, spawnPosition, Quaternion.identity, SlotDisplayInitialTransform);
            slotDisplayers.Add(slotDisplayer);
        }
    }

    public void SetSlotDisplay(int i, int count, Sprite sp)
    {
        if (i < 0 || i >= slotDisplayers.Count)
        {
            Debug.LogError("Invalid slot index");
            return;
        }

        GameObject slotDisplayer = slotDisplayers[i];

        Transform spriteImageTransform = slotDisplayer.transform.Find("SpriteImage");
        if (spriteImageTransform != null)
        {
            Image imageComponent = spriteImageTransform.GetComponent<Image>();
            
            if (imageComponent != null)
            {
                if (sp == null)
                {
                    spriteImageTransform.gameObject.SetActive(false);
                }
                else
                {
                    spriteImageTransform.gameObject.SetActive(true);
                    imageComponent.sprite = sp;
                }
            }
            else
            {
                Debug.LogError("Image component not found on SpriteImage object");
            }
        }
        else
        {
            Debug.LogError("SpriteImage child object not found");
        }

        Transform countsTransform = slotDisplayer.transform.Find("Counts");
        if (countsTransform != null)
        {
            TextMeshProUGUI textComponent = countsTransform.GetComponent<TextMeshProUGUI>();
           
             if (textComponent != null)
            {
                if (count == 0 || count == 1)
                {
                    textComponent.text = "";
                } else
                {
                    textComponent.text = count.ToString();
                }
            }
            else
            {
                Debug.LogError("TextMeshProUGUI component not found on Counts object");
            }
        }
        else
        {
            Debug.LogError("Counts child object not found");
        }
    }


    public void CreateDroppedItem(int id, int amount)
    {
        CreateDroppedItem(id, amount, playerTransform);
    }

    public void CreateDroppedItem(int id, int amount, Transform spawnTransform)
    {
        // Instantiate the dropped item at the specified transform's position
        GameObject droppedItem = Instantiate(droppedItemPrefab, spawnTransform.position, Quaternion.identity);

        // Find the INT_DroppedItem component on the instantiated object
        INT_DroppedItem itemScript = droppedItem.GetComponent<INT_DroppedItem>();

        // Check if the INT_DroppedItem component exists
        if (itemScript != null)
        {
            // Call the setItem method with id and amount as parameters
            ItemScriptableObject itemTemplate = allItems.FirstOrDefault(i => i.id == id);
            if (itemTemplate == null)
            {
                Debug.LogError($"Item with specified ID {id} not found.");
            }
            else
            {
                ItemScriptableObject newItem = Instantiate(itemTemplate);
                itemScript.setItem(newItem, amount);
            }
        }
        else
        {
            Debug.LogError("INT_DroppedItem component not found on the dropped item prefab.");
        }
    }



    public void EquipSlot(int i)
    {
        // Check if the index is within the range of the inventory
        if (i < 0 || i >= inventory.Count || inventory[i] == null || inventory[i].item == null)
        {
            Debug.LogError("Invalid inventory slot index or empty slot.");
            return;
        }

        EffectsManager.Instance.PlaySFX(10);
        UIManager.Instance.ShowMessage("You equiped: " + inventory[i].item.itemName);
        // Swap the item and quantity between EquipmentSlot and inventory[i] without swapping the objects
        ItemScriptableObject tempItem = EqupimentSlot.item;
        int tempQuantity = EqupimentSlot.quantity;

        EqupimentSlot.item = inventory[i].item;
        EqupimentSlot.quantity = inventory[i].quantity;

        inventory[i].item = tempItem;
        inventory[i].quantity = tempQuantity;

        UpdateInventoryDisplay();
    }




    // Update is called once per frame
    void Update()
    {
        if (GameController.Instance.IsPaused) return;


        Vector2 scrollValue = InputManager.scrollValue;
        float UseKeyDown = InputManager.IsUsingItem;
        float DropKeyDown = InputManager.IsDropingItem;


        if (scrollValue != Vector2.zero)
        {
            scrollAccumulated -= (scrollValue.y > 0 ? 1 : -1);

            if (Mathf.Abs(scrollAccumulated) >= scrollSensitivity)
            {
                if (scrollAccumulated > 0)
                {
                    int newslot = (selectedSlot + 1);
                    if (newslot > totalSlots-1)
                    {
                        selectedSlot = 0;
                    }
                    else
                    {
                        selectedSlot = newslot;
                    }
                    scrollAccumulated -= scrollSensitivity;
                }
                else if (scrollAccumulated < 0)
                {
                    int newslot = (selectedSlot - 1);
                    if(newslot < 0)
                    {
                        selectedSlot = totalSlots-1;
                    }
                    else
                    {
                        selectedSlot = newslot;
                    }
                    scrollAccumulated += scrollSensitivity;
                }
            }
            UpdateSelectedSlot(selectedSlot);
        } else if (UseKeyDown >0)
        {
            // Check if cooldown has passed
            if (Time.time >= lastUseTime + cooldownTime)
            {
                UseItemAtSlot(selectedSlot);


                lastUseTime = Time.time; // Update last use time
            }

        } else if(DropKeyDown > 0)
        {
            if (Time.time >= lastUseTime + cooldownTime)
            {
                DropItem(selectedSlot);
                lastUseTime = Time.time; // Update last use time
            }
        }

    }

    public void UseItemAtSlot(int i)
    {
        if (inventory[i].item == null && EqupimentSlot.item == null) return;

        // Check if the inventory slot is empty but EquipmentSlot has an item
        if (inventory[i].item == null && EqupimentSlot.item != null)
        {
            // Move the item from EquipmentSlot to the empty inventory slot
            inventory[i].item = EqupimentSlot.item;
            inventory[i].quantity = EqupimentSlot.quantity;

            // Clear the EquipmentSlot
            EqupimentSlot.item = null;
            EqupimentSlot.quantity = 0;

            UpdateInventoryDisplay();
            return; 
        } else if ((inventory[i].item is ConsumableItem))
        {
            HandleItemUse(inventory[i].item.id);// trigger item's use ability.

            inventory[i].quantity -= 1;
            if(inventory[i].quantity <=0)
            {
                inventory[i].item = null;
            }
            //remove the item if the slot is empty


            UpdateInventoryDisplay();
            //Finally, update display.

        } else if(inventory[i].item is AttributeBoostItem)
        {
            EquipSlot(selectedSlot);
        }


    }

    public bool DropItem(int index)
    {
        if (inventory[index].item == null)
        {
            return false; // cant drop item if that slot is empty
        }
        else
        {

            CreateDroppedItem(inventory[index].item.id, inventory[index].quantity);

            inventory[index].item = null;
            inventory[index].quantity = 0;

            UpdateInventoryDisplay();
            return true;
        }
    }



    public bool HandleItemUse(int id)
    {
        if (itemFunctions.itemUseActions.ContainsKey(id))
        {
            itemFunctions.itemUseActions[id].Invoke();
            return true;
        }
        else
        {
            Debug.LogError("Item use not defined for this ID: " + id);
            return false;
        }
    }


}
