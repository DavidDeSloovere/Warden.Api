﻿using System;
using Warden.Api.Core.Domain.Exceptions;
using Warden.Api.Core.Extensions;

namespace Warden.Api.Core.Domain.Users
{
    public class ApiKey : Entity, ITimestampable
    {
        public string Key { get; protected set; }
        public Guid UserId { get; protected set; }
        public DateTime CreatedAt { get; protected set; }

        protected ApiKey()
        {
        }

        public ApiKey(string key, User user)
        {
            if (key.Empty())
                throw new DomainException("API key can not be empty.");
            if (user == null)
                throw new DomainException("Can not create an API key without user.");

            Key = key;
            UserId = user.Id;
            CreatedAt = DateTime.UtcNow;
        }
    }
}