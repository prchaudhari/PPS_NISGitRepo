CREATE TABLE [NIS].[DynamicWidgetFilterDetail]
(
	[Id] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY, 
	[DynamicWidgetId] BIGINT NOT NULL,
	[FieldId] BIGINT NOT NULL,
	[Operator] nvarchar(50),
	[ConditionalOperator]  nvarchar(50),
	[Sequence] bigint not null,
	[Value] nvarchar(100) not null
)
