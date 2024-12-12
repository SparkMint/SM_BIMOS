using PD;
using UnityEngine;

namespace BIMOS
{
    public class PhysicsHand : MonoBehaviour
    {
        public Transform Target, Controller;
        public Vector3 TargetOffsetPosition;
        public Quaternion TargetOffsetRotation;
        [Space] 
        public bool pdEnabled;
        public float pGain;
        public float dGain;

        private Player _player;
        private PDVector3 _pdVector3;
        private ConfigurableJoint _handJoint;

        private void Awake()
        {
            _player = GetComponentInParent<Player>();

            GetComponent<Rigidbody>().solverIterations = 60;
            GetComponent<Rigidbody>().solverVelocityIterations = 10;

            _pdVector3 = new PDVector3(pGain, dGain);

            TargetOffsetRotation = Quaternion.identity;
            _handJoint = GetComponent<ConfigurableJoint>();
        }

        private void FixedUpdate()
        {
            Vector3 targetPosition = Target.TransformPoint(TargetOffsetPosition);
            Vector3 headOffset = targetPosition - _player.PhysicsRig.HeadRigidbody.position;
            _handJoint.targetPosition = headOffset;
            
            // SM Target Velocity Logic.
            _pdVector3.UpdateProportionalGain(pdEnabled ? pGain : 0f);
            _pdVector3.UpdateDerivativeGain(pdEnabled ? dGain : 0f);
            _handJoint.targetVelocity = _pdVector3.CalculatePD(_handJoint.transform.position, Target.TransformPoint(TargetOffsetPosition),
                Time.fixedDeltaTime);

            //Rotation
            _handJoint.targetRotation = Target.rotation * TargetOffsetRotation;
        }
    }
}