using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayButton : MonoBehaviour
{
    SceneGameManager sceneGameManager;
    Button playButton;

    // Start is called before the first frame update
    void Start()
    {
        sceneGameManager = FindFirstObjectByType<SceneGameManager>();
        playButton = GetComponent<Button>();

        playButton.onClick.AddListener(() => {
            foreach (var item in FindObjectsOfType<PlayButton>())
            {
                item.GetComponent<Button>().onClick.RemoveAllListeners();
            };
            sceneGameManager.TransitionToGame();
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
