using UnityEngine;

namespace TeamMingo.Entities.Runtime
{
  public class AnimatedEntity : Entity
  {
    public Animator animator;

    public void Play(string state)
    {
      animator.Play(state);
    }
  }
}