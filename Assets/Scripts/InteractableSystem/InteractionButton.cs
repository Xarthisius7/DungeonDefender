using UnityEngine;

public class InteractionButton : MonoBehaviour
{
    public InteractionTrigger interactionTrigger; 
    public float clickRadius = 0.5f;

    void OnMouseDown()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = transform.position.z;

        if (Vector2.Distance(mousePosition, transform.position) <= clickRadius)
        {
            interactionTrigger?.OnButtonClick();
        }
    }
}
