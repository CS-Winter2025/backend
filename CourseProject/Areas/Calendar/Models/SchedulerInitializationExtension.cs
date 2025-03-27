namespace CourseProject.Areas.Calendar.Models
{
    public static class SchedulerInitializerExtension
    {
        public static IHost InitializeCalendarDatabase(this IHost webHost)
        {
            var serviceScopeFactory =
            (IServiceScopeFactory?)webHost.Services.GetService(typeof(IServiceScopeFactory));

            using (var scope = serviceScopeFactory!.CreateScope())
            {
                var services = scope.ServiceProvider;
                var dbContext = services.GetRequiredService<DatabaseContext>();
                //dbContext.Database.EnsureDeleted();
                dbContext.Database.EnsureCreated();
                //SchedulerSeeder.Seed(dbContext);
            }

            return webHost;
        }
    }
}
