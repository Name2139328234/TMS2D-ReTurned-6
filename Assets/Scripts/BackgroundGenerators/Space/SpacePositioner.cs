using UnityEngine;



public class SpacePositioner : IPositioner
{
    private Bounds _spawnRange;



    public SpacePositioner(Bounds spawnRange)
    {
        _spawnRange = spawnRange;
    }
    public Vector3 GetPosition(GameObject target)
    {
        return target.transform.position + new Vector3(Random.Range(_spawnRange.min.x, _spawnRange.max.x), Random.Range(_spawnRange.min.y, _spawnRange.max.y));
    }
    public Quaternion GetRotation()
    {
        return Quaternion.identity;
    }
}
