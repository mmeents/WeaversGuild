using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weavers.Core.Entities {

  public class AppSetting {
    public int Id { get; private set; }
    public string Key { get; set; } = string.Empty;
    public string? Value { get; set; }
    public int? ValueInt { get; set; }
    public string? Description { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime ModifiedAt { get; set; }

    public AppSetting() { }

    public AppSetting(string key, string? value, int? valueInt, string? description = null) {
      Key = key;
      Value = value;
      ValueInt = valueInt;
      Description = description;
      CreatedAt = DateTime.UtcNow;
      ModifiedAt = DateTime.UtcNow;
    }

    public void UpdateValue(string? value, int? valueInt) {
      Value = value;
      ValueInt = valueInt;
      ModifiedAt = DateTime.UtcNow;
    }
  }

  public class AppSettingConfiguration : IEntityTypeConfiguration<AppSetting> {
    public void Configure(EntityTypeBuilder<AppSetting> builder) {
      builder.HasKey(s => s.Id);
      builder.Property(s => s.Key).IsRequired().HasMaxLength(200);
      builder.Property(s => s.Value).IsRequired(false).HasMaxLength(4000);
      builder.Property(s => s.ValueInt).IsRequired(false);
      builder.Property(s => s.Description).IsRequired(false).HasMaxLength(1000);
      builder.HasIndex(s => s.Key).IsUnique();
    }

  }
}
