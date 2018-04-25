using UnityEngine;
using UnityEditor;
using System;

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
    #endregion
    private bool _active;
    private EffectTimer.Task task;
    [HideInInspector]
    public Action<EffectActor> onActiveFalseFeedBack;
    public EffectAdaptor Adaptor;

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
            EffectTimer.Instance.RunTimerTask(DelayTime, EffectTimer.RunnerType.Once, Run);
        }
        else
        {
            Run();
        }
    }
    private void OnSetActiveFalse()
    {
        _active = false;
        if (ExitTime > 0)
        {
            EffectTimer.Instance.RunTimerTask(ExitTime, EffectTimer.RunnerType.Once, Close);
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
            task = EffectTimer.Instance.RunTimerTask(Duration, EffectTimer.RunnerType.Once, OnSetActiveFalse);
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
    }

    #region Func
    protected virtual void OnRun()
    {

    }
    protected virtual void OnClose()
    {

    }
    #endregion
}
[AddComponentMenu("EffectSys")]
public class ParticleSystemActor : EffectActor
{
    [SerializeField]
    private ParticleSystem particle;

    protected override void OnRun()
    {
        particle.gameObject.SetActive(true);
        particle.Play();
    }
    protected override void OnClose()
    {
        particle.gameObject.SetActive(false);
        particle.Stop();
    }
}