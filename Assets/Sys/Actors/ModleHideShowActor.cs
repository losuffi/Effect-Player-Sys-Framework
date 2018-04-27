using UnityEngine;

[EActor("模型显示")]
public class ModleHideShowActor : EffectActor
{
    [SerializeField]
    private Transform modle;

    protected override void OnRun()
    {
        if (modle == null)
        {
            return;
        }
        modle.gameObject.SetActive(true);
    }

    protected override void OnClose()
    {
        if (modle == null)
        {
            return;
        }
        modle.gameObject.SetActive(false);
    }
}