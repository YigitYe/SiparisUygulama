public class MenuEkleViewModel
{
    public int RestaurantId { get; set; }
    public string ItemName { get; set; } = null!;
    public string? Aciklama { get; set; }
    public decimal Fiyat { get; set; }
    public string? Kategori { get; set; }
}