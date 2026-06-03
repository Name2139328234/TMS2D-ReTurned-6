using TMPro;
using UnityEngine;
using UnityEngine.UI;



public class RecipePartUI : MonoBehaviour
{
    [SerializeField] private Image _resourcePreview;
    [SerializeField] private TextMeshProUGUI _amountText;



    public void Initiate(Sprite sprite, int amount)
    {
        _resourcePreview.sprite = sprite;
        _amountText.text = amount < 1000 ? amount.ToString() : (amount / 1000).ToString() + 'k';
    }
}
