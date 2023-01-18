namespace SQLWriter.InnerDb
{
	internal interface IGetSqlString
	{
		string Update { get; }
		string Insert { get; }
		string Delete { get; }
		string Query { get; }
		string CreateTable { get; }
	}

	internal sealed class SQLStringSqlString : IGetSqlString
	{
		public SQLStringSqlString() { }

		public string CreateTable =>
			@"create table [SQLString]([Rdbms] VARCHAR(20),[TableName] VARCHAR(20),[Action] INTEGER,[SQLString] VARCHAR(800)
				,Primary Key ([Rdbms],[TableName],[Action]))";
		public string Update =>
			@"update [SQLString] set [SQLString]=@SQLString
			where [Rdbms]=@Rdbms and [TableName]=@TableName and [Action]=@Action ";
		public string Insert =>
			@"insert into [SQLString]
				([Rdbms],[TableName],[Action],[SQLString])
			values
				(@Rdbms,@TableName,@Action,@SQLString)";
		public string Delete =>
			@"Delete from [SQLString] where [Rdbms]=@Rdbms and [TableName]=@TableName and [Action]=@Action";
		public string Query =>
			@"select [Rdbms],[TableName],[Action],[SQLString]
			from [SQLString]";
	}

	internal sealed class PropertySqlString : IGetSqlString
	{
		public PropertySqlString() { }

		public string CreateTable =>
			@"create table [Property]([EntityName] VARCHAR(20),[Name] VARCHAR(20),[DataType] INTEGER,[WordCount] INTEGER,
				[IsPrimaryKey] BOOLEAN,[IsAutoincrement] BOOLEAN,[IsUnique] BOOLEAN,Primary Key ([EntityName],[Name]))";
		public string Update =>
			@"update [Property] set [DataType]=@DataType,[WordCount]=@WordCount,[IsPrimaryKey]=@IsPrimaryKey,
			[IsAutoincrement]=@IsAutoincrement,[IsUnique]=@IsUnique
			where [EntityName]=@EntityName and [Name]=@Name ";
		public string Insert =>
			@"insert into [Property]
				([EntityName],[Name],[DataType],[WordCount],[IsPrimaryKey],[IsAutoincrement],[IsUnique])
			values
				(@EntityName,@Name,@DataType,@WordCount,@IsPrimaryKey,@IsAutoincrement,@IsUnique)";
		public string Delete =>
			@"Delete from [Property] where [EntityName]=@EntityName and [Name]=@Name";
		public string Query =>
			@"select [EntityName],[Name],[DataType],[WordCount],[IsPrimaryKey],[IsAutoincrement],[IsUnique]
			from [Property]";
	}

	internal sealed class ClassInfoSqlString : IGetSqlString
	{
		public ClassInfoSqlString() { }

		public string CreateTable =>
			@"create table [ClassInfo]([ClassName] VARCHAR(20),[TableName] VARCHAR(20), Primary Key ([ClassName]))";
		public string Update =>
			@"update [ClassInfo] set [TableName]=@TableName
			where [ClassName]=@ClassName ";
		public string Insert =>
			@"insert into [ClassInfo]
				([ClassName],[TableName])
			values
				(@ClassName,@TableName)";
		public string Delete =>
			@"Delete from [ClassInfo] where [ClassName]=@ClassName";
		public string Query =>
			@"select [ClassName],[TableName]
			from [ClassInfo]";
	}

}
