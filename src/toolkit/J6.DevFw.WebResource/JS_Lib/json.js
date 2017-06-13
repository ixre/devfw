
jr.extend({
    json: {
        prefix: 'field',
        _objreg: /(.+)\[([^\]]+)\]/,
        //_dtReg2: /^\/(Date\\([^\\)]+\\))\/$/,
        _dtReg: /^(\d{4}((\/|-)\d{2}){2})T(\d{2}(:\d{2}){2})((\.\d+)*)$/i,
        _each: function(list, callback) {
            for (var i = 0; i < list.length; i++) {
                if (callback) callback(i, list[i]);
            }
        },
        _getFields: function(pl) {
            var pre = this.prefix;
            var fields = {};
            var node;
            var proName, subProName, proValue; //属性名称
            if (!pl.nodeName) pl = document.getElementById(pl);

            var objreg = this._objreg;

            this._each(pl.getElementsByTagName('*'), function(i, e) {
                if (e.nodeName != '#text' && e.nodeName != '#comment') {
                    proName = e.getAttribute(pre);
                    if (proName) {
                        if (objreg.test(proName)) {
                            var match = objreg.exec(proName);
                            proName = match[1];
                            subProName = match[2];

                            if (fields[proName] == null) {
                                fields[proName] = {};
                            }
                            fields[proName][subProName] = e;
                        } else {
                            fields[proName] = e;
                        }
                        if (!e.name) e.setAttribute('name', pre + '_' + proName);
                    }
                }
            });

            return fields;
        },
        _bindField: function(node, proValue) {
            if (this._dtReg.test(proValue)) {
                var match = this._dtReg.exec(proValue);
                if (match[4] == '00:00:00') {
                    //如果为00:00:00则只显示日期
                    proValue = proValue.replace(this._dtReg, '$1');
                } else {
                    proValue = proValue.replace(this._dtReg, '$1 $4');
                }

                /*
                 eval("proValue=new " + this._dtReg.exec(proValue)[1]);

                 var val = '';
                 var v = proValue.getMonth() + 1;
                 if (v <= 9) v = '0' + v;

                 val = proValue.getFullYear() + '-' + v + '-';
                 v = proValue.getDate();
                 if (v <= 9) v = '0' + v;

                 val += v + ' ' + proValue.toLocaleTimeString();
                 proValue = val;
                 */

            }


            switch (node.nodeName) {
            case 'TEXTAREA':
            case 'INPUT':
                switch (node.type) {
                default:
                    node.value = proValue;
                    break;
                case "radio":
                    var radios = document.getElementsByName(node.name);
                    for (var i = 0; i < radios.length; i++) {
                        if (radios[i].value == proValue) {
                            radios[i].setAttribute('checked', 'checked');
                            break;
                        }
                    }
                    break;
                case 'checkbox':
                    var isChecked = false;
                    if ((proValue == true && proValue.toString() != '1') || proValue == node.value) {
                        isChecked = true;
                    } else if (proValue.length) {
                        for (var i in proValue) {
                            if (proValue[i] == node.value) {
                                isChecked = true;
                                break;
                            }
                        }
                    }
                    if (isChecked) {
                        node.setAttribute('checked', 'checked');
                    } else {
                        node.removeAttribute('checked');
                    }
                    break;
                }
                break;
            case 'IMG':
                node.src = proValue;
                break;
            case 'SELECT':
                node.value = proValue;
                break;
            default:
                node.innerHTML = proValue;
                break;
            }
        },
        _getFieldVal: function(node) {
            var proValue = '';
            switch (node.nodeName) {
            case 'TEXTAREA':
            case 'INPUT':
                switch (node.type) {
                default:
                    proValue = node.value; //.replace('\'', '\\\'');
                    break;
                case 'radio':
                    var radios = document.getElementsByName(node.name);
                    for (var i = 0; i < radios.length; i++) {
                        if (radios[i].checked) {
                            proValue = radios[i].value;
                            break;
                        }
                    }
                    break;
                case 'checkbox':
                    proValue = node.checked ? node.value : '';
                    break;
                }
                break;
            case 'IMG':
                proValue = node.src;
                break;
            case 'SELECT':
                proValue = node.selectedIndex == -1 ? '' : node.options[node.selectedIndex].value;
                break;
            default:
                proValue = node.innerHTML; //.replace('\'', '\\\'');
                break;
            }
            return proValue;
        },
        bind: function(pl, json, formatter) {

            var fields;
            var node;
            var proValue;

            fields = this._getFields(pl);

            for (var proName in fields) {
                node = fields[proName];
                //获取格式化后的值
                if (formatter && formatter instanceof Function) {
                    proValue = formatter(proName, json[proName]);
                } else {
                    proValue = json[proName];
                }

                if (proValue != null) {
                    //如果为对象
                    if (proValue instanceof Object) {
                        //针对列表
                        if (proValue.length) {
                            for (var i in node) {
                                this._bindField(node[i], proValue);
                            }
                        } else {
                            for (var i in proValue) {
                                if (node[i]) {
                                    this._bindField(node[i], proValue[i]);
                                }
                            }
                        }

                        continue;
                    }
                    this._bindField(node, proValue);
                }
            }
        },
        __convert: function(pl, format, formatter) {
            var fields;
            var node;
            var proValue;
            var obj = {};
            var queryString = '';

            fields = this._getFields(pl);

            for (var proName in fields) {
                node = fields[proName];
                if (node.nodeName) {
                    proValue = this._getFieldVal(node);

                    //获取格式化后的值
                    if (formatter && formatter instanceof Function) {
                        proValue = formatter(proName, proValue);
                    }

                    obj[proName] = proValue;
                    queryString += proName + '=' + proValue + '&';

                } else {
                    //如果为对象
                    obj[proName] = {};
                    var j = 0;
                    var isArray = false;

                    for (var i in node) {
                        if (j++ == 0 && /^\d+$/.test(i)) {
                            obj[proName] = [];
                            isArray = true;
                        }
                        if (node[i]) {
                            proValue = this._getFieldVal(node[i]);

                            //获取格式化后的值
                            if (formatter && formatter instanceof Function) {
                                proValue = formatter(proName, proValue);
                            }

                            if (proValue && proValue != '') {
                                if (isArray) {
                                    obj[proName].push(proValue);
                                } else {
                                    obj[proName][i] = proValue;
                                }
                            }

                            queryString += proName + '[' + i + ']=' + proValue + '&';
                        }
                    }
                }
            }
            return format == "object" ? obj : queryString.replace(/&$/g, '');
        },
        toObject: function(pl) {
            return this.__convert(pl, 'object');
        },
        toQueryString: function(pl) {
            return this.__convert(pl, 'string');
        },
        //转为 " id:1;name:刘铭 " 这样的格式guyy
        toString: function(pl) {
            return this.__convert(pl, 'string').replace(/&/g, ';').replace(/=/g, ':');
        },
        //将对象转为json字符串
        string: function(o) {
            var _this = this;
            var arr = [];
            var fmt = function(s) {
                if (typeof s == 'object' && s != null) _this.string(s);
                return /^(string|number)$/.test(typeof s) ? "'" + s + "'" : s;
            };
            for (var i in o) {
                if (o.hasOwnProperty(i)) {
                    var val = fmt(o[i]);
                    if (val.pop) {
                        arr.push("'" + i + "':[\'" + escape(val.join('\',\'')) + '\']');
                    } else {
                        arr.push("'" + i + "':" + escape(val));
                    }
                }
            }
            return '{' + escape(arr.join(',')) + '}';
        }
    }
});