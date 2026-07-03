using System.Collections.Generic;
using UnityEngine;

namespace GuildGame.Data
{
    [CreateAssetMenu(fileName = "HairAppearance", menuName = "GuildGame/Appearance/Hair", order = 32)]
    public class HairAppearanceSO : ScriptableObject
    {
        [SerializeField] private List<Sprite> _sprites = new();

        public IReadOnlyList<Sprite> Sprites => _sprites;

        public Sprite GetRandomSprite()
        {
            if (_sprites == null || _sprites.Count == 0)
                return null;

            return _sprites[Random.Range(0, _sprites.Count)];
        }
    }
}
