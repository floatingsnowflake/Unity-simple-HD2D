using System.Collections;
using TeamMingo.ScreenTransition.Runtime;
using UnityEngine;
using UnityEngine.UI;

namespace TeamMingo.ScreenTransition.Examples
{
  public class ScreenTransitionExample : MonoBehaviour
  {
    public Dropdown dropdown;

    private bool _playing;

    public void PlayTransition()
    {
      StartCoroutine(PlayTransitionInternal());
    }

    private IEnumerator PlayTransitionInternal()
    {
      if (_playing) yield break;
      _playing = true;
      yield return ScreenTransitions.Instance.Enter(dropdown.options[dropdown.value].text);
      yield return new WaitForSeconds(0.5f);
      yield return ScreenTransitions.Instance.Exit(dropdown.options[dropdown.value].text);
      yield return null;
      _playing = false;
    }
  }
}