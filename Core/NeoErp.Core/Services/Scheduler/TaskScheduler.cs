using NeoErp.Core.Infrastructure;
using NeoErp.Core.Services.CommonSetting;
using NeoErp.Core.Services.Scheduler;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NeoErp.Core.Services
{
    public class TaskScheduler
    {
        public static void Start()
        {
            IScheduler scheduler = EngineContext.Current.Resolve<IScheduler>();
            var GeneralSetting = EngineContext.Current.Resolve<ISettingService>().LoadSetting<GeneralSetting>();
            if (GeneralSetting.EnableSchedular == "true")
            {
                if (scheduler.IsShutdown)
                {
                    scheduler.Start();
                }
                scheduler.Start();
            }
            else
            {
                if (scheduler.IsStarted)
                {
                    scheduler.Shutdown();
                }
            }

            IJobDetail job = JobBuilder.Create<EmailJob>().Build();
            ITrigger trigger = TriggerBuilder.Create()
                .StartNow()
                .WithSchedule(SimpleScheduleBuilder.Create()
                .WithIntervalInMinutes(10)
                .RepeatForever()).Build();

            IJobDetail Schedularjob = JobBuilder.Create<SchedularEmail>().Build();
            ITrigger schedulartrigger = TriggerBuilder.Create()
                .StartNow()
                .WithSchedule(SimpleScheduleBuilder.Create()
                .WithIntervalInMinutes(10)
                .RepeatForever()).Build();

            IJobDetail mobileJob = JobBuilder.Create<MobileServerJob>().Build();
            ITrigger mobileTrigger = TriggerBuilder.Create()
                .StartNow()
                .WithSchedule(SimpleScheduleBuilder.Create()
                .WithIntervalInMinutes(1)
                .RepeatForever()).Build();

            scheduler.ScheduleJob(job, trigger);
            scheduler.ScheduleJob(Schedularjob, schedulartrigger);
            scheduler.ScheduleJob(mobileJob, mobileTrigger);

            //-----------------------------------Purchase RM Schedular task---------------------------

            IJobDetail mobileRMTask = JobBuilder.Create<MobileWebNotificationJob>().Build();
            ITrigger mobileRMTaskTrigger = TriggerBuilder.Create()
               .StartNow()
               .WithSchedule(SimpleScheduleBuilder.Create()
               .WithIntervalInMinutes(1)
               .RepeatForever()).Build();
            scheduler.ScheduleJob(mobileRMTask, mobileRMTaskTrigger);
        }
    }
}