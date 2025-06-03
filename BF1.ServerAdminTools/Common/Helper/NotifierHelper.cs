﻿using BF1.ServerAdminTools.Properties;

using Notification.Wpf;
using Notification.Wpf.Controls;
using Notification.Wpf.Constants;

namespace BF1.ServerAdminTools.Common.Helper;

public static class NotifierHelper
{
    private static readonly NotificationManager __NotificationManager = new();

    private const string AreaName = "WindowArea";
    private static readonly TimeSpan ExpirationTime = TimeSpan.FromSeconds(5); //tna from 2 to 5 seconds

    static NotifierHelper()
    {
        Resources.Culture = Thread.CurrentThread.CurrentUICulture;

        NotificationConstants.MessagePosition = NotificationPosition.BottomCenter;
        NotificationConstants.NotificationsOverlayWindowMaxCount = 5;

        NotificationConstants.MinWidth = 500D;
        NotificationConstants.MaxWidth = 500D;

        NotificationConstants.FontName = "微软雅黑";
        NotificationConstants.TitleSize = 14;
        NotificationConstants.MessageSize = 12;
        NotificationConstants.MessageTextAlignment = TextAlignment.Left;
        NotificationConstants.TitleTextAlignment = TextAlignment.Left;

        NotificationConstants.DefaultBackgroundColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#444444"));
        NotificationConstants.DefaultForegroundColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFF"));

        NotificationConstants.InformationBackgroundColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#409EFF"));
        NotificationConstants.SuccessBackgroundColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#67C23A"));
        NotificationConstants.WarningBackgroundColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E6A23C"));
        NotificationConstants.ErrorBackgroundColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F56C6C"));
    }

    /// <summary>
    /// 显示Toast通知
    /// </summary>
    /// <param name="type"></param>
    /// <param name="message"></param>
    public static void Show(NotifierType type, string message)
    {
        string title;
        switch (type)
        {
            case NotifierType.None:
                title = "";
                break;
            case NotifierType.Information:
                title = "Info";
                break;
            case NotifierType.Success:
                title = "Success";
                break;
            case NotifierType.Warning:
                title = "Warning";
                break;
            case NotifierType.Error:
                title = "Error";
                break;
            case NotifierType.Notification:
                title = "Notification";
                break;
            default:
                title = "";
                break;
        }

        var clickContent = new NotificationContent
        {
            Title = title,
            Message = message,
            Type = (NotificationType)type,
            TrimType = NotificationTextTrimType.NoTrim,
        };

        __NotificationManager.Show(clickContent, AreaName, ExpirationTime, null, null, true, false);
    }
}

public enum NotifierType
{
    /// <summary>
    /// 无
    /// </summary>
    None,
    /// <summary>
    /// 信息
    /// </summary>
    Information,
    /// <summary>
    /// 成功
    /// </summary>
    Success,
    /// <summary>
    /// 警告
    /// </summary>
    Warning,
    /// <summary>
    /// 错误
    /// </summary>
    Error,
    /// <summary>
    /// 通知
    /// </summary>
    Notification
}
