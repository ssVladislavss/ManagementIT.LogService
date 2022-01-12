using Contracts.Constants;
using Contracts.Logs;
using Contracts.ResponseModels;
using GreenPipes;
using Logs.Core.Abstractions;
using MassTransit;
using MassTransit.ConsumeConfigurators;
using MassTransit.Definition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Logs.WebHost.LogConsumer
{
    public class DeleteRangeLogsConsumer : IConsumer<DeleteRangeLogRequest>
    {
        private readonly IMongoService _logService;

        public DeleteRangeLogsConsumer(IMongoService logService)
        {
            _logService = logService ?? throw new ArgumentNullException(nameof(logService));
        }

        public async Task Consume(ConsumeContext<DeleteRangeLogRequest> context)
        {
            await _logService.DeleteRangeAsync();
            await context.RespondAsync<NotificationViewModel>(new NotificationViewModel());
        }
    }

    public class DeleteRangeLogsConsumerDefinition : ConsumerDefinition<DeleteRangeLogsConsumer>
    {
        public DeleteRangeLogsConsumerDefinition()
        {
            EndpointName = ApiShowConstants.DeleteRangeLogs;
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<DeleteRangeLogsConsumer> consumerConfigurator)
        {
            endpointConfigurator.UseRetry(x => x.Intervals(100, 500, 1000));
        }
    }
}
