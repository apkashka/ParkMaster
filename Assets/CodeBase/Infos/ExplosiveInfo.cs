using UniRx;
using UnityEngine;

namespace CodeBase.Infos
{
    public class ExplosiveInfo : MonoBehaviour
    {
        public readonly ReactiveCommand Exploded = new ReactiveCommand();
        
        public Collider hitCollider;
        public Rigidbody rb;
        public bool isExploded;
    }
}