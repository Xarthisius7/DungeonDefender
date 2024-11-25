using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction : MonoBehaviour
{
    // execute the shooting. will be a huge chunk of code.


    public GameObject bullet;
    public Transform bulletTransform;
    public GameObject rotationPoint;
    public GameObject crossHairCenter;

    public LineRenderer lineRenderer;
    public float dashSize = 0.1f; // size of the aimpoint's line

    public bool canFire;
    private float timer;

    //the default fire speed.
    private float defaultFireSpeed = 0.7f;
    private float timeBetweenFiring;


    private void Start()
    {
        timeBetweenFiring = defaultFireSpeed;

        lineRenderer.startWidth = 0.02f;
        lineRenderer.endWidth = 0.01f;
        lineRenderer.textureMode = LineTextureMode.Tile;

        lineRenderer.sortingLayerName = "InGameUI"; 
        lineRenderer.sortingOrder = 10; 
    }
    public void Shoot(Vector3 mousePos)
    {
        //setting the varable to mouse Position
        Vector3 rotation = mousePos - transform.position;

        UpdateCrossHair(rotation, mousePos);



        if (canFire)
        {
            float currentStatsShotSpeed = (PowerupManager.instance.GetAttributeValue("AttackSpeed") * 0.7f);
            
            timeBetweenFiring = Mathf.Max(0.05f, 1f / (currentStatsShotSpeed + 1f));

            canFire = false;

            Instantiate(bullet, bulletTransform.position, Quaternion.Euler(0, 0, 0));
            //create the bullet object
            if (UnityEngine.Random.value <= 0.5f) 
            {
                EffectsManager.Instance.PlaySFX(37);
            }
            else
            {
                EffectsManager.Instance.PlaySFX(3);
            }


            if (PowerupManager.instance.IsMagicActive(15))
            {
                Invoke("ExtraShoot", 0.1f);
            }

        } else
        {
        }
    }

    private void ExtraShoot()
    {

        Instantiate(bullet, bulletTransform.position, Quaternion.Euler(0, 0, 0));
        //create the bullet object
        if (UnityEngine.Random.value <= 0.5f)
        {
            EffectsManager.Instance.PlaySFX(37);
        }
        else
        {
            EffectsManager.Instance.PlaySFX(3);
        }
    }

    void UpdateCrossHair(Vector3 rotation, Vector3 mousePos)
    {
        float rotZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
        rotationPoint.transform.rotation = Quaternion.Euler(0, 0, rotZ);
        //update the rotation of the Aim point.
        lineRenderer.SetPosition(0, transform.position); // Starts from player's location
        lineRenderer.SetPosition(1, mousePos); // end point set to player's moust location
        UpdateDottedLine(transform.position, mousePos);

        //update crosshair's center's position:
        crossHairCenter.transform.position = mousePos;

        // if is shooting, set the rotation point being visiable
        rotationPoint.SetActive(true);
    }

    void UpdateDottedLine(Vector3 start, Vector3 end)
    {
        float distance = Vector3.Distance(start, end);
        lineRenderer.material.mainTextureScale = new Vector2(distance / dashSize, 1); 
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
