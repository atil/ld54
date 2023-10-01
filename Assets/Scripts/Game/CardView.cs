using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game
{
    public class CardView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField] private TextMeshProUGUI _nameText; 
        [SerializeField] private TextMeshProUGUI _weightText; 
        [SerializeField] private TextMeshProUGUI _moneyText;
        [SerializeField] private Image _artwork;

        public CardData Card { get; private set; }

        private GameMain _game;

        public void Set(CardData card)
        {
            _game = FindObjectOfType<GameMain>();
            Card = card;
            _nameText.text = card.Name;
            _weightText.text = $"{card.Weight}kg";
            _moneyText.text = $"{card.Money}";
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _game.OnCardClicked(Card);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _game.OnCardPointerEnter(Card);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _game.OnCardPointerExit(Card);
        }

    }
}
