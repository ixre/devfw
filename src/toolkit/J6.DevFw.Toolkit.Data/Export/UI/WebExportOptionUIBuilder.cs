using System.Text;

namespace J6.DevFw.Toolkit.Data.Export.UI
{
    public static class WebExportOptionUIBuilder
    {
        public static string BuildColumnCheckHtml(IDataExportPortal portal)
        {
            StringBuilder sb = new StringBuilder();


            //================ Output Javascript ==================//
            sb.Append(@"<script type=""text/javascript"">
                        var wbexp={
                            config:{
                                //处理请求的Url地址
                                urlHandler:'',  
                                //处理生成Json的对象
                                jsonHandler:null,
                                params:null,
                                page:null
                            },
                            doExport:function(portal){
                                var data=this.config.jsonHandler.toQueryString('ui-export');
                                if(this.config.params==null){
                                   var regMatch=/(\?|&)params=(.+)&*/i.exec(location.search);
                                   this.config.params=regMatch?regMatch[2]:'';
                                }
                                if(!this.config.page){
                                    this.config.page=document.getElementById('export_iframe');
                                }
                                this.config.page.src=this.config.urlHandler
                                             + (this.config.urlHandler.indexOf('?')==-1?'?':'&')
                                             + 'portal=' + portal + '&' + data
                                             + '&params=' + this.config.params;
                            }
                        };
                        </script>");


            sb.Append(@"<div class=""ui-export"" id=""ui-export"">");


            //====================== 导出格式 =====================//


            sb.Append(@"<div><strong>选择导出格式</strong></div>
                            <ul class=""columnList"">
                                <li class=""export_format_excel""><input checked=""checked"" field=""ExportFormat"" style=""border:none"" name=""export_format"" type=""radio"" value=""excel"" id=""export_format_excel""/>
                                    <label for=""wbexp_format_excel"">Excel文件</label>
                                </li>
                                <li class=""export_format_csv""><input type=""radio"" field=""ExportFormat"" style=""border:none"" name=""export_format"" value=""csv"" id=""export_format_csv""/>
                                    <label for=""wbexp_format_csv"">CSV数据文件</label>
                                </li>
                                <li class=""export_format_txt""><input type=""radio"" field=""ExportFormat"" style=""border:none"" name=""export_format"" value=""txt"" id=""export_format_txt""/>
                                    <label for=""wbexp_format_txt"">文本</label>
                                </li>
                            </ul><div style=""clear:both""></div><br />");


            //====================== 导出列 =======================//
            if (portal.ColumnNames == null || portal.ColumnNames.Length == 0)
            {
                sb.Append("<em><strong>该导出方案不包含可选择的导出列</strong></em>");
            }
            else
            {
                sb.Append(@"<div class=""selColumn""><strong>请选择要导出的列:</strong>
                            <ul class=""columnList"">");

                int tmpInt = 0;
                foreach (DataColumnMapping column in portal.ColumnNames)
                {
                    sb.Append(
                        "<li><input type=\"checkbox\" style=\"border:none\" checked=\"checked\" field=\"export_fields[")
                        .Append(tmpInt.ToString()).Append("]\"")
                        .Append(@" id=""export_column_")
                        .Append(column.Field)
                        .Append("\" value=\"").Append(column.Field)
                        .Append("\"/><label for=\"export_column_")
                        .Append(column.Field)
                        .Append("\">").Append(column.Name)
                        .Append("</label></li>");

                    tmpInt++;
                }

                sb.Append(@"</ul></div>");
            }

            sb.Append(@"
                    <iframe id=""export_iframe"" style=""display:none""></iframe>
                    <div style=""clear:both""></div>
                    </div>");
            sb.Append(@"<input type=""button"" class=""btn_export"" onclick=""wbexp.doExport('")
                .Append(portal.PortalKey).Append(@"')"" value="" 导出 ""/>");


            return sb.ToString();
        }

        public static string BuildColumnCheckHtml(string exportPortalClassFullName)
        {
            IDataExportPortal portal = ExportUtil.GetPortal(exportPortalClassFullName);
            return BuildColumnCheckHtml(portal);
        }
    }
}