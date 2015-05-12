using System.Collections;

public delegate void ElapsedEventHandler();

public interface ITimer
{

    double Interval
    {
        get;
        set;
    }

    void Start ();
    void Stop();

    // to keep parity with some of the .net functions we're using
    void Close();

    /// <summary>
    /// Method to be called by the client at regular intervals.
    /// Checks if it's time to trigger the Timer and calls callback if so.
    /// </summary>
    void Update ();

    void SetEventHandler(ElapsedEventHandler handler);
    void ClearEventHandler();
}
