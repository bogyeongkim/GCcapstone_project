using UnityEngine;

public class PlayerSpawner : MonoBehaviour {


private void Start() {
    Instantiate(CharacterGameManager.instance.currentCharacter.prefab, transform.position, Quaternion.identity);
}
}