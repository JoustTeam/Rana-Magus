using System;
using System.Collections.Generic;

//this script is a generic wrapper that notifies its listeners whenever its value changes
[Serializable]
public class Observable<T> 
{
    private T value;
    //raised when the value changes, passing in the new value
    public event Action<T> ValueChanged;

    public T Value 
    {
        get => value;
        set => Set(value);
    }

    //allows observable to be directly used as its underlying type
    //eg. int health = observableHealth;
    public static implicit operator T(Observable<T> observable) => observable.value;

    //creates a new observable with an initial value and 
    //an optional listener that does not trigger from its creation
    public Observable(T value, Action<T> onValueChanged = null) 
    {
        this.value = value;

        if (onValueChanged != null)
            ValueChanged += onValueChanged;
    }

    //Updates the value to notifiy listeners only if it has changed
    public void Set(T value)
    {
        if (EqualityComparer<T>.Default.Equals(this.value, value))
            return;
        this.value = value;
        Invoke();
    }

    //manualy notifies all listeners with the value even if it hasnt changed
    public void Invoke()
    {
        ValueChanged?.Invoke(value);
    }

    //hander subscription
    public void AddListener(Action<T> handler) 
    {
        ValueChanged += handler;
    }

    //handler unsubscription
    public void RemoveListener(Action<T> handler) 
    {
        ValueChanged -= handler;
    }

    //handles memory leaks by clearing all listeners and returning the value to default
    public void Dispose() 
    {
        ValueChanged = null;
        value = default;
    }
}