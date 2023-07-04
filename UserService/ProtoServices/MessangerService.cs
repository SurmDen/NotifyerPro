using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Logging;
using UserService.Models;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.IO;
using Microsoft.Extensions.Caching.Memory;

namespace UserService.ProtoServices
{
    public class MessangerService : Messanger.MessangerBase
    {
        private ILogger<MessangerService> logger;
        private DataContext Context;
        private IMemoryCache Cache;

        public MessangerService(ILogger<MessangerService> logger, DataContext Context, IMemoryCache cache)
        {
            Cache = cache;
            this.logger = logger;
            this.Context = Context;
        }

        public override async Task MessageStream(IAsyncStreamReader<MessageContext> requestStream, IServerStreamWriter<MessageContext> responseStream, ServerCallContext context)
        {
            long id = (long)Cache.Get("id");

            var innerMessagesSaver = Task.Run(async () =>
            {

                await foreach (MessageContext messages in requestStream.ReadAllAsync())
                {
                    Context.Messages.Add(new Message()
                    {
                        UsersId = messages.UsersId,
                        Context = messages.Context,
                        DialogId = messages.DialogId
                    });

                    await Task.Delay(1000);

                    await Context.SaveChangesAsync();
                }
            });

            foreach (Message message in Context.Dialogs.Include(d=>d.Messages).First(d=>d.Id == id).Messages)
            {
                message.Dialog = null;

                await responseStream.WriteAsync(new MessageContext()
                {
                    UsersId = message.UsersId,
                    Context = message.Context,
                    DialogId = message.DialogId
                });
            }


            await innerMessagesSaver;

        }
    }
}
