using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unity.Collections
{
    public interface INativeManager
    {
        void AllocateAndAutoDispose<T>(ref NativeArray<T> native, int length, Allocator allocator, NativeArrayOptions options = NativeArrayOptions.ClearMemory) where T : struct;
        void AllocateAndAutoDispose<T>(ref NativeQueue<T> native, Allocator allocator) where T : struct;
        void AllocateAndAutoDispose<T1, T2>(ref NativeHashMap<T1, T2> native, int capacity, Allocator allocator) where T1 : struct, IEquatable<T1> where T2 : struct;
        void AllocateAndAutoDispose<T1, T2>(ref NativeMultiHashMap<T1, T2> native, int capacity, Allocator allocator) where T1 : struct, IEquatable<T1> where T2 : struct;
        void AllocateAndAutoDispose<T>(ref NativeList<T> native, int capacity, Allocator allocator) where T : struct;
        void AllocateAndAutoDispose<T>(ref NativeList<T> native, Allocator allocator) where T : struct;

        NativeArray<T> AllocateAndAutoDisposeNativeArray<T>(int length, Allocator allocator, NativeArrayOptions options = NativeArrayOptions.ClearMemory) where T : struct;
        NativeQueue<T> AllocateAndAutoDisposeNativeQueue<T>(Allocator allocator) where T : struct;
        NativeHashMap<T1, T2> AllocateAndAutoDisposeNativeHashMap<T1, T2>(int capacity, Allocator allocator) where T1 : struct, IEquatable<T1> where T2 : struct;
        NativeMultiHashMap<T1, T2> AllocateAndAutoDisposeNativeMultiHashMap<T1, T2>(int capacity, Allocator allocator) where T1 : struct, IEquatable<T1> where T2 : struct;
        NativeList<T> AllocateAndAutoDisposeNativeList<T>(int capacity, Allocator allocator) where T : struct;
        NativeList<T> AllocateAndAutoDisposeNativeList<T>(Allocator allocator) where T : struct;
    }
}
