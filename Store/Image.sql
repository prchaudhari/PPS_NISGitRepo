﻿CREATE TABLE [NIS].[Image]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [Date] DATETIME NOT NULL, 
    [Title] NVARCHAR(100) NOT NULL, 
    [Image] NVARCHAR(MAX) NOT NULL
)
