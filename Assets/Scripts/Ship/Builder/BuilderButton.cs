using System;
using UnityEngine;
using UnityEngine.UI;



public class BuilderButton : MonoBehaviour
{
    public Action<PartInfo> OnPress;

    [SerializeField] private Button _button;
    [SerializeField] private Image _preview;

    private PartInfo _info;



    public void Initialize(PartKind kind, int level, Sprite preview, Color color)
    {
        _info = new(kind, level);
        _preview.sprite = preview;
        _preview.color = color;

        _button.onClick.AddListener(OnClickHandler);//this doesn't cause a heap allocation 
    }



    private void OnClickHandler()
    {
        OnPress?.Invoke(_info);
    }
}
