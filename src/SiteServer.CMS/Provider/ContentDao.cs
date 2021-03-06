﻿using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using Datory;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.Utils;
using SqlKata;
using Attr = SiteServer.CMS.Model.Attributes.ContentAttribute;

namespace SiteServer.CMS.Provider
{
    public partial class ContentDao : IDatabaseDao
    {
        private readonly Repository<ContentInfo> _repository;

        private ContentDao(string tableName)
        {
            _repository = new Repository<ContentInfo>(AppSettings.DatabaseType, AppSettings.ConnectionString, tableName);
        }

        public string TableName => _repository.TableName;
        public List<TableColumn> TableColumns => _repository.TableColumns;

        public static readonly List<TableColumn> TableColumnsDefault = DatoryUtils.GetTableColumns<ContentInfo>();

        private static readonly ConcurrentDictionary<string, ContentDao> ContentDaos = new ConcurrentDictionary<string, ContentDao>();

        public static ContentDao Instance(string tableName)
        {
            if (ContentDaos.TryGetValue(tableName, out ContentDao repository))
            {
                return repository;
            }

            repository = new ContentDao(tableName);

            ContentDaos[tableName] = repository;
            return repository;
        }

        public static ICollection<ContentDao> GetContentDaoList()
        {
            return ContentDaos.Values;
        }

        public static ContentDao Instance(SiteInfo siteInfo)
        {
            return Instance(siteInfo.TableName);
        }

        public static ContentDao Instance(ChannelInfo channelInfo)
        {
            var siteInfo = SiteManager.GetSiteInfo(channelInfo.SiteId);
            var tableName = ChannelManager.GetTableName(siteInfo, channelInfo);
            if (string.IsNullOrEmpty(tableName))
            {
                tableName = siteInfo.TableName;
            }
            return Instance(tableName);
        }

        public string GetOrderString(ChannelInfo channelInfo, string orderBy)
        {
            return ETaxisTypeUtils.GetContentOrderByString(ETaxisTypeUtils.GetEnumType(channelInfo.DefaultTaxisType), orderBy);
        }

        public static void QueryOrder(Query query, ETaxisType taxisType)
        {
            if (taxisType == ETaxisType.OrderById)
            {
                query.OrderByDesc(Attr.IsTop).OrderBy(Attr.Id);
            }
            else if (taxisType == ETaxisType.OrderByIdDesc)
            {
                query.OrderByDesc(Attr.IsTop, Attr.Id);
            }
            else if (taxisType == ETaxisType.OrderByChannelId)
            {
                query.OrderByDesc(Attr.IsTop).OrderBy(Attr.ChannelId).OrderByDesc(Attr.Id);
            }
            else if (taxisType == ETaxisType.OrderByChannelIdDesc)
            {
                query.OrderByDesc(Attr.IsTop, Attr.ChannelId, Attr.Id);
            }
            else if (taxisType == ETaxisType.OrderByAddDate)
            {
                query.OrderByDesc(Attr.IsTop).OrderBy(Attr.AddDate).OrderByDesc(Attr.Id);
            }
            else if (taxisType == ETaxisType.OrderByAddDateDesc)
            {
                query.OrderByDesc(Attr.IsTop, Attr.AddDate, Attr.Id);
            }
            else if (taxisType == ETaxisType.OrderByLastEditDate)
            {
                query.OrderByDesc(Attr.IsTop).OrderBy(Attr.LastEditDate).OrderByDesc(Attr.Id);
            }
            else if (taxisType == ETaxisType.OrderByLastEditDateDesc)
            {
                query.OrderByDesc(Attr.IsTop, Attr.LastEditDate, Attr.Id);
            }
            else if (taxisType == ETaxisType.OrderByTaxis)
            {
                query.OrderByDesc(Attr.IsTop).OrderBy(Attr.Taxis).OrderByDesc(Attr.Id);
            }
            else if (taxisType == ETaxisType.OrderByTaxisDesc)
            {
                query.OrderByDesc(Attr.IsTop, Attr.Taxis, Attr.Id);
            }
            else if (taxisType == ETaxisType.OrderByHits)
            {
                query.OrderByDesc(Attr.Hits, Attr.Id);
            }
            else if (taxisType == ETaxisType.OrderByHitsByDay)
            {
                query.OrderByDesc(Attr.HitsByDay, Attr.Id);
            }
            else if (taxisType == ETaxisType.OrderByHitsByWeek)
            {
                query.OrderByDesc(Attr.HitsByWeek, Attr.Id);
            }
            else if (taxisType == ETaxisType.OrderByHitsByMonth)
            {
                query.OrderByDesc(Attr.HitsByMonth, Attr.Id);
            }
            else if (taxisType == ETaxisType.OrderByRandom)
            {
                query.OrderByRandom(StringUtils.GetGuid());
            }
            else
            {
                query.OrderByDesc(Attr.IsTop, Attr.Taxis, Attr.Id);
            }
        }
    }
}
