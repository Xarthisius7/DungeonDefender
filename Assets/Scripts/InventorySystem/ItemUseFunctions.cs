using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemUseFunctions : MonoBehaviour
{
    // This class stores all the use() functions of different types of id items.


    // The editable list of all the items (and their IDs):
    // https://docs.google.com/spreadsheets/d/1KaqWPhWwRt81qpsXpCp1f_tODKXNXuQClTnDTaj015U/edit?usp=sharing
    
    
    public Dictionary<int, Action> itemUseActions;


    private void InitializeItemUseActions()
    {
        itemUseActions = new Dictionary<int, Action>
        {
            { 1, () => {
                
                
                Debug.Log("Using Health Potion: Healing for 10 HP.");

                PlayerController.Instance.UpdatePlayerHealth(10);




            } },


            { 2, () => { Debug.Log("Using Mana Potion: Restoring 30 mana.");

            PlayerController.Instance.UpdateCurrentPlayerStamina(30);
            
            
            
            
            } },
            { 3, () => { Debug.Log("The function of item can be nothing. Since the sctuature is a dictionary, you can even remove this!");  } },
            { 4, () => { Debug.Log("(to be added....)"); } }






        };
    }



    void Start()
    {
        InitializeItemUseActions();
    }

}
