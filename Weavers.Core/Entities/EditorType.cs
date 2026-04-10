using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;

namespace Weavers.Core.Entities {
  public class EditorType {
    public int Id { get; set; } = 0;
    public int Rank { get; set; } = 0;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsVisible { get; set; } = true;
    public bool IsReadOnly { get; set; } = false;
    public string IconName { get; set; } = string.Empty;

  }


  public class EditorTypeConfiguration : IEntityTypeConfiguration<EditorType> {
    public void Configure(EntityTypeBuilder<EditorType> builder) {
      builder.ToTable("EditorTypes");
      builder.HasKey(x => x.Id);
      builder.Property(x => x.Id).ValueGeneratedOnAdd();
      builder.Property(x => x.Rank).HasDefaultValue(0);      
      builder.Property(x => x.Name).IsRequired().HasMaxLength(500);
      builder.Property(x => x.Description).HasMaxLength(-1);
      builder.HasIndex(x => x.Name).IsUnique();      
      builder.Property(x => x.IsVisible).HasDefaultValue(true);
      builder.Property(x => x.IsReadOnly).HasDefaultValue(false);
      builder.Property(x => x.IconName).HasMaxLength(100).HasDefaultValue(string.Empty);

      builder.HasData(
        new EditorType { Id = (int)WeEditorType.None, Name = "None", Description = "No editor", IsVisible=false, IsReadOnly=true },
        new EditorType { Id = (int)WeEditorType.Hidden, Name = "Hidden", Description = "Hidden editor", IsVisible=false, IsReadOnly=true, IconName="pi-eye-slash" },
        new EditorType { Id = (int)WeEditorType.Boolean, Name = "Boolean", Description = "Boolean editor", IconName="pi-check" },
        new EditorType { Id = (int)WeEditorType.Integer, Name = "Integer", Description = "Integer editor", IconName="pi-pencil" },
        new EditorType { Id = (int)WeEditorType.String, Name = "String", Description = "String editor", IconName="pi-pencil" },
        new EditorType { Id = (int)WeEditorType.FileName, Name = "FileName", Description = "File name editor", IconName="pi-file" },
        new EditorType { Id = (int)WeEditorType.Date, Name = "Date", Description = "Date editor", IconName = "pi-calendar" },
        new EditorType { Id = (int)WeEditorType.Time, Name = "Time", Description = "Time editor", IconName = "pi-clock" },
        new EditorType { Id = (int)WeEditorType.Decimal, Name = "Decimal", Description = "Decimal editor", IconName="pi-dollar" },
        new EditorType { Id = (int)WeEditorType.Password, Name = "Password", Description = "Password editor", IconName="pi-lock" },
        new EditorType { Id = (int)WeEditorType.LookupTypeEditor, Name = "Lookup Type Editor", Description = "Lookup type editor", IconName="pi-search" },
        new EditorType { Id = (int)WeEditorType.LookupModelEditor, Name = "Lookup Model Editor", Description = "Lookup model editor", IconName="pi-search" },
        new EditorType { Id = (int)WeEditorType.Memo, Name = "Memo", Description = "Memo editor", IconName="pi-pencil" }
      );
    }
  }
}
