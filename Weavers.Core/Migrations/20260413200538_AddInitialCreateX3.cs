using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Weavers.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddInitialCreateX3 : Migration
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
                    { 10, "Owner Type of SQL Types", 10, "", true, "SqlTypes", null, 1 },
                    { 21, "sql float type", -1, "", true, "SqlFloatType", 21, 0 },
                    { 50, "Owner Type of C# Types", 10, "", true, "CSharpTypes", null, 1 },
                    { 100, "Project Folder", 4, "pi pi-folder", true, "ProjectFolderModel", null, 100 }
                });

            migrationBuilder.InsertData(
                table: "ItemPropertyDefaults",
                columns: new[] { "Id", "DefaultValue", "EditorTypeId", "IsVisible", "ItemTypeId", "Key", "Rank", "ReferenceItemTypeId", "ValueDataTypeId" },
                values: new object[,]
                {
                    { 1, "", 13, true, 100, "RootFolder", 1, null, 6 },
                    { 2, "1", 4, true, 100, "RepositoryUrl", 2, null, 6 }
                });

            migrationBuilder.InsertData(
                table: "ItemTypes",
                columns: new[] { "Id", "Description", "EditorTypeId", "IconName", "IsVisible", "Name", "ParentTypeId", "Rank" },
                values: new object[,]
                {
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
                    { 110, "Relative Folder", 4, "pi pi-folder", true, "RelativeFolderModel", 100, 110 }
                });

            migrationBuilder.InsertData(
                table: "ItemPropertyDefaults",
                columns: new[] { "Id", "DefaultValue", "EditorTypeId", "IsVisible", "ItemTypeId", "Key", "Rank", "ReferenceItemTypeId", "ValueDataTypeId" },
                values: new object[] { 3, "", 5, true, 110, "RelativeFolder", 1, null, 6 });

            migrationBuilder.InsertData(
                table: "ItemTypes",
                columns: new[] { "Id", "Description", "EditorTypeId", "IconName", "IsVisible", "Name", "ParentTypeId", "Rank" },
                values: new object[,]
                {
                    { 150, "File", 5, "pi pi-file", true, "FileModel", 110, 150 },
                    { 200, "Library", 4, "pi pi-book", true, "LibraryModel", 110, 200 }
                });

            migrationBuilder.InsertData(
                table: "ItemPropertyDefaults",
                columns: new[] { "Id", "DefaultValue", "EditorTypeId", "IsVisible", "ItemTypeId", "Key", "Rank", "ReferenceItemTypeId", "ValueDataTypeId" },
                values: new object[,]
                {
                    { 4, "", 5, true, 150, "FilePath", 1, null, 6 },
                    { 5, "", 4, true, 200, "FilePath", 1, null, 6 }
                });

            migrationBuilder.InsertData(
                table: "ItemTypes",
                columns: new[] { "Id", "Description", "EditorTypeId", "IconName", "IsVisible", "Name", "ParentTypeId", "Rank" },
                values: new object[,]
                {
                    { 300, "Dependency Injection", 4, "pi pi-cog", true, "DependencyInjectionModel", 200, 300 },
                    { 400, "Namespace", 4, "pi pi-globe", true, "NamespaceModel", 150, 400 }
                });

            migrationBuilder.InsertData(
                table: "ItemPropertyDefaults",
                columns: new[] { "Id", "DefaultValue", "EditorTypeId", "IsVisible", "ItemTypeId", "Key", "Rank", "ReferenceItemTypeId", "ValueDataTypeId" },
                values: new object[,]
                {
                    { 6, "", 4, true, 300, "FilePath", 1, null, 6 },
                    { 7, "", 4, true, 400, "FilePath", 1, null, 6 }
                });

            migrationBuilder.InsertData(
                table: "ItemTypes",
                columns: new[] { "Id", "Description", "EditorTypeId", "IconName", "IsVisible", "Name", "ParentTypeId", "Rank" },
                values: new object[,]
                {
                    { 302, "DI - DbContext", 4, "pi pi-database", true, "DiDbContextModel", 300, 302 },
                    { 304, "DI - Mediator", 4, "pi pi-cogs", true, "DiMediatorModel", 300, 304 },
                    { 420, "Interface", 4, "pi pi-plug", true, "InterfaceModel", 400, 420 },
                    { 440, "Record", 4, "", true, "RecordModel", 400, 440 },
                    { 460, "Struct", 4, "", true, "StructModel", 400, 460 },
                    { 500, "Class", 4, "pi pi-cubes", true, "ClassModel", 400, 500 },
                    { 600, "Entity", 4, "", true, "EntityModel", 400, 600 },
                    { 700, "Handler", 4, "pi pi-shield", true, "HandlerModel", 400, 700 }
                });

            migrationBuilder.InsertData(
                table: "ItemPropertyDefaults",
                columns: new[] { "Id", "DefaultValue", "EditorTypeId", "IsVisible", "ItemTypeId", "Key", "Rank", "ReferenceItemTypeId", "ValueDataTypeId" },
                values: new object[,]
                {
                    { 8, "", 4, true, 420, "BaseType", 1, null, 6 },
                    { 14, "", 4, true, 440, "Namespace", 1, null, 6 },
                    { 15, "", 10, true, 440, "BaseType", 2, 50, 3 },
                    { 16, "", 11, true, 440, "Interface", 3, 420, 3 },
                    { 17, "", 12, true, 440, "RecordContent", 4, null, 6 },
                    { 18, "", 4, true, 460, "Namespace", 1, null, 6 },
                    { 19, "", 10, true, 460, "BaseType", 2, 50, 3 },
                    { 20, "", 11, true, 460, "Interface", 3, 420, 3 },
                    { 21, "", 12, true, 460, "StructContent", 4, null, 6 },
                    { 22, "", 4, true, 500, "Namespace", 1, null, 6 },
                    { 23, "", 10, true, 500, "BaseType", 2, 50, 3 },
                    { 24, "", 11, true, 500, "Interface", 3, 420, 3 }
                });

            migrationBuilder.InsertData(
                table: "ItemTypes",
                columns: new[] { "Id", "Description", "EditorTypeId", "IconName", "IsVisible", "Name", "ParentTypeId", "Rank" },
                values: new object[,]
                {
                    { 422, "Interface Property", 4, "pi pi-plug", true, "InterfacePropertyModel", 420, 422 },
                    { 424, "Interface Method", 4, "pi pi-plug", true, "InterfaceMethodModel", 420, 424 },
                    { 522, "Class Property", 4, "pi pi-cube", true, "ClassPropertyModel", 500, 522 },
                    { 524, "Class Method", 4, "pi pi-cube", true, "ClassMethodModel", 500, 524 },
                    { 610, "Entity Class", 4, "", true, "EntityClassModel", 600, 610 },
                    { 612, "Entity Property", 4, "", true, "EntityPropertyModel", 600, 612 },
                    { 614, "Entity Navigation Property", 4, "", true, "EntityNavigationModel", 600, 614 },
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
                    { 9, "64", 10, true, 422, "PropertyType", 1, 50, 3 },
                    { 10, "1", 2, true, 422, "IsNullable", 2, null, 1 },
                    { 11, "64", 10, true, 424, "ReturnType", 1, 50, 3 },
                    { 25, "64", 10, true, 522, "PropertyType", 1, 50, 3 },
                    { 26, "", 4, true, 522, "PropertyTypeRefName", 2, null, 6 },
                    { 27, "1", 2, true, 522, "IsNullable", 3, null, 1 },
                    { 28, "64", 10, true, 524, "ReturnType", 1, 50, 3 },
                    { 29, "", 4, true, 524, "ReturnTypeRefName", 2, null, 6 },
                    { 30, "1", 2, true, 524, "IsAsync", 3, null, 1 },
                    { 31, "1", 2, true, 524, "IsVirtual", 4, null, 1 },
                    { 32, "1", 2, true, 524, "IsStatic", 5, null, 1 },
                    { 33, "1", 2, true, 524, "IsAbstract", 6, null, 1 },
                    { 34, "1", 2, true, 524, "IsSealed", 7, null, 1 }
                });

            migrationBuilder.InsertData(
                table: "ItemTypes",
                columns: new[] { "Id", "Description", "EditorTypeId", "IconName", "IsVisible", "Name", "ParentTypeId", "Rank" },
                values: new object[,]
                {
                    { 426, "Interface Method Parameter", 4, "pi pi-plug", true, "InterfaceMethodParameterModel", 424, 426 },
                    { 526, "Class Method Parameter", 4, "pi pi-cube", true, "ClassMethodParameterModel", 524, 526 },
                    { 622, "Entity Property Configuration", 4, "", true, "EntityPropertyConfigurationModel", 612, 622 },
                    { 732, "Handler Property", 4, "pi pi-shield", true, "HandlerPropertyModel", 730, 732 },
                    { 734, "Handler Method", 4, "pi pi-shield", true, "HandlerMethodModel", 730, 734 }
                });

            migrationBuilder.InsertData(
                table: "ItemPropertyDefaults",
                columns: new[] { "Id", "DefaultValue", "EditorTypeId", "IsVisible", "ItemTypeId", "Key", "Rank", "ReferenceItemTypeId", "ValueDataTypeId" },
                values: new object[,]
                {
                    { 12, "64", 10, true, 426, "ParameterType", 1, 50, 3 },
                    { 13, "1", 2, true, 426, "IsNullable", 2, null, 1 },
                    { 35, "64", 10, true, 526, "ParameterType", 1, 50, 3 },
                    { 36, "", 4, true, 526, "ParameterTypeRefName", 2, null, 6 },
                    { 37, "1", 2, true, 526, "IsNullable", 3, null, 1 }
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
