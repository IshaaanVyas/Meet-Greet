using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class person2Script : MonoBehaviour
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

    void resetClapping()
    {
        animator.SetBool("clapping", false);
    }

    void resetCheering()
    {
        animator.SetBool("cheering", false);
    }

    void resetTalking()
    {
        animator.SetBool("talking2", false);
    }
}
