using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour
{
    [SerializeField] private Image _item;
    [SerializeField] private Text _count;



    public void Initiate(Sprite item, int count)
    {
        _item.sprite = item;
        _count.text = count.ToString();
    }
}
