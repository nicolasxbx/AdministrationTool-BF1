namespace BF1.ServerAdminTools.Models;

public class PersonaIDNex //tna
{
    public string url { get; set; }    
    public string status { get; set; }
    public bool hacker { get; set; }
    public string originId { get; set;}
    public string originPersonaId { get; set;}
    public string originUserId { get; set; }
    public string cheatMethods { get; set; }
}

public class PersonaIDNexEmpty //tna
{    
    public bool hacker { get; set; }    
}

public class PersonaIDNexArray //tna
{
    public PersonaIDNex[] personaidnexarray { get; set; }
}

public class personaids : Dictionary<string, PersonaIDNex> { }



