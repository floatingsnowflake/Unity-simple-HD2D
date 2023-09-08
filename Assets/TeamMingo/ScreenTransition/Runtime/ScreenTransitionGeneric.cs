using System.Collections;

namespace TeamMingo.ScreenTransition.Runtime
{
  public abstract class ScreenTransitionGeneric<T> : ScreenTransitionBase where T : IScreenTransitionOptions
  {
    protected override void PrepareEnter(IScreenTransitionOptions options)
    {
      PrepareEnter((T) options);
    }

    protected override IEnumerator ProcessEnter(IScreenTransitionOptions options)
    {
      yield return ProcessEnter((T) options);
    }

    protected override void PrepareExit(IScreenTransitionOptions options)
    {
      PrepareExit((T) options);
    }

    protected override IEnumerator ProcessExit(IScreenTransitionOptions options)
    {
      yield return ProcessExit((T) options);
    }

    protected abstract void PrepareEnter(T options);

    protected abstract IEnumerator ProcessEnter(T options);

    protected abstract void PrepareExit(T options);

    protected abstract IEnumerator ProcessExit(T options);

  }
}