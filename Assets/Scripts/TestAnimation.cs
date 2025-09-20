using UnityEngine;

public class TestAnimation : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        Animator anim = GetComponent<Animator>();
        if (anim != null)
        {
            anim.Play(anim.GetCurrentAnimatorStateInfo(0).shortNameHash);
        }

    }


}
