using BrainCloud;

namespace BrainCloudTests
{
    public delegate void SpinTestRunMethod();

    public class SpinTest
    {
        private bool m_isDone = false;
       
        public SpinTest()
        {}

        /// <summary>
        /// Creates new instance and runs the run method right away
        /// </summary>
        /// <param name="method">Method to run</param>
        public SpinTest(SpinTestRunMethod method)
        {
            Run(method);
        }

        public void Run(SpinTestRunMethod method)
        {
            m_isDone = false;
            method();
            WaitAndSpin();
        }

        private bool IsDone()
        {
            return m_isDone; // maybe add timeout on this in the future
        }
        
        private void WaitAndSpin()
        {
            while (!IsDone())
            {
                BrainCloudClient.Get().Update();
            }
        }
    }
}