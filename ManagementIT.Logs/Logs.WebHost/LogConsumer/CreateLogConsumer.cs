using Contracts.Constants;
using Contracts.Logs;
using GreenPipes;
using Logs.Core.Abstractions;
using Logs.Core.Domain;
using MassTransit;
using MassTransit.ConsumeConfigurators;
using MassTransit.Definition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Logs.WebHost.LogConsumer
{
    public class CreateLogConsumer : IConsumer<CreateLog>
    {
        private readonly IMongoService _logService;

        public CreateLogConsumer(IMongoService logService)
        {
            _logService = logService ?? throw new ArgumentNullException(nameof(logService));
        }

        public async Task Consume(ConsumeContext<CreateLog> context)
        {
            var message = LogMessageEntity.GetLogMessage(context.Message.Address, context.Message.Message, context.Message.Iniciator, context.Message.Type);
            await _logService.Create(message);
            await Task.CompletedTask;
        }
    }

    public class CreateLogConsumerDefinition : ConsumerDefinition<CreateLogConsumer>
    {
        public CreateLogConsumerDefinition()
        {
            EndpointName = ApiShowConstants.CreateLog;
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<CreateLogConsumer> consumerConfigurator)
        {
            endpointConfigurator.UseRetry(x => x.Intervals(100, 500, 1000));
        }

    }
}
