using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elsa.Activities.Http.Extensions;
using Elsa.Activities.Timers.Extensions;
using Elsa.Dashboard.Extensions;
using Elsa.Persistence.EntityFrameworkCore.DbContexts;
using Elsa.Persistence.EntityFrameworkCore.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MyCastle
{
	public class Startup
	{
		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddSingleton(Console.Out);

			services.AddElsa(elsa => elsa
						.AddEntityFrameworkStores<SqliteContext>(options => options
							.UseSqlite("Data Source=mycastle.db;Cache=Shared")));

			services.AddActivity<OpenValve>();
			services.AddActivity<LogMessage>();
			
			services
					.AddHttpActivities()
					.AddTimerActivities();

			services.AddElsaDashboard();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseStaticFiles();
			app.UseDefaultFiles();

			app.UseHttpActivities();

			app.UseRouting();
			app.UseEndpoints(ep => ep.MapControllers());


			//app.UseEndpoints(endpoints =>
			//{
			//    endpoints.MapGet("/test", async context =>
			//    {
			//        await context.Response.WriteAsync("Hello World!");
			//    });
			//});
		}
	}
}
