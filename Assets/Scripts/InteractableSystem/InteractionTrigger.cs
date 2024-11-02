using UnityEngine;
using UnityEngine.UI;

public class InteractionTrigger : MonoBehaviour
{
    public GameObject interactionButtonPrefab;
    private GameObject interactionButton;

    private IInteractable currentInteractable;
    public float interactionDistance = 2.0f; // Interaction distance.
    private Transform playerTransform;

    private bool hasBeenUsed = false;

    void Start()
    {
        MonoBehaviour[] components = GetComponents<MonoBehaviour>();
        foreach (var component in components)
        {
            if (component is IInteractable interactable)
            {
                currentInteractable = interactable;
                break; 
            }
        }

        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        // Create the interaction button, default set to inactive.
        interactionButton = Instantiate(interactionButtonPrefab, transform);
        interactionButton.SetActive(false); 

        var interactionButtonScript = interactionButton.GetComponent<InteractionButton>();
        interactionButtonScript.interactionTrigger = GetComponent<InteractionTrigger>(); 
    }

    void Update()
    {
        if(!hasBeenUsed)
        {
            float distance = Vector2.Distance(playerTransform.position, transform.position);

            if (distance <= interactionDistance)
            {
                interactionButton.SetActive(true);
            }
            else
            {
                interactionButton.SetActive(false);
            }
        }
    }

    public void OnButtonClick()
    {
        hasBeenUsed = true;

        if (currentInteractable != null)
        {
            currentInteractable.OnInteraction();
        }
    }


    public void DisableButton()
    {
        interactionButton.SetActive(false);
    }
}
