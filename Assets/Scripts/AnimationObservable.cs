using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationObservable : MonoBehaviour
{
    List<Observer> observers = new List<Observer>();

    public void AddObserver(Observer obj)
    {
        observers.Add(obj);
    }

    public void NotifyObservers()
    {
        foreach(Observer obj in observers)
        {
            obj.Notify();
        }
    }
}

public interface Observer
{
    void Notify();
}
