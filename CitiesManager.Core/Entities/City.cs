using System.ComponentModel.DataAnnotations;

namespace CitiesManager.Core.Models;

public class City
{
    [Key]
    public Guid CityId { get; set; }
    [Required(ErrorMessage ="CityName can't be blank")]
    public string? CityName { get; set; }
}
