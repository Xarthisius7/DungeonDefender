using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float damage = 25f;
    void OnCollisionEnter2D(Collision2D collision){

        if (collision.gameObject.CompareTag("Player")){
            Debug.Log("Player was shot");
            PlayerController.Instance.PlayerTakesDamage(damage);
            Destroy(gameObject);
        }
        else{
            Destroy(gameObject);
        }
    }
}