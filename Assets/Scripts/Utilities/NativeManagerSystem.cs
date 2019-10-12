using System;
using System.Collections.Generic;
using Unity.Entities;

namespace Unity.Collections
{
    [AlwaysUpdateSystem]
    [UpdateAfter(typeof(EndFrameBarrier))]
    public class NativeManagerSystem : ComponentSystem
    {
        private Queue<IDisposable> tempJobDisposeQueue = new Queue<IDisposable>();

        private Queue<IDisposable> persistentDisposeQueue = new Queue<IDisposable>();

        private Queue<IDisposable> disposableProcessQueue = new Queue<IDisposable>();
        private Queue<Allocator> allocatorProcessQueue = new Queue<Allocator>();

        private void EnqueueNative(IDisposable native, Allocator allocator)
        {
            disposableProcessQueue.Enqueue(native);
            allocatorProcessQueue.Enqueue(allocator);
        }
        
        protected override void OnUpdate()
        {
            EntityManager.CompleteAllJobs();
            DisposeTempJobs();
            ProcessDisposables();
        }

        protected override void OnDestroyManager()
        {
            EntityManager.CompleteAllJobs();
            ProcessDisposables();
            DisposeTempJobs();
            DisposePersistents();
        }

        private void DisposeTempJobs()
        {
            int count = tempJobDisposeQueue.Count;
            for (int i = 0; i < count; i++)
            {
                tempJobDisposeQueue.Dequeue().Dispose();
            }
        }

        private void DisposePersistents()
        {
            int count = persistentDisposeQueue.Count;
            for (int i = 0; i < count; i++)
            {
                persistentDisposeQueue.Dequeue().Dispose();
            }
        }

        private void ProcessDisposables()
        {
            int count = disposableProcessQueue.Count;
            for (int i = 0; i < count; i++)
            {
                var manager = disposableProcessQueue.Dequeue();
                var allocator = allocatorProcessQueue.Dequeue();

                switch (allocator)
                {
                    case Allocator.Temp:
                        manager.Dispose();
                        break;
                    case Allocator.TempJob:
                        tempJobDisposeQueue.Enqueue(manager);
                        break;
                    case Allocator.Persistent:
                        persistentDisposeQueue.Enqueue(manager);
                        break;
                }
            }
        }

        public void AllocateAndAutoDispose<T>(ref NativeArray<T> native, int length, Allocator allocator, NativeArrayOptions options = NativeArrayOptions.ClearMemory) where T : struct
        {
            native = new NativeArray<T>(length, allocator, options);
            EnqueueNative(native, allocator);
        }
        public void AllocateAndAutoDispose<T>(ref NativeQueue<T> native, Allocator allocator) where T : struct
        {
            native = new NativeQueue<T>(allocator);
            EnqueueNative(new NativeQueueWrapper<T>(native), allocator);
        }
        public void AllocateAndAutoDispose<T1, T2>(ref NativeHashMap<T1, T2> native, int capacity, Allocator allocator) where T1 : struct, IEquatable<T1> where T2 : struct
        {
            native = new NativeHashMap<T1, T2>(capacity, allocator);
            EnqueueNative(new NativeHashMapWrapper<T1, T2>(native), allocator);
        }
        public void AllocateAndAutoDispose<T1, T2>(ref NativeMultiHashMap<T1, T2> native, int capacity, Allocator allocator) where T1 : struct, IEquatable<T1> where T2 : struct
        {
            native = new NativeMultiHashMap<T1, T2>(capacity, allocator);
            EnqueueNative(new NativeMultiHashMapWrapper<T1, T2>(native), allocator);
        }
        public void AllocateAndAutoDispose<T>(ref NativeList<T> native, int capacity, Allocator allocator) where T : struct
        {
            native = new NativeList<T>(capacity, allocator);
            EnqueueNative(native, allocator);
        }
        public void AllocateAndAutoDispose<T>(ref NativeList<T> native, Allocator allocator) where T : struct
        {
            native = new NativeList<T>(allocator);
            EnqueueNative(native, allocator);
        }

        public NativeArray<T> AllocateAndAutoDisposeNativeArray<T>(int length, Allocator allocator, NativeArrayOptions options = NativeArrayOptions.ClearMemory) where T : struct
        {
            var native = new NativeArray<T>(length, allocator);
            EnqueueNative(native, allocator);
            return native;
        }

        public NativeQueue<T> AllocateAndAutoDisposeNativeQueue<T>(Allocator allocator) where T : struct
        {
            var native = new NativeQueue<T>(allocator);
            EnqueueNative(new NativeQueueWrapper<T>(native), allocator);
            return native;
        }

        public NativeHashMap<T1, T2> AllocateAndAutoDisposeNativeHashMap<T1, T2>(int capacity, Allocator allocator)
            where T1 : struct, IEquatable<T1>
            where T2 : struct
        {
            var native = new NativeHashMap<T1, T2>(capacity, allocator);
            EnqueueNative(new NativeHashMapWrapper<T1, T2>(native), allocator);
            return native;
        }

        public NativeMultiHashMap<T1, T2> AllocateAndAutoDisposeNativeMultiHashMap<T1, T2>(int capacity, Allocator allocator)
            where T1 : struct, IEquatable<T1>
            where T2 : struct
        {
            var native = new NativeMultiHashMap<T1, T2>(capacity, allocator);
            EnqueueNative(new NativeMultiHashMapWrapper<T1, T2>(native), allocator);
            return native;
        }

        public NativeList<T> AllocateAndAutoDisposeNativeList<T>(int capacity, Allocator allocator) where T : struct
        {
            var native = new NativeList<T>(capacity, allocator);
            EnqueueNative(native, allocator);
            return native;
        }

        public NativeList<T> AllocateAndAutoDisposeNativeList<T>(Allocator allocator) where T : struct
        {
            var native = new NativeList<T>(allocator);
            EnqueueNative(native, allocator);
            return native;
        }

        

        public struct NativeQueueWrapper<T> : IDisposable where T : struct
        {
            private NativeQueue<T> queue;

            public NativeQueueWrapper(NativeQueue<T> queue)
            {
                this.queue = queue;
            }
            public void Dispose()
            {
                queue.Dispose();
            }
        }

        public struct NativeHashMapWrapper<T1, T2> : IDisposable where T1 : struct, IEquatable<T1> where T2 : struct
        {
            private NativeHashMap<T1, T2> hashMap;

            public NativeHashMapWrapper(NativeHashMap<T1, T2> hashMap)
            {
                this.hashMap = hashMap;
            }
            public void Dispose()
            {
                hashMap.Dispose();
            }
        }

        public struct NativeMultiHashMapWrapper<T1, T2> : IDisposable where T1 : struct, IEquatable<T1> where T2 : struct
        {
            private NativeMultiHashMap<T1, T2> hashMap;

            public NativeMultiHashMapWrapper(NativeMultiHashMap<T1, T2> hashMap)
            {
                this.hashMap = hashMap;
            }
            public void Dispose()
            {
                hashMap.Dispose();
            }
        }
    }
}
