﻿using System.Threading.Tasks;
using AutoMapper;
using Warden.Api.Core.Domain.Exceptions;
using Warden.Api.Core.Repositories;
using Warden.Api.Core.Types;
using Warden.Api.Infrastructure.DTO.Users;

namespace Warden.Api.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<UserDto> GetAsync(string externalUserId)
        {
            var user = await _userRepository.GetAsync(externalUserId);
            if (user.HasNoValue)
                throw new ServiceException($"Desired user does not exist, externalId: {externalUserId}");

            var result = _mapper.Map<UserDto>(user.Value);

            return result;
        }
    }
}