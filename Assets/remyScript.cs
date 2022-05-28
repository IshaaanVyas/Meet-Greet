using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class remyScript : MonoBehaviour
    

{
    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void resetASkingQ1()
    {
        animator.SetBool("askingQ1", false);
    }

    void resetThumbsUp1()
    {
        animator.SetBool("thumbsup1", false);
    }

    void resetTalking()
    {
        animator.SetBool("talking1", false);
    }
}
