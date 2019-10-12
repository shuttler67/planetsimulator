using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Jobs;

namespace Unity.Entities
{
    [AlwaysUpdateSystem]
    public class JobComponentSystem2 : ComponentSystem
    {
        protected override void OnDestroyManager()
        {
            base.OnDestroyManager();
            EntityManager.CompleteAllJobs();

            nativeManager.ProcessDisposables(new JobHandle());
            nativeManager.DisposePersistents();
            nativeManager.DisposeTempJobs();
        }

        private NativeManager nativeManager = new NativeManager();

        public INativeManager NativeManager 
        {
            get { return nativeManager; }
        }
        
        protected sealed override void OnUpdate()
        {
            var jobHandle = new JobHandle();

            try
            {
                foreach (var componentGroup in ComponentGroups)
                {
                    jobHandle = JobHandle.CombineDependencies(componentGroup.GetDependency(), jobHandle);
                }
                jobHandle = OnUpdate(jobHandle);

                foreach (var componentGroup in ComponentGroups)
                {
                    componentGroup.AddDependency(jobHandle);
                }
            }
            finally
            {
                nativeManager.DisposeTempJobs();
                nativeManager.ProcessDisposables(jobHandle);
            }
        }

        protected virtual JobHandle OnUpdate(JobHandle jobHandle) { return jobHandle; }
    }

}
