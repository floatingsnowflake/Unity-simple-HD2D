using TeamMingo.Entities.Runtime;
using UnityEngine;

namespace TeamMingo.Characters.Runtime
{
  [RequireComponent(typeof(AnimatedEntity))]
  public class CharacterBase : MonoBehaviour
  {
    public AnimatedEntity Entity
    {
      get
      {
        if (!_entity)
        {
          _entity = GetComponent<AnimatedEntity>();
        }

        return _entity;
      }
    }

    private AnimatedEntity _entity;
    
    public SpriteRenderer graphics;
    public CharacterInput input;
  }
}