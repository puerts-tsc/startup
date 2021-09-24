using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Runtime
{
    public static partial class Awaiters
    {
        public class UnityWebRequestAwaiter : INotifyCompletion
        {
            UnityWebRequestAsyncOperation asyncOp;
            Action                        continuation;

            public UnityWebRequestAwaiter( UnityWebRequestAsyncOperation asyncOp )
            {
                this.asyncOp = asyncOp;
                asyncOp.completed += OnRequestCompleted;
            }

            public bool IsCompleted => asyncOp.isDone;
            public void GetResult() { }

            public void OnCompleted( Action continuation )
            {
                this.continuation = continuation;
            }

            void OnRequestCompleted( AsyncOperation obj )
            {
                continuation();
            }
        }

        public static UnityWebRequestAwaiter GetAwaiter( this UnityWebRequestAsyncOperation asyncOp ) =>
            new UnityWebRequestAwaiter( asyncOp );

        /**
     *https://gist.github.com/mattyellen/d63f1f557d08f7254345bff77bfdc8b3
     * Allows the use of async/await (instead of yield) with any Unity AsyncOperation
     * Example:
var getRequest = UnityWebRequest.Get("http://www.google.com");
await getRequest.SendWebRequest();
var result = getRequest.downloadHandler.text;
     */
        public static TaskAwaiter GetAwaiter( this AsyncOperation asyncOp )
        {
            var tcs = new TaskCompletionSource<object>();
            asyncOp.completed += obj => { tcs.SetResult( null ); };
            return ( (Task)tcs.Task ).GetAwaiter();
        }
    }
}