using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public static class NavigationHelper
{


    static public void NoiseSpread<Node>(
            Node source,
        Func<Node, Node, double> distance,
        Action<Node ,double > operate
            ) where Node : IHasNeighbours<Node>
    {
        var closed = new HashSet<Node>();
        var queue = new PriorityQueue<double, Path<Node>>();
        queue.Enqueue(0, new Path<Node>(source));
        while (!queue.IsEmpty)
        {
            var path = queue.Dequeue();
            if (closed.Contains(path.LastStep))
            { continue; }
            closed.Add(path.LastStep);
            operate(path.LastStep, path.TotalCost);
            foreach (Node n in path.LastStep.Neighbours)
            {
                double d = distance(path.LastStep, n);
                var newPath = path.AddStep(n, d);
                //queue.Enqueue(newPath.TotalCost + estimate(n), newPath);
                queue.Enqueue(0D, newPath);
            }
        }
    }

    static public Path<Node> FindPath<Node>(
        Node start,
        Node destination,
            int maxStepCount,
        Func<Node, Node, double> distance,
        Func<Node, double> estimate)
        where Node : IHasNeighbours<Node>
    {

        var closed = new HashSet<Node>();
        var queue = new PriorityQueue<double, Path<Node>>();
        queue.Enqueue(0, new Path<Node>(start));

        while (!queue.IsEmpty)
        {
            var path = queue.Dequeue();
            if (closed.Contains(path.LastStep))
                continue;
            if (distance(start, destination) - distance(path.LastStep, destination) > maxStepCount)
            { return path; }
            if (path.LastStep.Equals(destination))
            { return path; }
            closed.Add(path.LastStep);
            foreach (Node n in path.LastStep.Neighbours)
            {
                double d = distance(path.LastStep, n);
                var newPath = path.AddStep(n, d);
                queue.Enqueue(newPath.TotalCost + estimate(n), newPath);
            }
        }
        return null;
    }
}