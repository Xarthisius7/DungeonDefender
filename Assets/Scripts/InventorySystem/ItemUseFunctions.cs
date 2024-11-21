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



            ,{ 10, () => {

                PlayerController.Instance.UpdatePlayerHealth(30);
                UIManager.Instance.ShowMessage("After Consuming a healing potion, your regenerated 30 health!");
                EffectsManager.Instance.PlaySFX(5);

            } }

            ,{ 11, () => {

                PlayerController.Instance.UpdatePlayerHealth(60);
                UIManager.Instance.ShowMessage("After Consuming a healing potion, your regenerated 60 health!");
                EffectsManager.Instance.PlaySFX(5);


            } }

            ,{ 12, () => {

                PlayerController.Instance.UpdatePlayerHealth(100);
                UIManager.Instance.ShowMessage("After Consuming a healing potion, your regenerated 100 health!");
                EffectsManager.Instance.PlaySFX(5);


            } }

            ,{ 13, () => {

                PlayerController.Instance.UpdateCurrentPlayerStamina(50);
                UIManager.Instance.ShowMessage("After Consuming a Stamina potion, your regenerated 50 Stamina!");
                EffectsManager.Instance.PlaySFX(6);


            } }

            ,{ 14, () => {

                PlayerController.Instance.UpdateCurrentPlayerStamina(100);
                UIManager.Instance.ShowMessage("After Consuming a Stamina potion, your regenerated 100 Stamina!");
                EffectsManager.Instance.PlaySFX(6);


            } }

            ,{ 15, () => {

                PlayerController.Instance.UpdateCurrentPlayerStamina(150);
                UIManager.Instance.ShowMessage("After Consuming a Stamina potion, your regenerated 150 Stamina!");
                EffectsManager.Instance.PlaySFX(6);


            } }

            ,{ 19, () => {

                UIManager.Instance.ShowMessage("Carefuly reading through the scroll, it feels that you become much stronger!");

                Powerup p = PowerupManager.instance.LoadRandomPowerups();
                PowerupManager.instance.GivePowerup(p.id);

                EffectsManager.Instance.PlaySFX(11);


            } }

            ,{ 20, () => {
                GameController.Instance.PauseGame();

                UIManager.Instance.OpenItemMenuFunc(PowerupManager.instance.LoadRandomPowerups(),PowerupManager.instance.LoadRandomPowerups(),PowerupManager.instance.LoadRandomPowerups());


            } }





        };
    }



    void Start()
    {
        InitializeItemUseActions();
    }

}
