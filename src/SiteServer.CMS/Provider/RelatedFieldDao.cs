using System;
using System.Collections.Generic;
using Datory;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.Utils;

namespace SiteServer.CMS.Provider
{
    public class RelatedFieldDao : IDatabaseDao
    {
        private readonly Repository<RelatedFieldInfo> _repository;
        public RelatedFieldDao()
        {
            _repository = new Repository<RelatedFieldInfo>(AppSettings.DatabaseType, AppSettings.ConnectionString);
        }

        public string TableName => _repository.TableName;
        public List<TableColumn> TableColumns => _repository.TableColumns;

        private static class Attr
        {
            public const string Id = nameof(RelatedFieldInfo.Id);
            public const string Title = nameof(RelatedFieldInfo.Title);
            public const string SiteId = nameof(RelatedFieldInfo.SiteId);
        }

        public int Insert(RelatedFieldInfo relatedFieldInfo)
        {
            return _repository.Insert(relatedFieldInfo);
        }

        public void Update(RelatedFieldInfo relatedFieldInfo)
        {
            _repository.Update(relatedFieldInfo);
        }

        public void Delete(int id)
        {
            _repository.Delete(id);
        }

        public RelatedFieldInfo GetRelatedFieldInfo(int id)
        {
            return id <= 0 ? null : _repository.Get(id);
        }

        public RelatedFieldInfo GetRelatedFieldInfo(int siteId, string title)
        {
            return _repository.Get(Q
                .Where(Attr.SiteId, siteId)
                .Where(Attr.Title, title));
        }

        public string GetTitle(int id)
        {
            return _repository.Get<string>(Q
                .Select(Attr.Title)
                .Where(Attr.Id, id));
        }

        public IList<RelatedFieldInfo> GetRelatedFieldInfoList(int siteId)
        {
            return _repository.GetAll(Q
                .Where(Attr.SiteId, siteId)
                .OrderBy(Attr.Id));
        }

        public IList<string> GetTitleList(int siteId)
        {
            return _repository.GetAll<string>(Q
                .Select(Attr.Title)
                .Where(Attr.SiteId, siteId)
                .OrderBy(Attr.Id));
        }

        public string GetImportTitle(int siteId, string relatedFieldName)
        {
            string importName;
            if (relatedFieldName.IndexOf("_", StringComparison.Ordinal) != -1)
            {
                var relatedFieldNameCount = 0;
                var lastName = relatedFieldName.Substring(relatedFieldName.LastIndexOf("_", StringComparison.Ordinal) + 1);
                var firstName = relatedFieldName.Substring(0, relatedFieldName.Length - lastName.Length);
                try
                {
                    relatedFieldNameCount = int.Parse(lastName);
                }
                catch
                {
                    // ignored
                }
                relatedFieldNameCount++;
                importName = firstName + relatedFieldNameCount;
            }
            else
            {
                importName = relatedFieldName + "_1";
            }

            var relatedFieldInfo = GetRelatedFieldInfo(siteId, relatedFieldName);
            if (relatedFieldInfo != null)
            {
                importName = GetImportTitle(siteId, importName);
            }

            return importName;
        }
    }
}

// using System;
// using System.Collections.Generic;
// using System.Data;
// using Datory;
// using SiteServer.CMS.Model;

// namespace SiteServer.CMS.Provider
// {
//     public class RelatedFieldDao
// 	{
//         public override string TableName => "siteserver_RelatedField";

//         public override List<TableColumn> TableColumns => new List<TableColumn>
//         {
//             new TableColumn
//             {
//                 AttributeName = nameof(RelatedFieldInfo.Id),
//                 DataType = DataType.Integer,
//                 IsIdentity = true,
//                 IsPrimaryKey = true
//             },
//             new TableColumn
//             {
//                 AttributeName = nameof(RelatedFieldInfo.Title),
//                 DataType = DataType.VarChar,
//                 DataLength = 50
//             },
//             new TableColumn
//             {
//                 AttributeName = nameof(RelatedFieldInfo.SiteId),
//                 DataType = DataType.Integer
//             },
//             new TableColumn
//             {
//                 AttributeName = nameof(RelatedFieldInfo.TotalLevel),
//                 DataType = DataType.Integer
//             },
//             new TableColumn
//             {
//                 AttributeName = nameof(RelatedFieldInfo.Prefixes),
//                 DataType = DataType.VarChar,
//                 DataLength = 255
//             },
//             new TableColumn
//             {
//                 AttributeName = nameof(RelatedFieldInfo.Suffixes),
//                 DataType = DataType.VarChar,
//                 DataLength = 255
//             }
//         };

//         private const string SqlUpdate = "UPDATE siteserver_RelatedField SET Title = @Title, TotalLevel = @TotalLevel, Prefixes = @Prefixes, Suffixes = @Suffixes WHERE Id = @Id";
//         private const string SqlDelete = "DELETE FROM siteserver_RelatedField WHERE Id = @Id";

//         private const string ParmId = "@Id";
//         private const string ParmTitle = "@Title";
// 		private const string ParmSiteId = "@SiteId";
//         private const string ParmTotalLevel = "@TotalLevel";
//         private const string ParmPrefixes = "@Prefixes";
//         private const string ParmSuffixes = "@Suffixes";

// 		public int Insert(RelatedFieldInfo relatedFieldInfo) 
// 		{
//             const string sqlString = "INSERT INTO siteserver_RelatedField (Title, SiteId, TotalLevel, Prefixes, Suffixes) VALUES (@Title, @SiteId, @TotalLevel, @Prefixes, @Suffixes)";

// 			var insertParms = new IDataParameter[]
// 			{
// 				GetParameter(ParmTitle, DataType.VarChar, 50, relatedFieldInfo.Title),
// 				GetParameter(ParmSiteId, DataType.Integer, relatedFieldInfo.SiteId),
//                 GetParameter(ParmTotalLevel, DataType.Integer, relatedFieldInfo.TotalLevel),
//                 GetParameter(ParmPrefixes, DataType.VarChar, 255, relatedFieldInfo.Prefixes),
//                 GetParameter(ParmSuffixes, DataType.VarChar, 255, relatedFieldInfo.Suffixes),
// 			};

//             return ExecuteNonQueryAndReturnId(TableName, nameof(RelatedFieldInfo.Id), sqlString, insertParms);
// 		}

//         public void Update(RelatedFieldInfo relatedFieldInfo) 
// 		{
// 			var updateParms = new IDataParameter[]
// 			{
// 				GetParameter(ParmTitle, DataType.VarChar, 50, relatedFieldInfo.Title),
//                 GetParameter(ParmTotalLevel, DataType.Integer, relatedFieldInfo.TotalLevel),
//                 GetParameter(ParmPrefixes, DataType.VarChar, 255, relatedFieldInfo.Prefixes),
//                 GetParameter(ParmSuffixes, DataType.VarChar, 255, relatedFieldInfo.Suffixes),
// 				GetParameter(ParmId, DataType.Integer, relatedFieldInfo.Id)
// 			};

//             ExecuteNonQuery(SqlUpdate, updateParms);
// 		}

// 		public void Delete(int id)
// 		{
// 			var relatedFieldInfoParms = new IDataParameter[]
// 			{
// 				GetParameter(ParmId, DataType.Integer, id)
// 			};

//             ExecuteNonQuery(SqlDelete, relatedFieldInfoParms);
// 		}

//         public RelatedFieldInfo GetRelatedFieldInfo(int id)
// 		{
//             if (id <= 0) return null;

//             RelatedFieldInfo relatedFieldInfo = null;

// 		    string sqlString =
// 		        $"SELECT Id, Title, SiteId, TotalLevel, Prefixes, Suffixes FROM siteserver_RelatedField WHERE Id = {id}";

// 		    using (var rdr = ExecuteReader(sqlString))
// 		    {
// 		        if (rdr.Read())
// 		        {
// 		            var i = 0;
// 		            relatedFieldInfo = new RelatedFieldInfo(GetInt(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i));
// 		        }
// 		        rdr.Close();
// 		    }

// 		    return relatedFieldInfo;
// 		}

//         public RelatedFieldInfo GetRelatedFieldInfo(int siteId, string relatedFieldName)
//         {
//             RelatedFieldInfo relatedFieldInfo = null;

//             string sqlString =
//                 $"SELECT Id, Title, SiteId, TotalLevel, Prefixes, Suffixes FROM siteserver_RelatedField WHERE SiteId = {siteId} AND Title = @Title";

//             var selectParms = new IDataParameter[]
// 			{
// 				GetParameter(ParmTitle, DataType.VarChar, 255, relatedFieldName)			 
// 			};

//             using (var rdr = ExecuteReader(sqlString, selectParms))
//             {
//                 if (rdr.Read())
//                 {
//                     var i = 0;
//                     relatedFieldInfo = new RelatedFieldInfo(GetInt(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i));
//                 }
//                 rdr.Close();
//             }

//             return relatedFieldInfo;
//         }

//         public string GetTitle(int id)
//         {
//             var relatedFieldName = string.Empty;

//             string sqlString =
//                 $"SELECT Title FROM siteserver_RelatedField WHERE Id = {id}";

//             using (var rdr = ExecuteReader(sqlString))
//             {
//                 if (rdr.Read())
//                 {
//                     relatedFieldName = GetString(rdr, 0);
//                 }
//                 rdr.Close();
//             }

//             return relatedFieldName;
//         }

// 		public List<RelatedFieldInfo> GetRelatedFieldInfoList(int siteId)
// 		{
// 			var list = new List<RelatedFieldInfo>();
//             string sqlString =
//                 $"SELECT Id, Title, SiteId, TotalLevel, Prefixes, Suffixes FROM siteserver_RelatedField WHERE SiteId = {siteId} ORDER BY Id";

// 			using (var rdr = ExecuteReader(sqlString)) 
// 			{
// 				while (rdr.Read())
// 				{
// 				    var i = 0;
//                     list.Add(new RelatedFieldInfo(GetInt(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i)));
// 				}
// 				rdr.Close();
// 			}

// 			return list;
// 		}

// 		public List<string> GetTitleList(int siteId)
// 		{
// 			var list = new List<string>();
//             string sqlString =
//                 $"SELECT Title FROM siteserver_RelatedField WHERE SiteId = {siteId} ORDER BY Id";

// 			using (var rdr = ExecuteReader(sqlString)) 
// 			{
// 				while (rdr.Read()) 
// 				{
//                     list.Add(GetString(rdr, 0));
// 				}
// 				rdr.Close();
// 			}

// 			return list;
// 		}

//         public string GetImportTitle(int siteId, string relatedFieldName)
//         {
//             string importName;
//             if (relatedFieldName.IndexOf("_", StringComparison.Ordinal) != -1)
//             {
//                 var relatedFieldNameCount = 0;
//                 var lastName = relatedFieldName.Substring(relatedFieldName.LastIndexOf("_", StringComparison.Ordinal) + 1);
//                 var firstName = relatedFieldName.Substring(0, relatedFieldName.Length - lastName.Length);
//                 try
//                 {
//                     relatedFieldNameCount = int.Parse(lastName);
//                 }
//                 catch
//                 {
//                     // ignored
//                 }
//                 relatedFieldNameCount++;
//                 importName = firstName + relatedFieldNameCount;
//             }
//             else
//             {
//                 importName = relatedFieldName + "_1";
//             }

//             var relatedFieldInfo = GetRelatedFieldInfo(siteId, relatedFieldName);
//             if (relatedFieldInfo != null)
//             {
//                 importName = GetImportTitle(siteId, importName);
//             }

//             return importName;
//         }
// 	}
// }