using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weavers.Core.Models;

namespace TheLoomApp.Extensions {
  public static class ItemPropertiesExt {
    public static void AddOrUpdateProperty(this ItemDto item, ItemPropertyDto property) {
      var existingProperty = item.Properties.FirstOrDefault(p => p.Name == property.Name);
      if (existingProperty != null) {
        existingProperty.Value = property.Value;
      } else {
        item.Properties.Add(property);
      }
    }
  }
}
