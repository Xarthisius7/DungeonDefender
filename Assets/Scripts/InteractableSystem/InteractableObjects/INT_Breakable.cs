using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class INT_Breakable : MonoBehaviour, IInteractable
{
    public Sprite DamagedSprite1;
    public Sprite DamagedSprite2;

    public int maxHits;
    private int hits;
    public int hitSound;
    public int breakSound;

    private SpriteRenderer spriteRenderer;
    void Awake()
    {
        hits = maxHits;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public bool TakeDamage(int damage)
    {
        hits -= damage;

        if (hits <= 0)
        {
            EffectsManager.Instance.PlaySFX(breakSound);
            MapManager.Instance.UpdateNavMesh();
            Destroy(gameObject);
            return true;
        }
        else
        {
            if (spriteRenderer != null)
            {
                if (hits < maxHits * 0.4f && DamagedSprite2 != null)
                {
                    spriteRenderer.sprite = DamagedSprite2;

                    Transform shadowBorder = transform.Find("ShadowBorder");
                    if (shadowBorder != null)
                    {
                        Destroy(shadowBorder.gameObject);
                    }
                    Transform shadowBorder2 = transform.Find("ShadowBorder2");
                    if (shadowBorder != null)
                    {
                        shadowBorder2.gameObject.SetActive(true);
                    }
                }
                else if (DamagedSprite1 != null)
                {
                    spriteRenderer.sprite = DamagedSprite1;
                }
            }
            EffectsManager.Instance.PlaySFX(hitSound);
            return false;
        }

    }

    public void OnInteraction()
    {
        Debug.Log("You tried to interact with a breakable object!");
        // TODO: Chest opening events

        //gameObject.GetComponent<SpriteRenderer>().sprite = openedSprite;


        //InteractionTrigger interactionTrigger = GetComponent<InteractionTrigger>();
        //if (interactionTrigger != null)
        //{
        //    // Call the DisableButton() method
        //    interactionTrigger.DisableButton();
        //}
    }
}
