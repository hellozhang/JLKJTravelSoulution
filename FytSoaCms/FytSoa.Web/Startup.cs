using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Threading.Tasks;
using FytSoa.Common;
using FytSoa.Extensions;
using FytSoa.Service.Implements;
using FytSoa.Service.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.IdentityModel.Tokens;
using NLog.Extensions.Logging;
using NLog.Web;
using Swashbuckle.AspNetCore.Swagger;

namespace FytSoa.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            this.Configuration = builder.Build();
            BaseConfigModel.SetBaseConfig(Configuration, env.ContentRootPath, env.WebRootPath);
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            #region ע�����
            services.AddTransient<ICmsSiteService, CmsSiteService>();
            services.AddTransient<ICmsImgTypeService, CmsImgTypeService>();
            services.AddTransient<ICmsImageService, CmsImageService>();
            services.AddTransient<ICmsColumnService, CmsColumnService>();
            services.AddTransient<ICmsTemplateService, CmsTemplateService>();
            services.AddTransient<ICmsArticleService, CmsArticleService>();
            services.AddTransient<ICmsAdvClassService, CmsAdvClassService>();
            services.AddTransient<ICmsAdvListService, CmsAdvListService>();
            services.AddTransient<ICmsMessageService, CmsMessageService>();
            services.AddTransient<ICmsDownloadService, CmsDownloadService>();

            services.AddTransient<ISysAppSettingService, SysAppSettingService>();
            services.AddTransient<ISysAuthorizeService, SysAuthorizeService>();
            services.AddTransient<ISysBtnFunService, SysBtnFunService>();
            services.AddTransient<ISysPermissionsService, SysPermissionsService>();
            services.AddTransient<ISysLogService, SysLogService>();
            services.AddTransient<ISysAdminService, SysAdminService>();
            services.AddTransient<ISysCodeService, SysCodeService>();
            services.AddTransient<ISysCodeTypeService, SysCodeTypeService>();
            services.AddTransient<ISysOrganizeService, SysOrganizeService>();
            services.AddTransient<ISysMenuService, SysMenuService>();
            services.AddTransient<ISysRoleService, SysRoleService>();
            #endregion

            //�����ͼ����������ı�������
            services.AddSingleton(HtmlEncoder.Create(UnicodeRanges.All));

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            #region ��֤
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            //services.AddAuthentication(options =>
            //{
            //    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            //    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            //})
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, o =>
            {
                o.LoginPath = new PathString("/fytadmin/login");
            })
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, o =>
            {
                JwtAuthConfigModel jwtConfig = new JwtAuthConfigModel();
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = "FytSos",
                    ValidAudience = "wr",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtConfig.JWTSecretKey)),

                    /***********************************TokenValidationParameters�Ĳ���Ĭ��ֵ***********************************/
                    RequireSignedTokens = true,
                    // SaveSigninToken = false,
                    // ValidateActor = false,
                    // ������������������Ϊfalse�����Բ���֤Issuer��Audience�����ǲ�������������
                    ValidateAudience = false,
                    ValidateIssuer = true,
                    ValidateIssuerSigningKey = true,
                    // �Ƿ�Ҫ��Token��Claims�б������ Expires
                    RequireExpirationTime = true,
                    // ����ķ�����ʱ��ƫ����
                    // ClockSkew = TimeSpan.FromSeconds(300),
                    // �Ƿ���֤Token��Ч�ڣ�ʹ�õ�ǰʱ����Token��Claims�е�NotBefore��Expires�Ա�
                    ValidateLifetime = true
                };
            });
            #endregion

            #region ��Ȩ
            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireApp", policy => policy.RequireRole("App").Build());
                options.AddPolicy("RequireAdmin", policy => policy.RequireRole("Admin").Build());
                options.AddPolicy("RequireAdminOrApp", policy => policy.RequireRole("Admin,App").Build());
            });
            #endregion

            #region ���� ��ȡ�����Ƿ�ʹ�����ֻ���ģʽ
            services.AddMemoryCache();
            if (Convert.ToBoolean(Configuration["Cache:IsUseRedis"]))
            {
                services.AddSingleton<ICacheService, RedisCacheService>();
            }
            else
            {
                services.AddSingleton<ICacheService, MemoryCacheService>();
            }
            #endregion

            #region ���� RedisCache
            //��Redis�ֲ�ʽ���������ӵ�������
            services.AddDistributedRedisCache(options =>
            {
                //��������Redis������ 
                options.Configuration = "localhost";// Configuration.GetConnectionString("RedisConnectionString");
                //Redisʵ����RedisDistributedCache
                options.InstanceName = "RedisInstance";
            });
            #endregion 

            services.AddMvc().AddRazorPagesOptions(options =>
            {
                options.Conventions.AddPageRoute("/web/index", "/");
            });

            #region Swagger UI
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "FytSoa API",
                    Contact = new Contact { Name = "feiyit", Email = "715515390@qq.com", Url = "http://www.feiyit.com" }
                });
                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                var xmlPath = Path.Combine(basePath, "FytSoa.Web.xml");
                var entityXmlPath = Path.Combine(basePath, "FytSoa.Core.xml");
                options.IncludeXmlComments(xmlPath, true);
                options.IncludeXmlComments(entityXmlPath);
                //���header��֤��Ϣ
                //c.OperationFilter<SwaggerHeader>();

                var security = new Dictionary<string, IEnumerable<string>> { { "Bearer", new string[] { } }, };
                //���һ�������ȫ�ְ�ȫ��Ϣ����AddSecurityDefinition����ָ���ķ�������Ҫһ�£�������Bearer��
                options.AddSecurityRequirement(security);
                options.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    Description = "JWT��Ȩ(���ݽ�������ͷ�н��д���) �����ṹ: \"Authorization: Bearer {token}\"",
                    //jwtĬ�ϵĲ�������
                    Name = "Authorization",
                    //jwtĬ�ϴ��Authorization��Ϣ��λ��(����ͷ��)
                    In = "header",
                    Type = "apiKey"
                });

            });
            #endregion

            #region CORS
            services.AddCors(c =>
            {
                c.AddPolicy("Any", policy =>
                {
                    policy.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
                });

                c.AddPolicy("Limit", policy =>
                {
                    policy
                    .WithOrigins("localhost:4909")
                    .WithMethods("get", "post", "put", "delete")
                    //.WithHeaders("Authorization");
                    .AllowAnyHeader();
                });
            });
            #endregion

            #region ���� ѹ��
            services.AddResponseCompression();
            #endregion
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                //app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            #region ���Ubuntu Nginx �����ܻ�ȡIP����
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
            #endregion

            //���NLog  
            loggerFactory.AddNLog();
            //��ȡNlog�����ļ� 
            env.ConfigureNLog("nlog.config");
            //Swagger UI
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "FytSoa API V1");
            });
            //�Զ����쳣����
            app.UseMiddleware<ExceptionFilter>();
            //��֤
            app.UseAuthentication();

            //��Ȩ
            app.UseMiddleware<JwtAuthorizationFilter>();

            //����ѹ��
            app.UseResponseCompression();

            app.UseStaticFiles(new StaticFileOptions
            {
                ServeUnknownFileTypes = true
                //ContentTypeProvider = new FileExtensionContentTypeProvider(new Dictionary<string, string>
                //{
                //  { ".apk","application/vnd.android.package-archive"},
                //  { ".nupkg","application/zip"}
                //})  //֧�������ļ����ش���
            });
            app.UseCookiePolicy();
            app.UseCors("AllowAll");
            app.UseMvc();
        }
    }
}
