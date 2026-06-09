using UnityEngine;



public interface IPositioner
{
    Vector3 GetPosition(GameObject target);
    Quaternion GetRotation();
}
