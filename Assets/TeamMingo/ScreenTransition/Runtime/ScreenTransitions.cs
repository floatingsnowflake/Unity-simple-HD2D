using System.Collections;
using System.Reflection;
using TeamMingo.Common.Runtime;
using TeamMingo.Common.Runtime.Extensions;
using TeamMingo.Configs.Runtime;
using TeamMingo.MTween;
using UnityEngine;

namespace TeamMingo.ScreenTransition.Runtime
{
  public class ScreenTransitions : PersistentSingleton<ScreenTransitions>
  {

    private ScreenTransitionSettings _settings;
    private ScreenTransitionSettings.TransitionItem _showingItem;
    private ScreenTransitionBase _showingTransition;
    
    protected override void Setup()
    {
      base.Setup();
      _settings = (ScreenTransitionSettings) Configuration.Get().screenTransitionSettings;
    }

    private ScreenTransitionSettings.TransitionItem GetTransitionItem(string transitionName)
    {
      if (_showingItem != null && _showingItem.name == transitionName)
      {
        return _showingItem;
      }
      var item = _settings.GetTransition(transitionName);
      return item;
    }

    private void EnsureTransitionObject(string transitionName)
    {
      var item = GetTransitionItem(transitionName);
      if (item != _showingItem)
      {
        if (_showingTransition)
        {
          Destroy(_showingTransition.gameObject);
        }

        GameObject prefab;
        if (item.overridePrefab)
        {
          prefab = item.overridePrefab;
        }
        else
        {
          var prefabName = item.options.GetType().GetCustomAttribute<SerializeReferenceDropdownNameAttribute>().Name;
          prefab = Resources.Load<GameObject>($"Prefabs/ScreenTransition-{prefabName}");
        }

        var transitionObj = Instantiate(prefab, transform);
        _showingItem = item;
        _showingTransition = transitionObj.GetComponent<ScreenTransitionBase>();
      }
    }

    public void PrepareEnter(string transitionName)
    {
      EnsureTransitionObject(transitionName);
      _showingTransition.PrepareEnterInternal(_showingItem.options);
    }

    public void PrepareExit(string transitionName)
    {
      EnsureTransitionObject(transitionName);
      _showingTransition.PrepareExitInternal(_showingItem.options);
    }

    public IEnumerator Enter(string transitionName)
    {
      EnsureTransitionObject(transitionName);
      _showingTransition.gameObject.SetActive(true);
      yield return _showingTransition.OnEnter(_showingItem.options);
    }

    public IEnumerator Exit(string transitionName)
    {
      EnsureTransitionObject(transitionName);
      var item = _showingItem;
      _showingTransition.gameObject.SetActive(true);
      yield return _showingTransition.OnExit(_showingItem.options);
      if (item != _showingItem) yield break;
      _showingTransition.gameObject.SetActive(false);
    }

    public void EnterTransition(string transitionName)
    {
      StartCoroutine(Enter(transitionName));
    }
    
    public void ExitTransition(string transitionName)
    {
      StartCoroutine(Exit(transitionName));
    }
  }
}