USE [FabricCoreV147]
GO

SELECT [Id]
      ,[ItemPropertyDefaultId]
      ,[ItemId]
      ,[Name]
      ,[Value]
      ,[ValueHash]
      ,[ValueDataTypeId]
      ,[ReferenceItemTypeId]
      ,[EditorTypeId]
      ,[IsRequired]
      ,[IsVisible]
      ,[IsReadOnly]
  FROM [dbo].[ItemProperties]

GO


WITH Descendants AS (
    SELECT  i.Id,
            i.Name,
            CAST(NULL AS int) AS ParentId,
            i.ItemTypeId,
            case when i.ItemTypeId in (1110, 1100) then 1 else 0 end isFolder,
            0 AS Lvl,
            CAST('/' + CAST(i.Id AS varchar(10)) + '/' AS varchar(900)) AS PathKey,
            ipFP.Value FilePath,
            ipES.Value EntrySha
    FROM    dbo.Items i
      join dbo.ItemProperties ipFP on i.Id = ipFP.ItemId and ipFP.Name in ( 'RelativeFolder', 'FilePath' )
      join dbo.ItemProperties ipES on i.Id = ipES.ItemId and ipES.Name = 'EntrySha'
    WHERE   i.Id = 13 AND i.IsActive = 1

    UNION ALL

    SELECT  i.Id,
            i.Name,             
            r.ItemId,
            i.ItemTypeId,            
            case when i.ItemTypeId in (1110, 1100) then 1 else 0 end isFolder,
            d.Lvl + 1,
            CAST(d.PathKey + CAST(i.Id AS varchar(10)) + '/' AS varchar(900)),
            ipFP.Value FilePath,
            ipES.Value EntrySha
            
    FROM    Descendants d
    JOIN    dbo.Relations r ON r.ItemId = d.Id
                           AND r.RelationTypeId = 20
    JOIN    dbo.Items     i ON i.Id = r.RelatedItemId
      join dbo.ItemProperties ipFP on i.Id = ipFP.ItemId and ipFP.Name in ( 'RelativeFolder', 'FilePath' )
      join dbo.ItemProperties ipES on i.Id = ipES.ItemId and ipES.Name = 'EntrySha'
    WHERE   i.IsActive = 1
      and  i.ItemTypeId in (1112)
      AND   d.PathKey NOT LIKE '%/' + CAST(i.Id AS varchar(10)) + '/%'
)
SELECT * FROM Descendants
OPTION (MAXRECURSION 256);

select * from RelationTypes
select * from ItemTypes where id in (1110, 1100)