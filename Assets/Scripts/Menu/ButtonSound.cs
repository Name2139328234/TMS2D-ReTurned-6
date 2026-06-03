using Reflex.Attributes;
using UnityEngine;
using UnityEngine.EventSystems;



public class ButtonSound : MonoBehaviour, IPointerEnterHandler
{
    [Inject] private ButtonAudioStorage _audio;



    public void OnPointerEnter(PointerEventData eventData)
    { 
        _audio.Play();
    }
}
