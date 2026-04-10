using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;

namespace Weavers.Core.Entities {
  public class DataType {
    public int Id { get; set; } = 0;    
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
  }


  public class DataTypeConfiguration : IEntityTypeConfiguration<DataType> {
    public void Configure(EntityTypeBuilder<DataType> builder) {
      builder.ToTable("DataTypes");
      builder.HasKey(x => x.Id);
      builder.Property(x => x.Id).ValueGeneratedOnAdd();
      builder.Property(x => x.Name).IsRequired().HasMaxLength(500);
      builder.Property(x => x.Description).IsRequired().HasMaxLength(1000);
      builder.HasIndex(x => x.Name).IsUnique();

      // Seeding logic: Iterate through the Enum to create the entity data
      var seedData = Enum.GetValues(typeof(WeDataType))
          .Cast<WeDataType>()
          .Select(e => new DataType {
            Id = (int)e,
            Name = e.ToString(),
            Description = e.Description()
          });

      builder.HasData(seedData);
    }
  }
}
