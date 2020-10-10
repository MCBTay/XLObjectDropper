using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorSwitcher : MonoBehaviour
{
    public GameObject character;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            character.GetComponent<Animator>().Play("Armature|CrouchingIdle");
        }
        if (Input.GetButtonDown("Fire2"))
        {
            character.GetComponent<Animator>().Play("Armature|StandingIdle");
        }
        if (Input.GetButtonDown("Fire3"))
        {
            character.GetComponent<Animator>().Play("Armature|TPose");
        }
    }
}
