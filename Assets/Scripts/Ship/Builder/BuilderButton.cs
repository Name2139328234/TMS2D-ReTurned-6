using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;



public class BuilderButton : MonoBehaviour
{
    public UnityAction<BuilderButton> OnPress;

    public PartKind Kind { get => _kind; }
    public int Level { get => _level; }

    [SerializeField] private Button _button;
    [SerializeField] private Image _preview;

    private PartKind _kind;
    private int _level;



    public void Initialize(PartKind kind, int level, Sprite preview, Color color)
    {
        _kind = kind;
        _level = level;
        _preview.sprite = preview;
        _preview.color = color;

        _button.onClick.AddListener(() => { OnPress?.Invoke(this); });
    }
}
