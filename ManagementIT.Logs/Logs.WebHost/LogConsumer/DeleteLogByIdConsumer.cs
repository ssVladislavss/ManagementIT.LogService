using AutoMapper;
using Contracts.Constants;
using Contracts.Enums;
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
    public class DeleteLogByIdConsumer : IConsumer<DeleteLogRequest>
    {
        private readonly IMongoService _logService;

        public DeleteLogByIdConsumer(IMongoService logService) => _logService = logService ?? throw new ArgumentNullException(nameof(logService));
        
        public async Task Consume(ConsumeContext<DeleteLogRequest> context)
        {
            var result = await _logService.DeleteAsync(context.Message.LogId);

            var response = result
                ? new NotificationViewModel()
                : new NotificationViewModel(new[] { TypeOfErrors.DeletionEntityError });
            await context.RespondAsync<NotificationViewModel>(response);
        }
    }

    public class DeleteLogByIdConsumerDefinition : ConsumerDefinition<DeleteLogByIdConsumer>
    {
        public DeleteLogByIdConsumerDefinition() => EndpointName = ApiShowConstants.DeleteLogById;
        

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<DeleteLogByIdConsumer> consumerConfigurator) =>
            endpointConfigurator.UseRetry(x => x.Intervals(100, 500, 1000));
    }
}
