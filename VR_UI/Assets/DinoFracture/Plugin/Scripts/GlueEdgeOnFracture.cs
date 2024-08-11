using System;
using UnityEngine;

namespace DinoFracture
{
    /// <summary>
    /// If the fracture pieces intersects with a specified trigger when
    /// created, the rigid body is destroyed and the piece becomes static.
    /// Otherwise, the piece will turn on gravity.  It’s best used if the
    /// FractureTemplate’s rigid body is set to not use gravity initially.  
    /// </summary>
    public class GlueEdgeOnFracture : MonoBehaviour
    {
        /// <summary>
        /// The piece will be glued if it intersects a trigger with this
        /// collision tag. Set to empty to allow any trigger to glue the piece.
        /// </summary>
        [UnityEngine.Tooltip("The piece will be glued if it intersects a trigger with this collision tag. Set to empty to allow any trigger to glue the piece.")]
        public string CollisionTag = "";

        private int _collisionCount;
        private int _frameCountLeft = 0;

        private RigidbodyConstraints _rigidBodyConstraints;
        private Vector3 _rigidBodyVelocity;
        private Vector3 _rigidBodyAngularVelocity;

        private Vector3 _impactPoint;
        private Vector3 _impactVelocity;
        private float _impactMass;

        private void Awake()
        {
            // We need to ensure we are kinematic to begin
            // with so that we don't move because of collision
            // interactions on frame 1.
            Rigidbody body = GetComponent<Rigidbody>();
            if (body != null)
            {
                body.isKinematic = true;
            }
        }

        private void OnCollisionEnter(Collision col)
        {
            if (String.IsNullOrEmpty(CollisionTag) || col.collider.CompareTag(CollisionTag))
            {
                _collisionCount++;
                this.enabled = true;
            }
            else
            {
                _impactMass += (col.rigidbody != null) ? col.rigidbody.mass : 1.0f;
                _impactVelocity += col.relativeVelocity;

                Vector3 impactPoint = Vector3.zero;
                for (int i = 0; i < col.contacts.Length; i++)
                {
                    impactPoint += col.contacts[i].point;
                }
                _impactPoint += impactPoint * 1.0f / col.contacts.Length;
            }
        }

        private void OnTriggerEnter(Collider col)
        {
            if (String.IsNullOrEmpty(CollisionTag) || col.CompareTag(CollisionTag))
            {
                _collisionCount++;
                this.enabled = true;
            }
        }

        private void FixedUpdate()
        {
            if (_frameCountLeft > 0)
            {
                SetGlued(_collisionCount > 0);

                _frameCountLeft--;
                if (_frameCountLeft <= 0)
                {
                    if (_collisionCount == 0)
                    {
                        Rigidbody body = GetComponent<Rigidbody>();
                        if (body != null)
                        {
                            body.constraints = _rigidBodyConstraints;
                            body.angularVelocity = _rigidBodyAngularVelocity;
                            body.velocity = _rigidBodyVelocity;

                            body.WakeUp();

                            Vector3 force = _impactMass * _impactVelocity / (body.mass + _impactMass);
                            body.AddForceAtPosition(force * body.mass, _impactPoint, ForceMode.Impulse);
                        }
                    }

                    // Stop receiving updates
                    this.enabled = false;
                    _collisionCount = 0;
                    _frameCountLeft = 0;
                }
            }
            else
            {
                this.enabled = false;
            }
        }

        private void OnFracture(OnFractureEventArgs fractureRoot)
        {
            Rigidbody body = GetComponent<Rigidbody>();
            if (body != null)
            {
                body.isKinematic = false;   // Need to turn off kinematic to get collision events

                _rigidBodyVelocity = body.velocity;
                _rigidBodyAngularVelocity = body.angularVelocity;
                _rigidBodyConstraints = body.constraints;
                body.constraints = RigidbodyConstraints.FreezeAll;

                _impactPoint = Vector3.zero;
                _impactVelocity = Vector3.zero;
                _impactMass = 0.0f;

                _frameCountLeft = 2;
                this.enabled = true;
            }
        }

        private void SetGlued(bool glued)
        {
            Rigidbody body = GetComponent<Rigidbody>();
            if (body != null)
            {
                if (glued)
                {
                    Destroy(body);
                }
            }
        }
    }
}