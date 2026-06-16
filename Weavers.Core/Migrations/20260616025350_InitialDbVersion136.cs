using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Weavers.Core.Migrations
{
    /// <inheritdoc />
    public partial class InitialDbVersion136 : Migration
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
                name: "MediatorLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SessionId = table.Column<int>(type: "int", nullable: false),
                    CalledAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OpName = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    InputJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OutputJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DurationMs = table.Column<int>(type: "int", nullable: false),
                    Success = table.Column<bool>(type: "bit", nullable: false),
                    ErrorMsg = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MediatorLogs", x => x.Id);
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
                    { 51, "Reference to Items graph see ReferenceItemTypeId for Item Properties", "Reference" },
                    { 54, "ASCII string", "StrAscii" },
                    { 55, "Boolean (true/false)", "Boolean" },
                    { 56, "Unknown data type", "Char" },
                    { 57, "32-bit integer", "Int" },
                    { 58, "64-bit integer", "Long" },
                    { 59, "16-bit integer", "Short" },
                    { 60, "128-bit decimal number", "Decimal" },
                    { 61, "64-bit floating point number", "Double" },
                    { 62, "32-bit floating point number", "Float" },
                    { 63, "Unknown data type", "Byte" },
                    { 64, "Date and time (1753-01-01 to 9999-12-31)", "DateTime" },
                    { 65, "Date (year, month, day)", "Date" },
                    { 66, "Time (hour, minute, second)", "Time" },
                    { 67, "Date and time with time zone awareness", "DateTimeOffset" },
                    { 68, "Binary data (byte array)", "Binary" },
                    { 69, "Globally Unique Identifier", "Guid" }
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
                    { 15, "URL editor", "pi-link", true, "URL" },
                    { 16, "Reference editor", "pi-book", true, "Reference" },
                    { 17, "Template editor", "pi-file", true, "Template" },
                    { 18, "Command picker editor", "pi-cog", true, "Command Picker" }
                });

            migrationBuilder.InsertData(
                table: "RelationTypes",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 10, "Item belongs to a type category", "TypeOf" },
                    { 20, "Structural parent contains child model", "Contains" },
                    { 30, "Item has associated documentation", "HasDocs" }
                });

            migrationBuilder.InsertData(
                table: "ItemTypes",
                columns: new[] { "Id", "Description", "EditorTypeId", "IconName", "IsVisible", "Name", "ParentTypeId", "Rank" },
                values: new object[,]
                {
                    { 1, "Not Set", -1, "", true, "NotSet", null, 0 },
                    { 2, "Active Item Types", -1, "", true, "ActiveItemTypes", null, 0 },
                    { 5, "Entity Nav Types", 10, "", true, "NavigationTypes", null, 1 },
                    { 10, "Owner Type of SQL Types", 10, "", true, "SqlTypes", null, 1 },
                    { 18, "sql float type", -1, "", true, "SqlFloatType", 18, 0 },
                    { 31, "Test Method Attributes", 10, "", true, "TestMethodTypes", null, 1 },
                    { 40, "Owner Type of C# Lifetimes", 10, "", true, "CSharpLifetimes", null, 1 },
                    { 50, "Owner Type of C# Types", 10, "", true, "CSharpTypes", null, 1 },
                    { 80, "Entity Delete Behaviors", -1, "", true, "EntityDeleteBehaviors", null, 1 },
                    { 90, "Accessibility Lookups", 10, "", true, "AccessibilityLookups", null, 1 },
                    { 100, "Review State", 10, "", true, "RatingStatus", null, 1 },
                    { 110, "Ratings", 10, "", true, "Ratings", null, 1 },
                    { 115, "Floor Status", 10, "", true, "FloorStatus", null, 1 },
                    { 120, "Loom Mcp Commands", 10, "", true, "LoomMcpCommands", null, 1 },
                    { 220, "Todo Statuses", 10, "", true, "TodoStatuses", null, 1 },
                    { 230, "Run Status", 10, "", true, "RunStatus", null, 1 },
                    { 250, "Desk Pre-Assert Check Types", 10, "", true, "DeskPreAssertCheckTypes", null, 1 },
                    { 1000, "Organization", -1, "", true, "OrganizationModel", null, 1000 },
                    { 1107, "DocRating", -1, "", true, "DocRating", 1107, 0 },
                    { 1311, "DbContext Documentation", 4, "", true, "DbContextDocs", 1311, 1 }
                });

            migrationBuilder.InsertData(
                table: "ItemPropertyDefaults",
                columns: new[] { "Id", "DefaultValue", "EditorTypeId", "IsVisible", "ItemTypeId", "Key", "Rank", "ReferenceItemTypeId", "ValueDataTypeId" },
                values: new object[,]
                {
                    { 1, "", 12, true, 1000, "Charter", 3, null, 54 },
                    { 2, "", 13, true, 1000, "RootFolder", 2, null, 54 },
                    { 3, "3", 3, true, 1000, "KeepDays", 1, null, 57 },
                    { 55, "112", 10, true, 1107, "Votes", 1, 110, 57 },
                    { 104, "104", 10, true, 1311, "Results", 1, 100, 57 }
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
                    { 101, "Unanimous Yes", 4, "", true, "UnanimousYes", 100, 1 },
                    { 102, "Majority Yes", 4, "", true, "MajorityYes", 100, 2 },
                    { 103, "Majority No", 4, "", true, "MajorityNo", 100, 3 },
                    { 104, "Tie", 4, "", true, "Tie", 100, 4 },
                    { 111, "Yes", 4, "", true, "RatingYes", 100, 1 },
                    { 112, "No", 4, "", true, "RatingNo", 100, 2 },
                    { 116, "Disabled", -1, "", true, "FloorDisabled", 115, 1 },
                    { 117, "Operational", -1, "", true, "FloorOperational", 115, 2 },
                    { 118, "Stopping", -1, "", true, "FloorStopping", 115, 3 },
                    { 122, "Help Command", 4, "", true, "CmdHelp", 120, 1 },
                    { 124, "List Projects Command", 4, "", true, "CmdListProjects", 120, 2 },
                    { 126, "Search Command", 4, "", true, "CmdSearch", 120, 3 },
                    { 128, "Get Summary By Id Command", 4, "", true, "CmdGetSummaryById", 120, 4 },
                    { 130, "Get Type Details Command", 4, "", true, "CmdGetTypeDetails", 120, 5 },
                    { 132, "Update Item Name Command", 4, "", true, "CmdUpdateItemName", 120, 6 },
                    { 134, "Update Item Content Command", 4, "", true, "CmdUpdateItemContent", 120, 7 },
                    { 136, "Update Item Property Command", 4, "", true, "CmdUpdateItemProperty", 120, 8 },
                    { 138, "Complete Todo Command", -1, "", true, "CmdCompleteTodo", 120, 9 },
                    { 139, "Reject Todo Command", -1, "", true, "CmdRejectTodo", 120, 10 },
                    { 140, "Review Pass Command", -1, "", true, "CmdReviewPass", 120, 11 },
                    { 141, "Review Fail Command", -1, "", true, "CmdReviewFail", 120, 12 },
                    { 142, "Add Org Desk Role Command", 4, "", true, "CmdAddOrgDeskRole", 120, 13 },
                    { 143, "Add Org Desk Command", -1, "", true, "CmdAddOrgDesk", 120, 14 },
                    { 144, "Add Desk Todo Command", -1, "", true, "CmdAddDeskTodo", 120, 15 },
                    { 145, "Add Digital Operator Command", -1, "", true, "CmdAddDigitalOperatior", 120, 16 },
                    { 146, "Add Org Folder Command", -1, "", true, "CmdAddOrgFolder", 120, 17 },
                    { 148, "Add Org File Command", -1, "", true, "CmdAddOrgFile", 120, 18 },
                    { 150, "Add Project Root Command", 4, "", true, "CmdAddProjectRoot", 120, 19 },
                    { 152, "Add Sub Folder Command", 4, "", true, "CmdAddSubFolder", 120, 20 },
                    { 154, "Add Solution Command", 4, "", true, "CmdAddSolution", 120, 21 },
                    { 156, "Add Solution Import Command", 4, "", true, "CmdAddSolutionImport", 120, 22 },
                    { 158, "Add Md File Command", 4, "", true, "CmdAddMdFile", 120, 23 },
                    { 160, "Add Html File Command", 4, "", true, "CmdAddHtmlFile", 120, 24 },
                    { 162, "Add Config File Command", 4, "", true, "CmdAddConfigFile", 120, 25 },
                    { 164, "Add Library Command", 4, "", true, "CmdAddLibrary", 120, 26 },
                    { 166, "Add Namespace Command", 4, "", true, "CmdAddNamespace", 120, 27 },
                    { 168, "Add Class Command", 4, "", true, "CmdAddClass", 120, 28 },
                    { 170, "Add Class Import Command", 4, "", true, "CmdAddClassImport", 120, 29 },
                    { 172, "Add Class Property Command", 4, "", true, "CmdAddClassProperty", 120, 30 },
                    { 174, "Add Class Method Command", 4, "", true, "CmdAddClassMethod", 120, 31 },
                    { 176, "Add Class Method Param Command", 4, "", true, "CmdAddClassMethodParam", 120, 32 },
                    { 178, "Add Entity Class Command", 4, "", true, "CmdAddEntityClass", 120, 33 },
                    { 180, "Add Entity Class Import Command", 4, "", true, "CmdAddEntityClassImport", 120, 34 },
                    { 182, "Add Entity Property Command", 4, "", true, "CmdAddEntityProperty", 120, 35 },
                    { 221, "Not Started", -1, "", true, "TodoNotStarted", 220, 1 },
                    { 222, "In Progress", -1, "", true, "TodoInProgress", 220, 2 },
                    { 223, "Complete Forward", -1, "", true, "TodoCompleteForward", 220, 3 },
                    { 224, "Aborted Push Back", -1, "", true, "TodoAbortedPushBack", 220, 4 },
                    { 225, "Failed Forward", -1, "", true, "TodoFailedForward", 220, 5 },
                    { 231, "In Progress", -1, "", true, "RunInProgress", 230, 1 },
                    { 232, "Completed", -1, "", true, "RunCompleted", 230, 2 },
                    { 233, "Failed", -1, "", true, "RunFailed", 230, 3 },
                    { 234, "Ran Without Close", -1, "", true, "RanWithoutClose", 230, 4 },
                    { 251, "Assert Item Exists", -1, "", true, "AssertItemExists", 250, 1 },
                    { 252, "Assert Item Is Type", -1, "", true, "AssertItemIsType", 250, 2 },
                    { 1010, "Harness App", -1, "", true, "HarnessAppModel", 1000, 1010 },
                    { 1020, "Harness Mcp", -1, "", true, "HarnessMcpModel", 1000, 1020 },
                    { 1026, "OrgDeskRolesModel", -1, "", true, "OrgDeskRolesModel", 1000, 0 },
                    { 1030, "Digital Operator Pool", -1, "", true, "DigitalOperatorPoolModel", 1000, 1030 },
                    { 1040, "Org Chart", 4, "", true, "OrgChartModel", 1000, 1040 },
                    { 1060, "Org Doc Folder", 4, "", true, "OrgDocFolderModel", 1000, 1060 },
                    { 1100, "Project Folder", 4, "pi pi-folder", true, "ProjectFolderModel", 1000, 1100 }
                });

            migrationBuilder.InsertData(
                table: "ItemPropertyDefaults",
                columns: new[] { "Id", "DefaultValue", "EditorTypeId", "IsVisible", "ItemTypeId", "Key", "Rank", "ReferenceItemTypeId", "ValueDataTypeId" },
                values: new object[,]
                {
                    { 4, "", 4, true, 1010, "MachineName", 5, null, 54 },
                    { 5, "", 4, true, 1010, "UserName", 4, null, 54 },
                    { 6, "0", 2, true, 1010, "HasLmStudio", 3, null, 55 },
                    { 12, "", 4, true, 1020, "MachineName", 2, null, 54 },
                    { 15, "", 14, true, 1026, "RelativeFolder", 1, null, 54 },
                    { 19, "", 14, true, 1030, "RelativeFolder", 1, null, 54 },
                    { 23, "116", 10, true, 1040, "FloorStatus", 9, 115, 57 },
                    { 24, "", 14, true, 1040, "RelativeFolder", 1, null, 54 },
                    { 50, "", 14, true, 1060, "RelativeFolder", 1, null, 54 },
                    { 52, "", 13, true, 1100, "RelativeFolder", 2, null, 54 },
                    { 53, "", 4, true, 1100, "RepoUrl", 1, null, 54 }
                });

            migrationBuilder.InsertData(
                table: "ItemTypes",
                columns: new[] { "Id", "Description", "EditorTypeId", "IconName", "IsVisible", "Name", "ParentTypeId", "Rank" },
                values: new object[,]
                {
                    { 1012, "Harness App Session", -1, "", true, "HarnessAppSessionModel", 1010, 1012 },
                    { 1014, "Lm Studio Gateway", -1, "", true, "PresenceLmStudioGatewayModel", 1010, 1014 },
                    { 1022, "Harness Mcp Session", -1, "", true, "HarnessMcpSessionModel", 1020, 1022 },
                    { 1028, "DeskRoleModel", -1, "", true, "DeskRoleModel", 1026, 0 },
                    { 1035, "Digital Operator", 4, "", true, "DigitalOperatorModel", 1030, 1035 },
                    { 1043, "Default Log Desk", 4, "", true, "DeskLogModel", 1040, 1043 },
                    { 1045, "Desk", 4, "", true, "DeskModel", 1040, 1045 },
                    { 1065, "Org Doc", 4, "", true, "OrgDocModel", 1060, 1065 },
                    { 1101, "Project Documentation", 4, "", true, "ProjectDocs", 1100, 1101 },
                    { 1110, "Relative Folder", 4, "pi pi-folder", true, "RelativeFolderModel", 1100, 1110 }
                });

            migrationBuilder.InsertData(
                table: "ItemPropertyDefaults",
                columns: new[] { "Id", "DefaultValue", "EditorTypeId", "IsVisible", "ItemTypeId", "Key", "Rank", "ReferenceItemTypeId", "ValueDataTypeId" },
                values: new object[,]
                {
                    { 7, "", 3, true, 1012, "ProcessId", 1, null, 57 },
                    { 8, "0", 2, true, 1014, "DoReSync", 4, null, 55 },
                    { 9, "", 4, true, 1014, "UrlBase", 3, null, 54 },
                    { 10, "", 9, true, 1014, "ApiToken", 2, null, 54 },
                    { 13, "", 3, true, 1022, "ProcessId", 1, null, 57 },
                    { 14, "", 4, true, 1022, "ProviderType", 2, null, 54 },
                    { 16, "", 5, true, 1028, "FilePath", 10, null, 54 },
                    { 17, "", 18, true, 1028, "RoleCmds", 8, 120, 54 },
                    { 18, "", 18, true, 1028, "PreAsserts", 7, 250, 54 },
                    { 21, "", 12, true, 1035, "SysPrompt", 7, null, 54 },
                    { 22, "", 5, true, 1035, "FilePath", 1, null, 54 },
                    { 25, "1", 2, true, 1043, "Enabled", 11, null, 55 },
                    { 26, "", 14, true, 1043, "RelativeFolder", 1, null, 54 },
                    { 27, "0", 10, true, 1045, "DeskRole", 12, 1028, 57 },
                    { 28, "0", 2, true, 1045, "Enabled", 11, null, 55 },
                    { 29, "", 10, true, 1045, "Operator", 10, 1035, 57 },
                    { 30, "", 17, true, 1045, "SysPrompt", 9, null, 54 },
                    { 31, "", 10, true, 1045, "OnSuccessTo", 7, 1045, 57 },
                    { 32, "", 10, true, 1045, "OnFailTo", 6, 1045, 57 },
                    { 33, "", 10, true, 1045, "OnPushbackTo", 5, 1045, 57 },
                    { 34, "3", 3, true, 1045, "MaxAttempts", 4, null, 57 },
                    { 36, "", 5, true, 1045, "FilePath", 1, null, 54 },
                    { 51, "", 5, true, 1065, "FilePath", 1, null, 54 },
                    { 54, "104", 10, true, 1101, "Results", 1, 100, 57 },
                    { 56, "", 14, true, 1110, "RelativeFolder", 1, null, 54 }
                });

            migrationBuilder.InsertData(
                table: "ItemTypes",
                columns: new[] { "Id", "Description", "EditorTypeId", "IconName", "IsVisible", "Name", "ParentTypeId", "Rank" },
                values: new object[,]
                {
                    { 1015, "Specific Lm Studio Model", -1, "", true, "PresModelLmStudioModel", 1014, 1015 },
                    { 1050, "Todo", 4, "", true, "TodoModel", 1045, 1050 },
                    { 1111, "Relative Folder Documentation", 4, "", true, "RelativeFolderDocs", 1110, 1111 },
                    { 1120, "Md File", 4, "pi pi-file", true, "FileMdModel", 1110, 1120 },
                    { 1130, "Html File", 4, "", true, "FileHtmlModel", 1110, 1130 },
                    { 1140, "Config File", 4, "", true, "FileConfigModel", 1110, 1140 },
                    { 1150, "Image File", 4, "", true, "FileImageModel", 1110, 0 },
                    { 1160, "Solution", 4, "pi pi-sitemap", true, "SolutionModel", 1110, 1160 },
                    { 1200, "Library", 4, "pi pi-book", true, "LibraryModel", 1110, 1200 }
                });

            migrationBuilder.InsertData(
                table: "ItemPropertyDefaults",
                columns: new[] { "Id", "DefaultValue", "EditorTypeId", "IsVisible", "ItemTypeId", "Key", "Rank", "ReferenceItemTypeId", "ValueDataTypeId" },
                values: new object[,]
                {
                    { 11, "", 4, true, 1015, "ModelName", 1, null, 54 },
                    { 20, "", 10, true, 1035, "Presence", 8, 1015, 57 },
                    { 35, "", 10, true, 1045, "CurrentTodo", 2, 1050, 57 },
                    { 37, "0", 2, true, 1050, "Ready", 12, null, 55 },
                    { 38, "221", 10, true, 1050, "Status", 11, 220, 57 },
                    { 39, "", 16, true, 1050, "RefItem", 10, null, 57 },
                    { 40, "", 17, true, 1050, "UserPrompt", 9, null, 54 },
                    { 41, "", 10, true, 1050, "FromTodo", 8, 1050, 57 },
                    { 42, "", 12, true, 1050, "CloseReason", 7, null, 54 },
                    { 43, "1", 3, true, 1050, "TodoDepth", 6, null, 57 },
                    { 57, "104", 10, true, 1111, "Results", 1, 100, 57 },
                    { 58, "", 5, true, 1120, "FilePath", 2, null, 54 },
                    { 59, ".md", 4, true, 1120, "FileExt", 1, null, 54 },
                    { 61, "", 5, true, 1130, "FilePath", 2, null, 54 },
                    { 62, ".html", 4, true, 1130, "FileExt", 1, null, 54 },
                    { 64, "", 5, true, 1140, "FilePath", 2, null, 54 },
                    { 65, ".json", 4, true, 1140, "FileExt", 1, null, 54 },
                    { 67, "", 5, true, 1150, "FilePath", 2, null, 54 },
                    { 68, ".png", 4, true, 1150, "FileExt", 1, null, 54 },
                    { 70, "", 5, true, 1160, "FilePath", 3, null, 54 },
                    { 71, ".sln", 1, true, 1160, "FileExt", 2, null, 54 },
                    { 72, "", 4, true, 1160, "SlnGuid", 1, null, 54 },
                    { 76, "", 4, true, 1200, "FilePath", 11, null, 54 },
                    { 77, "", 4, true, 1200, "NamespaceRoot", 10, null, 54 },
                    { 78, "net9.0", 4, true, 1200, "TargetFramework", 9, null, 54 },
                    { 79, "1", 2, true, 1200, "IsNullable", 8, null, 55 },
                    { 80, "1", 2, true, 1200, "ImplicitUsing", 7, null, 55 },
                    { 81, "1.0.0", 4, true, 1200, "Version", 6, null, 54 },
                    { 82, "1.0.0.0", 4, true, 1200, "AssemblyVersion", 5, null, 54 },
                    { 83, "1.0.0.0", 4, true, 1200, "FileVersion", 4, null, 54 },
                    { 84, "0", 2, true, 1200, "IsTestLib", 3, null, 55 },
                    { 85, ".csproj", 1, true, 1200, "FileExt", 2, null, 54 }
                });

            migrationBuilder.InsertData(
                table: "ItemTypes",
                columns: new[] { "Id", "Description", "EditorTypeId", "IconName", "IsVisible", "Name", "ParentTypeId", "Rank" },
                values: new object[,]
                {
                    { 1055, "Todo Attempt", 4, "", true, "TodoAttemptModel", 1050, 1055 },
                    { 1121, "Md File Documentation", 4, "", true, "FileMdDocs", 1120, 1121 },
                    { 1131, "Html File Documentation", 4, "", true, "FileHtmlDocs", 1130, 1131 },
                    { 1141, "Config File Documentation", 4, "", true, "FileConfigDocs", 1140, 1141 },
                    { 1151, "Image File Documentation", 4, "", true, "FileImageDocs", 1150, 0 },
                    { 1161, "Solution Documentation", 4, "", true, "SolutionDocs", 1160, 1161 },
                    { 1162, "Solution Import", 4, "pi pi-sitemap", true, "SolutionImportModel", 1160, 1162 },
                    { 1201, "Library Documentation", 4, "", true, "LibraryDocs", 1200, 1201 },
                    { 1210, "Package Ref", 4, "", true, "LibPackageRefModel", 1200, 1 },
                    { 1220, "Library Ref", 4, "", true, "LibLibraryRefModel", 1200, 2 },
                    { 1300, "Dependency Injection", 4, "pi pi-cog", true, "DependencyInjectionModel", 1200, 1 },
                    { 1400, "Namespace", 4, "pi pi-globe", true, "NamespaceModel", 1200, 1400 }
                });

            migrationBuilder.InsertData(
                table: "ItemPropertyDefaults",
                columns: new[] { "Id", "DefaultValue", "EditorTypeId", "IsVisible", "ItemTypeId", "Key", "Rank", "ReferenceItemTypeId", "ValueDataTypeId" },
                values: new object[,]
                {
                    { 44, "231", 10, true, 1055, "Status", 10, 230, 57 },
                    { 45, "", 10, true, 1055, "NextTodo", 9, 1050, 57 },
                    { 46, "", 12, true, 1055, "SysPrompt", 8, null, 54 },
                    { 47, "", 12, true, 1055, "UserPrompt", 7, null, 54 },
                    { 48, "", 12, true, 1055, "Response", 6, null, 54 },
                    { 49, "", 10, true, 1055, "Operator", 5, 1035, 57 },
                    { 60, "104", 10, true, 1121, "Results", 1, 100, 57 },
                    { 63, "104", 10, true, 1131, "Results", 1, 100, 57 },
                    { 66, "104", 10, true, 1141, "Results", 1, 100, 57 },
                    { 69, "104", 10, true, 1151, "Results", 1, 100, 57 },
                    { 73, "104", 10, true, 1161, "Results", 1, 100, 57 },
                    { 74, "", 10, true, 1162, "RegisterObj", 3, 1200, 57 },
                    { 75, "", 4, true, 1162, "ProjectGuid", 2, null, 54 },
                    { 86, "104", 10, true, 1201, "Results", 1, 100, 57 },
                    { 87, "", 4, true, 1210, "PackageInclude", 6, null, 54 },
                    { 88, "", 4, true, 1210, "PackageVersion", 5, null, 54 },
                    { 89, "", 4, true, 1210, "PrivateAssets", 4, null, 54 },
                    { 90, "", 4, true, 1210, "IncludeAssets", 3, null, 54 },
                    { 91, "", 10, true, 1220, "LibInclude", 1, 1200, 57 },
                    { 92, "", 4, true, 1300, "FilePath", 7, null, 54 },
                    { 93, ".cs", 1, true, 1300, "FileExt", 6, null, 54 },
                    { 94, "", 4, true, 1300, "Namespace", 5, null, 54 },
                    { 95, "", 2, true, 1300, "HasDbContext", 4, null, 55 },
                    { 96, "", 2, true, 1300, "HasMediator", 3, null, 55 },
                    { 106, "", 14, true, 1400, "FilePath", 3, null, 54 },
                    { 107, "", 4, true, 1400, "Namespace", 2, null, 54 }
                });

            migrationBuilder.InsertData(
                table: "ItemTypes",
                columns: new[] { "Id", "Description", "EditorTypeId", "IconName", "IsVisible", "Name", "ParentTypeId", "Rank" },
                values: new object[,]
                {
                    { 1301, "Dependency Injection Documentation", 4, "", true, "DependencyInjectionDocs", 1300, 1 },
                    { 1302, "DI - Import", 4, "pi pi-cogs", true, "DiImportModel", 1300, 1 },
                    { 1310, "DbContext", 4, "pi pi-database", true, "DbContextModel", 1300, 2 },
                    { 1401, "Namespace Documentation", 4, "", true, "NamespaceDocs", 1400, 1401 },
                    { 1420, "Interface", 4, "pi pi-plug", true, "InterfaceModel", 1400, 1420 },
                    { 1450, "Record", 4, "", true, "RecordModel", 1400, 1450 },
                    { 1460, "Struct", 4, "", true, "StructModel", 1400, 1460 },
                    { 1500, "Class", 4, "pi pi-cubes", true, "ClassModel", 1400, 1500 },
                    { 1600, "Entity Class", 4, "", true, "EntityClassModel", 1400, 1600 },
                    { 1700, "Handler", 4, "pi pi-shield", true, "HandlerModel", 1400, 1700 }
                });

            migrationBuilder.InsertData(
                table: "ItemPropertyDefaults",
                columns: new[] { "Id", "DefaultValue", "EditorTypeId", "IsVisible", "ItemTypeId", "Key", "Rank", "ReferenceItemTypeId", "ValueDataTypeId" },
                values: new object[,]
                {
                    { 97, "104", 10, true, 1301, "Results", 1, 100, 57 },
                    { 98, "42", 10, true, 1302, "LifetimeScope", 3, 40, 57 },
                    { 99, "", 10, true, 1302, "RegisterObj", 2, 1500, 57 },
                    { 100, "", 2, true, 1302, "RegisterIntf", 1, null, 55 },
                    { 101, "", 4, true, 1310, "FilePath", 3, null, 54 },
                    { 102, ".cs", 4, true, 1310, "FileExt", 2, null, 54 },
                    { 103, "", 4, true, 1310, "Namespace", 1, null, 54 },
                    { 108, "104", 10, true, 1401, "Results", 1, 100, 57 },
                    { 109, "91", 10, true, 1450, "AccessModifier", 17, 90, 57 },
                    { 110, ".cs", 4, true, 1450, "FileExt", 16, null, 54 },
                    { 111, "", 4, true, 1450, "FilePath", 15, null, 54 },
                    { 112, "", 4, true, 1450, "Namespace", 14, null, 54 },
                    { 113, "", 10, true, 1450, "BaseType", 13, 50, 57 },
                    { 114, "", 10, true, 1450, "Interface", 12, 1500, 57 },
                    { 116, "91", 10, true, 1460, "AccessModifier", 17, 90, 57 },
                    { 117, ".cs", 4, true, 1460, "FileExt", 16, null, 54 },
                    { 118, "", 4, true, 1460, "FilePath", 15, null, 54 },
                    { 119, "", 4, true, 1460, "Namespace", 14, null, 54 },
                    { 120, "", 10, true, 1460, "BaseType", 13, 50, 57 },
                    { 121, "", 2, true, 1460, "GenInterface", 12, null, 55 },
                    { 123, "0", 2, true, 1500, "TestClass", 19, null, 55 },
                    { 124, "91", 10, true, 1500, "AccessModifier", 18, 90, 57 },
                    { 125, ".cs", 1, true, 1500, "FileExt", 17, null, 54 },
                    { 126, "", 4, true, 1500, "FilePath", 16, null, 54 },
                    { 127, "", 4, true, 1500, "Namespace", 15, null, 54 },
                    { 128, "", 10, true, 1500, "BaseType", 14, 1500, 57 },
                    { 129, "", 2, true, 1500, "GenInterface", 13, null, 55 },
                    { 130, "", 2, true, 1500, "RegisterDI", 12, null, 55 },
                    { 131, "0", 2, true, 1500, "IsStatic", 11, null, 55 },
                    { 156, ".cs", 1, true, 1600, "FileExt", 17, null, 54 },
                    { 157, "", 4, true, 1600, "FilePath", 16, null, 54 },
                    { 158, "", 4, true, 1600, "Namespace", 15, null, 54 },
                    { 159, "dbo", 4, true, 1600, "DbSchema", 14, null, 54 },
                    { 160, "", 4, true, 1600, "DbTableName", 13, null, 54 },
                    { 182, ".cs", 1, true, 1700, "FileExt", 16, null, 54 },
                    { 183, "", 4, true, 1700, "FilePath", 15, null, 54 },
                    { 184, "", 4, true, 1700, "Namespace", 14, null, 54 }
                });

            migrationBuilder.InsertData(
                table: "ItemTypes",
                columns: new[] { "Id", "Description", "EditorTypeId", "IconName", "IsVisible", "Name", "ParentTypeId", "Rank" },
                values: new object[,]
                {
                    { 1312, "Db Entity Import", 4, "pi pi-database", true, "DbContextEntityImportModel", 1310, 1 },
                    { 1421, "Interface Documentation", 4, "", true, "InterfaceDocs", 1420, 1421 },
                    { 1422, "Interface Property", 4, "pi pi-plug", true, "InterfacePropertyModel", 1420, 1422 },
                    { 1430, "Interface Method", 4, "pi pi-plug", true, "InterfaceMethodModel", 1420, 1430 },
                    { 1451, "Record Documentation", 4, "", true, "RecordDocs", 1450, 1451 },
                    { 1461, "Struct Documentation", 4, "", true, "StructDocs", 1460, 1461 },
                    { 1501, "Class Documentation", 4, "", true, "ClassDocs", 1500, 1501 },
                    { 1502, "Class Import", 4, "pi pi-cube", true, "ClassImportModel", 1500, 1502 },
                    { 1510, "Class Property", 4, "pi pi-cube", true, "ClassPropertyModel", 1500, 1510 },
                    { 1520, "Class Method", 4, "pi pi-cube", true, "ClassMethodModel", 1500, 1520 },
                    { 1601, "Entity Class Documentation", 4, "", true, "EntityClassDocs", 1600, 1601 },
                    { 1602, "EntityClassImportModel", 4, "", true, "EntityClassImportModel", 1600, 1602 },
                    { 1610, "Entity Property", 4, "", true, "EntityPropertyModel", 1600, 1610 },
                    { 1630, "Inbound Nav Property", 4, "", true, "EntityInboundNavigationModel", 1600, 1630 },
                    { 1640, "Entity Configuration Class", 4, "", true, "EntityConfigurationModel", 1600, 1640 },
                    { 1710, "Handler Response", 4, "pi pi-shield", true, "HandlerResponseModel", 1700, 1710 },
                    { 1720, "Handler Command", 4, "pi pi-shield", true, "HandlerCommandModel", 1700, 1720 },
                    { 1800, "Handler Class", 4, "pi pi-shield", true, "HandlerClassModel", 1700, 1800 }
                });

            migrationBuilder.InsertData(
                table: "ItemPropertyDefaults",
                columns: new[] { "Id", "DefaultValue", "EditorTypeId", "IsVisible", "ItemTypeId", "Key", "Rank", "ReferenceItemTypeId", "ValueDataTypeId" },
                values: new object[,]
                {
                    { 105, "", 10, true, 1312, "RegisterObj", 2, 1600, 57 },
                    { 115, "104", 10, true, 1451, "Results", 1, 100, 57 },
                    { 122, "104", 10, true, 1461, "Results", 1, 100, 57 },
                    { 132, "104", 10, true, 1501, "Results", 1, 100, 57 },
                    { 133, "", 10, true, 1502, "ImportObj", 4, 1500, 57 },
                    { 134, "", 2, true, 1502, "UseIntf", 3, null, 55 },
                    { 135, "57", 10, true, 1510, "PropType", 5, 50, 57 },
                    { 136, "", 10, true, 1510, "PropClass", 4, 1500, 57 },
                    { 137, "1", 2, true, 1510, "IsNullable", 3, null, 55 },
                    { 138, "1", 2, true, 1510, "HasSetter", 2, null, 55 },
                    { 140, "32", 10, true, 1520, "TestMethod", 19, 31, 57 },
                    { 141, "91", 10, true, 1520, "AccessModifier", 18, 90, 57 },
                    { 142, "57", 10, true, 1520, "ReturnType", 17, 50, 57 },
                    { 143, "", 10, true, 1520, "ReturnClass", 16, 1500, 57 },
                    { 144, "0", 2, true, 1520, "ReturnNullable", 15, null, 55 },
                    { 145, "0", 2, true, 1520, "IsAsync", 14, null, 55 },
                    { 146, "0", 2, true, 1520, "IsVirtual", 13, null, 55 },
                    { 147, "0", 2, true, 1520, "IsStatic", 12, null, 55 },
                    { 148, "0", 2, true, 1520, "IsAbstract", 11, null, 55 },
                    { 149, "0", 2, true, 1520, "IsSealed", 10, null, 55 },
                    { 161, "104", 10, true, 1601, "Results", 1, 100, 57 },
                    { 162, "", 10, true, 1602, "ImportObj", 4, 1500, 57 },
                    { 163, "", 2, true, 1602, "UseIntf", 3, null, 55 },
                    { 164, "57", 10, true, 1610, "PropType", 8, 50, 57 },
                    { 165, "0", 2, true, 1610, "IsNullable", 7, null, 55 },
                    { 166, "1", 2, true, 1610, "HasSetter", 6, null, 55 },
                    { 167, "0", 2, true, 1610, "HasNav", 5, null, 55 },
                    { 168, "0", 2, true, 1610, "IsPrimaryKey", 4, null, 55 },
                    { 169, "-1", 3, true, 1610, "MaxSize", 3, null, 57 },
                    { 176, "", 10, true, 1630, "PropClass", 7, 1600, 57 },
                    { 177, "", 10, true, 1630, "ForeignKey", 6, 1610, 57 },
                    { 178, "8", 10, true, 1630, "HasNav", 5, 5, 57 },
                    { 179, "1", 2, true, 1630, "IsNullable", 4, null, 55 },
                    { 180, "", 4, true, 1630, "InverseNav", 3, null, 54 },
                    { 185, "57", 10, true, 1710, "PropType", 3, 50, 57 },
                    { 186, "", 10, true, 1710, "PropClass", 2, 1500, 57 },
                    { 187, "57", 10, true, 1720, "PropType", 3, 50, 57 },
                    { 188, "", 10, true, 1720, "PropClass", 2, 1500, 57 }
                });

            migrationBuilder.InsertData(
                table: "ItemTypes",
                columns: new[] { "Id", "Description", "EditorTypeId", "IconName", "IsVisible", "Name", "ParentTypeId", "Rank" },
                values: new object[,]
                {
                    { 1440, "Interface Method Parameter", 4, "pi pi-plug", true, "InterfaceMethodParameterModel", 1430, 1440 },
                    { 1511, "Class Property Documentation", 4, "", true, "ClassPropertyDocs", 1510, 1511 },
                    { 1521, "Class Method Documentation", 4, "", true, "ClassMethodDocs", 1520, 1521 },
                    { 1530, "Class Method Parameter", 4, "pi pi-cube", true, "ClassMethodParameterModel", 1520, 1530 },
                    { 1611, "Entity Property Documentation", 4, "", true, "EntityPropertyDocs", 1610, 1611 },
                    { 1620, "Entity Nav Property", 4, "", true, "EntityNavigationModel", 1610, 1620 },
                    { 1631, "Inbound Nav Property Documentation", 4, "", true, "EntityInboundNavigationDocs", 1630, 1631 },
                    { 1801, "Handler Class Documentation", 4, "", true, "HandlerClassDocs", 1800, 1801 },
                    { 1802, "Handler Class Import", -1, "", true, "HandlerClassImportModel", 1800, 1802 },
                    { 1811, "Handler Property", 4, "pi pi-shield", true, "HandlerPropertyModel", 1800, 1811 },
                    { 1820, "Primary Handler Method", 4, "", true, "HandlerHandlerMethodModel", 1800, 1820 },
                    { 1830, "Handler Method", 4, "pi pi-shield", true, "HandlerMethodModel", 1800, 1830 }
                });

            migrationBuilder.InsertData(
                table: "ItemPropertyDefaults",
                columns: new[] { "Id", "DefaultValue", "EditorTypeId", "IsVisible", "ItemTypeId", "Key", "Rank", "ReferenceItemTypeId", "ValueDataTypeId" },
                values: new object[,]
                {
                    { 139, "104", 10, true, 1511, "Results", 1, 100, 57 },
                    { 150, "104", 10, true, 1521, "Results", 1, 100, 57 },
                    { 151, "57", 10, true, 1530, "ParamType", 5, 50, 57 },
                    { 152, "", 10, true, 1530, "ParamClass", 4, 1500, 57 },
                    { 153, "1", 2, true, 1530, "IsNullable", 3, null, 55 },
                    { 154, "0", 2, true, 1530, "UseThis", 2, null, 55 },
                    { 170, "104", 10, true, 1611, "Results", 1, 100, 57 },
                    { 171, "", 10, true, 1620, "PropClass", 6, 1600, 57 },
                    { 172, "7", 10, true, 1620, "HasNav", 5, 5, 57 },
                    { 173, "84", 10, true, 1620, "DeleteBehavior", 4, 80, 57 },
                    { 174, "1", 2, true, 1620, "IsNullable", 3, null, 55 },
                    { 181, "104", 10, true, 1631, "Results", 1, 100, 57 },
                    { 189, "104", 10, true, 1801, "Results", 1, 100, 57 },
                    { 190, "", 10, true, 1802, "ImportObj", 4, 1500, 57 },
                    { 191, "", 2, true, 1802, "UseIntf", 3, null, 55 },
                    { 192, "57", 10, true, 1811, "PropType", 5, 50, 57 },
                    { 193, "", 10, true, 1811, "PropClass", 4, 1500, 57 },
                    { 194, "1", 2, true, 1811, "IsNullable", 3, null, 55 },
                    { 195, "1", 2, true, 1811, "HasSetter", 2, null, 55 },
                    { 196, "32", 10, true, 1830, "TestMethod", 19, 31, 57 },
                    { 197, "91", 10, true, 1830, "AccessModifier", 18, 90, 57 },
                    { 198, "57", 10, true, 1830, "ReturnType", 17, 50, 57 },
                    { 199, "", 10, true, 1830, "ReturnClass", 16, 1500, 57 },
                    { 200, "0", 2, true, 1830, "ReturnNullable", 15, null, 55 },
                    { 201, "0", 2, true, 1830, "IsAsync", 14, null, 55 },
                    { 202, "0", 2, true, 1830, "IsVirtual", 13, null, 55 },
                    { 203, "0", 2, true, 1830, "IsStatic", 12, null, 55 },
                    { 204, "0", 2, true, 1830, "IsAbstract", 11, null, 55 },
                    { 205, "0", 2, true, 1830, "IsSealed", 10, null, 55 }
                });

            migrationBuilder.InsertData(
                table: "ItemTypes",
                columns: new[] { "Id", "Description", "EditorTypeId", "IconName", "IsVisible", "Name", "ParentTypeId", "Rank" },
                values: new object[,]
                {
                    { 1531, "Class Method Parameter Documentation", 4, "", true, "ClassMethodParameterDocs", 1530, 1531 },
                    { 1621, "Entity Nav Property Documentation", 4, "", true, "EntityNavigationDocs", 1620, 1621 },
                    { 1831, "Handler Method Documentation", 4, "", true, "HandlerMethodDocs", 1830, 1831 },
                    { 1840, "Handler Method Parameter", 4, "", true, "HandlerMethodParameterModel", 1830, 1840 }
                });

            migrationBuilder.InsertData(
                table: "ItemPropertyDefaults",
                columns: new[] { "Id", "DefaultValue", "EditorTypeId", "IsVisible", "ItemTypeId", "Key", "Rank", "ReferenceItemTypeId", "ValueDataTypeId" },
                values: new object[,]
                {
                    { 155, "104", 10, true, 1531, "Results", 1, 100, 57 },
                    { 175, "104", 10, true, 1621, "Results", 1, 100, 57 },
                    { 206, "104", 10, true, 1831, "Results", 1, 100, 57 },
                    { 207, "57", 10, true, 1840, "ParamType", 5, 50, 57 },
                    { 208, "", 10, true, 1840, "ParamClass", 4, 1500, 57 },
                    { 209, "1", 2, true, 1840, "IsNullable", 3, null, 55 },
                    { 210, "0", 2, true, 1840, "UseThis", 2, null, 55 }
                });

            migrationBuilder.InsertData(
                table: "ItemTypes",
                columns: new[] { "Id", "Description", "EditorTypeId", "IconName", "IsVisible", "Name", "ParentTypeId", "Rank" },
                values: new object[] { 1841, "Handler Method Parameter Documentation", 4, "", true, "HandlerMethodParameterDocs", 1840, 1841 });

            migrationBuilder.InsertData(
                table: "ItemPropertyDefaults",
                columns: new[] { "Id", "DefaultValue", "EditorTypeId", "IsVisible", "ItemTypeId", "Key", "Rank", "ReferenceItemTypeId", "ValueDataTypeId" },
                values: new object[] { 211, "104", 10, true, 1841, "Results", 1, 100, 57 });

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
                name: "MediatorLogs");

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
