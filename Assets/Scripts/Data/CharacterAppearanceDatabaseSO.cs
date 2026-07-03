using System.Collections.Generic;
using UnityEngine;

namespace GuildGame.Data
{
    [CreateAssetMenu(fileName = "CharacterAppearanceDatabase", menuName = "GuildGame/Character Appearance Database", order = 30)]
    public class CharacterAppearanceDatabaseSO : ScriptableObject
    {
        [SerializeField] private List<CharacterAppearanceSO> _characters = new();

        public IReadOnlyList<CharacterAppearanceSO> Characters => _characters;

        public CharacterAppearanceSO GetRandomCharacter()
        {
            if (_characters == null || _characters.Count == 0)
                return null;

            return _characters[Random.Range(0, _characters.Count)];
        }
    }
}
