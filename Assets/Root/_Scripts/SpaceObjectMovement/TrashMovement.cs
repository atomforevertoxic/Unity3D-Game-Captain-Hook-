using UnityEngine;

namespace Root
{
    public class TrashMovement : PlatformMovement
    {
        [SerializeField] public GameObject _pivotObject;
        [SerializeField] public float _rotationSpeed;
        private Vector3 _axis;

        private void Start()
        {
            _axis = _pivotObject.transform.Find("Axis").transform.up; }

        protected override void Update()
        {
            base.Update();
            transform.RotateAround(_pivotObject.transform.position, _axis, _rotationSpeed * Time.deltaTime);
        }

        protected override void OnCollisionEnter(Collision collision)
        {
            base.OnCollisionEnter(collision);
        }

    }
}
