namespace Splat.ApplicationInsights
{
    public static class Helpers
    {
        public static void UseApplicationInsights()
        {
            var funcLogManager = new FuncLogManager(type => new ApplicationInsightsSplatLogger(type));

            Locator.CurrentMutable.RegisterConstant(funcLogManager, typeof(ILogManager));
        }
    }
}
