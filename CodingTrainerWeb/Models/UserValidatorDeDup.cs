// Original - Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

// Modified to remove user name validation

// If user name and e-mail are the same, and special characters are allowed in user names,
// then the only possible validation errors in user names would be duplicates of the same
// errors in the e-mail address. Therefore, use this class to avoid the duplicates

using CodingTrainer.CodingTrainerModels.Models.Security;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CodingTrainer.CodingTrainerWeb.Models
{
    /// <summary>
    /// Provides validation services for user classes.
    /// </summary>
    /// <typeparam name="TUser">The type encapsulating a user.</typeparam>
    public class UserValidatorDeDup : UserValidator<ApplicationUser>
    {
        private UserManager<ApplicationUser, string> manager;

        /// <summary>
        /// Creates a new instance of UserValidatorDeDup<typeparamref name="TUser"/>
        /// </summary>
        /// <param name="manager"></param>
        public UserValidatorDeDup(UserManager<ApplicationUser, string> manager)
            :base(manager)
        {
            this.manager = manager;
        }

        /// <summary>
        /// Validates the specified <paramref name="user"/> as an asynchronous operation.
        /// </summary>
        /// <param name="user">The user to validate.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/> of the validation operation.</returns>
        public override async Task<IdentityResult> ValidateAsync(ApplicationUser user)
        {
            if (manager == null)
            {
                throw new ArgumentNullException(nameof(manager));
            }
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            var errors = new List<string>();
            await ValidateEmail(manager, user, errors);
            return errors.Count > 0 ? IdentityResult.Failed(errors.ToArray()) : IdentityResult.Success;
        }

        // make sure email is not empty, valid, and unique
        private async Task ValidateEmail(UserManager<ApplicationUser, string> manager, ApplicationUser user, List<string> errors)
        {
            var email = user.Email;
            if (string.IsNullOrWhiteSpace(email))
            {
                errors.Add($"Email {email} is invalid.");
                return;
            }
            if (!new EmailAddressAttribute().IsValid(email))
            {
                errors.Add($"Email {email} is invalid.");
                return;
            }
            var owner = await manager.FindByEmailAsync(email);
            if (owner != null &&
                !string.Equals(owner.Id, user.Id))
            {
                errors.Add($"Email {email} is already taken.");
            }
        }
    }
}

