using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WDT2020_a2.Data;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using WDT2020_a2.Exceptions;

namespace WDT2020_a2.Services
{
    public class ScheduleHostService : IHostedService, IDisposable
    {

        private readonly ILogger _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly NwabContext _context;

        private readonly BankEngine _engine;

        private Timer _timer;



        public ScheduleHostService(ILogger<ScheduleHostService> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;

            var scope = _scopeFactory.CreateScope();

            _context = scope.ServiceProvider.GetRequiredService<NwabContext>();
            _engine = new BankEngine(_context);

        }


        public void Dispose()
        {
            _timer?.Dispose();
        }

        private void DoWork(object state)
        {
            try
            {
                var hasBillPay = _context.BillPays.Any();

                if (hasBillPay)
                {
                    _engine.ProcessBills();
                }
            }
            catch(CustomTransactionException e)
            {
                _logger.LogError(e.Message);
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Scheduler has started.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));

            //DoWork(this);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Hosted Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }
    }
}
