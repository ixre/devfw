//
//文件：数据表格
//版本: 1.0
//时间：2014-04-01
//


function datagrid(ele, config) {
    this.panel = ele.nodeName ? ele : jr.$(ele);
    this.columns = config.columns;
    //Id域
    this.idField = config.idField || "id";
    this.data_url = config.url;
    this.data = config.data;

    //加载完成后触发
    this.onLoaded = config.loaded;

    //列的长度
    //this.columns_width = [];

    this.loadbox = null;
    this.gridView = null;



    this.loading = function () {
        //初始化高度
        if (this.gridView.offsetHeight === 0) {
            var headerHeight = this.gridView.previousSibling.offsetHeight;
            var gridviewHeight = this.panel.offsetHeight - headerHeight;
            this.gridView.style.cssText = this.gridView.style.cssText
                .replace(/(\s*)height:[^;]+;/ig, ' height:' + (gridviewHeight > headerHeight ? gridviewHeight + 'px;' : 'auto'));


            var ldLft = Math.ceil((this.gridView.clientWidth - this.loadbox.offsetWidth) / 2);
            var ldTop = Math.ceil((this.gridView.clientHeight - this.loadbox.offsetHeight) / 2);

            this.loadbox.style.cssText = this.loadbox.style.cssText
                .replace(/(;\s*)*left:[^;]+;([\s\S]*)(\s)top:([^;]+)/ig,
                '$1left:' + ldLft + 'px;$2 top:'
                + (ldTop < 0 ? -ldTop : ldTop) + 'px');

        }

        this.loadbox.style.display = '';
    };

    this._initLayout = function () {
        var html = '';
        if (this.columns && this.columns.length != 0) {
            //添加头部
            html += '<div class="ui-datagrid-header"><table width="100%" cellspacing="0" cellpadding="0"><tr>';
            for (var i = 0; i < this.columns.length; i++) {
                // this.columns_width.push(this.columns[i].width);
                html += '<td'
                    + (i == 0 ? ' class="first"' : '')
                    + (this.columns[i].align ? ' align="' + this.columns[i].align + '"' : '')
                    + (this.columns[i].width ? ' width="' + this.columns[i].width + '"' : '')
                    + '><div class="ui-datagrid-header-title">'
                    + this.columns[i].title
                    + '</div></td>';
            }
            html += '</tr></table></div>';
            //添加内容页
            html += '<div class="ui-datagrid-msg" style="position: absolute; display: inline-block;min-width:60px;top:0;bottom:0;left:0;right:0;margin:auto;">加载中...</div>'
                    + '<div class="ui-datagrid-view"></div>';
        }
        this.panel.innerHTML = html;

        this.gridView = (this.panel.getElementsByClassName
            ? this.panel.getElementsByClassName('ui-datagrid-view')
            : jr.dom.getsByClass(this.panel, 'ui-datagrid-view'))[0];
        this.loadbox = this.gridView.previousSibling;
    };


    this._fill_data = function (data) {
        if (!data) return;

        var item;
        var col;
        var val;
        var html = '';
        var rows = data['rows'] || data;

        html += '<table width="100%" cellspacing="0" cellpadding="0">';

        for (var i = 0; i < rows.length; i++) {
            item = rows[i];
            html += '<tr'
                + (item[this.idField] != null ? ' data-indent="' + item[this.idField] + '"' : '')
                + '>';

            for (var j = 0; j < this.columns.length; j++) {
                col = this.columns[j];
                val = item[col.field];
                html += '<td'
                    + (j == 0 ? ' class="first"' : '')
                    + (col.align ? ' align="' + col.align + '"' : '')
                    + (i == 0 && col.width ? ' width="' + col.width + '"' : '')
                    + '><div class="field-value">'
                    + (col.formatter && col.formatter instanceof Function ? col.formatter(val, item, i) : val)
                    + '</div></td>';

            }
            html += '</tr>';
        }

        html += '</table><div style="clear:both"></div>';

        //gridview的第1个div
        this.gridView.innerHTML = html;

        //this._fixPosition();

        this.gridView.srcollTop = 0;

        this.loadbox.style.display = 'none';

        if (this.onLoaded && this.onLoaded instanceof Function)
            this.onLoaded(data);
    };


    this._fixPosition = function () {
    };

    this._load_data = function (func) {
        if (!this.data_url) return;
        var t = this;

        if (func) {
            if (!(func instanceof Function)) {
                func = null;
            }
        }

        jr.xhr.request({
            uri: this.data_url,
            data: 'json',
            params: this.data,
            method: 'POST'
        }, {
            success: function (json) {
                t._fill_data(json);
            }, error: function () {
                //alert('加载失败!');
            }
        });

    };

    /* 为兼容IE6 */
    //var resizeFunc = (function (t) {
    //    return function () {
    //        t.resize.apply(t);
    //    };
    //})(this);
    //jr.event.add(window, 'load', resizeFunc);
    //window.attachEvent('resize', resizeFunc);
    //jr.event.add(window, 'resize', this.resize.apply(this));

    this._initLayout();

    //重置尺寸
    //this._resize();

    //加载数据
    this.load();
}

datagrid.prototype.resize = function () {
    this._fixPosition();
};

datagrid.prototype.load = function (data) {
    //显示加载框
    this.loading();
    if (data && data instanceof Object) {
        this._fill_data(data);
        return;
    }
    this._load_data();
};

/* 重新加载 */
datagrid.prototype.reload = function (params, data) {
    if (params) {
        this.data = params;
    }
    this.load(data);
};

jr.extend({
    grid: function (ele, config) {
        return new datagrid(ele, config);
    },
    datagrid: function (ele, config) {
        return new datagrid(ele, config);
    }
});


