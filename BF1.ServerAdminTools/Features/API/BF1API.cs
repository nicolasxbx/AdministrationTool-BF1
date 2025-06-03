﻿using BF1.ServerAdminTools.Common.Utils;

using RestSharp;

namespace BF1.ServerAdminTools.Features.API;

public static class BF1API
{
    private const string Host = "https://sparta-gw.battlelog.com/jsonrpc/pc/api"; //tna https://answers.ea.com/t5/Battlefield-V/Error-Code-2000-High-Ping-Infinite-loading-screen/td-p/7186372

    private static RestClient client;
    private static Dictionary<string, string> headers;

    /// <summary>
    /// 初始化RestSharp
    /// </summary>
    public static void Init()
    {
        if (client == null)
        {
            var options = new RestClientOptions(Host)
            {
                MaxTimeout = 5000,
                ThrowOnAnyError = true
            };

            client = new RestClient(options);
            headers = new Dictionary<string, string>
            {
                ["User-Agent"] = "ProtoHttp 1.3/DS 15.1.2.1.0 (Windows)",
                ["X-GatewaySession"] = Vari.SessionId,
                ["X-ClientVersion"] = "release-bf1-lsu35_26385_ad7bf56a_tunguska_all_prod",
                ["X-DbId"] = "Tunguska.Shipping2PC.Win32",
                ["X-CodeCL"] = "3779779",
                ["X-DataCL"] = "3779779",
                ["X-SaveGameVersion"] = "26",
                ["X-HostingGameId"] = "tunguska", //tna https://www.reddit.com/r/Battlefield/comments/4i9t66/bf1_apparantly_the_code_name_for_bf1_was_tunguska/
                ["X-Sparta-Info"] = "tenancyRootEnv=unknown; tenancyBlazeEnv=unknown"
            };
        }
    }

    /// <summary>
    /// 获取玩家AuthCode
    /// </summary>
    public static async Task<RespContent> GetEnvIdViaAuthCode(string authCode)
    {
        var sw = new Stopwatch();
        sw.Start();

        var respContent = new RespContent();
        
        try
        {
            headers["X-GatewaySession"] = Vari.SessionId;
            respContent.IsSuccess = false;

            var reqBody = new
            {
                jsonrpc = "2.0",
                method = "Authentication.getEnvIdViaAuthCode",
                @params = new
                {
                    authCode = authCode,
                    locale = "en-us"
                },
                id = Guid.NewGuid()
            };

            var request = new RestRequest()
                .AddHeaders(headers)
                .AddJsonBody(reqBody);

            var response = await client.ExecutePostAsync(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                respContent.IsSuccess = true;
                respContent.Message = response.Content;
            }
            else
            {
                var respError = JsonUtil.JsonDese<RespError>(response.Content);

                respContent.Message = $"{respError.error.code} {respError.error.message}";
            }
        }
        catch (Exception ex)
        {
            respContent.Message = ex.Message;
            Log.Ex(ex);
        }

        sw.Stop();
        respContent.ExecTime = sw.Elapsed.TotalSeconds;

        return respContent;
    }

    /// <summary>
    /// 获取玩家SessionID
    /// </summary>
    public static async Task<RespContent> GetCareerForOwnedGamesByPersonaId(long personaId)
    {
        var sw = new Stopwatch();
        sw.Start();

        var respContent = new RespContent();

        try
        {
            headers["X-GatewaySession"] = Vari.SessionId;
            respContent.IsSuccess = false;

            var reqBody = new
            {
                jsonrpc = "2.0",
                method = "Stats.getCareerForOwnedGamesByPersonaId",
                @params = new
                {
                    game = "tunguska", //https://www.reddit.com/r/Battlefield/comments/4i9t66/bf1_apparantly_the_code_name_for_bf1_was_tunguska/
                    personaId = personaId
                },
                id = Guid.NewGuid()
            };

            var request = new RestRequest()
                .AddHeaders(headers)
                .AddJsonBody(reqBody);

            var response = await client.ExecutePostAsync(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                respContent.IsSuccess = true;
                respContent.Message = response.Content;
            }
            else
            {
                var respError = JsonUtil.JsonDese<RespError>(response.Content);
                respContent.Message = $"{respError.error.code} {respError.error.message}";
            }
        }
        catch (Exception ex)
        {
            respContent.Message = ex.Message;
            Log.Ex(ex);
    }

        sw.Stop();
        respContent.ExecTime = sw.Elapsed.TotalSeconds;

        return respContent;
    }

    /// <summary>
    /// 获取战地1欢迎语
    /// </summary>
    public static async Task<RespContent> GetWelcomeMessage()
    {
        var sw = new Stopwatch();
        sw.Start();

        var respContent = new RespContent();

        try
        {
            headers["X-GatewaySession"] = Vari.SessionId;
            respContent.IsSuccess = false;

            var reqBody = new
            {
                jsonrpc = "2.0",
                method = "Onboarding.welcomeMessage",
                @params = new
                {
                    game = "tunguska",
                    minutesToUTC = "-480"
                },
                id = Guid.NewGuid()
            };

            var request = new RestRequest()
                .AddHeaders(headers)
                .AddJsonBody(reqBody);

            var response = await client.ExecutePostAsync(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                respContent.IsSuccess = true;
                respContent.Message = response.Content;
            }
            else
            {
                var respError = JsonUtil.JsonDese<RespError>(response.Content);
                respContent.Message = $"{respError.error.code} {respError.error.message}";
            }
        }
        catch (Exception ex)
        {
            respContent.Message = ex.Message;
            Log.Ex(ex);
    }

        sw.Stop();
        respContent.ExecTime = sw.Elapsed.TotalSeconds;

        return respContent;
    }

    /// <summary>
    /// 设置API语言
    /// </summary>
    public static async Task<RespContent> SetAPILocale()
    {
        var sw = new Stopwatch();
        sw.Start();

        var respContent = new RespContent();

        try
        {
            headers["X-GatewaySession"] = Vari.SessionId;
            respContent.IsSuccess = false;

            var reqBody = new
            {
                jsonrpc = "2.0",
                method = "CompanionSettings.setLocale",
                @params = new
                {
                    locale = "en-us"
                },
                id = Guid.NewGuid()
            };

            var request = new RestRequest()
                .AddHeaders(headers)
                .AddJsonBody(reqBody);

            var response = await client.ExecutePostAsync(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                respContent.IsSuccess = true;
                respContent.Message = response.Content;
            }
            else
            {
                var respError = JsonUtil.JsonDese<RespError>(response.Content);
                respContent.Message = $"{respError.error.code} {respError.error.message}";
            }
        }
        catch (Exception ex)
        {
            respContent.Message = ex.Message;
            Log.Ex(ex);
    }

        sw.Stop();
        respContent.ExecTime = sw.Elapsed.TotalSeconds;

        return respContent;
    }

    /// <summary>
    /// 踢出指定玩家
    /// </summary>
    public static async Task<RespContent> AdminKickPlayer(long personaId, string reason)
    {
        var sw = new Stopwatch();
        sw.Start();

        var respContent = new RespContent();

        try
        {
            headers["X-GatewaySession"] = Vari.SessionId;
            respContent.IsSuccess = false;

            var reqBody = new
            {
                jsonrpc = "2.0",
                method = "RSP.kickPlayer",
                @params = new
                {
                    game = "tunguska",
                    gameId = Vari.GameId,
                    personaId = personaId,
                    reason = reason
                },
                id = Guid.NewGuid()
            };

            var request = new RestRequest()
                .AddHeaders(headers)
                .AddJsonBody(reqBody);

            var response = await client.ExecutePostAsync(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                respContent.IsSuccess = true;
                respContent.Message = response.Content;
            }
            else
            {
                var respError = JsonUtil.JsonDese<RespError>(response.Content);
                respContent.Message = $"{respError.error.code} {respError.error.message}";
            }
        }
        catch (Exception ex)
        {
            respContent.Message = ex.Message;
            Log.Ex(ex);
        }

        sw.Stop();
        respContent.ExecTime = sw.Elapsed.TotalSeconds;

        return respContent;
    }

    /// <summary>
    /// 更换指定玩家队伍
    /// </summary>
    public static async Task<RespContent> AdminMovePlayer(long personaId, int teamId)
    {
        var sw = new Stopwatch();
        sw.Start();

        var respContent = new RespContent();

        try
        {
            headers["X-GatewaySession"] = Vari.SessionId;
            respContent.IsSuccess = false;

            var reqBody = new
            {
                jsonrpc = "2.0",
                method = "RSP.movePlayer",
                @params = new
                {
                    game = "tunguska",
                    personaId = personaId,
                    gameId = Vari.GameId,
                    teamId = teamId,
                    forceKill = true,
                    moveParty = false
                },
                id = Guid.NewGuid()
            };

            var request = new RestRequest()
                .AddHeaders(headers)
                .AddJsonBody(reqBody);

            var response = await client.ExecutePostAsync(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                respContent.IsSuccess = true;
                respContent.Message = response.Content;
            }
            else
            {
                var respError = JsonUtil.JsonDese<RespError>(response.Content);
                respContent.Message = $"{respError.error.code} {respError.error.message}";
            }
        }
        catch (Exception ex)
        {
            respContent.Message = ex.Message;
            Log.Ex(ex);
        }

        sw.Stop();
        respContent.ExecTime = sw.Elapsed.TotalSeconds;

        return respContent;
    }

    /// <summary>
    /// 更换服务器地图
    /// </summary>
    public static async Task<RespContent> ChangeServerMap(string persistedGameId, int levelIndex)
    {
        var sw = new Stopwatch();
        sw.Start();

        var respContent = new RespContent();

        try
        {
            headers["X-GatewaySession"] = Vari.SessionId;
            respContent.IsSuccess = false;

            var reqBody = new
            {
                jsonrpc = "2.0",
                method = "RSP.chooseLevel",
                @params = new
                {
                    game = "tunguska",
                    persistedGameId = persistedGameId,
                    levelIndex = levelIndex
                },
                id = Guid.NewGuid()
            };

            var request = new RestRequest()
                .AddHeaders(headers)
                .AddJsonBody(reqBody);

            var response = await client.ExecutePostAsync(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                respContent.IsSuccess = true;
                respContent.Message = response.Content;
            }
            else
            {
                var respError = JsonUtil.JsonDese<RespError>(response.Content);
                respContent.Message = $"{respError.error.code} {respError.error.message}";
            }
        }
        catch (Exception ex)
        {
            respContent.Message = ex.Message;
            Log.Ex(ex);
        }

        sw.Stop();
        respContent.ExecTime = sw.Elapsed.TotalSeconds;

        return respContent;
    }

    /// <summary>
    /// 获取完整服务器详情
    /// </summary>
    public static async Task<RespContent> GetFullServerDetails()
    {
        var sw = new Stopwatch();
        sw.Start();

        var respContent = new RespContent();

        try
        {
            headers["X-GatewaySession"] = Vari.SessionId;
            respContent.IsSuccess = false;

            var reqBody = new
            {
                jsonrpc = "2.0",
                method = "GameServer.getFullServerDetails",
                @params = new
                {
                    game = "tunguska",
                    gameId = Vari.GameId
                },
                id = Guid.NewGuid()
            };

            var request = new RestRequest()
                .AddHeaders(headers)
                .AddJsonBody(reqBody);

            var response = await client.ExecutePostAsync(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                respContent.IsSuccess = true;
                respContent.Message = response.Content;
            }
            else
            {
                var respError = JsonUtil.JsonDese<RespError>(response.Content);
                respContent.Message = $"{respError.error.code} {respError.error.message}";
            }
        }
        catch (Exception ex)
        {
            respContent.Message = ex.Message;
            Log.Ex(ex);
        }

        sw.Stop();
        respContent.ExecTime = sw.Elapsed.TotalSeconds;

        return respContent;
    }

    /// <summary>
    /// 添加服务器管理员
    /// </summary>
    public static async Task<RespContent> AddServerAdmin(string personaName)
    {
        var sw = new Stopwatch();
        sw.Start();

        var respContent = new RespContent();

        try
        {
            headers["X-GatewaySession"] = Vari.SessionId;
            respContent.IsSuccess = false;

            var reqBody = new
            {
                jsonrpc = "2.0",
                method = "RSP.addServerAdmin",
                @params = new
                {
                    game = "tunguska",
                    serverId = Vari.ServerId,
                    personaName = personaName
                },
                id = Guid.NewGuid()
            };

            var request = new RestRequest()
                .AddHeaders(headers)
                .AddJsonBody(reqBody);

            var response = await client.ExecutePostAsync(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                respContent.IsSuccess = true;
                respContent.Message = response.Content;
            }
            else
            {
                var respError = JsonUtil.JsonDese<RespError>(response.Content);
                respContent.Message = $"{respError.error.code} {respError.error.message}";
            }
        }
        catch (Exception ex)
        {
            respContent.Message = ex.Message;
            Log.Ex(ex);
        }

        sw.Stop();
        respContent.ExecTime = sw.Elapsed.TotalSeconds;

        return respContent;
    }

    /// <summary>
    /// 移除服务器管理员
    /// </summary>
    public static async Task<RespContent> RemoveServerAdmin(string personaId)
    {
        var sw = new Stopwatch();
        sw.Start();

        var respContent = new RespContent();

        try
        {
            headers["X-GatewaySession"] = Vari.SessionId;
            respContent.IsSuccess = false;

            var reqBody = new
            {
                jsonrpc = "2.0",
                method = "RSP.removeServerAdmin",
                @params = new
                {
                    game = "tunguska",
                    serverId = Vari.ServerId,
                    personaId = personaId
                },
                id = Guid.NewGuid()
            };

            var request = new RestRequest()
                .AddHeaders(headers)
                .AddJsonBody(reqBody);

            var response = await client.ExecutePostAsync(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                respContent.IsSuccess = true;
                respContent.Message = response.Content;
            }
            else
            {
                var respError = JsonUtil.JsonDese<RespError>(response.Content);
                respContent.Message = $"{respError.error.code} {respError.error.message}";
            }
        }
        catch (Exception ex)
        {
            respContent.Message = ex.Message;
            Log.Ex(ex);
        }

        sw.Stop();
        respContent.ExecTime = sw.Elapsed.TotalSeconds;

        return respContent;
    }

    /// <summary>
    /// 添加服务器VIP
    /// </summary>
    public static async Task<RespContent> AddServerVip(string personaName)
    {
        var sw = new Stopwatch();
        sw.Start();

        var respContent = new RespContent();

        try
        {
            headers["X-GatewaySession"] = Vari.SessionId;
            respContent.IsSuccess = false;

            var reqBody = new
            {
                jsonrpc = "2.0",
                method = "RSP.addServerVip",
                @params = new
                {
                    game = "tunguska",
                    serverId = Vari.ServerId,
                    personaName = personaName
                },
                id = Guid.NewGuid()
            };

            var request = new RestRequest()
                .AddHeaders(headers)
                .AddJsonBody(reqBody);

            var response = await client.ExecutePostAsync(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                respContent.IsSuccess = true;
                respContent.Message = response.Content;
            }
            else
            {
                var respError = JsonUtil.JsonDese<RespError>(response.Content);
                respContent.Message = $"{respError.error.code} {respError.error.message}";
            }
        }
        catch (Exception ex)
        {
            respContent.Message = ex.Message;
            Log.Ex(ex);
        }

        sw.Stop();
        respContent.ExecTime = sw.Elapsed.TotalSeconds;

        return respContent;
    }

    /// <summary>
    /// 移除服务器VIP
    /// </summary>
    public static async Task<RespContent> RemoveServerVip(string personaId)
    {
        var sw = new Stopwatch();
        sw.Start();

        var respContent = new RespContent();

        try
        {
            headers["X-GatewaySession"] = Vari.SessionId;
            respContent.IsSuccess = false;

            var reqBody = new
            {
                jsonrpc = "2.0",
                method = "RSP.removeServerVip",
                @params = new
                {
                    game = "tunguska",
                    serverId = Vari.ServerId,
                    personaId = personaId
                },
                id = Guid.NewGuid()
            };

            var request = new RestRequest()
                .AddHeaders(headers)
                .AddJsonBody(reqBody);

            var response = await client.ExecutePostAsync(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                respContent.IsSuccess = true;
                respContent.Message = response.Content;
            }
            else
            {
                var respError = JsonUtil.JsonDese<RespError>(response.Content);
                respContent.Message = $"{respError.error.code} {respError.error.message}";
            }
        }
        catch (Exception ex)
        {
            respContent.Message = ex.Message;
            Log.Ex(ex);
        }

        sw.Stop();
        respContent.ExecTime = sw.Elapsed.TotalSeconds;

        return respContent;
    }

    /// <summary>
    /// 添加服务器BAN
    /// </summary>
    public static async Task<RespContent> AddServerBan(string personaName)
    {
        var sw = new Stopwatch();
        sw.Start();

        var respContent = new RespContent();

        try
        {
            headers["X-GatewaySession"] = Vari.SessionId;
            respContent.IsSuccess = false;

            var reqBody = new
            {
                jsonrpc = "2.0",
                method = "RSP.addServerBan",
                @params = new
                {
                    game = "tunguska",
                    serverId = Vari.ServerId,
                    personaName = personaName
                },
                id = Guid.NewGuid()
            };

            var request = new RestRequest()
                .AddHeaders(headers)
                .AddJsonBody(reqBody);

            var response = await client.ExecutePostAsync(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                respContent.IsSuccess = true;
                respContent.Message = response.Content;
            }
            else
            {
                var respError = JsonUtil.JsonDese<RespError>(response.Content);
                respContent.Message = $"{respError.error.code} {respError.error.message}";
            }
        }
        catch (Exception ex)
        {
            respContent.Message = ex.Message;
            Log.Ex(ex);
        }

        sw.Stop();
        respContent.ExecTime = sw.Elapsed.TotalSeconds;

        return respContent;
    }

    /// <summary>
    /// 移除服务器BAN
    /// </summary>
    public static async Task<RespContent> RemoveServerBan(string personaId)
    {
        var sw = new Stopwatch();
        sw.Start();

        var respContent = new RespContent();

        try
        {
            headers["X-GatewaySession"] = Vari.SessionId;
            respContent.IsSuccess = false;

            var reqBody = new
            {
                jsonrpc = "2.0",
                method = "RSP.removeServerBan",
                @params = new
                {
                    game = "tunguska",
                    serverId = Vari.ServerId,
                    personaId = personaId
                },
                id = Guid.NewGuid()
            };

            var request = new RestRequest()
                .AddHeaders(headers)
                .AddJsonBody(reqBody);

            var response = await client.ExecutePostAsync(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                respContent.IsSuccess = true;
                respContent.Message = response.Content;
            }
            else
            {
                var respError = JsonUtil.JsonDese<RespError>(response.Content);
                respContent.Message = $"{respError.error.code} {respError.error.message}";
            }
        }
        catch (Exception ex)
        {
            respContent.Message = ex.Message;
            Log.Ex(ex);
        }

        sw.Stop();
        respContent.ExecTime = sw.Elapsed.TotalSeconds;

        return respContent;
    }

    /// <summary>
    /// 获取服务器RSP信息
    /// </summary>
    public static async Task<RespContent> GetServerDetails()
    {
        var sw = new Stopwatch();
        sw.Start();

        var respContent = new RespContent();

        try
        {
            headers["X-GatewaySession"] = Vari.SessionId;
            respContent.IsSuccess = false;

            var reqBody = new
            {
                jsonrpc = "2.0",
                method = "RSP.getServerDetails",
                @params = new
                {
                    game = "tunguska",
                    serverId = Vari.ServerId
                },
                id = Guid.NewGuid()
            };

            var request = new RestRequest()
                .AddHeaders(headers)
                .AddJsonBody(reqBody);

            var response = await client.ExecutePostAsync(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                respContent.IsSuccess = true;
                respContent.Message = response.Content;
            }
            else
            {
                var respError = JsonUtil.JsonDese<RespError>(response.Content);
                respContent.Message = $"{respError.error.code} {respError.error.message}";
            }
        }
        catch (Exception ex)
        {
            respContent.Message = ex.Message;
            Log.Ex(ex);
        }

        sw.Stop();
        respContent.ExecTime = sw.Elapsed.TotalSeconds;

        return respContent;
    }

    /// <summary>
    /// 更新服务器信息
    /// </summary>
    public static async Task<RespContent> UpdateServer(UpdateServerReqBody reqBody)
    {
        var sw = new Stopwatch();
        sw.Start();

        var respContent = new RespContent();

        try
        {
            headers["X-GatewaySession"] = Vari.SessionId;
            respContent.IsSuccess = false;

            var request = new RestRequest()
                .AddHeaders(headers)
                .AddJsonBody(reqBody);

            var response = await client.ExecutePostAsync(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                respContent.IsSuccess = true;
                respContent.Message = response.Content;
            }
            else
            {
                var respError = JsonUtil.JsonDese<RespError>(response.Content);
                respContent.Message = $"{respError.error.code} {respError.error.message}";
            }
        }
        catch (Exception ex)
        {
            respContent.Message = ex.Message;
            Log.Ex(ex);
        }

        sw.Stop();
        respContent.ExecTime = sw.Elapsed.TotalSeconds;

        return respContent;
    }

    /// <summary>
    /// 搜索服务器
    /// </summary>
    /// <param name="serverName"></param>
    /// <returns></returns>
    public static async Task<RespContent> SearchServers(string serverName)
    {
        var sw = new Stopwatch();
        sw.Start();

        var respContent = new RespContent();

        try
        {
            headers["X-GatewaySession"] = Vari.SessionId;
            respContent.IsSuccess = false;

            var reqBody = new
            {
                jsonrpc = "2.0",
                method = "GameServer.searchServers",
                @params = new
                {
                    filterJson = "{\"version\":6,\"name\":\"" + serverName + "\"}",
                    game = "tunguska",
                    limit = 20,
                    protocolVersion = "3779779"
                },
                id = Guid.NewGuid()
            };

            var request = new RestRequest()
                .AddHeaders(headers)
                .AddJsonBody(reqBody);

            var response = await client.ExecutePostAsync(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                respContent.IsSuccess = true;
                respContent.Message = response.Content;
            }
            else
            {
                var respError = JsonUtil.JsonDese<RespError>(response.Content);
                respContent.Message = $"{respError.error.code} {respError.error.message}";
            }
        }
        catch (Exception ex)
        {
            respContent.Message = ex.Message;
            Log.Ex(ex);
        }

        sw.Stop();
        respContent.ExecTime = sw.Elapsed.TotalSeconds;

        return respContent;
    }

    /// <summary>
    /// 离开当前服务器
    /// </summary>
    /// <returns></returns>
    public static async Task<RespContent> LeaveGame()
    {
        var sw = new Stopwatch();
        sw.Start();

        var respContent = new RespContent();

        try
        {
            headers["X-GatewaySession"] = Vari.SessionId;
            respContent.IsSuccess = false;

            var reqBody = new
            {
                jsonrpc = "2.0",
                method = "Game.leaveGame",
                @params = new
                {
                    game = "tunguska",
                    gameId = Vari.GameId
                },
                id = Guid.NewGuid()
            };

            var request = new RestRequest()
                .AddHeaders(headers)
                .AddJsonBody(reqBody);

            var response = await client.ExecutePostAsync(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                respContent.IsSuccess = true;
                respContent.Message = response.Content;
            }
            else
            {
                var respError = JsonUtil.JsonDese<RespError>(response.Content);
                respContent.Message = $"{respError.error.code} {respError.error.message}";
            }
        }
        catch (Exception ex)
        {
            respContent.Message = ex.Message;
            Log.Ex(ex);
        }

        sw.Stop();
        respContent.ExecTime = sw.Elapsed.TotalSeconds;

        return respContent;
    }

    /// <summary>
    /// 获取玩家PersonasByIds
    /// </summary>
    /// <returns></returns>
    public static async Task<RespContent> GetPersonasByIds(long personaId)
    {
        var sw = new Stopwatch();
        sw.Start();

        var respContent = new RespContent();

        try
        {
            headers["X-GatewaySession"] = Vari.SessionId;
            respContent.IsSuccess = false;

            var reqBody = new
            {
                jsonrpc = "2.0",
                method = "RSP.getPersonasByIds",
                @params = new
                {
                    game = "tunguska",
                    personaIds = new[] { personaId }
                },
                id = Guid.NewGuid()
            };

            var request = new RestRequest()
                .AddHeaders(headers)
                .AddJsonBody(reqBody);

            var response = await client.ExecutePostAsync(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                respContent.IsSuccess = true;
                respContent.Message = response.Content;
            }
            else
            {
                var respError = JsonUtil.JsonDese<RespError>(response.Content);
                respContent.Message = $"{respError.error.code} {respError.error.message}";
            }
        }
        catch (Exception ex)
        {
            respContent.Message = ex.Message;
            Log.Ex(ex);
        }

        sw.Stop();
        respContent.ExecTime = sw.Elapsed.TotalSeconds;

        return respContent;
    }

    /// <summary>
    /// 获取玩家战绩
    /// </summary>
    public static async Task<RespContent> DetailedStatsByPersonaId(long personaId)
    {
        var sw = new Stopwatch();
        sw.Start();

        var respContent = new RespContent();

        try
        {
            headers["X-GatewaySession"] = Vari.SessionId;
            respContent.IsSuccess = false;

            var reqBody = new
            {
                jsonrpc = "2.0",
                method = "Stats.detailedStatsByPersonaId",
                @params = new
                {
                    game = "tunguska",
                    personaId = personaId
                },
                id = Guid.NewGuid()
            };

            var request = new RestRequest()
                .AddHeaders(headers)
                .AddJsonBody(reqBody);

            var response = await client.ExecutePostAsync(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                respContent.IsSuccess = true;
                respContent.Message = response.Content;
            }
            else
            {
                var respError = JsonUtil.JsonDese<RespError>(response.Content);
                respContent.Message = $"{respError.error.code} {respError.error.message}";
            }
        }
        catch (Exception ex)
        {
            respContent.Message = ex.Message;
            Log.Ex(ex);
        }

        sw.Stop();
        respContent.ExecTime = sw.Elapsed.TotalSeconds;

        return respContent;
    }

    /// <summary>
    /// 获取玩家武器
    /// </summary>
    public static async Task<RespContent> GetWeaponsByPersonaId(long personaId)
    {
        var sw = new Stopwatch();
        sw.Start();

        var respContent = new RespContent();

        try
        {
            headers["X-GatewaySession"] = Vari.SessionId;
            respContent.IsSuccess = false;

            var reqBody = new
            {
                jsonrpc = "2.0",
                method = "Progression.getWeaponsByPersonaId",
                @params = new
                {
                    game = "tunguska",
                    personaId = personaId
                },
                id = Guid.NewGuid()
            };

            var request = new RestRequest()
                .AddHeaders(headers)
                .AddJsonBody(reqBody);

            var response = await client.ExecutePostAsync(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                respContent.IsSuccess = true;
                respContent.Message = response.Content;
            }
            else
            {
                var respError = JsonUtil.JsonDese<RespError>(response.Content);
                respContent.Message = $"{respError.error.code} {respError.error.message}";
            }
        }
        catch (Exception ex)
        {
            respContent.Message = ex.Message;
            Log.Ex(ex);
        }

        sw.Stop();
        respContent.ExecTime = sw.Elapsed.TotalSeconds;

        return respContent;
    }

    /// <summary>
    /// 获取玩家载具
    /// </summary>
    public static async Task<RespContent> GetVehiclesByPersonaId(long personaId)
    {
        var sw = new Stopwatch();
        sw.Start();

        var respContent = new RespContent();

        try
        {
            headers["X-GatewaySession"] = Vari.SessionId;
            respContent.IsSuccess = false;

            var reqBody = new
            {
                jsonrpc = "2.0",
                method = "Progression.getVehiclesByPersonaId",
                @params = new
                {
                    game = "tunguska",
                    personaId = personaId
                },
                id = Guid.NewGuid()
            };

            var request = new RestRequest()
                .AddHeaders(headers)
                .AddJsonBody(reqBody);

            var response = await client.ExecutePostAsync(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                respContent.IsSuccess = true;
                respContent.Message = response.Content;
            }
            else
            {
                var respError = JsonUtil.JsonDese<RespError>(response.Content);
                respContent.Message = $"{respError.error.code} {respError.error.message}";
            }
        }
        catch (Exception ex)
        {
            respContent.Message = ex.Message;
            Log.Ex(ex);
        }

        sw.Stop();
        respContent.ExecTime = sw.Elapsed.TotalSeconds;

        return respContent;
    }

    /// <summary>
    /// 获取玩家正在游玩服务器
    /// </summary>
    public static async Task<RespContent> GetServersByPersonaIds(long personaId)
    {
        var sw = new Stopwatch();
        sw.Start();

        var respContent = new RespContent();

        try
        {
            headers["X-GatewaySession"] = Vari.SessionId;
            respContent.IsSuccess = false;

            var reqBody = new
            {
                jsonrpc = "2.0",
                method = "GameServer.getServersByPersonaIds",
                @params = new
                {
                    game = "tunguska",
                    personaIds = new[] { personaId }
                },
                id = Guid.NewGuid()
            };

            var request = new RestRequest()
                .AddHeaders(headers)
                .AddJsonBody(reqBody);

            var response = await client.ExecutePostAsync(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                respContent.IsSuccess = true;
                respContent.Message = response.Content;
            }
            else
            {
                var respError = JsonUtil.JsonDese<RespError>(response.Content);
                respContent.Message = $"{respError.error.code} {respError.error.message}";
            }
        }
        catch (Exception ex)
        {
            respContent.Message = ex.Message;
            Log.Ex(ex);
        }

        sw.Stop();
        respContent.ExecTime = sw.Elapsed.TotalSeconds;

        return respContent;
    }

    /// <summary>
    /// 获取玩家佩戴的图章
    /// </summary>
    public static async Task<RespContent> GetEquippedEmblem(long personaId)
    {
        var sw = new Stopwatch();
        sw.Start();

        var respContent = new RespContent();

        try
        {
            headers["X-GatewaySession"] = Vari.SessionId;
            respContent.IsSuccess = false;

            var reqBody = new
            {
                jsonrpc = "2.0",
                method = "Emblems.getEquippedEmblem",
                @params = new
                {
                    platform = "pc",
                    personaId = personaId
                },
                id = Guid.NewGuid()
            };

            var request = new RestRequest()
                .AddHeaders(headers)
                .AddJsonBody(reqBody);

            var response = await client.ExecutePostAsync(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                respContent.IsSuccess = true;
                respContent.Message = response.Content;
            }
            else
            {
                var respError = JsonUtil.JsonDese<RespError>(response.Content);
                respContent.Message = $"{respError.error.code} {respError.error.message}";
            }
        }
        catch (Exception ex)
        {
            respContent.Message = ex.Message;
            Log.Ex(ex);
        }

        sw.Stop();
        respContent.ExecTime = sw.Elapsed.TotalSeconds;

        return respContent;
    }

    public static async Task<RespContent> JoinGame() //test
    {
        var sw = new Stopwatch();
        sw.Start();

        var respContent = new RespContent();

        try
        {
            headers["X-GatewaySession"] = Vari.SessionId;
            respContent.IsSuccess = false;

            var reqBody = new
            {
                jsonrpc = "2.0",
                method = "Game.joinGame",
                @params = new
                {
                    game = "tunguska",
                    gameId = 7364757830856.ToString()
                },
                id = Guid.NewGuid()
            };

            var request = new RestRequest()
                .AddHeaders(headers)
                .AddJsonBody(reqBody);

            var response = await client.ExecutePostAsync(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                respContent.IsSuccess = true;
                respContent.Message = response.Content;
            }
            else
            {
                var respError = JsonUtil.JsonDese<RespError>(response.Content);
                respContent.Message = $"{respError.error.code} {respError.error.message}";
            }
        }
        catch (Exception ex)
        {
            respContent.Message = ex.Message;
            Log.Ex(ex);
        }

        sw.Stop();
        respContent.ExecTime = sw.Elapsed.TotalSeconds;

        return respContent;
    }
    public static async Task<RespContent> JoinGameTest(string input, string gameID) //test
    {
        var sw = new Stopwatch();
        sw.Start();

        var respContent = new RespContent();

        try
        {
            headers["X-GatewaySession"] = Vari.SessionId;
            respContent.IsSuccess = false;

            var reqBody = new
            {
                jsonrpc = "2.0",
                method = input,
                @params = new
                {
                    game = "tunguska",
                    gameId = gameID,
                },
                id = Guid.NewGuid()
            };

            var request = new RestRequest()
                .AddHeaders(headers)
                .AddJsonBody(reqBody);

            var response = await client.ExecutePostAsync(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                respContent.IsSuccess = true;
                respContent.Message = response.Content;
            }
            else
            {
                var respError = JsonUtil.JsonDese<RespError>(response.Content);
                respContent.Message = $"{respError.error.code} {respError.error.message}";
            }
        }
        catch (Exception ex)
        {
            respContent.Message = ex.Message;
            Log.Ex(ex);
        }

        sw.Stop();
        respContent.ExecTime = sw.Elapsed.TotalSeconds;

        return respContent;
    }
}
