using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public partial class SequenceBuilder : EditorWindow
{
    [MenuItem("Tools/Effect Sequence")]
    public static void Go()
    {
        SequenceBuilder w = EditorWindow.GetWindow<SequenceBuilder>();
        w.Show();
    }

    public static void OtherCall(EffectSequence s)
    {
        SequenceBuilder w = EditorWindow.GetWindow<SequenceBuilder>();
        w.Show();
        w.sequence = s;
    }

    private Transform header;
    private EffectSequence sequence;
    private List<EffectActor> HeadersSource;
    private EffectActor SelectedSource;

    private void OnGUI()
    {
        GUILayout.Label("--------目标-------------");
        if (!HasTargetView())
        {
            return;
        }
        GUILayout.Label("----------------------------");
        GUILayout.Label("--------序列表-------------");
        MatrixView();
        GUILayout.Label("----------------------------");
        GUILayout.Label("--------列编辑-------------");
        SequenceView();
        GUILayout.Label("----------------------------");
        GUILayout.Label("--------动作组编辑-------------");
        ActorView();
        GUILayout.Label("----------------------------");
        GUILayout.Label("--------动作组池-------------");
        SourceView();
        GUILayout.Label("----------------------------");
    }

    private bool HasTargetView()
    {
        GUILayout.Label("Target:");
        sequence = EditorGUILayout.ObjectField(sequence, typeof(EffectSequence), true) as EffectSequence;
        return (sequence != null);
    }

    private void SourceView()
    {
        if (HeadersSource == null)
        {
            HeadersSource = new List<EffectActor>();
            SelectedSource = null;
        }
        EffectActor dis = null;
        foreach (var s in HeadersSource)
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.ObjectField(s, s.GetType(), true);
            if (GUILayout.Button("删除"))
            {
                dis = s;
            }
            if (GUILayout.Button("选中"))
            {
                SelectedSource = s;
            }
            GUILayout.EndHorizontal();
        }
        if (dis != null)
        {
            GameObject.DestroyImmediate(dis.gameObject);
        }
        GUILayout.Label("当前选中：");
        GUILayout.Label(SelectedSource == null ? "空" : SelectedSource.Name);
    }

    private Rect RelRect(float x, float y, float width, float height)
    {
        return (new Rect(x * EditorGUIUtility.currentViewWidth, y * EditorGUIUtility.currentViewWidth,
            width * EditorGUIUtility.currentViewWidth, height * EditorGUIUtility.currentViewWidth));
    }
}

public partial class SequenceBuilder : EditorWindow
{
    private List<EffectActor> content;
    private List<Type> ActorType;
    private string[] typeNames;
    private int index;
    private EffectActor probe;
    private string Name;

    private void ActorEditorInit()
    {
        content = new List<EffectActor>();
        if (ActorType == null)
        {
            ActorType = new List<Type>();
            List<string> _typeNames = new List<string>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies().Where(res => res.FullName.Contains("Assembly-")))
            {
                foreach (Type type in assembly.GetTypes().Where(res => !res.IsAbstract && res.IsClass && res.IsSubclassOf(typeof(EffectActor))))
                {
                    ActorType.Add(type);
                    object[] attrs = type.GetCustomAttributes(true);
                    EActorAttribute ea = attrs.FirstOrDefault(res => res.GetType() == typeof(EActorAttribute)) as EActorAttribute;
                    if (ea != null)
                    {
                        _typeNames.Add(ea.name);
                    }
                    else
                    {
                        _typeNames.Add(type.Name);
                    }
                }
            }
            typeNames = _typeNames.ToArray();
        }
        index = 0;
        header = null;
        probe = null;
    }

    private void ActorEditorInit(EffectActor h)
    {
        ActorEditorInit();
        content.Add(h);
        while (h.next != null)
        {
            h = h.next;
            content.Add(h);
        }
    }

    private void ActorView()
    {
        if (GUILayout.Button("新建Actor组合："))
        {
            ActorEditorInit();
        }

        GUILayout.Label("Actor 组合:");
        if (typeNames != null && content != null)
        {
            GUILayout.Label("Actor 类型：");
            index = EditorGUILayout.Popup(index, typeNames);
            Name = EditorGUILayout.TextField(Name);
            if (GUILayout.Button("添加"))
            {
                if (content.Count == 0)
                {
                    header = new GameObject(Name).transform;
                    header.SetParent(sequence.transform);
                    probe = header.gameObject.AddComponent(ActorType[index]) as EffectActor;
                    probe.Name = Name;
                }
                else
                {
                    Transform child = header.Find("others");
                    if (child == null)
                    {
                        child = new GameObject("other").transform;
                        child.SetParent(header);
                    }
                    var probetemp = child.gameObject.AddComponent(ActorType[index]) as EffectActor;
                    probe.next = probetemp;
                    probe = probetemp;
                }
                content.Add(probe);
            }
        }

        GUILayout.Label("当前编辑Actor组合：");
        if (content != null)
        {
            EffectActor pre = null;
            EffectActor dis = null;
            foreach (EffectActor actor in content)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(actor.GetType().ToString());
                EditorGUILayout.ObjectField(actor, actor.GetType(), true);
                if (GUILayout.Button("删除"))
                {
                    if (pre != null)
                    {
                        pre.next = actor.next;
                        dis = actor;
                    }
                }
                GUILayout.EndHorizontal();
            }
            if (dis != null)
            {
                content.Remove(dis);
                GameObject.DestroyImmediate(dis.gameObject);
            }
            if (GUILayout.Button("保存当前Actor素材"))
            {
                var hsource = header.GetComponent<EffectActor>();
                if (!HeadersSource.Contains(hsource))
                {
                    HeadersSource.Add(hsource);
                }
            }
        }
    }
}

public partial class SequenceBuilder : EditorWindow
{
    private EffectSequence.MatrixRow currentEffectRow;

    private void SequenceView()
    {
        if (GUILayout.Button("添加新列"))
        {
            currentEffectRow = new EffectSequence.MatrixRow();
            sequence.GetMatrix().Add(currentEffectRow);
        }
        GUILayout.BeginHorizontal();
        GUILayout.Label("当前播放列：");
        GUILayout.Label((sequence.GetMatrix().IndexOf(currentEffectRow) + 1).ToString());
        GUILayout.EndHorizontal();
        if (currentEffectRow == null)
        {
            return;
        }
        EffectActor dis = null;
        foreach (EffectActor h in currentEffectRow.rows)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(h.name);
            EditorGUILayout.ObjectField(h, h.GetType(), true);
            if (GUILayout.Button("编辑"))
            {
                ActorEditorInit(h);
            }
            if (GUILayout.Button("删除"))
            {
                dis = h;
            }
            GUILayout.EndHorizontal();
        }
        currentEffectRow.rows.Remove(dis);
        if (GUILayout.Button("添加Actor组合"))
        {
            if (SelectedSource != null)
            {
                currentEffectRow.rows.Add(SelectedSource);
            }
        }
    }
}

public partial class SequenceBuilder : EditorWindow
{
    private string[] RowsIndex;
    private int ind;

    private void MatrixView()
    {
        RowsIndex = new string[sequence.GetMatrix().Count];
        for (int i = 0; i < RowsIndex.Length; i++)
        {
            RowsIndex[i] = "第" + (i + 1) + "列";
        }
        ind = EditorGUILayout.Popup(ind, RowsIndex);
        if (GUILayout.Button("选中"))
        {
            currentEffectRow = sequence.GetMatrix()[ind];
        }
    }
}