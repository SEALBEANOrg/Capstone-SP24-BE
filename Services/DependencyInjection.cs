using Microsoft.Extensions.DependencyInjection;
using Repositories.Implements;
using Repositories.Interfaces;
using Repositories.Models;
using Repositories;
using Services.Interfaces;
using Services.Mapper;
using Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;

namespace Services
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.FromFile("firebase.json")
            });

            return services;
        }
    }
}
