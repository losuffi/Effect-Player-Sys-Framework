using UnityEngine;

[EActor("粒子播放")]
public class ParticleSystemActor : EffectActor
{
    [SerializeField]
    private ParticleSystem particle;

    protected override void OnRun()
    {
        if (particle == null)
        {
            return;
        }
        particle.gameObject.SetActive(true);
        particle.Play();
    }

    protected override void OnClose()
    {
        if (particle == null)
        {
            return;
        }
        particle.gameObject.SetActive(false);
        particle.Stop();
    }
}