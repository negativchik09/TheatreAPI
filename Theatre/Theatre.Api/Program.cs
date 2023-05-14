using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Theatre;
using Theatre.Application.Abstractions;
using Theatre.Application.Requests.Actors;
using Theatre.Application.Requests.Contracts;
using Theatre.Application.Responses.Actors;
using Theatre.Application.Responses.Contracts;
using Theatre.Application.Responses.Roles;
using Theatre.Application.Responses.Shows;
using Theatre.Application.Responses.Transactions;
using Theatre.Application.Services;
using Theatre.Domain;
using Theatre.Domain.Aggregates.Actors;
using Theatre.Domain.Aggregates.Shows;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

builder.Services.AddControllers();

builder.Services.AddDbContext<ApplicationDbContext>(
    opts => opts.UseSqlServer(
        configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<IdentityUser, IdentityRole>(o =>
    {
        o.Password.RequireDigit = true;
        o.Password.RequireLowercase = true;
        o.Password.RequireUppercase = true;
        o.Password.RequireNonAlphanumeric = false;
        o.Password.RequiredLength = 8;

        o.SignIn.RequireConfirmedAccount = false;
        o.SignIn.RequireConfirmedEmail = false;
        o.SignIn.RequireConfirmedPhoneNumber = false;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
{
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey
            (Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = false,
        ValidateIssuerSigningKey = true
    };
});

builder.Services.AddAutoMapper(config =>
{
    config.CreateMap<Actor, ActorFlat>()
        .ForMember(actorFlat => actorFlat.FirstName,
            expr => expr
                .MapFrom(actor => actor.Name.FirstName))
        .ForMember(actorFlat => actorFlat.LastName,
            expr => expr
                .MapFrom(actor => actor.Name.LastName))
        .ForMember(actorFlat => actorFlat.MiddleName,
            expr => expr
                .MapFrom(actor => actor.Name.MiddleName));

    config.CreateMap<ActorFlat, Actor>()
        .ForMember(actor => actor.Name,
            expr => expr
                .MapFrom(actorFlat => new FullName(
                    actorFlat.FirstName,
                    actorFlat.LastName,
                    actorFlat.MiddleName)));

    config.CreateMap<Actor, UpdatePersonalInfoRequest>()
        .ForMember(actorFlat => actorFlat.FirstName,
            expr => expr
                .MapFrom(actor => actor.Name.FirstName))
        .ForMember(actorFlat => actorFlat.LastName,
            expr => expr
                .MapFrom(actor => actor.Name.LastName))
        .ForMember(actorFlat => actorFlat.MiddleName,
            expr => expr
                .MapFrom(actor => actor.Name.MiddleName));
    
    config.CreateMap<Actor, CreateActorResponse>()
        .ForMember(response => response.FirstName,
            expr => expr
                .MapFrom(actor => actor.Name.FirstName))
        .ForMember(response => response.LastName,
            expr => expr
                .MapFrom(actor => actor.Name.LastName))
        .ForMember(response => response.MiddleName,
            expr => expr
                .MapFrom(actor => actor.Name.MiddleName));
    
    config.CreateMap<UpdatePersonalInfoRequest, Actor>()
        .ForMember(actor => actor.Name,
            expr => expr
                .MapFrom(updateActorRequest => new FullName(
                    updateActorRequest.FirstName,
                    updateActorRequest.LastName,
                    updateActorRequest.MiddleName)));

    config.CreateMap<Contract, ContractFullInfo>()
        .ForMember(fullInfo => fullInfo.AlreadyPayed,
            expr => expr
                .MapFrom(contract => contract.Transactions.Sum(x => x.Sum.Amount)))
        .ForMember(fullInfo => fullInfo.YearCost,
            expr => expr
                .MapFrom(contract => contract.YearCost.Amount));

    config.CreateMap<Contract, ContractFlat>()
        .ForMember(fullInfo => fullInfo.Sum,
            expr => expr
                .MapFrom(contract => contract.YearCost.Amount));

    config.CreateMap<Transaction, TransactionFlat>()
        .ForMember(flat => flat.Sum,
            expr => expr
                .MapFrom(transaction => transaction.Sum.Amount));

    config.CreateMap<Role, RoleFlat>();

    config.CreateMap<Show, ShowTableView>()
        .ForMember(flat => flat.TotalBudget, 
            expr => expr
            .MapFrom(show => show.TotalBudget.Amount))
        .ForMember(flat => flat.AlreadySpent,
            expr => expr
                .MapFrom(show => show.Contracts.Sum(x => x.YearCost.Amount)))
        .ForMember(flat => flat.RoleCount,
            expr => expr
                .MapFrom(show => show.Roles.Count))
        .ForMember(flat => flat.ActorsCount,
            expr => expr
                .MapFrom(show => show.Contracts.DistinctBy(x => x.ActorId).Count()));

    config.CreateMap<Show, ShowFlat>()
        .ForMember(flat => flat.TotalBudget,
            expr => expr
                .MapFrom(show => show.TotalBudget.Amount));
});

builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IActorsService, ActorsService>();
builder.Services.AddScoped<IContractService, ContractService>();
builder.Services.AddScoped<IShowService, ShowService>();

builder.Services.AddAuthorization();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo()
    {
        Title = "Theatre API",
        Version = "v1",
    });
    c.ResolveConflictingActions(apiDescriptor => apiDescriptor.First());
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme."
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme()
            {
                Reference = new OpenApiReference()
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new List<string>()
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseCors(x => x
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

Seeder.Initialize(app.Services);

app.Run();