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

        DisplayCollectedItems();
        UnityEngine.Debug.Log("item display");

        // DragonReaction 스크립트 비활성화
        if (dragonReactionScript != null)
        {
            dragonReactionScript.enabled = false;
        }
    }

    void DisplayCollectedItems()
    {
        // ScoreManager에서 수집된 아이템 가져오기
        List<GameObject> collectedItems = ScoreManager.instance.GetCollectedItems();

        // collectedItems 리스트와 itemSlot1이 유효한지 확인
        if (collectedItems != null && collectedItems.Count > 0 && itemSlot1 != null)
        {
            itemSlot1.sprite = collectedItems[0].GetComponent<SpriteRenderer>().sprite;
            itemSlot1.gameObject.SetActive(true);
            
            AddClickEvent(itemSlot1.gameObject, collectedItems[0], 1);
        }
        // collectedItems 리스트와 itemSlot2가 유효한지 확인
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
            trigger.triggers.Clear(); // 중복 트리거 추가 방지
        }

        var entry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerClick
        };
        entry.callback.AddListener((data) => { OnItemClick(itemData, slotNumber); });
        trigger.triggers.Add(entry);
    }

    // 아이템 클릭
    public void OnItemClick(GameObject item, int slotNumber)
    {
        effect.Play();

        // 아이템 용의 위치로 이동
        item.transform.position = new Vector3(dragonPosition.position.x - 1.0f, dragonPosition.position.y + 2.9f, dragonPosition.position.z - 1.0f);
        
        UnityEngine.Debug.Log("Item clicked!");

        StartCoroutine(DeactivateItemAfterDelay(item, 1.0f));

        // 클릭된 슬롯 추적
        if (slotNumber == 1)
        {
            isItemSlot1Clicked = true;
            arrow1.SetActive(false);
        }
        else if (slotNumber == 2)
        {
            isItemSlot2Clicked = true;
            arrow2.SetActive(false);
        }

        // 두 슬롯이 모두 클릭되었는지
        if (isItemSlot1Clicked && isItemSlot2Clicked)
        {
            itemSlot1.gameObject.SetActive(false);
            itemSlot2.gameObject.SetActive(false);

            // DragonReaction 스크립트 활성화
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

    // IPointerClickHandler 인터페이스
    public void OnPointerClick(PointerEventData eventData)
    {
        UnityEngine.Debug.Log("Item clicked!");
    }
}
