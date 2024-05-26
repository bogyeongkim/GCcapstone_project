using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemTransfer : MonoBehaviour, IPointerClickHandler
{
    public Image itemSlot1;
    public Image itemSlot2;
    public Transform dragonPosition; // 용의 위치를 저장하는 변수

    // Start is called before the first frame update
    void Start()
    {
        DisplayCollectedItems();
        UnityEngine.Debug.Log("Item display");
    }

    void DisplayCollectedItems()
    {
        
        List<GameObject> collectedItems = ScoreManager.instance.GetCollectedItems();

        if (collectedItems.Count > 0)
        {
            itemSlot1.sprite = collectedItems[0].GetComponent<SpriteRenderer>().sprite;
            itemSlot1.gameObject.SetActive(true);
            // 클릭 이벤트 리스너 추가
            AddClickEvent(itemSlot1.gameObject, collectedItems[0]);
        }
        if (collectedItems.Count > 1)
        {
            itemSlot2.sprite = collectedItems[1].GetComponent<SpriteRenderer>().sprite;
            itemSlot2.gameObject.SetActive(true);
            // 클릭 이벤트 리스너 추가
            AddClickEvent(itemSlot2.gameObject, collectedItems[1]);
        }
    }
    void AddClickEvent(GameObject itemGameObject, GameObject itemData)
    {
        EventTrigger trigger = itemGameObject.AddComponent<EventTrigger>();
        var entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((data) => { OnItemClick(itemData); });
        trigger.triggers.Add(entry);
    }

    // 아이템 클릭 시 실행
    public void OnItemClick(GameObject item)
    {
        // 아이템을 용의 위치로 이동
        item.transform.position = new Vector3(dragonPosition.position.x - 1.0f, dragonPosition.position.y + 2.9f, dragonPosition.position.z - 1.0f);
        UnityEngine.Debug.Log("clicked!");
    }

    // IPointerClickHandler 인터페이스
    public void OnPointerClick(PointerEventData eventData)
    {
        UnityEngine.Debug.Log("Item clicked!");
    }
}
