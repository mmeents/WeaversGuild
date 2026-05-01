using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Weavers.Core.Migrations
{
    /// <inheritdoc />
    public partial class IsCreateVersion110 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Key = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ValueInt = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DataTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EditorTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Rank = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Name = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", maxLength: -1, nullable: false),
                    IsVisible = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsReadOnly = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IconName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, defaultValue: "")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EditorTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RelationTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RelationTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ItemTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParentTypeId = table.Column<int>(type: "int", nullable: true),
                    EditorTypeId = table.Column<int>(type: "int", nullable: true),
                    Rank = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", maxLength: -1, nullable: false),
                    IsVisible = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsReadOnly = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IconName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemTypes_EditorTypes_EditorTypeId",
                        column: x => x.EditorTypeId,
                        principalTable: "EditorTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ItemTypes_ItemTypes_ParentTypeId",
                        column: x => x.ParentTypeId,
                        principalTable: "ItemTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ItemPropertyDefaults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemTypeId = table.Column<int>(type: "int", nullable: false),
                    Rank = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    Key = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DefaultValue = table.Column<string>(type: "nvarchar(max)", maxLength: -1, nullable: false, defaultValue: ""),
                    ValueDataTypeId = table.Column<int>(type: "int", nullable: true),
                    ReferenceItemTypeId = table.Column<int>(type: "int", nullable: true),
                    EditorTypeId = table.Column<int>(type: "int", nullable: true),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsVisible = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsReadOnly = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemPropertyDefaults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemPropertyDefaults_DataTypes_ValueDataTypeId",
                        column: x => x.ValueDataTypeId,
                        principalTable: "DataTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItemPropertyDefaults_EditorTypes_EditorTypeId",
                        column: x => x.EditorTypeId,
                        principalTable: "EditorTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItemPropertyDefaults_ItemTypes_ItemTypeId",
                        column: x => x.ItemTypeId,
                        principalTable: "ItemTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItemPropertyDefaults_ItemTypes_ReferenceItemTypeId",
                        column: x => x.ReferenceItemTypeId,
                        principalTable: "ItemTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemTypeId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", maxLength: -1, nullable: false),
                    Data = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "{}"),
                    Established = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Items_ItemTypes_ItemTypeId",
                        column: x => x.ItemTypeId,
                        principalTable: "ItemTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ItemProperties",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemPropertyDefaultId = table.Column<int>(type: "int", nullable: true),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", maxLength: -1, nullable: true),
                    ValueDataTypeId = table.Column<int>(type: "int", nullable: true),
                    ReferenceItemTypeId = table.Column<int>(type: "int", nullable: true),
                    EditorTypeId = table.Column<int>(type: "int", nullable: true),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false),
                    IsVisible = table.Column<bool>(type: "bit", nullable: false),
                    IsReadOnly = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemProperties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemProperties_DataTypes_ValueDataTypeId",
                        column: x => x.ValueDataTypeId,
                        principalTable: "DataTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItemProperties_EditorTypes_EditorTypeId",
                        column: x => x.EditorTypeId,
                        principalTable: "EditorTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItemProperties_ItemPropertyDefaults_ItemPropertyDefaultId",
                        column: x => x.ItemPropertyDefaultId,
                        principalTable: "ItemPropertyDefaults",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItemProperties_ItemTypes_ReferenceItemTypeId",
                        column: x => x.ReferenceItemTypeId,
                        principalTable: "ItemTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItemProperties_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Relations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    RelationTypeId = table.Column<int>(type: "int", nullable: false),
                    RelatedItemId = table.Column<int>(type: "int", nullable: true),
                    Established = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    Rank = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Relations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Relations_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Relations_Items_RelatedItemId",
                        column: x => x.RelatedItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Relations_RelationTypes_RelationTypeId",
                        column: x => x.RelationTypeId,
                        principalTable: "RelationTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "DataTypes",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { -1, "None", "None" },
                    { 1, "Boolean (true/false)", "Boolean" },
                    { 2, "16-bit integer", "Int16" },
                    { 3, "32-bit integer", "Int32" },
                    { 4, "64-bit integer", "Int64" },
                    { 5, "Globally Unique Identifier", "Guid" },
                    { 6, "ASCII string", "StrAscii" },
                    { 7, "Unicode string", "StrUnicode" },
                    { 8, "32-bit floating point number", "Float32" },
                    { 9, "64-bit floating point number", "Float64" },
                    { 10, "128-bit decimal number", "Decimal128" },
                    { 11, "Date (year, month, day)", "Date" },
                    { 12, "Time (hour, minute, second)", "Time" },
                    { 13, "Small date and time (1900-01-01 to 2079-06-06)", "SmallDateTime" },
                    { 14, "Date and time (1753-01-01 to 9999-12-31)", "DateTime" },
                    { 15, "Date and time with larger range and precision than DateTime", "DateTime2" },
                    { 16, "Date and time with time zone awareness", "DateTimeOffset" },
                    { 17, "Binary data (byte array)", "Binary" },
                    { 18, "Reference to Items graph see ReferenceItemTypeId for Item Properties", "Reference" }
                });

            migrationBuilder.InsertData(
                table: "EditorTypes",
                columns: new[] { "Id", "Description", "IconName", "IsReadOnly", "Name" },
                values: new object[,]
                {
                    { -1, "No editor", "", true, "None" },
                    { 1, "Hidden editor", "pi-eye-slash", true, "Hidden" }
                });

            migrationBuilder.InsertData(
                table: "EditorTypes",
                columns: new[] { "Id", "Description", "IconName", "IsVisible", "Name" },
                values: new object[,]
                {
                    { 2, "Boolean editor", "pi-check", true, "Boolean" },
                    { 3, "Integer editor", "pi-pencil", true, "Integer" },
                    { 4, "String editor", "pi-pencil", true, "String" },
                    { 5, "File name editor", "pi-file", true, "FileName" },
                    { 6, "Date editor", "pi-calendar", true, "Date" },
                    { 7, "Time editor", "pi-clock", true, "Time" },
                    { 8, "Decimal editor", "pi-dollar", true, "Decimal" },
                    { 9, "Password editor", "pi-lock", true, "Password" },
                    { 10, "Lookup type editor", "pi-search", true, "Lookup Type Editor" },
                    { 11, "Lookup model editor", "pi-search", true, "Lookup Model Editor" },
                    { 12, "Memo editor", "pi-pencil", true, "Memo" },
                    { 13, "Folder editor", "pi-folder", true, "Folder" },
                    { 14, "Relative folder editor", "pi-folder", true, "Relative Folder" },
                    { 15, "URL editor", "pi-link", true, "URL" }
                });

            migrationBuilder.InsertData(
                table: "RelationTypes",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 10, "Item belongs to a type category", "TypeOf" },
                    { 20, "Structural parent contains child model", "Contains" }
                });

            migrationBuilder.InsertData(
                table: "ItemTypes",
                columns: new[] { "Id", "Description", "EditorTypeId", "IconName", "IsVisible", "Name", "ParentTypeId", "Rank" },
                values: new object[,]
                {
                    { 2, "Inbound Nav Types", 10, "", true, "InboundNavTypes", null, 1 },
                    { 6, "Entity Nav Types", 10, "", true, "NavigationTypes", null, 1 },
                    { 10, "Owner Type of SQL Types", 10, "", true, "SqlTypes", null, 1 },
                    { 21, "sql float type", -1, "", true, "SqlFloatType", 21, 0 },
                    { 40, "Owner Type of C# Lifetimes", 10, "", true, "CSharpLifetimes", null, 1 },
                    { 50, "Owner Type of C# Types", 10, "", true, "CSharpTypes", null, 1 },
                    { 90, "Accessibility Lookups", 10, "", true, "AccessibilityLookups", null, 1 },
                    { 100, "Project Folder", 4, "pi pi-folder", true, "ProjectFolderModel", null, 100 }
                });

            migrationBuilder.InsertData(
                table: "ItemPropertyDefaults",
                columns: new[] { "Id", "DefaultValue", "EditorTypeId", "IsVisible", "ItemTypeId", "Key", "Rank", "ReferenceItemTypeId", "ValueDataTypeId" },
                values: new object[,]
                {
                    { 1, "", 13, true, 100, "RootFolder", 1, null, 6 },
                    { 2, "", 4, true, 100, "RepositoryUrl", 2, null, 6 }
                });

            migrationBuilder.InsertData(
                table: "ItemTypes",
                columns: new[] { "Id", "Description", "EditorTypeId", "IconName", "IsVisible", "Name", "ParentTypeId", "Rank" },
                values: new object[,]
                {
                    { 3, "With One", 2, "", true, "NavWithOne", 2, 1 },
                    { 4, "With Many", 2, "", true, "NavWithMany", 2, 2 },
                    { 7, "Has One", 2, "", true, "NavHasOne", 6, 1 },
                    { 8, "Has Many", 2, "", true, "NavHasMany", 6, 2 },
                    { 11, "sql bit type", 2, "", true, "SqlBitType", 10, 2 },
                    { 12, "sql smallint type", 3, "", true, "SqlSmallIntType", 10, 3 },
                    { 13, "sql int type", 3, "", true, "SqlIntType", 10, 4 },
                    { 14, "sql bigint type", 3, "", true, "SqlBigIntType", 10, 5 },
                    { 16, "sql uniqueidentifier type", 4, "", true, "SqlGuidType", 10, 6 },
                    { 18, "sql varchar type", 4, "", true, "SqlVarcharType", 10, 7 },
                    { 20, "sql nvarchar type", 4, "", true, "SqlNVarcharType", 10, 8 },
                    { 22, "sql decimal type", 8, "", true, "SqlDecimalType", 10, 9 },
                    { 24, "sql datetime type", 6, "", true, "SqlDateTimeType", 10, 10 },
                    { 25, "sql datetime2 type", 6, "", true, "SqlDateTime2Type", 10, 11 },
                    { 26, "sql date type", 6, "", true, "SqlDateType", 10, 12 },
                    { 28, "sql time type", 7, "", true, "SqlTimeType", 10, 13 },
                    { 29, "sql datetimeoffset type", 4, "", true, "SqlDateTimeOffsetType", 10, 14 },
                    { 30, "sql binary type", -1, "", true, "SqlBinaryType", 10, 15 },
                    { 41, "C# Singleton Lifetime", -1, "", true, "CSLifetimeSingleton", 40, 1 },
                    { 42, "C# Scoped Lifetime", -1, "", true, "CSLifetimeScoped", 40, 2 },
                    { 43, "C# Transient Lifetime", -1, "", true, "CSLifetimeTransient", 40, 3 },
                    { 52, "C# Class Type", 11, "", true, "CSharpClassType", 50, 2 },
                    { 54, "C# Record Type", 11, "", true, "CSharpRecordType", 50, 3 },
                    { 56, "C# Struct Type", 11, "", true, "CSharpStructType", 50, 4 },
                    { 58, "C# String Type", 4, "", true, "CSharpStringType", 50, 5 },
                    { 60, "C# Bool Type", 2, "", true, "CSharpBoolType", 50, 6 },
                    { 62, "C# Char Type", 4, "", true, "CSharpCharType", 50, 7 },
                    { 64, "C# Int Type", 3, "", true, "CSharpIntType", 50, 8 },
                    { 66, "C# Long Type", 3, "", true, "CSharpLongType", 50, 9 },
                    { 68, "C# Short Type", 3, "", true, "CSharpShortType", 50, 10 },
                    { 70, "C# Decimal Type", 8, "", true, "CSharpDecimalType", 50, 11 },
                    { 72, "C# Double Type", 8, "", true, "CSharpDoubleType", 50, 12 },
                    { 74, "C# Float Type", 8, "", true, "CSharpFloatType", 50, 13 },
                    { 76, "C# Byte Type", 3, "", true, "CSharpByteType", 50, 14 },
                    { 78, "C# DateTime Type", 6, "", true, "CSharpDateTimeType", 50, 15 },
                    { 79, "C# DateTime2 Type", 6, "", true, "CSharpDateTime2Type", 50, 16 },
                    { 80, "C# Date Type", 6, "", true, "CSharpDateType", 50, 17 },
                    { 82, "C# Time Type", 7, "", true, "CSharpTimeType", 50, 18 },
                    { 84, "C# DateTimeOffset Type", 4, "", true, "CSharpDateTimeOffsetType", 50, 19 },
                    { 86, "C# Byte Array Type", -1, "", true, "CSharpByteArrayType", 50, 20 },
                    { 88, "C# Guid Type", 4, "", true, "CSharpGuidType", 50, 21 },
                    { 91, "public", 4, "", true, "WePublic", 90, 1 },
                    { 92, "internal", 4, "", true, "WeInternal", 90, 2 },
                    { 93, "private", 4, "", true, "WePrivate", 90, 3 },
                    { 94, "protected", 4, "", true, "WeProtected", 90, 4 },
                    { 95, "protected internal", 4, "", true, "WeProtectedInternal", 90, 5 },
                    { 110, "Relative Folder", 4, "pi pi-folder", true, "RelativeFolderModel", 100, 110 }
                });

            migrationBuilder.InsertData(
                table: "ItemPropertyDefaults",
                columns: new[] { "Id", "DefaultValue", "EditorTypeId", "IsVisible", "ItemTypeId", "Key", "Rank", "ReferenceItemTypeId", "ValueDataTypeId" },
                values: new object[] { 3, "", 14, true, 110, "RelativeFolder", 1, null, 6 });

            migrationBuilder.InsertData(
                table: "ItemTypes",
                columns: new[] { "Id", "Description", "EditorTypeId", "IconName", "IsVisible", "Name", "ParentTypeId", "Rank" },
                values: new object[,]
                {
                    { 150, "File", 5, "pi pi-file", true, "FileModel", 110, 150 },
                    { 160, "Solution", 4, "pi pi-sitemap", true, "SolutionModel", 110, 160 },
                    { 200, "Library", 4, "pi pi-book", true, "LibraryModel", 110, 200 }
                });

            migrationBuilder.InsertData(
                table: "ItemPropertyDefaults",
                columns: new[] { "Id", "DefaultValue", "EditorTypeId", "IsVisible", "ItemTypeId", "Key", "Rank", "ReferenceItemTypeId", "ValueDataTypeId" },
                values: new object[,]
                {
                    { 4, "", 5, true, 150, "FilePath", 1, null, 6 },
                    { 5, ".md", 4, true, 150, "FileExtension", 2, null, 6 },
                    { 6, "", 5, true, 160, "FilePath", 1, null, 6 },
                    { 7, ".sln", 4, true, 160, "FileExtension", 2, null, 6 },
                    { 8, "", 4, true, 160, "SolutionGuid", 3, null, 6 },
                    { 11, "", 4, true, 200, "FilePath", 1, null, 6 },
                    { 12, ".csproj", 4, true, 200, "FileExtension", 2, null, 6 },
                    { 13, "", 4, true, 200, "NamespaceRoot", 3, null, 6 },
                    { 14, "0", 2, true, 200, "IsTestLibrary", 4, null, 1 }
                });

            migrationBuilder.InsertData(
                table: "ItemTypes",
                columns: new[] { "Id", "Description", "EditorTypeId", "IconName", "IsVisible", "Name", "ParentTypeId", "Rank" },
                values: new object[,]
                {
                    { 162, "Solution Import", 4, "pi pi-sitemap", true, "SolutionImportModel", 160, 162 },
                    { 210, "Library Import", 4, "", true, "LibraryImportModel", 200, 1 },
                    { 300, "Dependency Injection", 4, "pi pi-cog", true, "DependencyInjectionModel", 200, 1 },
                    { 400, "Namespace", 4, "pi pi-globe", true, "NamespaceModel", 200, 400 }
                });

            migrationBuilder.InsertData(
                table: "ItemPropertyDefaults",
                columns: new[] { "Id", "DefaultValue", "EditorTypeId", "IsVisible", "ItemTypeId", "Key", "Rank", "ReferenceItemTypeId", "ValueDataTypeId" },
                values: new object[,]
                {
                    { 9, "", 10, true, 162, "RegisterObject", 2, 200, 3 },
                    { 10, "", 4, true, 162, "ProjectGuid", 3, null, 6 },
                    { 15, "0", 2, true, 210, "IsPackageReference", 1, null, 1 },
                    { 16, "", 4, true, 210, "PackageInclude", 2, null, 6 },
                    { 17, "", 4, true, 210, "PackageVersion", 3, null, 6 },
                    { 18, "0", 2, true, 210, "IsLibraryReference", 4, null, 1 },
                    { 19, "", 10, true, 210, "LibraryInclude", 5, 200, 3 },
                    { 20, "", 4, true, 300, "FilePath", 1, null, 6 },
                    { 21, ".cs", 4, true, 300, "FileExtension", 2, null, 6 },
                    { 22, "", 4, true, 300, "Namespace", 3, null, 6 },
                    { 23, "", 2, true, 300, "HasDbContext", 4, null, 1 },
                    { 24, "", 2, true, 300, "HasMediator", 6, null, 1 },
                    { 32, "", 14, true, 400, "FilePath", 1, null, 6 },
                    { 33, "", 4, true, 400, "Namespace", 2, null, 6 }
                });

            migrationBuilder.InsertData(
                table: "ItemTypes",
                columns: new[] { "Id", "Description", "EditorTypeId", "IconName", "IsVisible", "Name", "ParentTypeId", "Rank" },
                values: new object[,]
                {
                    { 306, "DI - Import", 4, "pi pi-cogs", true, "DiImportModel", 300, 1 },
                    { 310, "DbContext", 4, "pi pi-database", true, "DbContextModel", 300, 2 },
                    { 420, "Interface", 4, "pi pi-plug", true, "InterfaceModel", 400, 420 },
                    { 440, "Record", 4, "", true, "RecordModel", 400, 440 },
                    { 460, "Struct", 4, "", true, "StructModel", 400, 460 },
                    { 500, "Class", 4, "pi pi-cubes", true, "ClassModel", 400, 500 },
                    { 600, "Entity Class", 4, "", true, "EntityClassModel", 400, 600 },
                    { 700, "Handler", 4, "pi pi-shield", true, "HandlerModel", 400, 700 }
                });

            migrationBuilder.InsertData(
                table: "ItemPropertyDefaults",
                columns: new[] { "Id", "DefaultValue", "EditorTypeId", "IsVisible", "ItemTypeId", "Key", "Rank", "ReferenceItemTypeId", "ValueDataTypeId" },
                values: new object[,]
                {
                    { 25, "42", 10, true, 306, "LifetimeScope", 1, 40, 3 },
                    { 26, "", 10, true, 306, "RegisterObject", 2, 500, 3 },
                    { 27, "", 2, true, 306, "RegisterInterface", 3, null, 1 },
                    { 28, "", 4, true, 310, "FilePath", 1, null, 6 },
                    { 29, ".cs", 4, true, 310, "FileExtension", 2, null, 6 },
                    { 30, "", 4, true, 310, "Namespace", 3, null, 6 },
                    { 34, "91", 10, true, 440, "AccessModifier", 11, 90, 3 },
                    { 35, ".cs", 4, true, 440, "FileExtension", 12, null, 6 },
                    { 36, "", 4, true, 440, "FilePath", 13, null, 6 },
                    { 37, "", 4, true, 440, "Namespace", 14, null, 6 },
                    { 38, "", 10, true, 440, "BaseType", 15, 50, 3 },
                    { 39, "", 10, true, 440, "Interface", 16, 500, 3 },
                    { 40, "91", 10, true, 460, "AccessModifier", 11, 90, 3 },
                    { 41, ".cs", 4, true, 460, "FileExtension", 12, null, 6 },
                    { 42, "", 4, true, 460, "FilePath", 13, null, 6 },
                    { 43, "", 4, true, 460, "Namespace", 14, null, 6 },
                    { 44, "", 10, true, 460, "BaseType", 15, 50, 3 },
                    { 45, "", 2, true, 460, "GenerateInterface", 16, null, 1 },
                    { 46, "91", 10, true, 500, "AccessModifier", 11, 90, 3 },
                    { 47, ".cs", 4, true, 500, "FileExtension", 12, null, 6 },
                    { 48, "", 4, true, 500, "FilePath", 13, null, 6 },
                    { 49, "", 4, true, 500, "Namespace", 14, null, 6 },
                    { 50, "", 10, true, 500, "BaseType", 15, 500, 3 },
                    { 51, "", 2, true, 500, "GenerateInterface", 16, null, 1 },
                    { 52, "", 2, true, 500, "RegisterDI", 17, null, 1 },
                    { 53, "0", 2, true, 500, "IsStatic", 18, null, 1 },
                    { 72, ".cs", 4, true, 600, "FileExtension", 12, null, 6 },
                    { 73, "", 4, true, 600, "FilePath", 13, null, 6 },
                    { 74, "", 4, true, 600, "Namespace", 14, null, 6 }
                });

            migrationBuilder.InsertData(
                table: "ItemTypes",
                columns: new[] { "Id", "Description", "EditorTypeId", "IconName", "IsVisible", "Name", "ParentTypeId", "Rank" },
                values: new object[,]
                {
                    { 312, "Db Entity Import", 4, "pi pi-database", true, "DbContextEntityImportModel", 310, 1 },
                    { 422, "Interface Property", 4, "pi pi-plug", true, "InterfacePropertyModel", 420, 422 },
                    { 424, "Interface Method", 4, "pi pi-plug", true, "InterfaceMethodModel", 420, 424 },
                    { 510, "Class Import", 4, "pi pi-cube", true, "ClassImportModel", 500, 510 },
                    { 522, "Class Property", 4, "pi pi-cube", true, "ClassPropertyModel", 500, 522 },
                    { 524, "Class Method", 4, "pi pi-cube", true, "ClassMethodModel", 500, 524 },
                    { 605, "EntityClassImportModel", 4, "", true, "EntityClassImportModel", 600, 605 },
                    { 610, "Entity Property", 4, "", true, "EntityPropertyModel", 600, 610 },
                    { 616, "Inbound Nav Property", 4, "", true, "EntityInboundNavigationModel", 600, 616 },
                    { 620, "Entity Configuration Class", 4, "", true, "EntityConfigurationModel", 600, 620 },
                    { 710, "Handler Response", 4, "pi pi-shield", true, "HandlerResponseModel", 700, 710 },
                    { 720, "Handler Command", 4, "pi pi-shield", true, "HandlerCommandModel", 700, 720 },
                    { 730, "Handler Class", 4, "pi pi-shield", true, "HandlerClassModel", 700, 730 }
                });

            migrationBuilder.InsertData(
                table: "ItemPropertyDefaults",
                columns: new[] { "Id", "DefaultValue", "EditorTypeId", "IsVisible", "ItemTypeId", "Key", "Rank", "ReferenceItemTypeId", "ValueDataTypeId" },
                values: new object[,]
                {
                    { 31, "", 10, true, 312, "RegisterObject", 2, 600, 3 },
                    { 54, "", 10, true, 510, "ImportObject", 2, 500, 3 },
                    { 55, "", 2, true, 510, "UseInterface", 3, null, 1 },
                    { 56, "64", 10, true, 522, "PropertyType", 1, 50, 3 },
                    { 57, "", 10, true, 522, "PropertyClass", 2, 500, 3 },
                    { 58, "1", 2, true, 522, "IsNullable", 3, null, 1 },
                    { 59, "1", 2, true, 522, "HasSetter", 4, null, 1 },
                    { 60, "91", 10, true, 524, "AccessModifier", 11, 90, 3 },
                    { 61, "64", 10, true, 524, "ReturnType", 12, 50, 3 },
                    { 62, "", 10, true, 524, "ReturnClass", 13, 500, 3 },
                    { 63, "0", 2, true, 524, "IsAsync", 14, null, 1 },
                    { 64, "0", 2, true, 524, "IsVirtual", 15, null, 1 },
                    { 65, "0", 2, true, 524, "IsStatic", 16, null, 1 },
                    { 66, "0", 2, true, 524, "IsAbstract", 17, null, 1 },
                    { 67, "0", 2, true, 524, "IsSealed", 18, null, 1 },
                    { 75, "", 10, true, 605, "ImportObject", 2, 500, 3 },
                    { 76, "", 2, true, 605, "UseInterface", 3, null, 1 },
                    { 77, "64", 10, true, 610, "PropertyType", 1, 50, 3 },
                    { 78, "0", 2, true, 610, "IsNullable", 3, null, 1 },
                    { 79, "1", 2, true, 610, "HasSetter", 4, null, 1 },
                    { 80, "0", 2, true, 610, "HasNavigation", 5, null, 1 },
                    { 81, "0", 2, true, 610, "IsPrimaryKey", 6, null, 1 },
                    { 82, "-1", 3, true, 610, "MaxSize", 7, null, 3 },
                    { 86, "", 10, true, 616, "PropertyClass", 2, 600, 3 },
                    { 87, "", 10, true, 616, "ForeignKey", 3, 610, 3 },
                    { 88, "4", 10, true, 616, "HasNavigation", 4, 2, 3 },
                    { 89, "1", 2, true, 616, "IsNullable", 5, null, 1 },
                    { 90, ".cs", 4, true, 620, "FileExtension", 12, null, 6 },
                    { 91, "", 4, true, 620, "FilePath", 13, null, 6 },
                    { 92, "", 4, true, 620, "Namespace", 14, null, 6 }
                });

            migrationBuilder.InsertData(
                table: "ItemTypes",
                columns: new[] { "Id", "Description", "EditorTypeId", "IconName", "IsVisible", "Name", "ParentTypeId", "Rank" },
                values: new object[,]
                {
                    { 426, "Interface Method Parameter", 4, "pi pi-plug", true, "InterfaceMethodParameterModel", 424, 426 },
                    { 526, "Class Method Parameter", 4, "pi pi-cube", true, "ClassMethodParameterModel", 524, 526 },
                    { 614, "Entity Nav Property", 4, "", true, "EntityNavigationModel", 610, 614 },
                    { 622, "Entity Property Configuration", 4, "", true, "EntityPropertyConfigurationModel", 620, 622 },
                    { 624, "Entity Nav Configuration", 4, "", true, "EntityNavigationConfigurationModel", 620, 624 },
                    { 626, "EntityInboundNavConfigurationModel", -1, "", true, "EntityInboundNavConfigurationModel", 620, 0 },
                    { 732, "Handler Property", 4, "pi pi-shield", true, "HandlerPropertyModel", 730, 732 },
                    { 734, "Handler Method", 4, "pi pi-shield", true, "HandlerMethodModel", 730, 734 }
                });

            migrationBuilder.InsertData(
                table: "ItemPropertyDefaults",
                columns: new[] { "Id", "DefaultValue", "EditorTypeId", "IsVisible", "ItemTypeId", "Key", "Rank", "ReferenceItemTypeId", "ValueDataTypeId" },
                values: new object[,]
                {
                    { 68, "64", 10, true, 526, "ParamType", 1, 50, 3 },
                    { 69, "", 10, true, 526, "ParamClass", 2, 500, 3 },
                    { 70, "1", 2, true, 526, "IsNullable", 3, null, 1 },
                    { 71, "0", 2, true, 526, "UseThis", 4, null, 1 },
                    { 83, "", 10, true, 614, "PropertyClass", 2, 600, 3 },
                    { 84, "7", 10, true, 614, "HasNavigation", 3, 6, 3 },
                    { 85, "1", 2, true, 614, "IsNullable", 3, null, 1 },
                    { 93, "", 10, true, 622, "ParamClass", 2, 610, 3 },
                    { 94, "1", 2, true, 622, "IsNullable", 3, null, 1 },
                    { 95, "", 10, true, 624, "ParamClass", 2, 614, 3 },
                    { 96, "1", 2, true, 624, "IsNullable", 3, null, 1 },
                    { 97, "", 10, true, 626, "ParamClass", 2, 616, 3 },
                    { 98, "1", 2, true, 626, "IsNullable", 3, null, 1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppSettings_Key",
                table: "AppSettings",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DataTypes_Name",
                table: "DataTypes",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EditorTypes_Name",
                table: "EditorTypes",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ItemProperties_EditorTypeId",
                table: "ItemProperties",
                column: "EditorTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemProperties_ItemId",
                table: "ItemProperties",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemProperties_ItemId_Name",
                table: "ItemProperties",
                columns: new[] { "ItemId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ItemProperties_ItemPropertyDefaultId",
                table: "ItemProperties",
                column: "ItemPropertyDefaultId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemProperties_Name",
                table: "ItemProperties",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_ItemProperties_ReferenceItemTypeId",
                table: "ItemProperties",
                column: "ReferenceItemTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemProperties_ValueDataTypeId",
                table: "ItemProperties",
                column: "ValueDataTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemPropertyDefaults_EditorTypeId",
                table: "ItemPropertyDefaults",
                column: "EditorTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemPropertyDefaults_ItemTypeId_Key",
                table: "ItemPropertyDefaults",
                columns: new[] { "ItemTypeId", "Key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ItemPropertyDefaults_ReferenceItemTypeId",
                table: "ItemPropertyDefaults",
                column: "ReferenceItemTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemPropertyDefaults_ValueDataTypeId",
                table: "ItemPropertyDefaults",
                column: "ValueDataTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_ItemTypeId",
                table: "Items",
                column: "ItemTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemTypes_EditorTypeId",
                table: "ItemTypes",
                column: "EditorTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemTypes_Name",
                table: "ItemTypes",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ItemTypes_ParentTypeId",
                table: "ItemTypes",
                column: "ParentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemTypes_ParentTypeId_Rank",
                table: "ItemTypes",
                columns: new[] { "ParentTypeId", "Rank" });

            migrationBuilder.CreateIndex(
                name: "IX_Relations_ItemId_RelationTypeId_RelatedItemId",
                table: "Relations",
                columns: new[] { "ItemId", "RelationTypeId", "RelatedItemId" },
                unique: true,
                filter: "[RelatedItemId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Relations_RelatedItemId",
                table: "Relations",
                column: "RelatedItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Relations_RelationTypeId",
                table: "Relations",
                column: "RelationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_RelationTypes_Name",
                table: "RelationTypes",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppSettings");

            migrationBuilder.DropTable(
                name: "ItemProperties");

            migrationBuilder.DropTable(
                name: "Relations");

            migrationBuilder.DropTable(
                name: "ItemPropertyDefaults");

            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "RelationTypes");

            migrationBuilder.DropTable(
                name: "DataTypes");

            migrationBuilder.DropTable(
                name: "ItemTypes");

            migrationBuilder.DropTable(
                name: "EditorTypes");
        }
    }
}
