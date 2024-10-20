using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction : MonoBehaviour
{
    // execute the shooting. will be a huge chunk of code.

    private Camera mainCam;
    private Vector3 mousePos;

    public GameObject bullet;
    public Transform bulletTransform;
    public GameObject rotationPoint;
    public bool canFire;
    private float timer;

    //the default fire speed.
    public float defaultFireSpeed;
    public float timeBetweenFiring;

    private void Start()
    {
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        timeBetweenFiring = defaultFireSpeed;
    }
    public void Shoot()
    {
        mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        //setting the varable to mouse Position
        Vector3 rotation = mousePos - transform.position;

        float rotZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
        rotationPoint.transform.rotation = Quaternion.Euler(0, 0, rotZ);
        //update the rotation.

        if (canFire)
        {
            canFire = false;
            Instantiate(bullet, bulletTransform.position, Quaternion.identity);
            //create the bullet object
            EffectsManager.Instance.PlaySFX(1);
        }
    }

    void Update()
    {
        //update the fire cooldown, even if the player is not shooting.
        if (!canFire)
        {
            timer += Time.deltaTime;
            //update the timer, and re-allow player to fire after some period of time.
            if (timer > timeBetweenFiring)
            {
                canFire = true;
                timer = 0;
            }
        }
    }
}
