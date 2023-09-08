using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TeamMingo.ScreenTransition.Runtime
{
  public class SceneSwitcher : MonoBehaviour
  {
    public string sceneName;
    public LoadSceneMode sceneLoadMode = LoadSceneMode.Single;
    
    [ScreenTransitionSelector]
    public string transitionBeforeSwitch;
    [ScreenTransitionSelector]
    public string transitionAfterSwitch;

    public void Switch()
    {
      ScreenTransitions.Instance.StartCoroutine(SwitchInternal());
    }

    private IEnumerator SwitchInternal()
    {
      yield return ScreenTransitions.Instance.Enter(transitionBeforeSwitch);
      yield return LoadScene();
      yield return ScreenTransitions.Instance.Exit(transitionAfterSwitch);
    }

    protected virtual IEnumerator LoadScene()
    {
      SceneManager.LoadScene(sceneName, sceneLoadMode);
      yield return null;
    }

  }
}