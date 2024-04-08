using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSelectNode : MonoBehaviour
{
    //아이템 선택창 노드에서 사용하는 스크립트

    Button btn;
    GameMgr gameMgr;
    Inventory inventory;

    //아이템 변수들
    public ItemSO nodeItem;
    public Image itemIcon;
    public Text itemName;
    public Text itemStat;
    public Text itemDesc;
    public Text itemGrade;
    public GameObject selectCheck;

    void Start()
    {
        btn = GetComponent<Button>();
        gameMgr = GameObject.Find("GameMgr").GetComponent<GameMgr>();
        inventory = GameObject.Find("Player").GetComponent<Inventory>();

        //노드 클릭시 이벤트
        btn.onClick.AddListener(() =>
        {
            if(gameMgr.selectedItem == nodeItem)
            {
                gameMgr.selectedItem = null;
            }
            else
            {
                gameMgr.selectedItem = nodeItem;
            }

            //inventory.AddItem(nodeItem); //인벤토리에 아이템 추가
            //gameMgr.ItemSelectPopDown(); //아이템 선택창 비활성화
            //gameMgr.GamePlay(); //게임 다시진행
        });
    }

    private void Update()
    {
        if(gameMgr.selectedItem == nodeItem)
        {
            selectCheck.SetActive(true);
        }
        else
        {
            selectCheck.SetActive(false);
        }
    }

    public void SetItem(ItemSO item)
    {
        //아이템 텍스트, 아이콘 초기화
        nodeItem = item;
        itemIcon.sprite = nodeItem.itemIcon;
        itemName.text = nodeItem.itemName;
        itemStat.text = nodeItem.itemStatDesc;
        itemDesc.text = nodeItem.itemDescription;
        if(nodeItem.itemGrade == ItemGrade.nomal)
        {
            itemGrade.color = new Color32(255, 255, 255, 255);
            itemGrade.text = "일반";
        }
        else if(nodeItem.itemGrade == ItemGrade.epic)
        {
            itemGrade.color = new Color32(85, 123, 255, 255);
            itemGrade.text = "에픽";
        }
        else
        {
            itemGrade.color = new Color32(255, 175, 85, 255);
            itemGrade.text = "전설";
        }
    }
}
