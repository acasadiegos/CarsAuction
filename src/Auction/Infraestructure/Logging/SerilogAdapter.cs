using Application.Interfaces;
using Serilog;

namespace Infraestructure.Logging
{
    public class SerilogAdapter<T> : IAppLogger<T>
    {
        public void LogError(Exception ex, string messageTemplate, params object[] args)
        {
            Log.ForContext<T>().Error(ex, messageTemplate, args);
        }

        public void LogInformation(string messageTemplate, params object[] args)
        {
            Log.ForContext<T>().Information(messageTemplate, args);
        }

        public void LogWarning(string messageTemplate, params object[] args)
        {
            Log.ForContext<T>().Warning(messageTemplate, args);
        }
    }
}
