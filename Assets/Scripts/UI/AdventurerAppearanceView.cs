using GuildGame.Data;
using GuildGame.Gameplay.Models;
using UnityEngine;
using UnityEngine.UI;

namespace GuildGame.UI
{
    public class AdventurerAppearanceView : UIViewBase
    {
        [SerializeField] private CharacterAppearanceDatabaseSO _database;

        [Header("Layer Images")]
        [SerializeField] private Image _bodyImage;
        [SerializeField] private Image _faceImage;
        [SerializeField] private Image _hairImage;
        [SerializeField] private Image _uniformImage;

        private CharacterAppearanceSO _currentCharacter;
        private FaceAppearanceSO _currentFace;

        public Gender CurrentGender { get; private set; }
        public CharacterEmotion CurrentEmotion { get; private set; } = CharacterEmotion.Default;

        protected override void OnBind()
        {
            Context.CaseStarted += OnCaseStarted;
        }

        private void OnCaseStarted(AdventurerCase adventurerCase)
        {
            Randomize();
        }

        public void Randomize()
        {
            if (_database == null)
                return;

            _currentCharacter = _database.GetRandomCharacter();
            if (_currentCharacter == null)
                return;

            CurrentGender = _currentCharacter.Gender;
            CurrentEmotion = CharacterEmotion.Default;
            _currentFace = _currentCharacter.GetRandomFace();

            Apply(_bodyImage, _currentCharacter.BodySprite, Vector2.zero);
            Apply(_hairImage, _currentCharacter.Hair?.GetRandomSprite(), _currentCharacter.HairLocalPosition);
            Apply(_uniformImage, _currentCharacter.Uniform?.GetRandomSprite(), _currentCharacter.UniformLocalPosition);
            ApplyFace(CurrentEmotion);
        }

        public void SetEmotion(CharacterEmotion emotion)
        {
            CurrentEmotion = emotion;
            ApplyFace(CurrentEmotion);
        }

        private void ApplyFace(CharacterEmotion emotion)
        {
            if (_currentCharacter == null || _currentFace == null)
            {
                Clear(_faceImage);
                return;
            }

            Apply(_faceImage, _currentFace.GetSprite(emotion), _currentCharacter.FaceLocalPosition);
        }

        private static void Apply(Image image, Sprite sprite, Vector2 localPosition)
        {
            if (image == null)
                return;

            image.sprite = sprite;
            image.enabled = sprite != null;
            image.rectTransform.anchoredPosition = localPosition;
        }

        private static void Clear(Image image)
        {
            if (image == null)
                return;

            image.sprite = null;
            image.enabled = false;
        }

        private void OnDestroy()
        {
            if (Context != null)
                Context.CaseStarted -= OnCaseStarted;
        }
    }
}
