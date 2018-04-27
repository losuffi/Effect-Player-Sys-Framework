using System.Collections.Generic;
using UnityEngine;

public class EffectSequence : MonoBehaviour, IStackObject
{
    [System.Serializable]
    public class MatrixRow
    {
        public List<EffectActor> rows;

        public EffectActor this[int index]
        {
            get
            {
                if (index >= rows.Count||index<0)
                {
                    return null;
                }
                return rows[index];
            }
        }

        public int Length
        {
            get
            {
                return rows.Count;
            }
        }

        public MatrixRow()
        {
            rows = new List<EffectActor>();
            IsRun = true;
        }

        public bool IsRun=true;
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
        Init();
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
        Synchronize();
    }

    private void Synchronize()
    {
        TransformAdaptorUnit unit = Adaptor.GetUnit<TransformAdaptorUnit>();
        if (unit == null)
        {
            return;
        }
        transform.position = unit.Parent.position;
        transform.eulerAngles = unit.Parent.eulerAngles;
        transform.localScale = unit.Parent.localScale;
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
        for (int i = 0; i < probes.Length; i++)
        {
            probes[i] = 0;
        }
    }

    private void InitProbes()
    {
        if (probes == null)
        {
            probes = new int[matrix.Count];
        }
        for (int i = 0; i < probes.Length; i++)
        {
            probes[i] = 0;
        }
    }

    private void InitActor()
    {
        InitProbes();
        for (int i = 0; i < matrix.Count; i++)
        {
            for (int j = 0; j < matrix[i].Length; j++)
            {
                matrix[i][j].onActiveFalseFeedBack += SequenceUpdate;
                matrix[i][j].Adaptor = Adaptor;
            }
        }
    }

    private void SequenceUpdate(EffectActor actor)
    {
        for (int i = 0; i < probes.Length; i++)
        {
            if (!matrix[i].IsRun)
            {
                continue;
            }
            if (probes[i] < matrix[i].Length)
            {
                if (matrix[i][probes[i]] == actor)
                {
                    probes[i] = probes[i] + 1;
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
            if (!matrix[i].IsRun)
            {
                continue;
            }
            if (probes[i] < matrix[i].Length)
            {
                isFull = false;
                if (matrix[i][probes[i]] != null)
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
        if (CompleteFeedback != null)
        {
            CompleteFeedback(this);
        }
        else
        {
            Debug.Log("空回调");
        }
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

    #region Editor Use

    public List<MatrixRow> GetMatrix()
    {
        if (matrix == null)
        {
            matrix = new List<MatrixRow>();
        }
        return matrix;
    }

    public MatrixRow AddSequence()
    {
        MatrixRow temp = new MatrixRow();
        matrix.Add(temp);
        return temp;
    }

    #endregion Editor Use
}