using DG.Tweening;
using UniRx;
using UnityEngine;

namespace CodeBase.Infos
{
    public class CarInfo : MonoBehaviour
    {
        public readonly ReactiveCommand MovementStopped = new ReactiveCommand();
        
        public Vector3[] pathPoints;
        public Transform startSpot;
        public Transform parkSpot;

        public Tween rotateTween;
        public Tween moveTween;

        public ExplosiveInfo explosiveInfo;
        public bool isParked;
        public ParticleSystem parkedFX;
    }
}
