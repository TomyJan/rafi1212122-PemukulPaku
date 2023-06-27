using Newtonsoft.Json;
using HttpServer.Models;
using Common.Database;
using Newtonsoft.Json.Linq;
using Common;
using Common.Utils;

namespace HttpServer.Controllers
{
    public class AccountController
    {
        public static void AddHandlers(WebApplication app)
        {
            app.Map("/account/risky/api/check", (HttpContext ctx) =>
            {
                RiskyCheck rsp = new()
                {
                    Retcode = 0,
                    Message = "",
                    Data = new RiskyCheck.DataScheme()
                    {
                        Id = "",
                        Action = "ACTION_NONE",
                        Geetest = null
                    }
                };

                ctx.Response.Headers.Add("Content-Type", "application/json");

                return ctx.Response.WriteAsync(JsonConvert.SerializeObject(rsp));
            });

#pragma warning disable CS8600, CS8602 // Converting null literal or possible null value to non-nullable type.
            app.MapPost("/{game_biz}/combo/granter/login/v2/login", (ctx) =>
            {
                StreamReader Reader = new(ctx.Request.Body);
                GranterLoginBody Data = JsonConvert.DeserializeObject<GranterLoginBody>(Reader.ReadToEndAsync().Result);
                GranterLoginBody.GranterLoginBodyData GranterLoginData = JsonConvert.DeserializeObject<GranterLoginBody.GranterLoginBodyData>(Data.Data);
                

                return ctx.Response.WriteAsJsonAsync(new
                {
                    retcode = 0,
                    message = "OK",
                    data = new {
                        combo_id = "0",
                        open_id = GranterLoginData.Uid,
                        combo_token = GranterLoginData.Token,
                        data = JsonConvert.SerializeObject(new
                        {
                            guest = GranterLoginData.Guest
                        }),
                        heartbeat = false,
                        account_type = 1,
                    }
                });
            });
            
            app.MapPost("/{game_biz}/mdk/shield/api/verify", (ctx) =>
            {
                StreamReader Reader = new(ctx.Request.Body);
                ShieldVerifyBody Data = JsonConvert.DeserializeObject<ShieldVerifyBody>(Reader.ReadToEndAsync().Result);
                UserScheme? user = User.FromToken(Data.Token);

                ShieldLoginResponse rsp = new()
                {
                    Retcode = 0,
                    Message = "OK",
                    Data = new()
                    {
                        Account = null
                    }
                };

                if (user != null)
                {
                    rsp.Data = new()
                    {
                        Account = new()
                        {
                            Uid = user.Uid,
                            Name = user.Name,
                            Email = "",
                            Mobile = "",
                            IsEmailVerify = "0",
                            Realname = "",
                            IdentityCard = "",
                            Token = user.Token.ToString(),
                            SafeMobile = "",
                            FacebookName = "",
                            GoogleName = "",
                            TwitterName = "",
                            GameCenterName = "",
                            AppleName = "",
                            SonyName = "",
                            TapName = "",
                            Country = "SG",
                            ReactivateTicket = "",
                            AreaCode = "**",
                            DeviceGrantTicket = "",
                            SteamName = "",
                            UnmaskedEmail = "",
                            UnmaskedEmailType = 0
                        },
                        DeviceGrantRequired = false,
                        SafeMoblieRequired = false,
                        RealpersonRequired = false,
                        ReactivateRequired = false,
                        RealnameOperation = "None"
                    };
                }

                ctx.Response.Headers.Add("Content-Type", "application/json");

                return ctx.Response.WriteAsync(JsonConvert.SerializeObject(rsp));
            });

            app.MapPost("/{game_biz}/mdk/shield/api/login", (ctx) =>
            {
                StreamReader Reader = new(ctx.Request.Body);
                ShieldLoginBody Data = JsonConvert.DeserializeObject<ShieldLoginBody>(Reader.ReadToEndAsync().Result);

                UserScheme user;
                ShieldLoginResponse rsp;
                if (!User.NameExisted(Data.Account)) // 用户不存在
                {
                    if(!Global.config.CreateAccountOnLoginAttempt) // 关闭了自动注册
                    {
                        rsp = new()
                        {
                            Retcode = -107,
                            Message = "用户不存在",
                            Data = new()
                            { }
                        };
                    }
                    else
                    {
                        //
                        if (Global.config.MinEntropyOfAutoCreate > 0 && Misc.CalculateEntropy(Data.Account) > Global.config.MinEntropyOfAutoCreate) // 复杂度满足
                        {
                            user = User.FromName(Data.Account);
                            rsp = new()
                            {
                                Retcode = -107,
                                Message = "成功注册账号" + Data.Account + ", 再次点击以登录",
                                Data = new()
                                { }
                            };
                        }
                        else // 账号复杂度不满足
                        {
                            rsp = new()
                            {
                                Retcode = -107,
                                Message = "账号创建失败: 复杂度太低 (" + Misc.CalculateEntropy(Data.Account).ToString("F2") + "/" + Global.config.MinEntropyOfAutoCreate.ToString("F2") + "), 请重试",
                                Data = new()
                                { }
                            };
                        }
                    }

                    
                    
                }   
                else
                {
                    user = User.FromName(Data.Account);
                    rsp = new()
                    {
                        Retcode = 0,
                        Message = "OK",
                        Data = new()
                        {
                            Account = new()
                            {
                                Uid = user.Uid,
                                Name = user.Name,
                                Email = "",
                                Mobile = "",
                                IsEmailVerify = "0",
                                Realname = "",
                                IdentityCard = "",
                                Token = user.Token.ToString(),
                                SafeMobile = "",
                                FacebookName = "",
                                GoogleName = "",
                                TwitterName = "",
                                GameCenterName = "",
                                AppleName = "",
                                SonyName = "",
                                TapName = "",
                                Country = "**",
                                ReactivateTicket = "",
                                AreaCode = "**",
                                DeviceGrantTicket = "",
                                SteamName = "",
                                UnmaskedEmail = "",
                                UnmaskedEmailType = 0
                            },
                            DeviceGrantRequired = false,
                            SafeMoblieRequired = false,
                            RealpersonRequired = false,
                            ReactivateRequired = false,
                            RealnameOperation = "None"
                        }
                    };
                }

                ctx.Response.Headers.Add("Content-Type", "application/json");

                return ctx.Response.WriteAsync(JsonConvert.SerializeObject(rsp));
            });
#pragma warning restore CS8600, CS8602 // Converting null literal or possible null value to non-nullable type.
        }
    }
}
