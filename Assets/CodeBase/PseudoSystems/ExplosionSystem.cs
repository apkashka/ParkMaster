using CodeBase.Infos;
using CodeBase.Scriptable;
using CodeBase.Static;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace CodeBase.PseudoSystems
{
    public class ExplosionSystem : MonoBehaviour
    {
        private GameManager _game;
        private CoreConfig _config;
        private ExplosiveInfo[] _affectedComponents;


        public void Construct(GameManager game, CoreConfig config)
        {
            _game = game;
            _config = config;

            //TEMP prototype solution due to lack of norm ECS
            _affectedComponents = FindObjectsOfType<ExplosiveInfo>();
           
            HandleReset();
            HandleCollision();
        }

        private void HandleReset()
        {
            _game.DrawStarted.Subscribe(_ =>
            {
                foreach (var explosiveInfo in _affectedComponents)
                {
                    explosiveInfo.hitCollider.isTrigger = false;
                    explosiveInfo.rb.isKinematic = true;
                    explosiveInfo.isExploded = false;
                }
            }).AddTo(this);
            
            _game.DrawEnded.Subscribe(_ =>
            {
                foreach (var explosiveInfo in _affectedComponents)
                {
                    explosiveInfo.hitCollider.isTrigger = true;
                }
            }).AddTo(this);
        }

        private void HandleCollision()
        {
            foreach (var car in _affectedComponents)
            {
                car.hitCollider.OnTriggerEnterAsObservable()
                    .Where(col => col.gameObject.layer == CashedLayerMasks.CarLayer || col.gameObject.layer == CashedLayerMasks.ObstacleLayer).Subscribe(col =>
                    {
                        CreateExplosion(car, col.transform.position);
                    })
                    .AddTo(this).AddTo(car);
            }
        }

        private void CreateExplosion(ExplosiveInfo explosiveInfo, Vector3 point)
        {
            explosiveInfo.Exploded.Execute();
            explosiveInfo.isExploded = true;

            explosiveInfo.hitCollider.isTrigger = false;
            explosiveInfo.rb.isKinematic = false;

            var explosionPosition =
                (explosiveInfo.transform.position + point) / 2;
            explosionPosition.y += _config.explosionHeightOffset;
            
            explosiveInfo.rb.AddExplosionForce(_config.explosionForce, explosionPosition,
                _config.explosionRadius, 3f);
        }
    }
}