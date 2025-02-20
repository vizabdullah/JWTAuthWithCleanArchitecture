﻿using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Contracts
{
    public interface IUser
    {
        Task<RegistrationResponse> RegisterUserAsync(RegisterUserDTO registerUser);
        Task<LoginResponse> LoginUserAsync(LoginDTO login);

    }
}
