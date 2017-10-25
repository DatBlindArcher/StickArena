using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public KeyCode attack;
    private Animation anim;

    private void Start()
    {
        anim = GetComponent<Animation>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(attack))
        {
            anim.Stop();
            anim.Play();
        }
    }
}