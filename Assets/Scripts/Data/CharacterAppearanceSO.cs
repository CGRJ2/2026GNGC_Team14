using System.Collections.Generic;
using UnityEngine;

namespace GuildGame.Data
{
    [CreateAssetMenu(fileName = "CharacterAppearance", menuName = "GuildGame/Appearance/Character", order = 31)]
    public class CharacterAppearanceSO : ScriptableObject
    {
        [SerializeField] private Gender _gender;
        [SerializeField] private Sprite _bodySprite;

        [Header("Hair")]
        [SerializeField] private HairAppearanceSO _hair;
        [SerializeField] private Vector2 _hairLocalPosition;

        [Header("Face")]
        [SerializeField] private List<FaceAppearanceSO> _faces = new();
        [SerializeField] private Vector2 _faceLocalPosition;

        [Header("Uniform")]
        [SerializeField] private UniformAppearanceSO _uniform;
        [SerializeField] private Vector2 _uniformLocalPosition;

        public Gender Gender => _gender;
        public Sprite BodySprite => _bodySprite;
        public HairAppearanceSO Hair => _hair;
        public Vector2 HairLocalPosition => _hairLocalPosition;
        public IReadOnlyList<FaceAppearanceSO> Faces => _faces;
        public Vector2 FaceLocalPosition => _faceLocalPosition;
        public UniformAppearanceSO Uniform => _uniform;
        public Vector2 UniformLocalPosition => _uniformLocalPosition;

        public FaceAppearanceSO GetRandomFace()
        {
            if (_faces == null || _faces.Count == 0)
                return null;

            return _faces[Random.Range(0, _faces.Count)];
        }
    }
}
