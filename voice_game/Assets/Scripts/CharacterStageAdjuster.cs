using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterStageAdjuster : MonoBehaviour
{
    public Collider2D characterCollider; // 캐릭터의 Collider2D 컴포넌트
    public Rigidbody2D characterRigidbody; // 캐릭터의 Rigidbody2D 컴포넌트
    public Vector3 scaleForStage1 = new Vector3(1.5f, 1.5f, 1.5f); // Stage1에서의 캐릭터 스케일
    private Vector3 defaultScale; // 캐릭터의 기본 스케일 값
    private float defaultGravityScale; // 캐릭터의 기본 중력 스케일 값

    private Player playerScript; 

    void Start()
    {
        defaultScale = transform.localScale; // 시작할 때 캐릭터의 현재 스케일을 기본 스케일 값으로 저장
        defaultGravityScale = characterRigidbody.gravityScale; // 시작할 때 캐릭터의 현재 중력 스케일을 기본 값으로 저장
        playerScript = GetComponent<Player>();

        AdjustSettingsBasedOnScene();
    }

    void AdjustSettingsBasedOnScene()
    {
        // 현재 씬의 이름을 가져옴
        string currentSceneName = SceneManager.GetActiveScene().name;
        
        if (currentSceneName.ToLower() == "stage1") // 대소문자 구분 없이 비교
        {
            // 씬 이름이 "stage1"이면 콜라이더를 비활성화하고, Rigidbody2D를 비활성화하며, 스케일을 조정함
            if (characterCollider != null) characterCollider.enabled = false;
            if (characterRigidbody != null) characterRigidbody.simulated = false; // Rigidbody2D 비활성화
            if (playerScript != null) playerScript.enabled = false; // Player 스크립트 비활성화
            transform.localScale = scaleForStage1;
        }
        else
        {
            // 그 외의 씬에서는 캐릭터의 콜라이더와 Rigidbody2D를 활성화하고, 기본 스케일로 돌아감
            if (characterCollider != null) characterCollider.enabled = true;
            if (characterRigidbody != null) characterRigidbody.simulated = true; // Rigidbody2D 활성화
            if (playerScript != null) playerScript.enabled = true; // Player 스크립트 활성화
            transform.localScale = defaultScale;
        }
    }
}
