﻿CREATE TABLE [NIS].[Video]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [Date] DATETIME NOT NULL, 
    [Title] NVARCHAR(100) NOT NULL, 
    [Video] NVARCHAR(MAX) NOT NULL
)