USE [FabricCoreV134]
GO


SELECT it.[Id]
      ,it.[ItemTypeId]
      ,it.[Name]
      ,desk.Name DeskName
      ,desk.ItemTypeId DeskItemTypeId       
      ,it.[Established]      
  FROM [dbo].[Items] it
    inner join dbo.Relations pr on pr.RelatedItemId = it.Id
      inner join dbo.Items desk on pr.ItemId = desk.Id and desk.ItemTypeId = 1045  -- desk type
    inner join dbo.ItemProperties itPD on it.Id = itPD.ItemId and itPD.Name = 'TodoDepth'
    inner join dbo.ItemProperties itPS on it.Id = itPS.ItemId and itPS.Name = 'Status'
    inner join dbo.ItemProperties itR on it.Id = itR.ItemId and itR.Name = 'Ready' and cast(itR.Value as bit) = 0  -- bool on value 0 not ready 1 ready.
  where it.ItemTypeId = 1050 and  -- todo type 
    cast(itPs.Value as int) = 221 -- 221 not started    
    and it.IsActive = 1 -- not archived
  order by cast(itPD.Value as int) desc, it.Established asc  
GO


SELECT itToDo.[Id]
      ,[ItemTypeId]
      ,itToDo.[Name]
      ,[Description]
      ,[Data]
      ,[Established]
      ,[WrittenAt]
      ,[IsActive]
      ,itPD.*
      
  FROM [dbo].[Items] itToDo
    left outer join dbo.ItemProperties itPD on itToDo.Id = itPD.ItemId

  where ItemTypeId = 1050 
