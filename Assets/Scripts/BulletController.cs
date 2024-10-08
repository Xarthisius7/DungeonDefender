using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{

    private Vector3 mousePos;
    private Camera mainCam;
    private Rigidbody2D rb;
    public float force;

    void Start()
    {
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        rb = GetComponent<Rigidbody2D>();
        mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = mousePos - transform.position;
        Vector3 rotation = transform.position - mousePos;

        //set the velocity of bullet  - and normalized, where we get a 1; then * by force, 
        rb.velocity = new Vector2(direction.x, direction.y).normalized * force;

        float rot = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rot + 90);
    }
    void Update()
    {
        
    }


    void OnCollisionEnter2D(Collision2D collision)
    {

        //trigger the event: check the hit gameobject's type.
        if (collision.gameObject.CompareTag("Enemy"))
        {
            //collision.gameObject.GetComponent<SpriteRenderer>().color = Color.red;
            //collision.gameObject.GetComponent<AIChase>().destorySelf();

            collision.gameObject.GetComponent<SampleEnemy>().takeBulletDamage();

            rb.velocity = Vector2.zero;
            // Desctory the bullet gameobject
            Destroy(gameObject);
        }
        if (collision.gameObject.CompareTag("Player"))
        {
            //prevent it collde with player.
        }
        else
        {

            rb.velocity = Vector2.zero;
            // Desctory the bullet gameobject
            Destroy(gameObject);

        }


    }
}
