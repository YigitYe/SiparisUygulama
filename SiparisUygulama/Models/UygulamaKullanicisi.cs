using Microsoft.AspNetCore.Identity;

public class UygulamaKullanicisi : IdentityUser
{
    public string? AdSoyad { get; set; }
    public string? Rol { get; set; }
}