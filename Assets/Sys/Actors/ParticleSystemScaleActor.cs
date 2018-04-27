using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
[EActor("粒子Scale通道")]
public class ParticleSystemScaleActor:EffectActor
{
    private enum Type
    {
        StartSpeed,
        StartSize,
        ShapeRadius,
        Max,
    }
    [SerializeField]
    private Type type;
    [SerializeField]
    private ParticleSystem Target;
    protected override void OnRun()
    {
        if (Target == null)
        {
            return;
        }
        ScaleUnit sunit = Adaptor.GetUnit<ScaleUnit>();
        if (sunit == null)
        {
            return;
        }
        SetScale(sunit.Scale);
        Target.gameObject.SetActive(true);
        Target.Play();
    }
    protected override void OnClose()
    {
        if (Target == null)
        {
            return;
        }
        ScaleUnit sunit = Adaptor.GetUnit<ScaleUnit>();
        if (sunit == null)
        {
            return;
        }
        SetScale(1/sunit.Scale);
        Target.gameObject.SetActive(false);
        Target.Stop();
    }
    private void SetScale(float scale)
    {
        switch (type)
        {
            case Type.StartSpeed:
                Target.startSpeed *= scale;
                break;
            case Type.StartSize:
                Target.startSize *= scale;
                break;
            case Type.ShapeRadius:
                ParticleSystem.ShapeModule s = Target.shape;
                s.radius *= scale;
                break;
            default:
                break;
        }
    }
}
