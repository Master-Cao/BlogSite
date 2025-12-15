using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Serilog;
using YjSite.DbContext;
using Microsoft.OpenApi.Models;
using SqlSugar;
using YjSite.Config;
using YjSite.OssHelper;
using YjSite.Filters;
using YjSite.Services.BlogsService;
using YjSite.Services.BlogTagsService;
using YjSite.Services.PlansService;
using YjSite.Services.Auth;
using YjSite.Services.Files;
using YjSite.Services.DefaultImageService;
using YjSite.Services.UserService;
using YjSite.Services.LifeShareService;
using YjSite.Services.OssService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options =>
{
    options.Filters.Add<GlobalExceptionFilter>();
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("cors", p =>
    {
        p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});
// 注册内存缓存
builder.Services.AddMemoryCache();

// 注册应用服务
builder.Services.AddScoped<IBlogService, BlogService>();
builder.Services.AddScoped<IBlogTagsService, BlogTagsService>();
builder.Services.AddScoped<IPlanService, PlanService>();
builder.Services.AddScoped<IDefaultImageService, DefaultImageService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ILifeShareService, LifeShareService>();
builder.Services.AddScoped<IOssService, OssService>();
// TODO: 注册其他服务
// builder.Services.AddScoped<IAuthService, AuthService>();
// builder.Services.AddScoped<IFileService, FileService>();

builder.Services.Configure<OssConfiguration>(builder.Configuration.GetSection("OssConfiguration"));
builder.Services.AddSqlsugarSetup(builder.Configuration, "DefaultConnection");

#region 初始化日志
Log.Logger = new LoggerConfiguration()
       .MinimumLevel.Error()
       .WriteTo.File(Path.Combine("Logs", @"Log.txt"), rollingInterval: RollingInterval.Day)
       .CreateLogger();
#endregion

#region 身份验证
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidAudience = "yjcabin.com",
        ValidIssuer = "yjcabin.com",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSecretKey"]!))
    };
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            context.Request.Cookies.TryGetValue("x-access-token", out var accessToken);
            if (!string.IsNullOrEmpty(accessToken))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
});
#endregion

builder.Services.AddScoped<OssHelper>();
builder.Services.AddSwaggerGen(s =>
{
    //添加安全定义
    s.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "请输入token,格式为 Bearer xxxxxxxx，注意中间必需有空格",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    //添加安全要求
    s.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme{
                Reference =new OpenApiReference{
                    Type = ReferenceType.SecurityScheme,
                    Id ="Bearer"
                }
            },new string[]{ }
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
app.UseCors("cors");
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
