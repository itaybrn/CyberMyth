using System;
using System.Collections.Generic;
using UnityEngine;

public class BoundedStack<T>
{
    private int capacity;
    private LinkedList<T> stack;

    public BoundedStack(int capacity)
    {
        this.capacity = capacity;
        this.stack = new LinkedList<T>();
    }

    public void Push(T item)
    {
        if (stack.Count == capacity)
        {
            stack.RemoveFirst();
        }

        Debug.Log(stack.Count + " items in stack.");
        stack.AddLast(item);
    }

    public T Pop()
    {
        if (stack.Count == 0)
        {
            throw new InvalidOperationException("Stack is empty");
        }

        T item = stack.Last.Value;
        stack.RemoveLast();
        Debug.Log(stack.Count + " items in stack.");
        return item;
    }

    public T Peek()
    {
        if (stack.Count == 0)
        {
            throw new InvalidOperationException("Stack is empty");
        }

        return stack.Last.Value;
    }

    public bool IsEmpty()
    {
        return stack.Count == 0;
    }

    public bool IsFull()
    {
        return stack.Count == capacity;
    }

    public int Size()
    {
        return stack.Count;
    }
}
