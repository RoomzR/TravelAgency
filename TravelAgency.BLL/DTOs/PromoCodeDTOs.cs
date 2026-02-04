namespace TravelAgency.BLL.DTOs
{
    public class PromoCodeDTO
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal DiscountPercent { get; set; }
        public decimal? MaxDiscountAmount { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public int? MaxUses { get; set; }
        public int CurrentUses { get; set; }
        public bool IsActive { get; set; }
    }

    public class PromoCodeCreateDTO
    {
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal DiscountPercent { get; set; }
        public decimal? MaxDiscountAmount { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public int? MaxUses { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class PromoCodeUpdateDTO : PromoCodeCreateDTO
    {
        public int Id { get; set; }
    }
}