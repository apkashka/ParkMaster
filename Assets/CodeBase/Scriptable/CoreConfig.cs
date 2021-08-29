using UnityEngine;

namespace CodeBase.Scriptable
{
    [CreateAssetMenu(fileName = "CoreConfig", menuName = "Scriptables/CoreConfig")]
    public class CoreConfig : ScriptableObject
    {
        [Header("Car")]
        public float speed = 3;
        public float pointsDistance = 0.3f;
        public float parkingDistance = 0.1f;
        public float resetDuration = 0.3f;
        
        [Header("Explosion")]
        public float explosionRadius;
        public float explosionForce;
        public float explosionHeightOffset;
    }
}
