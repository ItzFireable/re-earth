using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//https://answers.unity.com/questions/444500/how-to-get-current-animation-name.html

public class PlayerAnimationFix : MonoBehaviour
{
    [SerializeField] private Animator animator;
    AnimatorClipInfo[] animatorinfo;
    AnimationClip current_animation;

    [SerializeField] private AnimationClip[] attackAimations;


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        animatorinfo = this.animator.GetCurrentAnimatorClipInfo(0);
        if(current_animation != animatorinfo[0].clip)
        {
            current_animation = animatorinfo[0].clip;
            foreach (AnimationClip clip in attackAimations)
            {
                if (clip.Equals(current_animation))
                {
                    StartCoroutine("ChangeSize");
                }
            }

            //if(attackAimations.Find(current_animation))
                
        }
        
    }

    IEnumerator ChangeSize()
    {
        transform.position = new Vector3(transform.position.x + 1.25f, transform.position.y, transform.position.z);
        yield return new WaitForSeconds(current_animation.length);
        transform.position = new Vector3(transform.position.x - 1.25f, transform.position.y, transform.position.z);
    }
}
