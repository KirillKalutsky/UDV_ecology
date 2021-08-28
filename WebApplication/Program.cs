using Crawler.Downloader;
using DataSources;
using DBLayer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            CreateDbIfNotExists(host);

            IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            await scheduler.Start();

            IJobDetail job = JobBuilder.Create<Downloader<IDownloadable>>().Build();

            ITrigger trigger = TriggerBuilder.Create()  // ������� �������
                .WithIdentity("trigger1", "group1")     // �������������� ������� � ������ � �������
                .StartNow()                            // ������ ����� ����� ������ ����������
                .WithSimpleSchedule(x => x            // ����������� ���������� ��������
                    .WithIntervalInMinutes(5)          // ����� 1 ������
                    .RepeatForever())                   // ����������� ����������
                .Build();                               // ������� �������

            await scheduler.ScheduleJob(job, trigger);        // �������� ���������� ������

            host.Run();
        }

        private static void CreateDbIfNotExists(IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<DBLayerContext>();
                    DBInitializer.Init(context);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred creating the DB.");
                }
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
