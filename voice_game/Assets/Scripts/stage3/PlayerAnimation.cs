using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AnimParam
{
    idle,
    attack,
    hurt,
    die,
    isRun,
    isJump,
    isKickBoard
}

public class PlayerAnimation : MonoBehaviour
{
    private Animator playerAnimator;

    private void Start()
    {
        playerAnimator = transform.GetChild(0).GetComponent<Animator>();
    }

    public void SetTrigger(AnimParam anim)
    {
        playerAnimator.SetTrigger(anim.ToString());
    }

    public void SetBool(AnimParam anim, bool value)
    {
        playerAnimator.SetBool(anim.ToString(), value);
    }
}
