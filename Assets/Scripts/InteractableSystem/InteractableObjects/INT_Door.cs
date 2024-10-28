using UnityEngine;

public class INT_Door : MonoBehaviour, IInteractable
{
    public Animator animator;
    private const string playAnimation = "Play";
    void start()
    {
        //animator = gameObject.GetComponent<Animator>();
        //if (animator == null)
        //{
        //    Debug.LogError("Animator component not found on the object!");
        //}
    }
    public void OnInteraction()
    {
        Debug.Log("You opened the DOOR!");
        // TODO: DOOR opening events

        animator.SetBool(playAnimation, true);
    }
}
