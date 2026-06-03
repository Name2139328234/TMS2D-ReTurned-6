using Reflex.Attributes;
using Reflex.Core;
using Reflex.Injectors;
using UnityEngine;



public class BuilderUI : MonoBehaviour
{
    [SerializeField] private Builder _builder;
    [SerializeField] private Transform _canvas;
    [SerializeField] private Vector2 _buttonDistance;
    [SerializeField] private GameObject _previewPrefab;
    [SerializeField] private BuilderButtonTip _tooltip;

    [Inject] private Container _container;



    void Start()
    {
        for (int y = 0; y < _builder.AvailableParts.Length; y++)
        {
            var partLevelsInfo = _builder.AvailableParts[y];

            for (int x = 0; x < partLevelsInfo.PartLevels.Length; x++)
            {
                var level = partLevelsInfo.PartLevels[x];
                var partSprite = level.GetComponent<SpriteRenderer>();

                var preview = Instantiate(_previewPrefab, _canvas);
                GameObjectInjector.InjectObject(preview, _container);
                preview.transform.localPosition = new (x * _buttonDistance.x, y * _buttonDistance.y, 0);

                var button = preview.GetComponent<BuilderButton>();
                button.Initialize(partLevelsInfo.Kind, x, partSprite.sprite, partSprite.color);
                button.OnPress += button =>
                {
                    _builder.SelectedInfo = new PartInfo(button.Kind, button.Level);
                    _tooltip.SetData(_builder.GetPriceInfo(button.Kind).GetLevelledCost(button.Level), button.Kind.ToString()); 
                };
            }
        }

        _tooltip.SetData(_builder.GetPriceInfo(_builder.SelectedInfo.Kind).GetLevelledCost(_builder.SelectedInfo.Level), _builder.SelectedInfo.Kind.ToString());
    }
}
