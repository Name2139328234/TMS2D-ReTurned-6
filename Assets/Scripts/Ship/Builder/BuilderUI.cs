using Closures;
using UnityEngine;



public class BuilderUI : MonoBehaviour
{
    [SerializeField] private Builder _builder;
    [SerializeField] private Transform _canvas;
    [SerializeField] private Vector2 _buttonDistance;
    [SerializeField] private GameObject _previewPrefab;
    [SerializeField] private BuilderButtonTip _tooltip;



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
                preview.transform.localPosition = new (x * _buttonDistance.x, y * _buttonDistance.y, 0);

                var button = preview.GetComponent<BuilderButton>();
                button.Initialize(partLevelsInfo.Kind, x, partSprite.sprite, partSprite.color);

                button.OnPress += Closure.Action<(Builder, BuilderButtonTip), PartInfo>((_builder, _tooltip), (builderInfo, partInfo) =>
                {
                    builderInfo.Item1.SelectedInfo = partInfo;
                    builderInfo.Item2.SetData(_builder.GetPriceInfo(partInfo.Kind).GetLevelledCost(partInfo.Level), partInfo.Kind.ToString());
                }).AsAction();//TODO "AsAction" creates ~4ns processor overhead. Ask someone experienced whether or not I should consider alternatives
            }
        }

        _tooltip.SetData(_builder.GetPriceInfo(_builder.SelectedInfo.Kind).GetLevelledCost(_builder.SelectedInfo.Level), _builder.SelectedInfo.Kind.ToString());
    }
}
