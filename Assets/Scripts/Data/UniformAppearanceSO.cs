using System.Collections.Generic;
using UnityEngine;

namespace GuildGame.Data
{
    [CreateAssetMenu(fileName = "UniformAppearance", menuName = "GuildGame/Appearance/Uniform", order = 33)]
    public class UniformAppearanceSO : ScriptableObject
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
