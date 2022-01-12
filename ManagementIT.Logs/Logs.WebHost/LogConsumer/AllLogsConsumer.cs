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
    public class AllLogsConsumer : IConsumer<LogMessage>
    {
        private readonly IMongoService _logService;
        private readonly IMapper _mapper;

        public AllLogsConsumer(IMongoService logService, IMapper mapper)
        {
            _logService = logService ?? throw new ArgumentNullException(nameof(logService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task Consume(ConsumeContext<LogMessage> context)
        {
            var result = await _logService.GetLogsAsync(null, null);
            var response = new AllLogMessageResponse();
            if (result == null) response.Notification = new NotificationViewModel(new[] { TypeOfErrors.InternalServerError });
            else if (!result.Any()) response.Notification = new NotificationViewModel(new[] { TypeOfErrors.NoContent });
            else
            {
                response.Notification = new NotificationViewModel();
                response.Model = _mapper.Map<IEnumerable<LogMessage>>(result);
            }

            await context.RespondAsync<AllLogMessageResponse>(response);
        }
    }

    public class AllLogsConsumerDefinition : ConsumerDefinition<AllLogsConsumer>
    {
        public AllLogsConsumerDefinition()
        {
            EndpointName = ApiShowConstants.GetAllLog;
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<AllLogsConsumer> consumerConfigurator)
        {
            endpointConfigurator.UseRetry(x => x.Intervals(100, 500, 1000));
        }
    }
}
