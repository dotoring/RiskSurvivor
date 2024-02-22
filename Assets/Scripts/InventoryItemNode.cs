using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemNode : MonoBehaviour
{
    //플레이어 보유 아이템 목록 노드에 사용하는 스크립트

    public Image itemIcon; //아이템 아이콘
    public Text itemQuantity; //아이템 갯수

    public void SetItem(ItemSO item) //초기화 함수
    {
        itemIcon.sprite = item.itemIcon;
        itemQuantity.text = item.quantity.ToString();
    }
}
