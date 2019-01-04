﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Extras.DynamicProxy;
using AutoMapper;
using JLKJTravelSoulution.AOP;
using JLKJTravelSoulution.AuthHelper;
using JLKJTravelSoulution.Common;
using JLKJTravelSoulution.Filter;
using JLKJTravelSoulution.Log;
using log4net;
using log4net.Config;
using log4net.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Swagger;
using static JLKJTravelSoulution.SwaggerHelper.CustomApiVersion;

namespace JLKJTravelSoulution
{
    public class Startup
    {

        /// <summary>
        /// log4net 仓储库
        /// </summary>
        public static ILoggerRepository repository { get; set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            //log4net
            repository = LogManager.CreateRepository("JLKJTravelSoulution");
            //指定配置文件
            XmlConfigurator.Configure(repository, new FileInfo("log4net.config"));

        }

        public IConfiguration Configuration { get; }
        private const string ApiName = "JLKJTravelSoulution";

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            //注入全局异常捕获
            services.AddMvc(o =>
            {
                o.Filters.Add(typeof(GlobalExceptionsFilter));
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            //缓存注入
            services.AddScoped<ICaching, MemoryCaching>();
            services.AddSingleton<IMemoryCache>(factory =>
            {
                var cache = new MemoryCache(new MemoryCacheOptions());
                return cache;
            });
            //Redis注入
            services.AddScoped<IRedisCacheManager, RedisCacheManager>();
            //log日志注入
            services.AddSingleton<ILoggerHelper, LogHelper>();


            #region 初始化DB
            services.AddScoped<JLKJTravelSoulution.Model.Models.DBSeed>();
            services.AddScoped<JLKJTravelSoulution.Model.Models.MyContext>(); 
            #endregion


            #region Automapper
            services.AddAutoMapper(typeof(Startup));
            #endregion

            #region CORS
            //跨域第二种方法，声明策略，记得下边app中配置
            services.AddCors(c =>
            {
                //↓↓↓↓↓↓↓注意正式环境不要使用这种全开放的处理↓↓↓↓↓↓↓↓↓↓
                c.AddPolicy("AllRequests", policy =>
                {
                    policy
                    .AllowAnyOrigin()//允许任何源
                    .AllowAnyMethod()//允许任何方式
                    .AllowAnyHeader()//允许任何头
                    .AllowCredentials();//允许cookie
                });
                //↑↑↑↑↑↑↑注意正式环境不要使用这种全开放的处理↑↑↑↑↑↑↑↑↑↑


                //一般采用这种方法
                c.AddPolicy("LimitRequests", policy =>
                {
                    policy
                    .WithOrigins("http://127.0.0.1:1818", "http://localhost:8080", "http://localhost:8021", "http://localhost:8081", "http://localhost:1818")//支持多个域名端口，注意端口号后不要带/斜杆：比如localhost:8000/，是错的
                    .AllowAnyHeader()//Ensures that the policy allows any header.
                    .AllowAnyMethod();
                });
            });

            //跨域第一种办法，注意下边 Configure 中进行配置
            //services.AddCors();
            #endregion

            #region Swagger UI Service
            var basePath = Microsoft.DotNet.PlatformAbstractions.ApplicationEnvironment.ApplicationBasePath;
            services.AddSwaggerGen(c =>
            {
                //c.SwaggerDoc("v1", new Info
                //{
                //    Version = "v0.1.0",
                //    Title = "JLKJTravelSoulution API",
                //    Description = "框架说明文档",
                //    TermsOfService = "None",
                //    Contact = new Swashbuckle.AspNetCore.Swagger.Contact { Name = "JLKJTravelSoulution", Email = "JLKJTravelSoulution@xxx.com", Url = "https://www.jianshu.com/u/94102b59cc2a" }
                //});

                //遍历出全部的版本，做文档信息展示
                typeof(ApiVersions).GetEnumNames().ToList().ForEach(version =>
                {
                    c.SwaggerDoc(version, new Info
                    {
                        // {ApiName} 定义成全局变量，方便修改
                        Version = version,
                        Title = $"{ApiName} 接口文档",
                        Description = $"{ApiName} HTTP API " + version,
                        TermsOfService = "None",
                        Contact = new Contact { Name = "JLKJTravelSoulution", Email = "JLKJTravelSoulution@xxx.com", Url = "https://www.jianshu.com/u/94102b59cc2a" }
                    });
                });


                //就是这里


                var xmlPath = Path.Combine(basePath, "JLKJTravelSoulution.xml");//这个就是刚刚配置的xml文件名
                c.IncludeXmlComments(xmlPath, true);//默认的第二个参数是false，这个是controller的注释，记得修改

                var xmlModelPath = Path.Combine(basePath, "JLKJTravelSoulution.Model.xml");//这个就是Model层的xml文件名
                c.IncludeXmlComments(xmlModelPath);

                #region Token绑定到ConfigureServices

                //添加header验证信息
                //c.OperationFilter<SwaggerHeader>();

                // 发行人
                var IssuerName = (Configuration.GetSection("Audience"))["Issuer"];
                var security = new Dictionary<string, IEnumerable<string>> { { IssuerName, new string[] { } }, };
                c.AddSecurityRequirement(security);

                //方案名称“JLKJTravelSoulution”可自定义，上下一致即可
                c.AddSecurityDefinition(IssuerName, new ApiKeyScheme
                {
                    Description = "JWT授权(数据将在请求头中进行传输) 直接在下框中输入Bearer {token}（注意两者之间是一个空格）\"",
                    Name = "Authorization",//jwt默认的参数名称
                    In = "header",//jwt默认存放Authorization信息的位置(请求头中)
                    Type = "apiKey"
                });
                #endregion
            });

            #endregion


            #region JWT Token Service
            //读取配置文件
            var audienceConfig = Configuration.GetSection("Audience");
            var symmetricKeyAsBase64 = audienceConfig["Secret"];
            var keyByteArray = Encoding.ASCII.GetBytes(symmetricKeyAsBase64);
            var signingKey = new SymmetricSecurityKey(keyByteArray);

            // 令牌验证参数
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,
                ValidateIssuer = true,
                ValidIssuer = audienceConfig["Issuer"],//发行人
                ValidateAudience = true,
                ValidAudience = audienceConfig["Audience"],//订阅人
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                RequireExpirationTime = true,

            };
            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            // 注意使用RESTful风格的接口会更好，因为只需要写一个Url即可，比如：/api/values 代表了Get Post Put Delete等多个。
            // 如果想写死，可以直接在这里写。
            //var permission = new List<Permission> {
            //                  new Permission {  Url="/api/values", Role="Admin"},
            //                  new Permission {  Url="/api/values", Role="System"},
            //                  new Permission {  Url="/api/claims", Role="Admin"},
            //              };

            // 如果要数据库动态绑定，这里先留个空，后边处理器里动态赋值
            var permission = new List<Permission>();

            // 角色与接口的权限要求参数
            var permissionRequirement = new PermissionRequirement(
                "/api/denied",// 拒绝授权的跳转地址（目前无用）
                permission,
                ClaimTypes.Role,//基于角色的授权
                audienceConfig["Issuer"],//发行人
                audienceConfig["Audience"],//听众
                signingCredentials,//签名凭据
                expiration: TimeSpan.FromSeconds(60*2)//接口的过期时间
                );


            services.AddAuthorization(options =>
            {
                options.AddPolicy("Client", 
                    policy => policy.RequireRole("Client").Build());
                options.AddPolicy("Admin", 
                    policy => policy.RequireRole("Admin").Build());
                options.AddPolicy("SystemOrAdmin", 
                    policy => policy.RequireRole("Admin", "System"));

                // 自定义权限要求
                options.AddPolicy("Permission",
                         policy => policy.Requirements.Add(permissionRequirement));
            })

            .AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })

            .AddJwtBearer(o =>
            {
                o.TokenValidationParameters = tokenValidationParameters;
                //o.TokenValidationParameters = new TokenValidationParameters
                //{
                //    ValidateIssuer = true,//是否验证Issuer
                //    ValidateAudience = true,//是否验证Audience 
                //    ValidateIssuerSigningKey = true,//是否验证IssuerSigningKey 
                //    ValidIssuer = "JLKJTravelSoulution",
                //    ValidAudience = "wr",
                //    ValidateLifetime = true,//是否验证超时  当设置exp和nbf时有效 同时启用ClockSkew 
                //    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(JwtHelper.secretKey)),
                //    //注意这是缓冲过期时间，总的有效时间等于这个时间加上jwt的过期时间
                //    ClockSkew = TimeSpan.FromSeconds(30)

                //};
            });



            services.AddSingleton<IAuthorizationHandler, PermissionHandler>();
            services.AddSingleton(permissionRequirement);

            #endregion

            #region AutoFac
            //实例化 AutoFac  容器   
            var builder = new ContainerBuilder();
            //注册要通过反射创建的组件
            //builder.RegisterType<AdvertisementServices>().As<IAdvertisementServices>();
            builder.RegisterType<BlogCacheAOP>();//可以直接替换其他拦截器
            //builder.RegisterType<BlogLogAOP>();//这样可以注入第二个

            //var assemblysServices1 = Assembly.Load("JLKJTravelSoulution.Services");


            // ※※★※※ 如果你是第一次下载项目，请先F6编译，然后再F5执行，※※★※※
            // ※※★※※ 因为解耦，bin文件夹没有以下两个dll文件，会报错，所以先编译生成这两个dll ※※★※※


            //获取项目绝对路径，请注意，这个是实现类的dll文件，不是接口 IService.dll ，注入容器当然是Activatore
            var servicesDllFile = Path.Combine(basePath, "JLKJTravelSoulution.Services.dll");
            var assemblysServices = Assembly.LoadFile(servicesDllFile);//直接采用加载文件的方法

            //builder.RegisterAssemblyTypes(assemblysServices).AsImplementedInterfaces();//指定已扫描程序集中的类型注册为提供所有其实现的接口。

            builder.RegisterAssemblyTypes(assemblysServices)
                      .AsImplementedInterfaces()
                      .InstancePerLifetimeScope()
                      .EnableInterfaceInterceptors()//引用Autofac.Extras.DynamicProxy;
                      // 如果你想注入两个，就这么写  InterceptedBy(typeof(BlogCacheAOP), typeof(BlogLogAOP));
                      .InterceptedBy(typeof(BlogCacheAOP));//允许将拦截器服务的列表分配给注册。

            var repositoryDllFile = Path.Combine(basePath, "JLKJTravelSoulution.Repository.dll");
            var assemblysRepository = Assembly.LoadFile(repositoryDllFile);
            builder.RegisterAssemblyTypes(assemblysRepository).AsImplementedInterfaces();

            //将services填充到Autofac容器生成器中
            builder.Populate(services);

            //使用已进行的组件登记创建新容器
            var ApplicationContainer = builder.Build();

            #endregion

            return new AutofacServiceProvider(ApplicationContainer);//第三方IOC接管 core内置DI容器

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                // 在开发环境中，使用异常页面，这样可以暴露错误堆栈信息，所以不要放在生产环境。
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // 在非开发环境中，使用HTTP严格安全传输(or HSTS) 对于保护web安全是非常重要的。
                // 强制实施 HTTPS 在 ASP.NET Core，配合 app.UseHttpsRedirection
                //app.UseHsts();

            }

            #region Swagger
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                //之前是写死的
                //c.SwaggerEndpoint("/swagger/v1/swagger.json", "ApiHelp V1");
                //c.RoutePrefix = "";//路径配置，设置为空，表示直接在根域名（localhost:8001）访问该文件,注意localhost:8001/swagger是访问不到的，去launchSettings.json把launchUrl去掉

                //根据版本名称倒序 遍历展示
                typeof(ApiVersions).GetEnumNames().OrderByDescending(e => e).ToList().ForEach(version =>
                {
                    c.SwaggerEndpoint($"/swagger/{version}/swagger.json", $"{ApiName} {version}");
                });
            });
            #endregion

            #region Authen
            //app.UseMiddleware<JwtTokenAuth>();//注意此授权方法已经放弃，请使用下边的官方验证方法。但是如果你还想传User的全局变量，还是可以继续使用中间件
            app.UseAuthentication();
            #endregion

            #region CORS
            //跨域第二种方法，使用策略，详细策略信息在ConfigureService中
            app.UseCors("LimitRequests");//将 CORS 中间件添加到 web 应用程序管线中, 以允许跨域请求。


            //跨域第一种版本，请要ConfigureService中配置服务 services.AddCors();
            //    app.UseCors(options => options.WithOrigins("http://localhost:8021").AllowAnyHeader()
            //.AllowAnyMethod()); 
            #endregion

            // 跳转https
            app.UseHttpsRedirection();
            // 使用静态文件
            app.UseStaticFiles();
            // 使用cookie
            app.UseCookiePolicy();
            // 返回错误码
            app.UseStatusCodePages();//把错误码返回前台，比如是404

            app.UseMvc();
        }

    }
}
