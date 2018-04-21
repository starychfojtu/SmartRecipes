﻿using System;

namespace SmartRecipes.Mobile.Models
{
    public class Account
    {
        public Account(Guid id, string email)
        {
            Id = id;
            Email = email;
        }

        public Guid Id { get; }

        public string Email { get; }
    }
}
