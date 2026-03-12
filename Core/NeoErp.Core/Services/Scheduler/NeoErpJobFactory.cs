using Quartz.Spi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Quartz;
using NeoErp.Core.Infrastructure;
using System.Reflection;
using NeoErp.Core.Infrastructure.DependencyManagement;

namespace NeoErp.Core.Services.Scheduler
{
    public class NeoErpJobFactory : IJobFactory
    {
        private ContainerManager _containerManager;

        public NeoErpJobFactory(ContainerManager containerManager)
        {
            this._containerManager = containerManager;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            return (IJob)this._containerManager.ResolveKeyed<IJob>(bundle.JobDetail.JobType.Name);
        }

        public void ReturnJob(IJob job)
        {
            throw new NotImplementedException();
        }
    }
}