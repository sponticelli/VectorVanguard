using UnityEngine;

namespace VectorVanguard.Actors
{
    public class ActorUnityPhysics2D : AActorPhysics
    {
        [SerializeField] private Rigidbody2D _rigidbody2D;

        private float _internalRotationForce;
        private Vector3 _internalLinearForce;
        private float _externalRotationForce;
        private Vector3 _externalLinearForce;

        public override void Initialization(Actor actor)
        {
            _actor = actor;
            if (_rigidbody2D == null)
            {
                _rigidbody2D = GetComponent<Rigidbody2D>();
                if (_rigidbody2D == null)
                {
                    Debug.LogError("Rigidbody2D is missing on the actor.");
                }
            }
            _internalLinearForce = Vector3.zero;
            _internalRotationForce = 0;
            _externalLinearForce = Vector3.zero;
            _externalRotationForce = 0;
        }

        private void FixedUpdate()
        {
            _internalLinearForce.z = 0;
            _externalLinearForce.z = 0;
            _rigidbody2D.AddTorque(_internalRotationForce + _externalRotationForce);
            _rigidbody2D.AddForce(_internalLinearForce + _externalLinearForce, ForceMode2D.Force);

            _internalLinearForce = Vector3.zero;
            _internalRotationForce = 0;
            _externalLinearForce = Vector3.zero;
            _externalRotationForce = 0;
        }

        public override void AddRotationForce(float rotation)
        {
            _internalRotationForce += rotation;
        }

        public override void AddForce(Vector3 force)
        {
            _internalLinearForce += force;
        }

        public override void AddExternalRotationForce(float rotation)
        {
            _externalRotationForce += rotation;
        }

        public override void AddExternalForce(Vector3 force)
        {
            _externalLinearForce += force;
        }
        
        public override float GetSpeed()
        {
            return _rigidbody2D.velocity.magnitude;
        }
    }
}
