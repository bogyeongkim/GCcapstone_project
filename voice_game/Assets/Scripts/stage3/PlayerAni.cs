using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AnimParam2
{
    idle,
    attack,
    hurt,
    die,
    isRun,
    isJump,
    isKickBoard
}

public class PlayerAnin : MonoBehaviour
{
    // [추가됨] 인스펙터에서 에셋으로 만든 오버라이드 컨트롤러를 여기에 할당하세요.
    public AnimatorOverrideController baseOverrideController;

    private Animator playerAnimator;

    private void Awake()
    {
        // 1. 애니메이터 컴포넌트 가져오기 (기존 로직 유지)
        playerAnimator = transform.GetChild(0).GetComponent<Animator>();
        if (playerAnimator == null)
        {
            playerAnimator = GetComponent<Animator>();
        }

        // 2. [핵심 수정] 오버라이드 컨트롤러가 할당되어 있다면, 복사본(Instance)을 만들어 적용
        if (baseOverrideController != null)
        {
            // 에셋 원본(base)을 기반으로 메모리에 새로운 복사본 생성
            AnimatorOverrideController runtimeController = new AnimatorOverrideController(baseOverrideController);

            // 복사본을 애니메이터에 장착
            playerAnimator.runtimeAnimatorController = runtimeController;

            // Debug.Log("오버라이드 컨트롤러가 런타임 인스턴스로 교체되었습니다.");
        }
    }

    public void SetTrigger(AnimParam anim)
    {
        if (playerAnimator != null)
            playerAnimator.SetTrigger(anim.ToString());
    }

    public void SetBool(AnimParam anim, bool value)
    {
        if (playerAnimator != null)
            playerAnimator.SetBool(anim.ToString(), value);
    }
}