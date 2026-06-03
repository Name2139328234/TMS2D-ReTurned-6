using System.Collections.Generic;
using TMPro;
using UnityEngine;



public class BuilderButtonTip : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _blockName;
    [SerializeField] private Vector2 _costDisplayPoint;
    [SerializeField] private Vector2 _costDisplayDelta;
    [SerializeField] private GameObject _costDisplayPrefab;
    [SerializeField] private ItemInfoStorage _itemInfoStorage;

    private List<GameObject> _spawned = new();



    public void SetData(ItemStack[] recipe, string name)
    {
        foreach (var spawned in _spawned)
        {
            Destroy(spawned);
        }
        _spawned.Clear();

        _blockName.text = name;

        for (int i = 0; i < recipe.Length; i++)
        {
            var spawned = Instantiate(_costDisplayPrefab, transform);
            spawned.transform.localPosition = _costDisplayPoint + _costDisplayDelta * i;
            spawned.GetComponent<RecipePartUI>().Initiate(_itemInfoStorage.Get(recipe[i].Kind).Preview, recipe[i].Count);
            _spawned.Add(spawned);
        }
    }
}
