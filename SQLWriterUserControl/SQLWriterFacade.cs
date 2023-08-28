using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Threading;
using SQLWriter.Entities;
using SQLWriter.Exceptions;
using SQLWriter.InnerDb;
using SQLWriter.Interfaces;
using SQLWriter.RDBMS.SQLite;

namespace SQLWriter
{

	public static class SQLWriterFacade
	{
		/// <summary>
		/// Domainを動的に設定するためのアセンブリファイルのパス
		/// </summary>
		public static string AssemblyFilePath { get; private set; }
		/// <summary>
		/// Domainを動的に設定するためのアセンブリファイルのファイル名
		/// </summary>
		public static string AssemblyFile { get; private set; }
		/// <summary>
		/// Domainを動的に設定するためのアセンブリファイル内のエンティティが格納されているnamespace
		/// </summary>
		public static string NameSpace { get; private set; } 

		/// <summary>
		/// Domainへの参照を設定する。
		/// </summary>
		/// <param name="assemblyFilePath">アセンブリファイルのフルパスを設定</param>
		/// /// <param name="assmblyFile">アセンブリファイル名を設定</param>
		/// <param name="nameSpace">アセンブリファイル内のDB接続が必要なエンティティが格納されているnamespaceを指定。</param>
		public static void SetReferenceToDomain(string assemblyFilePath, string assmblyFile, string nameSpace)
		{
			AssemblyFilePath = assemblyFilePath;
			AssemblyFile = assmblyFile;
			NameSpace = nameSpace;
		}

		/// <summary>
		/// SQLWriterの内部DB(対象DomainのTable名称やProperty設定,および自動生成したSQL文など)を初期化する。
		/// </summary>
		public static void InitializeInnerDB()
		{
			SQLWriterHelper.InitializeSQLWriterDB();
		}

		/// <summary>
		/// 
		/// </summary>
		public static void Initialize()
		{
			SQLiteHelper.Initialize();
		}
	}
}
