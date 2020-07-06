using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopulateGrid : MonoBehaviour
{
    public GameObject prefab; //This is the prefab object that is exposed in the editor

    public int numberToCreate; //Number of objects to create, exposed in editor

    // Start is called before the first frame update
    void Start()
    {
        Populate();
    }

    void Populate()
    {
        GameObject newObj; //Create GameObject instance

        for (int i = 0; i < numberToCreate; i++)
        {
            newObj = (GameObject)Instantiate(prefab, transform); //Create new instances of the prefab until as many as specified in numberToCreate have been made
            newObj.GetComponent<UnityEngine.UI.Image>().color = Random.ColorHSV();
        }
    }
}
