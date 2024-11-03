using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Task_Flow.Business.Abstract;
using Task_Flow.Business.Concrete;
using Task_Flow.DataAccess.Abstract;
using Task_Flow.DataAccess.Concrete;
using Task_Flow.Entities.Data;
using Task_Flow.Entities.Models;
using Task_Flow.WebAPI.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddCors(p => p.AddPolicy("corsapp", builder =>
{
    builder.WithOrigins("http://localhost:3001/").AllowAnyMethod().AllowAnyHeader().AllowCredentials();
}));
 
builder.Services.AddSignalR();
// Database connection
builder.Services.AddDbContext<TaskFlowDbContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Configuration.AddJsonFile("SMTP.json", optional: true, reloadOnChange: true);

builder.Services.AddTransient<MailService>();

builder.Services.AddScoped<IUserDal, UserDal>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICommentDal, CommentDal>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<IQuizDal, QuizDal>();
builder.Services.AddScoped<IQuizService, QuizService>();
builder.Services.AddScoped<IProjectDal, ProjectDal>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<ITaskDal, TaskDal>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<ITaskCustomizeDal, TaskCustomizeDal>();
builder.Services.AddScoped<ITaskCustomizeService, TaskCustomizeService>();
builder.Services.AddScoped<ITeamMemberDal, TeamMemberDal>();
builder.Services.AddScoped<ITeamMemberService, TeamMemberService>();
builder.Services.AddScoped<IMessageDal, MessageDal>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IFriendDal, FriendDal>();
builder.Services.AddScoped<IFriendService, FriendService>();
builder.Services.AddScoped<ITaskAssignDal, TaskAssignDal>();
builder.Services.AddScoped<ITaskAssignService, TaskAssigneService>();
builder.Services.AddScoped<INotificationDal, NotificationDal>();
builder.Services.AddScoped<INotificationService, NotificationService>();

// Identity configuration (only user management, no roles)
builder.Services.AddIdentity<CustomUser, IdentityRole>()
    .AddEntityFrameworkStores<TaskFlowDbContext>()
    .AddDefaultTokenProviders();

// JWT Authentication configuration
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

// App configuration
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(x =>
{
    x.AllowAnyMethod()
    .AllowAnyHeader()
    .SetIsOriginAllowed(origin => true)
    .AllowCredentials();
});



app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<ConnectionHub>("/connect");
app.Run();
