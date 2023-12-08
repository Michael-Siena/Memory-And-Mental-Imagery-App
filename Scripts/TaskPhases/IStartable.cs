using System.Collections;
using UnityEngine;

namespace TaskPhases
{
    public interface IWaitable
    {
        GameObject StartPanel { get; }
        GameObject EndPanel { get; }

        IEnumerator WaitToStart();
        IEnumerator WaitToEnd();
    }
}