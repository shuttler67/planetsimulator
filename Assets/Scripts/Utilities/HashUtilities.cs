using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

public struct GridHash
{
    public readonly static int3[] cellOffsets =
    {
        new int3(0, 0, 0),
        new int3(-1, 0, 0),
        new int3(0, -1, 0),
        new int3(0, 0, -1),
        new int3(1, 0, 0),
        new int3(0, 1, 0),
        new int3(0, 0, 1)
    };

    public readonly static int2[] cell2DOffsets =
    {
        new int2(0, 0),
        new int2(-1, 0),
        new int2(0, -1),
        new int2(1, 0),
        new int2(0, 1),
        new int2(-1, 1),
        new int2(1, -1),
        new int2(1, 1),
        new int2(-1, -1),
    };

    public static int Hash(float3 v, float cellSize)
    {
        return Hash(Quantize(v, cellSize));
    }

    public static int3 Quantize(float3 v, float cellSize)
    {
        return new int3(math.floor(v / cellSize));
    }

    public static int Hash(float2 v, float cellSize)
    {
        return Hash(Quantize(v, cellSize));
    }

    public static int2 Quantize(float2 v, float cellSize)
    {
        return new int2(math.floor(v / cellSize));
    }

    public static int Hash(int3 grid)
    {
        unchecked
        {
            // Simple int3 hash based on a pseudo mix of :
            // 1) https://en.wikipedia.org/wiki/Fowler%E2%80%93Noll%E2%80%93Vo_hash_function
            // 2) https://en.wikipedia.org/wiki/Jenkins_hash_function
            int hash = grid.x;
            hash = (hash * 397) ^ grid.y;
            hash = (hash * 397) ^ grid.z;
            hash += hash << 3;
            hash ^= hash >> 11;
            hash += hash << 15;
            return hash;
        }
    }

    public static int Hash(int2 grid)
    {
        unchecked
        {
            // Simple int2 hash based on a pseudo mix of :
            // 1) https://en.wikipedia.org/wiki/Fowler%E2%80%93Noll%E2%80%93Vo_hash_function
            // 2) https://en.wikipedia.org/wiki/Jenkins_hash_function
            int hash = grid.x;
            hash = (hash * 397) ^ grid.y;
            hash += hash << 3;
            hash ^= hash >> 11;
            hash += hash << 15;
            return hash;
        }
    }

    public static ulong Hash(ulong hash, ulong key)
    {
        const ulong m = 0xc6a4a7935bd1e995UL;
        const int r = 47;

        ulong h = hash;
        ulong k = key;

        k *= m;
        k ^= k >> r;
        k *= m;

        h ^= k;
        h *= m;

        h ^= h >> r;
        h *= m;
        h ^= h >> r;

        return h;
    }
}

public struct HashJobs
{
    [BurstCompile]
    struct HashJob<TKey, TValue> : IJobParallelFor where TKey : struct, System.IEquatable<TKey> where TValue : struct
    {
        [ReadOnly]
        public NativeArray<TKey> keys;
        public NativeArray<TValue> values;
        public NativeHashMap<TKey, TValue>.Concurrent hashMap;

        public void Execute(int i)
        {
            hashMap.TryAdd(keys[i], values[i]);
        }
    }
    [BurstCompile]
    public struct ClearMultiHashMapJob<TKey, TValue> : IJob where TKey : struct, System.IEquatable<TKey> where TValue : struct
    {
        public NativeMultiHashMap<TKey, TValue> hashMap;

        public void Execute()
        {
            hashMap.Clear();
        }
    }
    [BurstCompile]
    public struct ClearHashMapJob<TKey, TValue> : IJob where TKey : struct, System.IEquatable<TKey> where TValue : struct
    {
        public NativeHashMap<TKey, TValue> hashMap;

        public void Execute()
        {
            hashMap.Clear();
        }
    }
}

//public interface IJobParallelMultiHashMapIteration<TKey, TValue> : IJobParallelFor where TKey : struct, System.IEquatable<TKey> where TValue : struct
//{
//    TKey GetHashCode(int index);

//    void Execute(int index, TValue hashmapValue);
//}

//public static class JobParallelMultiHashMapIterationExtensions<TKey, TValue> where TKey : struct, System.IEquatable<TKey> where TValue : struct
//{

//    public static void Schedule<T>(this T jobData, int length, NativeMultiHashMap<TKey, TValue> hashmap, JobHandle inputDeps) where T : IJobParallelMultiHashMapIteration<TKey, TValue>
//    {
//        TValue value;
//        NativeMultiHashMapIterator<TKey> it;


//        bool success = hashmap.TryGetFirstValue(, out value, out it);

//        while (success)
//        {
//            success = hashmap.TryGetNextValue(out value, ref it);
//        }
//    }
//}
