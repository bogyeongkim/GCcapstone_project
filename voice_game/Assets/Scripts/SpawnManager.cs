using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public Transform SpawnPos;

    private void Start()
    {
        Init();
    }

    void Init()
    {
        GameObject playerObj = Instantiate(CharacterGameManager.instance.currentCharacter.prefab, SpawnPos.position,
            Quaternion.identity);
    }
}
