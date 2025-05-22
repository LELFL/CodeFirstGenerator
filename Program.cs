using CodeFirstGenerator.Controllers;
using CodeFirstGenerator.Data;
using CodeFirstGenerator.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Yitter.IdGenerator;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.



// 创建 IdGeneratorOptions 对象，可在构造函数中输入 WorkerId：
var options = new IdGeneratorOptions(1);
// options.WorkerIdBitLength = 10; // 默认值6，限定 WorkerId 最大值为2^6-1，即默认最多支持64个节点。
// options.SeqBitLength = 6; // 默认值6，限制每毫秒生成的ID个数。若生成速度超过5万个/秒，建议加大 SeqBitLength 到 10。
// options.BaseTime = Your_Base_Time; // 如果要兼容老系统的雪花算法，此处应设置为老系统的BaseTime。
// ...... 其它参数参考 IdGeneratorOptions 定义。

// 保存参数（务必调用，否则参数设置不生效）：
YitIdHelper.SetIdGenerator(options);

// 以上过程只需全局一次，且应在生成ID之前完成。

builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
    });
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlite(builder.Configuration["ConnectionStrings:Default"]);
});

var corsPolicy = "_myAllowSpecificOrigins";

builder.Services.AddCors(optons =>
{
    optons.AddPolicy(name: corsPolicy,
                      policy =>
                      {
                          policy.WithOrigins("http://localhost", "http://localhost:8000").AllowAnyHeader().AllowAnyMethod().AllowCredentials();
                      });
});

builder.Services.AddScoped<IGeneratorService, GeneratorService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseFileServer();

app.UseCors(corsPolicy);

app.UseAuthorization();

app.MapControllers();

app.Run();
