using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemTransfer : MonoBehaviour, IPointerClickHandler
{
    public Image itemSlot1;
    public Image itemSlot2;
    public Transform dragonPosition; // ���� ��ġ�� �����ϴ� ����

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
            // Ŭ�� �̺�Ʈ ������ �߰�
            AddClickEvent(itemSlot1.gameObject, collectedItems[0]);
        }
        if (collectedItems.Count > 1)
        {
            itemSlot2.sprite = collectedItems[1].GetComponent<SpriteRenderer>().sprite;
            itemSlot2.gameObject.SetActive(true);
            // Ŭ�� �̺�Ʈ ������ �߰�
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

    // ������ Ŭ�� �� ����
    public void OnItemClick(GameObject item)
    {
        // �������� ���� ��ġ�� �̵�
        item.transform.position = new Vector3(dragonPosition.position.x - 1.0f, dragonPosition.position.y + 2.9f, dragonPosition.position.z - 1.0f);
        UnityEngine.Debug.Log("clicked!");
    }

    // IPointerClickHandler �������̽�
    public void OnPointerClick(PointerEventData eventData)
    {
        UnityEngine.Debug.Log("Item clicked!");
    }
}
