using Contracts.Enums;
using Contracts.Logs;
using Contracts.ResponseModels;
using Logs.Core.Abstractions;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Logs.WebHost.LogConsumer
{
    public class DeleteSelectedLogsConsumer : IConsumer<DeleteSelectLogRequest>
    {
        private readonly IMongoService _service;

        public DeleteSelectedLogsConsumer(IMongoService service) =>  _service = service ?? throw new ArgumentNullException(nameof(service));
        
        public async Task Consume(ConsumeContext<DeleteSelectLogRequest> context)
        {
            var result = await _service.DeleteSelectedAsync(context.Message.LogIds);

            var response = result
                ? new NotificationViewModel()
                : new NotificationViewModel(new[] { TypeOfErrors.DeletionEntityError });
            await context.RespondAsync<NotificationViewModel>(response);
        }
    }
}
