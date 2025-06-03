using CommunityToolkit.Mvvm.ComponentModel;

namespace BF1.ServerAdminTools.Models;

public class DetailModel : ObservableObject
{
    private string _serverName = "";
    
    public string ServerName
    {
        get => _serverName;
        set => SetProperty(ref _serverName, value);
    }

    private string _serverDescription = "";
    
    public string ServerDescription
    {
        get => _serverDescription;
        set => SetProperty(ref _serverDescription, value);
    }

    private string _serverID = "";
    
    public string ServerID
    {
        get => _serverID;
        set => SetProperty(ref _serverID, value);
    }

    private string _serverGameID = "";
    
    public string ServerGameID
    {
        get => _serverGameID;
        set => SetProperty(ref _serverGameID, value);
    }

    private string _serverOwnerName = "";
    
    public string ServerOwnerName
    {
        get => _serverOwnerName;
        set => SetProperty(ref _serverOwnerName, value);
    }

    private string _serverOwnerPersonaId = "";
    
    public string ServerOwnerPersonaId
    {
        get => _serverOwnerPersonaId;
        set => SetProperty(ref _serverOwnerPersonaId, value);
    }

    private string _serverOwnerImage = "";
    
    public string ServerOwnerImage
    {
        get => _serverOwnerImage;
        set => SetProperty(ref _serverOwnerImage, value);
    }
}
