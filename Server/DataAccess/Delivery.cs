namespace DataAccess;

public class Delivery
{
    public int id { get; set; }
    
    public double longitude { get; set; }
    
    public double latitude { get; set; }
    
    public DateTime timestamp { get; set; }

    public double composite_tax_rate { get; set; }
    
    public double state_rate { get; set; } = 0.04;
    
    public double county_rate { get; set; }
    
    public double special_rates { get; set; }
    
    public double subtotal { get; set; }
    
    public double tax_amount { get; set; }
    
    public double total_amount { get; set; }
    
    
}