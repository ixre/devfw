/*******************************************
* 文 件 名：dgloader.js
* 文件描述：用于加载datagrid控件
* 创 建 人：刘成文
* 创建日期：2012-002 10:09:33
********************************************/
var pageTag = 'datagrid';
if (!window.dgp) alert('初始化失败!');
$(function () {
    $('#'+dgp.id).datagrid({
        queryParams: dgp.query,
        width: dgp.width,
        height: dgp.height,
        nowrap: true,
        cache: false,
        striped: true,
        singleSelect: false,
        idfield: dgp.idField,
        pagination: dgp.paging,
        pageSize: dgp.pageSize,
        fitColumns: true, //列根据宽度调整
        fit: true,
        url: dgp.dataUrl,

        //列
        columns: dgp.columns || null,
        //工具栏
        toolbar: [

            ],
        //选择列
        onHeaderContextMenu: function (e, field) {
            //DataGridExtend.headerContextMenu(e, field, $(this).attr('ID'), pageTag + '.' + $(this).attr('ID'))
        },
        //加载成功的时候，初始化隐藏列
        onLoadSuccess: function (data) {
            //DataGridExtend.loadSuccessInit($(this).attr('ID'), pageTag + '.' + $(this).attr('ID'));
           // DataGridExtend.ShowTip_Url();
        },
        onLoadError: function () {
            $.messager.alert('提示', '数据加载错误，请重新登录系统', 'error');
        }
    });
    //搜素栏
    $(".datagrid-toolbar").append($(".searchBar"));
    //工具栏
    $(".datagrid-toolbar").append($(".toolBar"));
    //messageBar
    $(".datagrid-toolbar").append($("#messageBar"));

    $(window).resize(function () {
        $("#"+pageTag).datagrid("resize");
    });

});


function test() {
    alert('功能未实现!');
}