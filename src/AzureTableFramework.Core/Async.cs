﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace AzureTableFramework.Core
{
    /*
    public static class RunAll
    {
        public static async Task MethodsInObjectWithAttributeInParallel(object obj, Type att)
        {
            var methods = new List<Task>();
            var objType = obj.GetType();

            AsyncHelpers.RunSync(async () =>
            {
                Parallel.ForEach(objType.GetMethods()
                        .Where(methodInfo => methodInfo.GetCustomAttributes(att, false).Any()), methodInfo =>
                {
                    if (methodInfo.GetCustomAttributes(typeof(AsyncStateMachineAttribute), false).Any())
                        methods.Add(Task.Run(() => objType.GetMethod(methodInfo.Name).Invoke(obj, null)));
                    else
                        objType.GetMethod(methodInfo.Name).Invoke(obj, null);
                });

                await Task.WhenAll(methods.ToArray());
            });
            await Task.WhenAll(methods.ToArray());
        }

        public static async Task MethodsInObjectWithAttributeInParallelWithDoubleCheck(object obj, Type att, Type doubleCheckAtt)
        {
            await MethodsInObjectWithAttributeInParallel(obj, att);
            var objType = obj.GetType();

            var notDone = true;
            while (notDone)
            {
                var allDone = true;

                foreach (var prop in objType.GetProperties()
                    .Where(prop => prop.GetCustomAttributes(doubleCheckAtt, false).Any())
                    .Where(prop => !(bool)Utils.GetVal(obj, prop.Name)))
                    allDone = false;

                if (allDone) notDone = false;
                else await Task.Delay(25);
            }
        }

        public static async Task MethodsInObjectWithAttribute(object obj, Type attType)
        {
            var objType = obj.GetType();

            foreach (var methodInfo in objType.GetMethods()
                .Where(methodInfo => methodInfo.GetCustomAttributes(attType, false).Any()))

                if (methodInfo.GetCustomAttributes(typeof(AsyncStateMachineAttribute), false).Any())
                    await Task.Run(() => objType.GetMethod(methodInfo.Name).Invoke(obj, null));
                else
                    objType.GetMethod(methodInfo.Name).Invoke(obj, null);
        }
    }

    */

    //customerList = AsyncHelpers.RunSync<List<Customer>>(() => GetCustomers());
    public static class AsyncHelpers
    {
        /// <summary>
        /// Execute's an async Task<T> method which has a void return value synchronously
        /// </summary>
        /// <param name="task">Task<T> method to execute</param>
        public static void RunSync(Func<Task> task)
        {
            var oldContext = SynchronizationContext.Current;
            var synch = new ExclusiveSynchronizationContext();
            SynchronizationContext.SetSynchronizationContext(synch);
            synch.Post(async _ =>
            {
                try
                {
                    await task();
                }
                catch (Exception e)
                {
                    synch.InnerException = e;
                    //TODO: fix this
                    //throw;
                }
                finally
                {
                    synch.EndMessageLoop();
                }
            }, null);
            synch.BeginMessageLoop();

            SynchronizationContext.SetSynchronizationContext(oldContext);
        }

        /// <summary>
        /// Execute's an async Task<T> method which has a T return type synchronously
        /// </summary>
        /// <typeparam name="T">Return Type</typeparam>
        /// <param name="task">Task<T> method to execute</param>
        /// <returns></returns>
        public static T RunSync<T>(Func<Task<T>> task)
        {
            var oldContext = SynchronizationContext.Current;
            var synch = new ExclusiveSynchronizationContext();
            SynchronizationContext.SetSynchronizationContext(synch);
            T ret = default(T);
            synch.Post(async _ =>
            {
                try
                {
                    ret = await task();
                }
                catch (Exception e)
                {
                    synch.InnerException = e;
                    throw;
                }
                finally
                {
                    synch.EndMessageLoop();
                }
            }, null);
            synch.BeginMessageLoop();
            SynchronizationContext.SetSynchronizationContext(oldContext);
            return ret;
        }

        private class ExclusiveSynchronizationContext : SynchronizationContext
        {
            private bool done;
            public Exception InnerException { get; set; }
            private readonly AutoResetEvent workItemsWaiting = new AutoResetEvent(false);

            private readonly Queue<Tuple<SendOrPostCallback, object>> items =
                new Queue<Tuple<SendOrPostCallback, object>>();

            public override void Send(SendOrPostCallback d, object state)
            {
                throw new NotSupportedException("We cannot send to our same thread");
            }

            public override void Post(SendOrPostCallback d, object state)
            {
                lock (items)
                {
                    items.Enqueue(Tuple.Create(d, state));
                }
                workItemsWaiting.Set();
            }

            public void EndMessageLoop()
            {
                Post(_ => done = true, null);
            }

            public void BeginMessageLoop()
            {
                while (!done)
                {
                    Tuple<SendOrPostCallback, object> task = null;
                    lock (items)
                    {
                        if (items.Count > 0)
                        {
                            task = items.Dequeue();
                        }
                    }
                    if (task != null)
                    {
                        task.Item1(task.Item2);
                        if (InnerException != null) // the method threw an exeption
                        {
                            throw new AggregateException("AsyncHelpers.Run method threw an exception.", InnerException);
                        }
                    }
                    else
                    {
                        workItemsWaiting.WaitOne();
                    }
                }
            }

            public override SynchronizationContext CreateCopy()
            {
                return this;
            }
        }
    }
}