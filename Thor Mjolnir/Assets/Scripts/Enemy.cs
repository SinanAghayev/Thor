using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    private Animator anim;
    private Collider coll;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        coll = GetComponent<CapsuleCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        

        if (Input.GetKeyDown(KeyCode.X))
        {
            anim.SetBool("Death",false);
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "FwLightning")
        {
            anim.SetBool("Death",true);
        }
        
    }
}
