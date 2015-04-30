#if !(DOT_NET)

using System;
using System.Collections;


public class SingleThreadedTimer : ITimer
{
private event ElapsedEventHandler m_elapsed;

private DateTime m_nextTrigger;
private bool m_enabled;
private double m_interval;

public double Interval
{
    get
    {
        return m_interval;
    }
    set
    {
        m_interval = value;
        m_nextTrigger = DateTime.Now.AddMilliseconds(m_interval);
        }
    }

    public SingleThreadedTimer()
    {
        m_interval = 1000;
        m_enabled = false;
    }
    public SingleThreadedTimer(Double d)
    {
        m_interval = d;
        m_enabled = false;
    }

    public void Start ()
    {
        m_enabled = true;
        m_nextTrigger = DateTime.Now.AddMilliseconds(m_interval);
    }

    public void Stop()
    {
        m_enabled = false;
    }

    // to keep parity with some of the .net functions we're using
    public void Close()
    {
        Stop ();
    }

    /// <summary>
    /// Method to be called by the client at regular intervals.
    /// Checks if it's time to trigger the Timer and calls callback if so.
    /// </summary>
    public void Update ()
    {
        if (m_enabled)
        {
            if (DateTime.Now >= m_nextTrigger)
            {
                if (m_elapsed != null)
                {
                    m_elapsed();
                }
                m_nextTrigger = DateTime.Now.AddMilliseconds(m_interval);
            }
        }
    }

    public void SetEventHandler(ElapsedEventHandler handler)
    {
        m_elapsed = handler;
    }

    public void ClearEventHandler()
    {
        m_elapsed = null;
    }
}
#endif
