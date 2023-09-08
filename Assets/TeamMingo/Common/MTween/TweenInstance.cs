using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace TeamMingo.MTween {

	using EasingFunc = Func<float, float>;
  using InterpolationFunc = Func<float[], float, float>;

	public class TweenInstance {

    public interface ITargetWrapper
    {
      public object Target { get; }
      public object GetValue(string key);

      public void SetValue(string key, object value, object delta);
    }

    private static int _keyGen = 0;
    
    public static string CreateKey()
    {
      return $"key-{++_keyGen}";
    }

    public object target { get { return wrapper.Target; } }
		public float duration { get; private set; }
    public float time { get; private set; }
    public bool started { get; private set; }
    public bool completed { get; private set; }
    public bool paused { get; private set; }
    public int repeatTimes { get; private set; }
    public bool yoyo { get; private set; }
    public float easingValue { get; private set; }
    
    public ETweenUpdateMode updateMode { get; private set; }
    public ETweenUpdateTime updateTime { get; private set; }

    private EasingFunc easingFunc = MTween.Easing.Linear;
    private InterpolationFunc interpolationFunc = MTween.Interpolation.Linear;
    private bool playingYoyo = false;
    private float unitTime = 0;

    private TweenProps startValues = new TweenProps();
    private TweenProps lastValues = new TweenProps();
    private TweenProps endValues = new TweenProps();
    private string[] keys;
    private Dictionary<string, IValueHandler> valueHandlerDict;

		private readonly ITargetWrapper wrapper;
    private Action onComplete;
    private Action onUpdate;

		public TweenInstance(ITargetWrapper wrapper) {
      this.duration = 1f;
      this.time = 0;
      this.repeatTimes = 0;
			this.wrapper = wrapper;
      this.valueHandlerDict = new Dictionary<string, IValueHandler>();
      this.updateMode = ETweenUpdateMode.Update;
      this.updateTime = ETweenUpdateTime.Normal;
    }

    public TweenInstance To(params object[] properties) {
      return To(TweenProps.FromParams(properties));
    }

    public TweenInstance By(params object[] properties) {
      return By(TweenProps.FromParams(properties));
    }

    public TweenInstance By(TweenProps properties) {
      List<string> keys = new List<string>();
      foreach(var entry in properties) {
        var valueStart = wrapper.GetValue(entry.Key);
        var valueType = ValueUtils.GetValueType(valueStart);
        var valueHandler = ValueHandlers.valueHandlerTable.GetValueHandler(valueType);
        var valueEnd = valueHandler.DoPlus(valueStart, entry.Value);
        valueHandlerDict.Add(entry.Key, valueHandler);
        startValues.Add(entry.Key, valueStart);
        lastValues.Add(entry.Key, valueStart);
        endValues.Add(entry.Key, valueEnd);
        keys.Add(entry.Key);
      }
      this.keys = keys.ToArray();
      started = true;
      return this;
    }

		public TweenInstance To(TweenProps properties) {
      List<string> keys = new List<string>();
      foreach(var entry in properties) {
        var valueStart = wrapper.GetValue(entry.Key);
        var valueEnd = entry.Value;
        var valueType = ValueUtils.GetValueType(valueStart);
        var valueHandler = ValueHandlers.valueHandlerTable.GetValueHandler(valueType);
        valueHandlerDict.Add(entry.Key, valueHandler);
        startValues.Add(entry.Key, valueStart);
        lastValues.Add(entry.Key, valueStart);
        endValues.Add(entry.Key, valueEnd);
        keys.Add(entry.Key);
      }
      this.keys = keys.ToArray();
      started = true;
      return this;
    }

    public TweenInstance Mode(ETweenUpdateMode mode)
    {
      updateMode = mode;
      return this;
    }

    public TweenInstance TimeMode(ETweenUpdateTime timeMode)
    {
      updateTime = timeMode;
      return this;
    }

    public TweenInstance Duration(float duration) {
      this.duration = duration;
      return this;
    }

    public TweenInstance Easing(EasingFunc easingFunc) {
      this.easingFunc = easingFunc;
      return this;
    }

    public TweenInstance Easing(EasingType type)
    {
      return Easing(type.ToFunc());
    }

    public TweenInstance Interpolation(InterpolationFunc interpolationFunc) {
      this.interpolationFunc = interpolationFunc;
      return this;
    }

    public TweenInstance Yoyo(bool yoyo = true) {
      this.yoyo = yoyo;
      return this;
    }

    public TweenInstance Repeat(int times=int.MaxValue) {
      this.repeatTimes = times;
      return this;
    }

    public TweenInstance OnComplete(Action onComplete)
    {
      this.onComplete = onComplete;
      return this;
    }

    public IEnumerator WaitForComplete() {
      while(!completed) {
        yield return null;
      }
    }
    

    public TweenInstance Stop(bool complete=false) {
      if(complete) {
        this.Update(this.duration);
      }
      this.completed = complete;
      return this;
    }

    public TweenInstance Pause() {
      this.paused = true;
      return this;
    }

    public TweenInstance Resume() {
      this.paused = false;
      return this;
    }

    public bool Update(float deltaTime) {
      if(!this.started) return false;
      if(this.paused) return false;
      if(this.completed) return true;

      this.time += deltaTime;
      this.unitTime += deltaTime;
      float elapsed = this.unitTime / this.duration;
      elapsed = elapsed > 1 ? 1 : elapsed;
      easingValue = this.easingFunc(this.playingYoyo ? (1-elapsed) : elapsed);

      for(int i=0,len=this.keys.Length; i<len; i++) {
        string key = this.keys[i];
        object valueStart = this.startValues[key];
        object valueLast = this.lastValues[key];
        object valueEnd = this.endValues[key];
        object value, delta;
        var valueHandler = valueHandlerDict[key];
        value = valueHandler.DoEasing(valueStart, valueEnd, easingValue);
        delta = valueHandler.DoSubtract(value, valueLast);
        this.lastValues[key] = value;
        wrapper.SetValue(key, value, delta);
      }
      
      this.onUpdate?.Invoke();

      if(elapsed >= 1) {
        this.time = this.time - this.time % this.duration + float.Epsilon;
        this.unitTime = 0;

        if (this.yoyo) {
          if (!this.playingYoyo) {
            this.playingYoyo = true;
            return false;
          } else {
            this.playingYoyo = false;
            if(this.repeatTimes >= 1) {
              this.repeatTimes --;
              return false;
            }
          }
        } else {
          if(this.repeatTimes >= 1) {
            this.repeatTimes --;
            return false;
          }
        }

        this.completed = true;
        onComplete?.Invoke();
      }

      return this.completed;
    }

    public TweenInstance OnUpdate(Action onUpdate)
    {
      this.onUpdate = onUpdate;
      return this;
    }
  }

}
