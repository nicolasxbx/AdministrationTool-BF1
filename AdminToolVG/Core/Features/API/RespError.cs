﻿namespace BF1.ServerAdminTools.Features.API;

public class RespError
{
    public string jsonrpc { get; set; }
    public string id { get; set; }
    public Error error { get; set; }
}

public class Error
{
    public string message { get; set; }
    public int code { get; set; }
}
