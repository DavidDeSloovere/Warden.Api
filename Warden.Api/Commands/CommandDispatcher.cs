﻿using System.Threading.Tasks;
using RawRabbit;
using Warden.Api.Domain.Exceptions;
using Warden.Messages.Commands;

namespace Warden.Api.Commands
{
    public class CommandDispatcher : ICommandDispatcher
    {
        private readonly IBusClient _bus;

        public CommandDispatcher(IBusClient bus)
        {
            _bus = bus;
        }

        public async Task DispatchAsync<T>(T command) where T : ICommand
        {
            if (command == null)
                throw new ServiceException("Command can not be null.");

            await _bus.PublishAsync(command);
        }
    }
}