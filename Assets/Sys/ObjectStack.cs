using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class ObjectStack<T> where T: IStackObject
{
    private Stack<T> stack;
    private System.Func<T> InitialFromSource;
    public ObjectStack(System.Func<T> InitialFromSource)
    {
        stack = new Stack<T>();
        this.InitialFromSource = InitialFromSource;
    }
    public T Pop()
    {
        if (stack.Count < 1)
        {
            return this.InitialFromSource();
        }
        T item = stack.Pop();
        item.onPop();
        return item;
    } 
    public void Push(T item)
    {
        if (item == null)
        {
            return;
        }
        stack.Push(item);
        item.onPush();
    }
    public void Destroy()
    {
        while (stack.Count > 0)
        {
            T item = stack.Pop();
            item.Destroy();
        }
        stack = null;
    }
}
public interface IStackObject
{
    void onPop();
    void onPush();
    void Destroy();
}
