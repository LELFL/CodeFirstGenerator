using CodeFirstGenerator.Controllers;
using CodeFirstGenerator.Data;
using CodeFirstGenerator.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Yitter.IdGenerator;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.



// ���� IdGeneratorOptions ���󣬿��ڹ��캯�������� WorkerId��
var options = new IdGeneratorOptions(1);
// options.WorkerIdBitLength = 10; // Ĭ��ֵ6���޶� WorkerId ���ֵΪ2^6-1����Ĭ�����֧��64���ڵ㡣
// options.SeqBitLength = 6; // Ĭ��ֵ6������ÿ�������ɵ�ID�������������ٶȳ���5���/�룬����Ӵ� SeqBitLength �� 10��
// options.BaseTime = Your_Base_Time; // ���Ҫ������ϵͳ��ѩ���㷨���˴�Ӧ����Ϊ��ϵͳ��BaseTime��
// ...... ���������ο� IdGeneratorOptions ���塣

// �����������ص��ã�����������ò���Ч����
YitIdHelper.SetIdGenerator(options);

// ���Ϲ���ֻ��ȫ��һ�Σ���Ӧ������ID֮ǰ��ɡ�

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
