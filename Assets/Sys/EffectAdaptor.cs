using System.Collections.Generic;
using UnityEngine;



public abstract class EffectAdaptorUnit
{

}
/// <summary>
/// If Modify
/// Aim: Search Performance(Data Struct)
/// </summary>
public class EffectAdaptor
{
    public string Name;
    private EffectAdaptor parent;
    public EffectAdaptor Parent { get { return parent; } }
    private List<EffectAdaptor> childs;
    public List<EffectAdaptor> Childs
    {
        get
        {
            if (childs == null)
            {
                childs = new List<EffectAdaptor>();
            }
            return childs;
        }
    }
    private List<EffectAdaptorUnit> units;

    public EffectAdaptor(string name)
    {
        Name = name;
        parent = null;
    }

    public void AddUnit<T>(T adaptor) where T : EffectAdaptorUnit
    {
        if (units == null)
        {
            units = new List<EffectAdaptorUnit>();
        }
        units.Add(adaptor);
    }
    public T GetUnit<T>() where T : EffectAdaptorUnit
    {
        if (units == null)
        {
            return null;
        }
        T unit = null;
        for(int i = 0; i < units.Count; i++)
        {
            unit = units[i] as T;
            if (unit != null)
            {
                break;
            }
        }
        return unit;
    }
    public void SetParent(EffectAdaptor parent)
    {
        this.parent = parent;
        if (parent != null)
        {
            parent.Childs.Add(this);
        }
    }

    /// <summary>
    /// Time：O(n^2),Space:O(n)——Tree Traverse .Dev-reduce Space
    /// </summary>
    /// <param name="name">Index Find</param>
    /// <returns></returns>
    public EffectAdaptor FindAdator(string name)
    {
        if (Name.Equals(name))
        {
            return this;
        }
        EffectAdaptor adaptor = null;
        for(int i = 0; i < Childs.Count; i++)
        {
            adaptor = Childs[i].FindAdator(name);
            if (adaptor != null)
            {
                break;
            }
        }
        return adaptor;
    }
}

