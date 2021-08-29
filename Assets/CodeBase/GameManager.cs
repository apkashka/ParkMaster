using CodeBase.PseudoSystems;
using CodeBase.Scriptable;
using CodeBase.UI;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CodeBase
{
    public class GameManager : MonoBehaviour
    {
        public readonly ReactiveCommand DrawStarted = new ReactiveCommand();
        public readonly ReactiveCommand DrawEnded = new ReactiveCommand();
        public readonly ReactiveCommand CarsParked = new ReactiveCommand();

        public readonly IntReactiveProperty Money = new IntReactiveProperty();

        [SerializeField] private CoreConfig _config;

        [Header("UI")] 
        [SerializeField] private SimpleUI _uiView;
        
        [Header("Systems")] 
        [SerializeField] private ExplosionSystem _explosionSystem;
        [SerializeField] private PathsBuildingSystem _pathsBuildingSystem;
        [SerializeField] private CheckParkingSystem _checkParkingSystem;
        [SerializeField] private MovementSystem _movementSystem;
        [SerializeField] private CashSystem _cashSystem;

        // Start is called before the first frame update
        private void Awake()
        {
            InitSystems();
            InitUI();
        }

        private void InitSystems()
        {
            _pathsBuildingSystem.Construct(this, _config, Camera.main);
            _checkParkingSystem.Construct(this, _config);
            _movementSystem.Construct(this, _config);
            _explosionSystem.Construct(this, _config);
            _cashSystem.Construct(this);
        }
        
        private void InitUI()
        {
            _uiView.Init(Money);
            _uiView.RestartButtonClicked += RestartGame;
            CarsParked.Subscribe(_ => _uiView.ShowWin()).AddTo(this);
        }

        private void RestartGame()
        {
            SceneManager.LoadScene(0);
        }
    }
}