using System.Linq;
using CodeBase.Infos;
using CodeBase.Scriptable;
using UniRx;
using UnityEngine;

namespace CodeBase.PseudoSystems
{
    public class CheckParkingSystem : MonoBehaviour
    {
        private GameManager _game;
        private CoreConfig _config;

        private CarInfo[] _affectedComponents;

        public void Construct(GameManager game, CoreConfig config)
        {
            _game = game;
            _config = config;

            //TEMP prototype solution due to lack of norm ECS
            _affectedComponents = FindObjectsOfType<CarInfo>();
            
            HandleParking();
        }

        private void HandleParking()
        {
            foreach (var car in _affectedComponents)
            {
                car.MovementStopped.Subscribe(_ =>
                {
                    if (car.parkSpot == null)
                    {
                        Debug.LogError($"No parkSpot for {car.gameObject}");
                        return;
                    }

                    if (Vector3.Distance(car.transform.position, car.parkSpot.transform.position) <
                        _config.parkingDistance)
                    {
                        car.isParked = true;

                        car.parkedFX.Play();
                    }

                    if (_affectedComponents.Any(checkedCar => !checkedCar.isParked))
                    {
                        return;
                    }

                    _game.CarsParked.Execute();
                }).AddTo(this).AddTo(car);
            }
        }
    }
}