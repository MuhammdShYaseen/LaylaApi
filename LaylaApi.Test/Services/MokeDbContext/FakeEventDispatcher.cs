using LaylaApi.DomainEvents.Domain.Dispatcher;
using LaylaApi.DomainEvents.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaylaApi.Test.Services.MokeDbContext
{
    public class FakeEventDispatcher : IEventDispatcher
    {
        public Task EnqueueAsync(IEvent @event, CancellationToken ct = default)
        {
            return Task.CompletedTask;
        }
    } 
}
