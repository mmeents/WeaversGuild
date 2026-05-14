using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Weavers.Core.Migrations
{
    /// <inheritdoc />
    public partial class InitialVersion117 : Migration
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
                    WrittenAt = table.Column<DateTime>(type: "datetime2", nullable: true),
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
                name: "Builds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StartedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    BuildOutput = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompilerOutput = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LibraryItemId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Builds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Builds_Items_LibraryItemId",
                        column: x => x.LibraryItemId,
                        principalTable: "Items",
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

            migrationBuilder.CreateTable(
                name: "BuildFiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BuildId = table.Column<int>(type: "int", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(500)", nullable: false),
                    WasWritten = table.Column<bool>(type: "bit", nullable: false),
                    WasDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuildFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BuildFiles_Builds_BuildId",
                        column: x => x.BuildId,
                        principalTable: "Builds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BuildFiles_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
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
                    { 5, "Entity Nav Types", 10, "", true, "NavigationTypes", null, 1 },
                    { 10, "Owner Type of SQL Types", 10, "", true, "SqlTypes", null, 1 },
                    { 18, "sql float type", -1, "", true, "SqlFloatType", 18, 0 },
                    { 31, "Test Method Attributes", 10, "", true, "TestMethodTypes", null, 1 },
                    { 40, "Owner Type of C# Lifetimes", 10, "", true, "CSharpLifetimes", null, 1 },
                    { 50, "Owner Type of C# Types", 10, "", true, "CSharpTypes", null, 1 },
                    { 80, "Entity Delete Behaviors", -1, "", true, "EntityDeleteBehaviors", null, 1 },
                    { 90, "Accessibility Lookups", 10, "", true, "AccessibilityLookups", null, 1 },
                    { 100, "Project Folder", 4, "pi pi-folder", true, "ProjectFolderModel", null, 100 }
                });

            migrationBuilder.InsertData(
                table: "ItemPropertyDefaults",
                columns: new[] { "Id", "DefaultValue", "EditorTypeId", "IsVisible", "ItemTypeId", "Key", "Rank", "ReferenceItemTypeId", "ValueDataTypeId" },
                values: new object[,]
                {
                    { 1, "", 13, true, 100, "RootFolder", 1, null, 6 },
                    { 2, "", 4, true, 100, "RepoUrl", 2, null, 6 }
                });

            migrationBuilder.InsertData(
                table: "ItemTypes",
                columns: new[] { "Id", "Description", "EditorTypeId", "IconName", "IsVisible", "Name", "ParentTypeId", "Rank" },
                values: new object[,]
                {
                    { 6, "Has One to One", 2, "", true, "NavHasOneToOne", 5, 1 },
                    { 7, "Has One to Many", 2, "", true, "NavHasOneToMany", 5, 2 },
                    { 8, "Has Many to One", 2, "", true, "NavHasManyToOne", 5, 3 },
                    { 9, "Has Many to Many", 2, "", true, "NavHasManyToMany", 5, 4 },
                    { 11, "sql bit type", 2, "", true, "SqlBitType", 10, 2 },
                    { 12, "sql smallint type", 3, "", true, "SqlSmallIntType", 10, 3 },
                    { 13, "sql int type", 3, "", true, "SqlIntType", 10, 4 },
                    { 14, "sql bigint type", 3, "", true, "SqlBigIntType", 10, 5 },
                    { 15, "sql uniqueidentifier type", 4, "", true, "SqlGuidType", 10, 6 },
                    { 16, "sql varchar type", 4, "", true, "SqlVarcharType", 10, 7 },
                    { 17, "sql nvarchar type", 4, "", true, "SqlNVarcharType", 10, 8 },
                    { 19, "sql decimal type", 8, "", true, "SqlDecimalType", 10, 9 },
                    { 20, "sql datetime type", 6, "", true, "SqlDateTimeType", 10, 10 },
                    { 21, "sql datetime2 type", 6, "", true, "SqlDateTime2Type", 10, 11 },
                    { 22, "sql date type", 6, "", true, "SqlDateType", 10, 12 },
                    { 23, "sql time type", 7, "", true, "SqlTimeType", 10, 13 },
                    { 24, "sql datetimeoffset type", 4, "", true, "SqlDateTimeOffsetType", 10, 14 },
                    { 25, "sql binary type", -1, "", true, "SqlBinaryType", 10, 15 },
                    { 32, "Not A Test", 2, "", true, "NoTestAttribute", 31, 1 },
                    { 33, "Ignore Test", 2, "", true, "TestIgnoreAttribute", 31, 2 },
                    { 34, "TestMethod", 2, "", true, "TestMethodAttribute", 31, 3 },
                    { 35, "TestInitialize", 2, "", true, "TestInitialize", 31, 4 },
                    { 36, "TestCleanup", 2, "", true, "TestCleanup", 31, 5 },
                    { 37, "TestClassInitialize", 2, "", true, "TestClassInitialize", 31, 6 },
                    { 38, "TestClassCleanup", 2, "", true, "TestClassCleanup", 31, 7 },
                    { 41, "C# Singleton Lifetime", -1, "", true, "CSLifetimeSingleton", 40, 1 },
                    { 42, "C# Scoped Lifetime", -1, "", true, "CSLifetimeScoped", 40, 2 },
                    { 43, "C# Transient Lifetime", -1, "", true, "CSLifetimeTransient", 40, 3 },
                    { 51, "C# Class Type", 11, "", true, "CSharpClassType", 50, 2 },
                    { 52, "C# Record Type", 11, "", true, "CSharpRecordType", 50, 3 },
                    { 53, "C# Struct Type", 11, "", true, "CSharpStructType", 50, 4 },
                    { 54, "C# String Type", 4, "", true, "CSharpStringType", 50, 5 },
                    { 55, "C# Bool Type", 2, "", true, "CSharpBoolType", 50, 6 },
                    { 56, "C# Char Type", 4, "", true, "CSharpCharType", 50, 7 },
                    { 57, "C# Int Type", 3, "", true, "CSharpIntType", 50, 8 },
                    { 58, "C# Long Type", 3, "", true, "CSharpLongType", 50, 9 },
                    { 59, "C# Short Type", 3, "", true, "CSharpShortType", 50, 10 },
                    { 60, "C# Decimal Type", 8, "", true, "CSharpDecimalType", 50, 11 },
                    { 61, "C# Double Type", 8, "", true, "CSharpDoubleType", 50, 12 },
                    { 62, "C# Float Type", 8, "", true, "CSharpFloatType", 50, 13 },
                    { 63, "C# Byte Type", 3, "", true, "CSharpByteType", 50, 14 },
                    { 64, "C# DateTime Type", 6, "", true, "CSharpDateTimeType", 50, 15 },
                    { 65, "C# Date Type", 6, "", true, "CSharpDateType", 50, 17 },
                    { 66, "C# Time Type", 7, "", true, "CSharpTimeType", 50, 18 },
                    { 67, "C# DateTimeOffset Type", 4, "", true, "CSharpDateTimeOffsetType", 50, 19 },
                    { 68, "C# Byte Array Type", -1, "", true, "CSharpByteArrayType", 50, 20 },
                    { 69, "C# Guid Type", 4, "", true, "CSharpGuidType", 50, 21 },
                    { 81, "ClientSetNull", -1, "", true, "EntityDeleteClientSetNull", 80, 1 },
                    { 82, "Restrict", -1, "", true, "EntityDeleteRestrict", 80, 2 },
                    { 83, "SetNull", -1, "", true, "EntityDeleteSetNull", 80, 3 },
                    { 84, "Cascade", -1, "", true, "EntityDeleteCascade", 80, 4 },
                    { 85, "ClientCascade", -1, "", true, "EntityDeleteClientCascade", 80, 5 },
                    { 86, "NoAction", -1, "", true, "EntityDeleteNoAction", 80, 6 },
                    { 87, "ClientNoAction", -1, "", true, "EntityDeleteClientNoAction", 80, 7 },
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
                    { 7, ".sln", 1, true, 160, "FileExtension", 2, null, 6 },
                    { 8, "", 4, true, 160, "SlnGuid", 3, null, 6 },
                    { 11, "", 4, true, 200, "FilePath", 1, null, 6 },
                    { 12, "", 4, true, 200, "NamespaceRoot", 2, null, 6 },
                    { 13, "net9.0", 4, true, 200, "TargetFramework", 3, null, 6 },
                    { 14, "1", 2, true, 200, "IsNullable", 4, null, 1 },
                    { 15, "1", 2, true, 200, "ImplicitUsing", 5, null, 1 },
                    { 16, "1.0.0", 4, true, 200, "Version", 6, null, 6 },
                    { 17, "1.0.0.0", 4, true, 200, "AssemblyVersion", 7, null, 6 },
                    { 18, "1.0.0.0", 4, true, 200, "FileVersion", 8, null, 6 },
                    { 19, "0", 2, true, 200, "IsTestLibrary", 9, null, 1 },
                    { 20, ".csproj", 1, true, 200, "FileExtension", 10, null, 6 }
                });

            migrationBuilder.InsertData(
                table: "ItemTypes",
                columns: new[] { "Id", "Description", "EditorTypeId", "IconName", "IsVisible", "Name", "ParentTypeId", "Rank" },
                values: new object[,]
                {
                    { 162, "Solution Import", 4, "pi pi-sitemap", true, "SolutionImportModel", 160, 162 },
                    { 210, "Package Ref", 4, "", true, "LibPackageRefModel", 200, 1 },
                    { 220, "Library Ref", 4, "", true, "LibLibraryRefModel", 200, 2 },
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
                    { 21, "", 4, true, 210, "PackageInclude", 2, null, 6 },
                    { 22, "", 4, true, 210, "PackageVersion", 3, null, 6 },
                    { 23, "", 4, true, 210, "PrivateAssets", 4, null, 6 },
                    { 24, "", 4, true, 210, "IncludeAssets", 5, null, 6 },
                    { 25, "", 10, true, 220, "LibraryInclude", 1, 200, 3 },
                    { 26, "", 4, true, 300, "FilePath", 1, null, 6 },
                    { 27, ".cs", 1, true, 300, "FileExtension", 2, null, 6 },
                    { 28, "", 4, true, 300, "Namespace", 3, null, 6 },
                    { 29, "", 2, true, 300, "HasDbContext", 4, null, 1 },
                    { 30, "", 2, true, 300, "HasMediator", 6, null, 1 },
                    { 38, "", 14, true, 400, "FilePath", 1, null, 6 },
                    { 39, "", 4, true, 400, "Namespace", 2, null, 6 }
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
                    { 31, "42", 10, true, 306, "LifetimeScope", 1, 40, 3 },
                    { 32, "", 10, true, 306, "RegisterObject", 2, 500, 3 },
                    { 33, "", 2, true, 306, "RegisterInterface", 3, null, 1 },
                    { 34, "", 4, true, 310, "FilePath", 1, null, 6 },
                    { 35, ".cs", 4, true, 310, "FileExtension", 2, null, 6 },
                    { 36, "", 4, true, 310, "Namespace", 3, null, 6 },
                    { 40, "91", 10, true, 440, "AccessModifier", 11, 90, 3 },
                    { 41, ".cs", 4, true, 440, "FileExtension", 12, null, 6 },
                    { 42, "", 4, true, 440, "FilePath", 13, null, 6 },
                    { 43, "", 4, true, 440, "Namespace", 14, null, 6 },
                    { 44, "", 10, true, 440, "BaseType", 15, 50, 3 },
                    { 45, "", 10, true, 440, "Interface", 16, 500, 3 },
                    { 46, "91", 10, true, 460, "AccessModifier", 11, 90, 3 },
                    { 47, ".cs", 4, true, 460, "FileExtension", 12, null, 6 },
                    { 48, "", 4, true, 460, "FilePath", 13, null, 6 },
                    { 49, "", 4, true, 460, "Namespace", 14, null, 6 },
                    { 50, "", 10, true, 460, "BaseType", 15, 50, 3 },
                    { 51, "", 2, true, 460, "GenerateInterface", 16, null, 1 },
                    { 52, "0", 2, true, 500, "TestClass", 11, null, 1 },
                    { 53, "91", 10, true, 500, "AccessModifier", 11, 90, 3 },
                    { 54, ".cs", 1, true, 500, "FileExtension", 12, null, 6 },
                    { 55, "", 4, true, 500, "FilePath", 13, null, 6 },
                    { 56, "", 4, true, 500, "Namespace", 14, null, 6 },
                    { 57, "", 10, true, 500, "BaseType", 15, 500, 3 },
                    { 58, "", 2, true, 500, "GenerateInterface", 16, null, 1 },
                    { 59, "", 2, true, 500, "RegisterDI", 17, null, 1 },
                    { 60, "0", 2, true, 500, "IsStatic", 18, null, 1 },
                    { 80, ".cs", 1, true, 600, "FileExtension", 12, null, 6 },
                    { 81, "", 4, true, 600, "FilePath", 13, null, 6 },
                    { 82, "", 4, true, 600, "Namespace", 14, null, 6 },
                    { 83, "dbo", 4, true, 600, "DbSchema", 15, null, 6 },
                    { 84, "", 4, true, 600, "DbTableName", 16, null, 6 }
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
                    { 37, "", 10, true, 312, "RegisterObject", 2, 600, 3 },
                    { 61, "", 10, true, 510, "ImportObject", 2, 500, 3 },
                    { 62, "", 2, true, 510, "UseInterface", 3, null, 1 },
                    { 63, "57", 10, true, 522, "PropertyType", 1, 50, 3 },
                    { 64, "", 10, true, 522, "PropertyClass", 2, 500, 3 },
                    { 65, "1", 2, true, 522, "IsNullable", 3, null, 1 },
                    { 66, "1", 2, true, 522, "HasSetter", 4, null, 1 },
                    { 67, "32", 10, true, 524, "TestMethod", 10, 31, 3 },
                    { 68, "91", 10, true, 524, "AccessModifier", 11, 90, 3 },
                    { 69, "57", 10, true, 524, "ReturnType", 12, 50, 3 },
                    { 70, "", 10, true, 524, "ReturnClass", 13, 500, 3 },
                    { 71, "0", 2, true, 524, "IsAsync", 14, null, 1 },
                    { 72, "0", 2, true, 524, "IsVirtual", 15, null, 1 },
                    { 73, "0", 2, true, 524, "IsStatic", 16, null, 1 },
                    { 74, "0", 2, true, 524, "IsAbstract", 17, null, 1 },
                    { 75, "0", 2, true, 524, "IsSealed", 18, null, 1 },
                    { 85, "", 10, true, 605, "ImportObject", 2, 500, 3 },
                    { 86, "", 2, true, 605, "UseInterface", 3, null, 1 },
                    { 87, "64", 10, true, 610, "PropertyType", 1, 50, 3 },
                    { 88, "0", 2, true, 610, "IsNullable", 3, null, 1 },
                    { 89, "1", 2, true, 610, "HasSetter", 4, null, 1 },
                    { 90, "0", 2, true, 610, "HasNavigation", 5, null, 1 },
                    { 91, "0", 2, true, 610, "IsPrimaryKey", 6, null, 1 },
                    { 92, "-1", 3, true, 610, "MaxSize", 7, null, 3 },
                    { 97, "", 10, true, 616, "PropertyClass", 2, 600, 3 },
                    { 98, "", 10, true, 616, "ForeignKey", 3, 610, 3 },
                    { 99, "6", 10, true, 616, "HasNavigation", 4, 5, 3 },
                    { 100, "1", 2, true, 616, "IsNullable", 5, null, 1 },
                    { 101, "", 4, true, 616, "InverseNav", 6, null, 6 }
                });

            migrationBuilder.InsertData(
                table: "ItemTypes",
                columns: new[] { "Id", "Description", "EditorTypeId", "IconName", "IsVisible", "Name", "ParentTypeId", "Rank" },
                values: new object[,]
                {
                    { 426, "Interface Method Parameter", 4, "pi pi-plug", true, "InterfaceMethodParameterModel", 424, 426 },
                    { 526, "Class Method Parameter", 4, "pi pi-cube", true, "ClassMethodParameterModel", 524, 526 },
                    { 614, "Entity Nav Property", 4, "", true, "EntityNavigationModel", 610, 614 },
                    { 732, "Handler Property", 4, "pi pi-shield", true, "HandlerPropertyModel", 730, 732 },
                    { 734, "Handler Method", 4, "pi pi-shield", true, "HandlerMethodModel", 730, 734 }
                });

            migrationBuilder.InsertData(
                table: "ItemPropertyDefaults",
                columns: new[] { "Id", "DefaultValue", "EditorTypeId", "IsVisible", "ItemTypeId", "Key", "Rank", "ReferenceItemTypeId", "ValueDataTypeId" },
                values: new object[,]
                {
                    { 76, "57", 10, true, 526, "ParamType", 1, 50, 3 },
                    { 77, "", 10, true, 526, "ParamClass", 2, 500, 3 },
                    { 78, "1", 2, true, 526, "IsNullable", 3, null, 1 },
                    { 79, "0", 2, true, 526, "UseThis", 4, null, 1 },
                    { 93, "", 10, true, 614, "PropertyClass", 2, 600, 3 },
                    { 94, "7", 10, true, 614, "HasNavigation", 3, 5, 3 },
                    { 95, "84", 10, true, 614, "DeleteBehavior", 4, 80, 3 },
                    { 96, "1", 2, true, 614, "IsNullable", 5, null, 1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppSettings_Key",
                table: "AppSettings",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BuildFiles_BuildId",
                table: "BuildFiles",
                column: "BuildId");

            migrationBuilder.CreateIndex(
                name: "IX_BuildFiles_ItemId",
                table: "BuildFiles",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Builds_LibraryItemId",
                table: "Builds",
                column: "LibraryItemId");

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
                name: "BuildFiles");

            migrationBuilder.DropTable(
                name: "ItemProperties");

            migrationBuilder.DropTable(
                name: "Relations");

            migrationBuilder.DropTable(
                name: "Builds");

            migrationBuilder.DropTable(
                name: "ItemPropertyDefaults");

            migrationBuilder.DropTable(
                name: "RelationTypes");

            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "DataTypes");

            migrationBuilder.DropTable(
                name: "ItemTypes");

            migrationBuilder.DropTable(
                name: "EditorTypes");
        }
    }
}
