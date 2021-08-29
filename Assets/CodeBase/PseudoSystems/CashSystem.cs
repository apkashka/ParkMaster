using CodeBase.Infos;
using CodeBase.Static;
using DG.Tweening;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace CodeBase.PseudoSystems
{
    public class CashSystem : MonoBehaviour
    {
        private GameManager _game;
        private MoneyInfo[] _affectedComponents;

        public void Construct(GameManager game)
        {
            _game = game;

            //TEMP prototype solution due to lack of norm ECS
            _affectedComponents = FindObjectsOfType<MoneyInfo>();
            
            HandleRotation();
            HandleCollision();
            HandleReset();
        }

        private void HandleReset()
        {
            _game.DrawStarted.Subscribe(_ =>
            {
                _game.Money.Value = 0;

                foreach (var moneyInfo in _affectedComponents)
                {
                    moneyInfo.render.enabled = true;
                    moneyInfo.hitCollider.enabled = false;

                }
            }).AddTo(this);

            _game.DrawEnded.Subscribe(_ =>
            {
                foreach (var moneyInfo in _affectedComponents)
                {
                    moneyInfo.hitCollider.enabled = true;
                }
            }).AddTo(this);
        }

        private void HandleRotation()
        {
            foreach (var moneyInfo in _affectedComponents)
            {
                moneyInfo.transform.DORotate(new Vector3(0, 360, 0), 1, RotateMode.FastBeyond360)
                    .SetLoops(-1, LoopType.Incremental);
            }
        }

        private void HandleCollision()
        {
            foreach (var moneyInfo in _affectedComponents)
            {
                moneyInfo.hitCollider.OnTriggerEnterAsObservable()
                    .Where(col => col.gameObject.layer == CashedLayerMasks.CarLayer).Subscribe(_ =>
                    {
                        _game.Money.Value++;
                        moneyInfo.hitCollider.enabled = false;
                        moneyInfo.render.enabled = false;
                    });
            }
        }
    }
}