using System;
using System.Collections.Generic;
using UnityEngine;

namespace GuildGame.Data
{
    [Serializable]
    public class FaceEmotionSprite
    {
        public CharacterEmotion emotion = CharacterEmotion.Default;
        public Sprite sprite;
    }

    [CreateAssetMenu(fileName = "FaceAppearance", menuName = "GuildGame/Appearance/Face", order = 34)]
    public class FaceAppearanceSO : ScriptableObject
    {
        [SerializeField] private List<FaceEmotionSprite> _sprites = new();

        public IReadOnlyList<FaceEmotionSprite> Sprites => _sprites;

        public Sprite GetSprite(CharacterEmotion emotion)
        {
            Sprite exactMatch = FindSprite(emotion);
            if (exactMatch != null)
                return exactMatch;

            Sprite defaultSprite = FindSprite(CharacterEmotion.Default);
            if (defaultSprite != null)
                return defaultSprite;

            if (_sprites == null)
                return null;

            for (int i = 0; i < _sprites.Count; i++)
            {
                if (_sprites[i] != null && _sprites[i].sprite != null)
                    return _sprites[i].sprite;
            }

            return null;
        }

        private Sprite FindSprite(CharacterEmotion emotion)
        {
            if (_sprites == null)
                return null;

            for (int i = 0; i < _sprites.Count; i++)
            {
                FaceEmotionSprite entry = _sprites[i];
                if (entry != null && entry.emotion == emotion && entry.sprite != null)
                    return entry.sprite;
            }

            return null;
        }
    }
}
