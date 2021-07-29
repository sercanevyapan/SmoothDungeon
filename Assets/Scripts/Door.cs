using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Collidable
{
    private Animator anim;
    protected override void Start()
    {
        base.Start();
        anim = GetComponent<Animator>();
    }
    protected override void OnCollide(Collider2D coll)
    {
        if (coll.name == "Player")
            anim.SetTrigger("door_open");
    }
}
