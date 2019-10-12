using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using Unity.Collections;
using NUnit.Framework.Constraints;
using System;
using Unity.Entities;

[TestFixture]
public class NativeManagerFixture {

    NativeArray<int> array1;
    NativeArray<int> array2;
    NativeManager n;

    JobComponentSystem2 jobComponentSystem2;

    [SetUp]
    public void Setup()
    {
        n = new NativeManager();
        array1 = new NativeArray<int>();
        array2 = new NativeArray<int>();
        jobComponentSystem2 = new JobComponentSystem2();
    }
    [Test]
    public void TestTempAllocationAndDisposal()
    {
        n.AllocateAndAutoDispose(ref array1, 10, Allocator.Temp);
        n.AllocateAndAutoDispose(ref array2, 10, Allocator.Temp);

        Assert.That(array1.IsCreated);
        Assert.AreEqual(array1.Length, 10);
        n.ProcessDisposables(new Unity.Jobs.JobHandle());
        Assert.Throws<InvalidOperationException>(array1.Dispose);
        Assert.Throws<InvalidOperationException>(array2.Dispose);
    }
    [Test]
    public void TestTempJobAllocationAndDisposal()
    {
        n.AllocateAndAutoDispose(ref array1, 10, Allocator.TempJob);
        n.AllocateAndAutoDispose(ref array2, 10, Allocator.TempJob);

        n.ProcessDisposables(new Unity.Jobs.JobHandle());
        n.DisposeTempJobs();
        Assert.Throws<InvalidOperationException>( array1.Dispose);
        Assert.Throws<InvalidOperationException>( array2.Dispose);
    }
    [Test]
    public void TestPersistentAllocationAndDisposal()
    {
        n.AllocateAndAutoDispose(ref array1, 10, Allocator.Persistent);
        n.AllocateAndAutoDispose(ref array2, 10, Allocator.Persistent);

        n.ProcessDisposables(new Unity.Jobs.JobHandle());
        n.DisposePersistents();
        Assert.Throws<InvalidOperationException>(array1.Dispose);
        Assert.Throws<InvalidOperationException>(array2.Dispose);
    }



    [Test]
    public void TestJobComponentSystem2Disposes()
    {
        n.AllocateAndAutoDispose(ref array1, 10, Allocator.Persistent);
        n.AllocateAndAutoDispose(ref array2, 10, Allocator.Persistent);

        n.ProcessDisposables(new Unity.Jobs.JobHandle());
        n.DisposePersistents();
        Assert.Throws<InvalidOperationException>(array1.Dispose);
        Assert.Throws<InvalidOperationException>(array2.Dispose);
    }

    // A UnityTest behaves like a coroutine in PlayMode
    // and allows you to yield null to skip a frame in EditMode
    [UnityTest]
    public IEnumerator NativeManagerFixtureWithEnumeratorPasses() {
        // Use the Assert class to test conditions.
        // yield to skip a frame
        yield return null;
    }
}
