using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class EffectSequence : MonoBehaviour,IStackObject
{
    [System.Serializable]
    private struct MatrixRow
    {
        public EffectActor[] rows;
        public EffectActor this[int index]
        {
            get
            {
                return rows[index];
            }
        }
        public int Length
        {
            get
            {
                return rows.Length;
            }
        }
    }
    [SerializeField]
    private List<MatrixRow> matrix;
    [SerializeField]
    private CollectType type;
    [HideInInspector]
    public System.Action<EffectSequence> CompleteFeedback;
    private int[] probes;
    private enum CollectType
    {
        Once,
        Iterator,
    }
    private bool IsRuntime;
    private EffectAdaptor Adaptor;
    public void Play()
    {
        if (probes == null)
        {
            Init();
        }
        if (!IsRuntime)
        {
            IsRuntime = true;
            InitProbes();
            ActorsResponse();
        }
    }
    public void Play(EffectAdaptor adaptor)
    {
        Adaptor = adaptor;
        Play();
    }
    public void Init()
    {
        InitActor();
    }
    private void Close()
    {
        if (probes == null)
        {
            return;
        }
        for(int i = 0; i < probes.Length; i++)
        {
            probes[i] = 0;
        }
        for(int m = 0; m < matrix.Count; m++)
        {
            for(int n = 0; n < matrix[m].Length; n++)
            {
                matrix[m][n].SetActive(false);
            }
        }
    }
    private void InitProbes()
    {
        if (probes == null)
        {
            probes = new int[matrix.Count];
        }
        for(int i = 0; i < probes.Length; i++)
        {
            probes[i] = 0;
        }
    }
    private void InitActor()
    {
        InitProbes();
        for(int i = 0; i < matrix.Count; i++)
        {
            for(int j = 0; j < matrix[i].Length; j++)
            {
                matrix[i][j].onActiveFalseFeedBack += SequenceUpdate;
                matrix[i][j].Adaptor = Adaptor;
            }
        }
    }
    private void SequenceUpdate(EffectActor actor)
    {
        for(int i = 0; i < probes.Length; i++)
        {
            if (probes[i] < matrix[i].Length)
            {
                if (matrix[i][probes[i]] == actor)
                {
                    probes[i]++;
                }
            }
        }
        ActorsResponse();
    }
    private void ActorsResponse()
    {
        bool isFull = true;
        for (int i = 0; i < probes.Length; i++)
        {
            if (probes[i] < matrix[i].Length)
            {
                isFull = false;
                if (matrix[i][probes[i]]!=null)
                {
                    matrix[i][probes[i]].SetActive(true);
                }
            }
        }
        if (isFull)
        {
            SequenceComplete();
        }
    }
    private void SequenceComplete()
    {
        IsRuntime = false;
        if (type == CollectType.Once)
        {
            Destroy(gameObject);
        }
        else
        {
            Close();
        }
        CompleteFeedback(this);

    }

    public void onPop()
    {
        gameObject.SetActive(true);
    }

    public void onPush()
    {
        gameObject.SetActive(false);
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}

[System.Serializable]
public class EffectActor
{
    public Action<EffectActor> onActiveFalseFeedBack;
    public EffectAdaptor Adaptor;

    public virtual void SetActive(bool isTrigger)
    {

    }
}


