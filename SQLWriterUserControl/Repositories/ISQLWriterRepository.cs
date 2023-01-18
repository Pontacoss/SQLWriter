using System.Collections.Generic;


namespace SQLWriter.Repositories
{
	public interface ISQLWriterRepository
	{
		int Save<T>(T x) where T : class;
		int Update<T>(T x) where T : class;
		int Insert<T>(T x) where T : class;
		void Delete<T>(T x) where T : class;

		/// <summary>
		/// Queryの実行。Filterなし。
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		List<T> Load<T>() where T : class;
		/// <summary>
		/// Queryの実行。Filterが1つの場合。
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="filter"></param>
		/// <returns></returns>
		List<T> Load<T>((string parameterName, string operatorStrig, object filteringValue) filter) where T : class;
		/// <summary>
		/// Queryの実行。Filterが複数ある場合。
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="filters"></param>
		/// <returns></returns>
		List<T> Load<T>(List<(string parameterName, string operatorStrig, object filteringValue)> filters) where T : class;

	}
}
