#if (DOT_NET)

    using System;
using System.Collections;
using System.Timers;

public class MultiThreadedTimer : ITimer
{
    private Timer m_timer;
    private ElapsedEventHandler m_eventHandler;

    public double Interval
    {
        get
        {
            return m_timer.Interval;
        }
        set
        {
            m_timer.Interval = value;
        }
    }

    public MultiThreadedTimer()
    {
        m_timer = new Timer();
    }

    public MultiThreadedTimer(Double d)
    {
        m_timer = new Timer(d);
    }

    public void Start ()
    {
        m_timer.Start();
        m_timer.Enabled = true;
    }

    public void Stop()
    {
        m_timer.Stop();
    }

    // to keep parity with some of the .net functions we're using
    public void Close()
    {
        m_timer.Close ();
    }

    public void Update ()
    {
    }

    public void TimerElapsedEventHandler(object obj, System.Timers.ElapsedEventArgs args)
    {
        if (m_eventHandler != null)
        {
            m_eventHandler();
        }
    }

    public void SetEventHandler(ElapsedEventHandler handler)
    {
        m_eventHandler = handler;
        m_timer.Elapsed += this.TimerElapsedEventHandler;
    }

    public void ClearEventHandler()
    {
        m_eventHandler = null;
        m_timer.Elapsed -= this.TimerElapsedEventHandler;
    }
}
#endif
