﻿using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.BackgroundPages.Controls;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.Utils.Enumerations;
using SiteServer.BackgroundPages.Core;

namespace SiteServer.BackgroundPages.Cms
{
    public class ModalContentExport : BasePageCms
    {
        public DropDownList DdlExportType;
        public DropDownList DdlPeriods;
        public DateTimeTextBox TbStartDate;
        public DateTimeTextBox TbEndDate;
        public PlaceHolder PhDisplayAttributes;
        public CheckBoxList CblDisplayAttributes;
        public DropDownList DdlIsChecked;

        private int _channelId;

        public static string GetOpenWindowString(int siteId, int channelId)
        {
            return LayerUtils.GetOpenScriptWithCheckBoxValue("导出内容",
                PageUtilsEx.GetCmsUrl(siteId, nameof(ModalContentExport), new NameValueCollection
                {
                    {"channelId", channelId.ToString()}
                }), "contentIdCollection", string.Empty);
        }

        private void LoadDisplayAttributeCheckBoxList()
        {
            var nodeInfo = ChannelManager.GetChannelInfo(SiteId, _channelId);
            var styleInfoList = ContentUtility.GetAllTableStyleInfoList(TableStyleManager.GetContentStyleInfoList(SiteInfo, nodeInfo));

            foreach (var styleInfo in styleInfoList)
            {
                var listItem = new ListItem(styleInfo.DisplayName, styleInfo.AttributeName)
                {
                    Selected = true
                };
                CblDisplayAttributes.Items.Add(listItem);
            }
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _channelId = AuthRequest.GetQueryInt("channelId", SiteId);
            if (IsPostBack) return;

            LoadDisplayAttributeCheckBoxList();
            ConfigSettings(true);
        }

        private void ConfigSettings(bool isLoad)
        {
            if (isLoad)
            {
                if (!string.IsNullOrEmpty(SiteInfo.ConfigExportType))
                {
                    DdlExportType.SelectedValue = SiteInfo.ConfigExportType;
                }
                if (!string.IsNullOrEmpty(SiteInfo.ConfigExportPeriods))
                {
                    DdlPeriods.SelectedValue = SiteInfo.ConfigExportPeriods;
                }
                if (!string.IsNullOrEmpty(SiteInfo.ConfigExportDisplayAttributes))
                {
                    var displayAttributes = TranslateUtils.StringCollectionToStringList(SiteInfo.ConfigExportDisplayAttributes);
                    ControlUtils.SelectMultiItems(CblDisplayAttributes, displayAttributes);
                }
                if (!string.IsNullOrEmpty(SiteInfo.ConfigExportIsChecked))
                {
                    DdlIsChecked.SelectedValue = SiteInfo.ConfigExportIsChecked;
                }
            }
            else
            {
                SiteInfo.ConfigExportType = DdlExportType.SelectedValue;
                SiteInfo.ConfigExportPeriods = DdlPeriods.SelectedValue;
                SiteInfo.ConfigExportDisplayAttributes = ControlUtils.GetSelectedListControlValueCollection(CblDisplayAttributes);
                SiteInfo.ConfigExportIsChecked = DdlIsChecked.SelectedValue;
                DataProvider.SiteDao.Update(SiteInfo);
            }
        }

        public void DdlExportType_SelectedIndexChanged(object sender, EventArgs e)
        {
            PhDisplayAttributes.Visible = DdlExportType.SelectedValue != "ContentZip";
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var displayAttributes = ControlUtils.GetSelectedListControlValueCollection(CblDisplayAttributes);
            if (PhDisplayAttributes.Visible && string.IsNullOrEmpty(displayAttributes))
            {
                FailMessage("必须至少选择一项！");
                return;
            }

            ConfigSettings(false);

            var isPeriods = false;
            var startDate = string.Empty;
            var endDate = string.Empty;
            if (DdlPeriods.SelectedValue != "0")
            {
                isPeriods = true;
                if (DdlPeriods.SelectedValue == "-1")
                {
                    startDate = TbStartDate.Text;
                    endDate = TbEndDate.Text;
                }
                else
                {
                    var days = int.Parse(DdlPeriods.SelectedValue);
                    startDate = DateUtils.GetDateString(DateTime.Now.AddDays(-days));
                    endDate = DateUtils.GetDateString(DateTime.Now);
                }
            }
            var checkedState = ETriStateUtils.GetEnumType(DdlPeriods.SelectedValue);
            var redirectUrl = ModalExportMessage.GetRedirectUrlStringToExportContent(SiteId, _channelId, DdlExportType.SelectedValue, AuthRequest.GetQueryString("contentIdCollection"), displayAttributes, isPeriods, startDate, endDate, checkedState);
            FxUtils.Page.Redirect(redirectUrl);
        }
    }
}
