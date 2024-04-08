using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemGrade
{
    nomal,
    epic,
    legend
}

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemSO : ScriptableObject
{
    public string itemName; //아이템 이름
    public float itemStat; //아이템 스탯
    public string itemStatDesc; //아이템 스탯 설명
    public string itemDescription; //아이템 설명
    public Sprite itemIcon; //아이템 아이콘

    public int quantity; //아이템 갯수
    public GameObject itemPrefab; //아이템 프리팹
    public ItemGrade itemGrade;
}
