using UnityEngine;

namespace CubeChan {
public class CameraMovement : MonoBehaviour{
    [SerializeField] private float _speed = 1f;
    [SerializeField] private float _smoothing = 5f;
    [SerializeField] private Vector2 _range = new (100, 100);
    [SerializeField] private Transform _holder;
		
    private Vector3 _targetPosition;
    private Vector3 _input;

    private void Awake() {
        _targetPosition = _holder.position;
    }
			
    private void HandleInput() {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 right = _holder.right * x;
        Vector3 forward = _holder.forward * z;

        _input = (forward + right).normalized;
    }

    private void Move() {
        Vector3 nextTargetPosition = _targetPosition + _input * _speed;
        if (IsInBounds(nextTargetPosition)) _targetPosition = nextTargetPosition;
        _holder.position = Vector3.Lerp(_holder.position, _targetPosition, Time.deltaTime * _smoothing);
    }

    private bool IsInBounds(Vector3 position) {
        return position.x > -_range.x &&
               position.x < _range.x &&
               position.z > -_range.y &&
               position.z < _range.y;
    }
		
    private void Update() {
        HandleInput();
        Move();
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        // Gizmos.DrawSphere(transform.position, 5f);
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(_range.x * 2f, 5f, _range.y * 2f));
    }
}
}