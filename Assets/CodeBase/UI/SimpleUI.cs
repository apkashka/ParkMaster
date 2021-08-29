using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace CodeBase.UI
{
    public class SimpleUI : MonoBehaviour
    {
        public event System.Action RestartButtonClicked;

        [SerializeField] private CanvasGroup _cg;
        [SerializeField] private Text _cashText;
        [SerializeField] private Button _restartButton;
        
        public void Init(IntReactiveProperty moneyProp)
        {
            _restartButton.onClick.AddListener(ButtonClickedHandler);
            moneyProp.Subscribe(money =>
            {
                _cashText.text = $"Деняк: {money.ToString()}";
            }).AddTo(this);
        }

        private void ButtonClickedHandler()
        {
            RestartButtonClicked?.Invoke();
        }

        public void ShowWin()
        {
            _cg.alpha = 0;
            _cg.gameObject.SetActive(true);
            _cg.DOFade(1,1);
        }
    }
}
