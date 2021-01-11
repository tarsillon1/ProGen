using System;
using System.Threading;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Collections;
using UnityEngine;

namespace ProGen.Unity
{
    static class SyncContextUtil
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Install()
        {
            UnitySynchronizationContext = SynchronizationContext.Current;
            UnityThreadId = Thread.CurrentThread.ManagedThreadId;
        }

        public static int UnityThreadId
        {
            get; private set;
        }

        public static SynchronizationContext UnitySynchronizationContext
        {
            get; private set;
        }
    }

    class AsyncCoroutineRunner : MonoBehaviour
    {
        static AsyncCoroutineRunner _instance;

        public static AsyncCoroutineRunner Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameObject("AsyncCoroutineRunner")
                        .AddComponent<AsyncCoroutineRunner>();
                }

                return _instance;
            }
        }

        void Awake()
        {
            // Don't show in scene hierarchy
            gameObject.hideFlags = HideFlags.HideAndDontSave;

            DontDestroyOnLoad(gameObject);
        }
    }

    public static class AwaiterExtentions
    {
        public static SimpleCoroutineAwaiter GetAwaiter(this WaitForUpdate instruction)
        {
            return GetAwaiterReturnVoid(instruction);
        }

        static class InstructionWrappers
        {
            public static IEnumerator ReturnVoid(
                SimpleCoroutineAwaiter awaiter, object instruction)
            {
                yield return instruction;
                awaiter.Complete(null);
            }
        }

        static SimpleCoroutineAwaiter GetAwaiterReturnVoid(object instruction)
        {
            var awaiter = new SimpleCoroutineAwaiter();
            RunOnUnityScheduler(() => AsyncCoroutineRunner.Instance.StartCoroutine(
                InstructionWrappers.ReturnVoid(awaiter, instruction)));
            return awaiter;
        }

        static void RunOnUnityScheduler(Action action)
        {
            if (SynchronizationContext.Current == SyncContextUtil.UnitySynchronizationContext)
            {
                action();
            }
            else
            {
                SyncContextUtil.UnitySynchronizationContext.Post(_ => action(), null);
            }
        }

        static void Assert(bool condition)
        {
            if (!condition)
            {
                throw new Exception("an thread error occured");
            }
        }

        public class SimpleCoroutineAwaiter : INotifyCompletion
        {
            bool _isDone;
            Exception _exception;
            Action _continuation;

            public bool IsCompleted
            {
                get { return _isDone; }
            }

            public void GetResult()
            {
                Assert(_isDone);

                if (_exception != null)
                {
                    ExceptionDispatchInfo.Capture(_exception).Throw();
                }
            }

            public void Complete(Exception e)
            {
                Assert(!_isDone);

                _isDone = true;
                _exception = e;

                // Always trigger the continuation on the unity thread when awaiting on unity yield
                // instructions
                if (_continuation != null)
                {
                    RunOnUnityScheduler(_continuation);
                }
            }

            void INotifyCompletion.OnCompleted(Action continuation)
            {
                Assert(_continuation == null);
                Assert(!_isDone);

                _continuation = continuation;
            }
        }
    }

    public class WaitForUpdate : CustomYieldInstruction
    {
        public override bool keepWaiting
        {
            get { return false; }
        }
    }
}