
using System;
using System.Reflection;
using Unity.Entities;

namespace Unity.Collections
{
    public enum Lifetime
    {
        SystemLifetime, EveryFrame
    }
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class InjectNativeAttribute : Attribute
    {
        public Lifetime Lifetime { get; private set; }

        public ComponentGroup GroupToMatchSize { get; private set; }

        public InjectNativeAttribute(Lifetime lifetime, ComponentGroup groupToMatchSize = null)
        {
            Lifetime = lifetime;
            GroupToMatchSize = groupToMatchSize;
        }
    }
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class DeallocateOnDestroyManagerAttribute : Attribute
    {
    }

    public static class NativeArrayExtensions
    {
        public static NativeArray<T> EnsureSize<T>(this NativeArray<T> array, int size) where T : struct
        {
            if (!array.IsCreated || array.Length < size)
            {
                if (array.IsCreated)
                    array.Dispose();
                return new NativeArray<T>(size, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            }
            else
            {
                return array;
            }
        }
    }

    public interface IAllocatable
    {
        void Allocate(INativeManager allocator);
    }
    

    public interface IDisposableFields : IDisposable { }

    public static class IDisposableFieldsExtensions
    {
        public static void DisposeFields(this IDisposableFields obj)
        {
            FieldInfo[] fieldInfo = obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public); ;

            for (int i = 0; i < fieldInfo.Length; i++)
            {
                var value = fieldInfo[i].GetValue(obj);
                var isCreated = value.GetType().GetMethod("IsCreated");
                var dispose = value.GetType().GetMethod("Dispose");

                //if (value is IDisposable)
                //{
                //    ((IDisposable)value).Dispose();
                //}
                if (isCreated != null && dispose != null && (bool)isCreated.Invoke(obj, new object[0]))
                {
                    dispose.Invoke(obj, new object[0]);
                }
            }
        }
    }
}
namespace Unity.Mathematics
{
    public struct bytebool
    {
        private readonly byte _value;
        public bytebool(bool value) { _value = (byte)(value ? 1 : 0); }
        public static implicit operator bytebool(bool value) { return new bytebool(value); }
        public static implicit operator bool(bytebool value) { return value._value != 0; }
    }
}

