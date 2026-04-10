using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weavers.Core.Models {
  public class DataTypeDto {
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

  }

  public static class DataTypeDtoExtensions {
    public static DataTypeDto ToDto(this Core.Entities.DataType dataType) {
      return new DataTypeDto {
        Id = dataType.Id,
        Name = dataType.Name,
        Description = dataType.Description
      };
    }
  }
}
