using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<ItemSO> itemList = new List<ItemSO>(); //플레이어가 보유하는 아이템 리스트

    public void AddItem(ItemSO item) //인벤토리에 아이템 추가함수
    {
        foreach(ItemSO existingItem in itemList) //인벤토리에 존재하는 아이템인지 확인
        {
            if(existingItem == item)
            {
                existingItem.quantity++; //인벤토리에 존재하는 아이템이면 갯수 추가
                ApplyItemEffects(item);
                return;
            }
        }

        //인벤토리에 없는 아이템이라면 새로 추가하기
        item.quantity = 1;
        itemList.Add(item);
        ApplyItemEffects(item);
    }

    public void RemoveItem(ItemSO item)
    {
        foreach (ItemSO existingItem in itemList) //인벤토리에 존재하는 아이템인지 확인
        {
            if (existingItem == item)
            {
                existingItem.quantity--; //인벤토리에 존재하는 아이템 갯수 하나 감소
                if(existingItem.quantity <= 0) //0개가 되면 리스트에서 제거
                {
                    itemList.Remove(existingItem);
                }
                ApplyItemEffects(item);
                return;
            }
        }
    }

    void ApplyItemEffects(ItemSO item)
    {
        switch(item.itemName)
        {
            case "체력템":
                PlayerValue.Instance.IncreaseMaxHp((int)item.itemStat);
                break;
            case "체력재생 템":
                PlayerValue.Instance.IncreaseHpRegen(item.itemStat);
                break;
            case "공속템":
                PlayerValue.Instance.IncreaseAttackSpeed(item.itemStat);
                break;
            case "치명타 확률업":
                PlayerValue.Instance.IncreaseCritChance((int)item.itemStat);
                break;
            case "이동속도 템":
                PlayerValue.Instance.IncreaseMoveSpeed(item.itemStat);
                break;
            case "달리기속도 템":
                PlayerValue.Instance.IncreaseSprintSpeed(item.itemStat);
                break;
            case "위성":
                PlayerValue.Instance.Satellite(item.itemPrefab, item.quantity);
                break;
            case "멧돌이":
                PlayerValue.Instance.Metdolee(item.itemPrefab);
                break;
            case "미사일":
                PlayerValue.Instance.Missile(item.itemPrefab, item.quantity);
                break;
            case "질뿜버섯":
                PlayerValue.Instance.SprintMushroom((int)item.itemStat, item.quantity);
                break;
            case "자석반지":
                PlayerValue.Instance.IncreasePickUpRange((int)item.itemStat);
                break;
            case "보조탄창":
                PlayerValue.Instance.IncreaseSkillCount((int)item.itemStat);
                break;
            case "불길":
                PlayerValue.Instance.FireRoad(item.itemPrefab, item.quantity);
                break;
            case "흡혈씨앗":
                PlayerValue.Instance.SetLeechingSeed(item.quantity);
                break;
            case "흡혈이빨":
                PlayerValue.Instance.SetVampiricTooth(item.quantity);
                break;
            case "집중":
                PlayerValue.Instance.FocusOn(item.itemStat, item.quantity);
                break;
            case "럭키샷":
                PlayerValue.Instance.SetLuckyShot(item.itemStat, item.quantity);
                break;
            case "부활":
                PlayerValue.Instance.SetRebirth(item);
                break;
            case "파멸의고리":
                PlayerValue.Instance.SetRingOfDoom(item.quantity);
                break;
            case "깃털":
                PlayerValue.Instance.IncreaseJumpCount((int)item.itemStat);
                break;
        }
    }
}
