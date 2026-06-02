using TMPro;
using UnityEngine;
using UnityEngine.UI;



public class ItemUI : MonoBehaviour
{
    [SerializeField] private Image _item;
    [SerializeField] private TextMeshProUGUI _count;



    public void Initiate(Sprite item, int count)
    {
        _item.sprite = item;
        _count.text = count.ToString();
    }
    public void UpdateCount(int count)
    {
        _count.text = count.ToString();
    }
}
