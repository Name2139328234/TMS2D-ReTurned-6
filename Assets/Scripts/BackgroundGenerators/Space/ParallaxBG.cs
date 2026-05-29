using UnityEngine;
using UnityEngine.Tilemaps;



public class ParallaxBG : MonoBehaviour
{
    [SerializeField] private Transform _player;
    [SerializeField] private float _baseSpeed;
    [SerializeField] private float _scaledSpeed;



    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, _player.position, (_baseSpeed + Mathf.Pow(Vector3.Distance(_player.position, transform.position) * _scaledSpeed, 3f)) * Time.deltaTime);
    }
}
