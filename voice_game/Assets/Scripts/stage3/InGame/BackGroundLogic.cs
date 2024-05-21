using UnityEngine;

public class BackGroundLogic : MonoBehaviour
{
    public Vector3 repositionPos;
    public float moveSpeed;
    public Vector3 endPos;
    public Transform[] backGrounds;

    private void Start()
    {
        backGrounds = GetComponentsInChildren<Transform>()[1..];
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.isGameEnd) return;
        
        for (int i = backGrounds.Length-1; i >= 0; i--)
        {
            if (backGrounds[i].transform.position.x <= endPos.x)
            {
                if (i == 0)
                    backGrounds[0].transform.position = backGrounds[^1].transform.position + repositionPos;
                else
                    backGrounds[i].transform.position = backGrounds[i - 1].transform.position + repositionPos;
            }

            backGrounds[i].transform.position += Time.fixedDeltaTime * moveSpeed * Vector3.left;
        }
    }
}
