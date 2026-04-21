using UnityEngine;
using UnityEngine.UIElements;

public class Rotater : MonoBehaviour
{
    [SerializeField] float rotationSpeed;
    void Update()
    {
        transform.Rotate(0,0,rotationSpeed*Time.deltaTime);
    }
}
