using R3;
using TMPro;
using UnityEngine;



public class GalaxyModeSelect : MonoBehaviour
{
    [SerializeField] private GameObject[] _modeViews;
    [SerializeField] private TMP_Dropdown _selector;
    [SerializeField] private int _emptyStatesOffset = 1;   



    private void Start()
    {
        _selector.onValueChanged
            .AsObservable()
            .Where(index => index >= 0 && index < _modeViews.Length + _emptyStatesOffset)
            .Subscribe(i => SelectMode(i))
            .AddTo(gameObject);
    }



    private void SelectMode(int index)
    {
        index -= _emptyStatesOffset;

        for (int i = 0; i < _modeViews.Length; i++)
            _modeViews[i].SetActive(index == i);
    }
}
