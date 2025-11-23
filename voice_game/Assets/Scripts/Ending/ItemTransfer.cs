using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Diagnostics;

public class ItemTransfer : MonoBehaviour, IPointerClickHandler
{
    public Image itemSlot1;
    public Image itemSlot2;
    public Transform dragonPosition;
    public DragonReaction dragonReactionScript;
    public GameObject arrow1;
    public GameObject arrow2;
    public AudioSource effect;

    private bool isItemSlot1Clicked = false;
    private bool isItemSlot2Clicked = false;


    void Start()
    {
        arrow1.SetActive(true);
        arrow2.SetActive(true);

        itemSlot1.enabled = true;
        itemSlot2.enabled = true;

        DisplayCollectedItems();
        UnityEngine.Debug.Log("item display");

        // DragonReaction ��ũ��Ʈ ��Ȱ��ȭ
        if (dragonReactionScript != null)
        {
            dragonReactionScript.enabled = false;
        }
    }

    void DisplayCollectedItems()
    {
        // ScoreManager���� ������ ������ ��������
        List<GameObject> collectedItems = ScoreManager.instance.GetCollectedItems();

        // collectedItems ����Ʈ�� itemSlot1�� ��ȿ���� Ȯ��
        if (collectedItems != null && collectedItems.Count > 0 && itemSlot1 != null)
        {
            itemSlot1.sprite = collectedItems[0].GetComponent<SpriteRenderer>().sprite;
            itemSlot1.gameObject.SetActive(true);
            
            AddClickEvent(itemSlot1.gameObject, collectedItems[0], 1);
        }
        // collectedItems ����Ʈ�� itemSlot2�� ��ȿ���� Ȯ��
        if (collectedItems != null && collectedItems.Count > 1 && itemSlot2 != null)
        {
            itemSlot2.sprite = collectedItems[1].GetComponent<SpriteRenderer>().sprite;
            itemSlot2.gameObject.SetActive(true);
            
            AddClickEvent(itemSlot2.gameObject, collectedItems[1], 2);
        }
    }

    void AddClickEvent(GameObject itemGameObject, GameObject itemData, int slotNumber)
    {
        EventTrigger trigger = itemGameObject.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = itemGameObject.AddComponent<EventTrigger>();
        }
        else
        {
            trigger.triggers.Clear(); // �ߺ� Ʈ���� �߰� ����
        }

        var entry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerClick
        };
        entry.callback.AddListener((data) => { OnItemClick(itemData, slotNumber); });
        trigger.triggers.Add(entry);
    }

    // ������ Ŭ��
    public void OnItemClick(GameObject item, int slotNumber)
    {
        effect.Play();

        // ������ ���� ��ġ�� �̵�
        item.transform.position = new Vector3(dragonPosition.position.x - 1.0f, dragonPosition.position.y + 2.9f, dragonPosition.position.z - 1.0f);
        
        UnityEngine.Debug.Log("Item clicked!");

        StartCoroutine(DeactivateItemAfterDelay(item, 1.0f));

        // Ŭ���� ���� ����
        if (slotNumber == 1)
        {
            isItemSlot1Clicked = true;
            itemSlot1.gameObject.SetActive(false);
            arrow1.SetActive(false);
        }
        else if (slotNumber == 2)
        {
            isItemSlot2Clicked = true;
            itemSlot2.gameObject.SetActive(false);
            arrow2.SetActive(false);
        }

        // �� ������ ��� Ŭ���Ǿ�����
        if (isItemSlot1Clicked && isItemSlot2Clicked)
        {
            //itemSlot1.gameObject.SetActive(false);
            //itemSlot2.gameObject.SetActive(false);

            // DragonReaction ��ũ��Ʈ Ȱ��ȭ
            if (dragonReactionScript != null)
            {
                dragonReactionScript.enabled = true;
                dragonReactionScript.React();
            }
        }
    }

    IEnumerator DeactivateItemAfterDelay(GameObject item, float delay)
    {
        yield return new WaitForSeconds(delay);
        item.SetActive(false);
    }

    // IPointerClickHandler �������̽�
    public void OnPointerClick(PointerEventData eventData)
    {
        UnityEngine.Debug.Log("Item clicked!");
    }
}
