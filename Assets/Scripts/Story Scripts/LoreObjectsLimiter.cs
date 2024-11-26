using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoreObjectsLimiter : MonoBehaviour
{
    public GameObject[] LoreNotes;
    public int LimitNbNotes; //Sets the max number of Notes in the map except for the one in the tutorial (The one in the tutorial will remain untagged to avoid been removed by accident)


    private int[] listRandomIndexes;
    private int ObjectIndex;
    private int finalNbNotes;

    private bool toDestroy;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(2f);

        if (LoreNotes.Length == 0)
        {
            LoreNotes = GameObject.FindGameObjectsWithTag("LoreNote");
            Debug.Log("List of Objects with tag created");
        }
        else
            Debug.Log("List of Obejct with tag NOT created");

        Debug.Log("There are " + LoreNotes.Length + " in the map before Limit");

        if (LimitNbNotes < LoreNotes.Length)
            listRandomIndexes = GetListofRandomIndexes(LoreNotes.Length);
        else if (LimitNbNotes == LoreNotes.Length)
            listRandomIndexes = new int[1] { -1 };
        else
        {
            listRandomIndexes = new int[1] { -1 };
            Debug.Log("Limit of Notes is BIGGER than nb notes in map");
        }

        Debug.Log("List of random indexes created");
        ObjectIndex = 1;
        foreach (GameObject note in LoreNotes)
        {
            toDestroy = true;
            for (int i = 0; i < listRandomIndexes.Length; i++)
            {
                if (ObjectIndex == listRandomIndexes[i])
                    toDestroy = false;
            }

            if (toDestroy)
                Destroy(note);
            ObjectIndex++;
        }

        finalNbNotes = 0;
        foreach (GameObject note in LoreNotes)
        {
            if (note != null)
                finalNbNotes++;
        }


        Debug.Log("There are " + finalNbNotes + " in the map after Limit");
    }

    private int[] GetListofRandomIndexes(int maxIndexValue)
    {
        int[] RNG = new int[LimitNbNotes];
        int randomIndex;
        bool UniqueIndex;

        for (int i = 0; i < RNG.Length;)
        {
            randomIndex = Random.Range(1, maxIndexValue + 1);
            UniqueIndex = true;

            for (int j = 0; j < RNG.Length; j++)
            {
                if (randomIndex == RNG[j])
                    UniqueIndex = false;
            }

            if (UniqueIndex)
            {
                RNG[i] = randomIndex;
                i++;
            }
        }

        return RNG;

    }
}
