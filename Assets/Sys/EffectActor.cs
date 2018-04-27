using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Class)]
public class EActorAttribute : Attribute
{
    public string name;

    public EActorAttribute(string name)
    {
        this.name = name;
    }
}

public class EffectActor : MonoBehaviour
{
    #region BaseFields

    [Header("基础-参数")]
    [SerializeField]
    private float ExitTime;

    [SerializeField]
    private float Duration;

    [SerializeField]
    private float DelayTime;

    [SerializeField]
    public EffectActor next;

    [Header("功能-参数")]
    [SerializeField]
    public string Name;

    #endregion BaseFields

    private bool _active;
    private EffectTimer.Task task;

    [HideInInspector]
    public Action<EffectActor> onActiveFalseFeedBack;

    private EffectAdaptor adaptor;
    public EffectAdaptor Adaptor
    {
        get
        {
            return adaptor;
        }
        set
        {
            adaptor = value;
            if (next != null)
            {
                next.Adaptor = adaptor;
            }
        }
    }

    public void SetActive(bool isTrigger)
    {
        if (next != null)
        {
            next.SetActive(isTrigger);
        }
        if (_active == isTrigger)
        {
            return;
        }
        if (!_active)
        {
            OnSetActiveTrue();
        }
        else
        {
            OnSetActiveFalse();
        }
    }

    private void OnSetActiveTrue()
    {
        _active = true;
        if (DelayTime > 0)
        {
            EffectTimer.Instance.RunTimerTask(DelayTime, EffectTimer.RunnerType.Once, Run,"Delay");
        }
        else
        {
            Run();
        }
    }

    private void OnSetActiveFalse()
    {
        if (ExitTime > 0)
        {
            EffectTimer.Instance.RunTimerTask(ExitTime, EffectTimer.RunnerType.Once, Close,"Close");
        }
        else
        {
            Close();
        }
    }

    private void Run()
    {
        OnRun();
        if (Duration > 0)
        {
            task = EffectTimer.Instance.RunTimerTask(Duration, EffectTimer.RunnerType.Once, OnSetActiveFalse,"Duration");
        }
        else
        {
            OnSetActiveFalse();
        }
    }

    private void Close()
    {
        OnClose();
        EffectTimer.Instance.DestroyTask(task);
        if (onActiveFalseFeedBack != null)
        {
            onActiveFalseFeedBack(this);
        }
        _active = false;
    }

    #region Func

    protected virtual void OnRun()
    {
    }

    protected virtual void OnClose()
    {
    }

    #endregion Func
}