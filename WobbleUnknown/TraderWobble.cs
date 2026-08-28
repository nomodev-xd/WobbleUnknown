using System.Collections.Generic;
using UnityEngine;

namespace WobbleUnknown
{
    public sealed class TraderWobble : MonoBehaviour
    {
        public float stiffness = 500f;
        public float damping = 4f;
        public float rotationSensitivity = 8f;

        private readonly List<LimbState> _limbs = new List<LimbState>();
        private TraderScript _trader;

        private sealed class LimbState
        {
            internal Transform Transform;
            internal Vector2 Offset;
            internal Vector2 Velocity;
            internal float Weight;
        }

        private void Start()
        {
            _trader = GetComponent<TraderScript>();
            if (_trader == null) return;

            AddLimb(_trader.torso, 0.5f);
            AddLimb(_trader.head, 1.2f);
            AddLimb(_trader.lArm, 1.8f);
            AddLimb(_trader.rArm, 1.8f);
            AddLimb(_trader.lThigh, 0.2f);
            AddLimb(_trader.rThigh, 0.2f);
            AddLimb(_trader.lFoot, 0.05f);
            AddLimb(_trader.rFoot, 0.05f);
        }

        private void AddLimb(Transform limbTransform, float weight)
        {
            if (limbTransform != null)
                _limbs.Add(new LimbState { Transform = limbTransform, Weight = weight });
        }

        public void TriggerWobble()
        {
            foreach (var limb in _limbs)
            {
                var direction = Random.insideUnitCircle.normalized;
                limb.Velocity = direction * Random.Range(18f, 28f) * limb.Weight;
            }

            SoundLoader.PlayRandSound();
        }

        public void ApplyWobble()
        {
            var deltaTime = Mathf.Min(Time.deltaTime, 0.15f);
            foreach (var limb in _limbs)
            {
                if (limb.Transform == null) continue;

                var force = -stiffness * limb.Offset - damping * limb.Velocity;
                limb.Velocity += force * deltaTime;
                limb.Offset += limb.Velocity * deltaTime;
                limb.Transform.localPosition += (Vector3)limb.Offset;
                limb.Transform.localEulerAngles += new Vector3(0f, 0f, -limb.Velocity.x * rotationSensitivity);
            }
        }
    }
}