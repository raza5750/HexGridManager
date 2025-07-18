using System.Collections.Generic;

public class SimplePriorityQueue<T>
{
    private readonly List<(T item, int priority)> data = new();

    public int Count => data.Count;

    public void Enqueue(T item, int priority)
    {
        data.Add((item, priority));
        int ci = data.Count - 1;
        while (ci > 0)
        {
            int pi = (ci - 1) / 2;
            if (data[ci].priority >= data[pi].priority) break;
            (data[ci], data[pi]) = (data[pi], data[ci]);
            ci = pi;
        }
    }

    public T Dequeue()
    {
        var front = data[0].item;
        data[0] = data[^1];
        data.RemoveAt(data.Count - 1);
        int pi = 0;
        while (true)
        {
            int li = 2 * pi + 1, ri = li + 1, ci = pi;
            if (li < data.Count && data[li].priority < data[ci].priority) ci = li;
            if (ri < data.Count && data[ri].priority < data[ci].priority) ci = ri;
            if (ci == pi) break;
            (data[pi], data[ci]) = (data[ci], data[pi]);
            pi = ci;
        }
        return front;
    }
}
