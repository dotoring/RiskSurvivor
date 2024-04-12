using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpGem : MonoBehaviour
{
    public int exp;

    public void GetExp()
    {
        PlayerValue.Instance.GainExp(exp);
    }

    //private void OnDestroy()
    //{
    //    PlayerValue.Instance.GainExp(exp);
    //}
}
