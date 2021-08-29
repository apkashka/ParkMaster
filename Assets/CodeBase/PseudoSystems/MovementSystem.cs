using System.Linq;
using CodeBase.Infos;
using CodeBase.Scriptable;
using DG.Tweening;
using UnityEngine;
using UniRx;

namespace CodeBase.PseudoSystems
{
    public class MovementSystem : MonoBehaviour
    {
        private CarInfo[] _affectedComponents;
        private CoreConfig _config;
        private GameManager _game;

        public void Construct(GameManager game, CoreConfig config)
        {
            _game = game;
            _config = config;

            //TEMP prototype solution due to lack of norm ECS
            _affectedComponents = FindObjectsOfType<CarInfo>();

            HandleMovement();
            HandleUpdatePath();
            HandleReset();
            HandleExplosion();
        }

        private void HandleExplosion()
        {
            foreach (var car in _affectedComponents)
            {
                car.explosiveInfo.Exploded.Subscribe(_ => { car.moveTween.Kill(); });
            }
        }

        private void HandleMovement()
        {
            _game.DrawEnded.Subscribe(_ =>
            {
                foreach (var car in _affectedComponents)
                {
                    car.moveTween.Kill();
                    var time = car.pathPoints.Length * _config.pointsDistance / _config.speed;
                    car.moveTween = car.transform.DOPath(car.pathPoints, time)
                        .SetLookAt(0).OnComplete(() => { car.MovementStopped.Execute(); });
                }
            }).AddTo(this);
        }

        private void HandleReset()
        {
            foreach (var car in _affectedComponents)
            {
                car.transform.position = car.startSpot.position;
                car.transform.rotation = car.startSpot.rotation;
            }

            _game.DrawStarted.Subscribe(_ =>
            {
                foreach (var car in _affectedComponents)
                {
                    car.isParked = false;
                    car.moveTween.Kill();
                    car.moveTween = car.transform.DOMove(car.startSpot.position, _config.resetDuration);
                    car.rotateTween.Kill();
                    car.rotateTween = car.transform.DORotateQuaternion(car.startSpot.rotation, _config.resetDuration);
                }
            }).AddTo(this);
        }


        private void HandleUpdatePath()
        {
            foreach (var car in _affectedComponents)
            {
                var pathInfo = car.GetComponentInParent<PathInfo>();
                pathInfo.PointsUpdated.Subscribe(points =>
                {
                    car.pathPoints = points.ToArray();
                }).AddTo(this).AddTo(car);
            }
        }
    }
}