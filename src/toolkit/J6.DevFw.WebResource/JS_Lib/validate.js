

//===================== 验证器(2012-09-30) =====================//
jr.extend({
    validator: {
        //设置提示
        setTip: function (e, success, summaryKey, msg) {
            var tipID = e.attr('validate_id');
            //根据键值获取提示信息
            if (summaryKey) {
                var summary = e.attr('summary');
                if (summary) {
                    summary = JSON.parse(summary.replace(/'/g, '"'));
                    if (summary[summaryKey]) {
                        msg = summary[summaryKey];
                    }
                }
            }

            //如果设置了提示信息容器
            if (e.attr('tipin')) {
                var tipin = jr.$fn("#" + e.attr('tipin'));
                if (tipin) {
                    if (tipin.hasClass('validator')) {
                        tipin.addClass("validator");
                    }
                    var tipInId = tipin.attr('valid-src');
                    if (!success) {
                        tipin.attr('valid-src', tipID);
                        tipin.html('<span class="valid-error"><span class="msg">' + msg + '</span></span>');
                    } else if (tipInId == tipId) {
                        tipin.html('<span class="valid-right"><span class="msg">' + msg + '</span></span>');
                    }
                    return false;
                }
            }


            //未指定提示信息容器,则生成显示的容器
            var tipEle = jr.$fn("#" + tipID);
            if (tipEle.len() == 0) {
                var elem = document.createElement('DIV');
                document.body.appendChild(elem);
                var pos = jr.getPosition(elem);
                tipEle = jr.$fn(elem);
                tipEle.css({
                    "position": "fixed", "left": (pos.right + document.documentElement.scrollLeft) + "px",
                    "top": (pos.top + document.documentElement.scrollTop) + 'px'
                });
                tipEle.attr("id", tipID);
                tipEle.attr("className", "validator");
                //console.log("---", pos, "|", document.documentElement.scrollLeft,
                //    "|", document.documentElement.clientWidth);
            }

            //设置值
            tipEle.html('<span class="' + (success ? 'valid-right' : 'valid-error') +
                '"><span class="msg">' + msg + '</span></span>');

        },
        //移除提示
        removeTip: function (e) {
            //如果指定了提示信息容器
            if (e.attr('tipin')) {
                var tipin = jr.$fn("#" + e.attr('tipin'));
                if (tipin.len() > 0) {
                    tipin.html("");
                    return false;
                }
            }
            //如果未指定提示信息容器
            var tipEle = jr.$fn("#" + e.attr('validate_id'));
            if (tipEle.len() > 0) {
                document.body.removeChild(tipEle.elem());
            }
        },
        //验证结果
        result: function (id) {
            //指定了父元素
            if (!id) {
                return jr.$fn(".valid-error").len() == 0;
            }
            var isSuccess = true;
            jr.$fn("#" + id + " .ui-validate").each(function (i, e) {
                if (!isSuccess) return;
                var tipIn = e.attr("tipin");
                if (tipIn) {
                    // 获取显示在指定位置的信息
                    var c = e.find("#" + tipIn);
                    if (c.len() > 0 && c.html().indexOf("valid-error") != -1) {
                        isSuccess = false;
                    }
                } else {
                    // 获取浮动的信息
                    var c = e.find("#" + e.attr("validate_id"));
                    if (c.len() > 0 && c.find(".validate-error").len() > 0) {
                        isSuccess = false;
                    }
                }
            });
            return isSuccess;
        },

        //初始化事件
        init: function () {
            var $J = jr;
            if (!jr) {
                alert("未引用核心库!");
                return false;
            }
            var t = this;
            jr.$fn(".ui-validate").each(function (i, e) {
                var tipID = e.attr("validate_id");
                while (tipID == null) {
                    tipID = e.attr("id");
                    if (tipID && tipID != '') {
                        tipID = 'validate_item_' + tipID;
                    } else {
                        tipID = 'validate_item_' + parseInt(Math.random() * 1000).toString();
                    }
                    if (document.getElementById(tipID) != null) {
                        tipID = null;
                    } else {
                        e.attr('validate_id', tipID);
                    }
                }


                //只能输入数字
                if (e.attr('isnumber') == "true") {
                    e.css({ "ime-mode": "disabled" });
                    var func = (function (validater, e) {
                        return function () {
                            if (/\D/.test(e.val())) {
                                e.val(e.val().replace(/\D/g, ''));
                            }
                            e.val(e.val().replace(/^0([0-9])/, '$1'));
                        };
                    })(t, e);
                    e.keyup(func);
                    e.change(func);
                }

                //只能浮点数
                if (e.attr('isfloat') == 'true') {
                    e.css({ "ime-mode": "disabled" });
                    var func = (function (validater, e) {
                        return function () {
                            if (/[^\d\.]/.test(e.val())) {
                                e.val(e.val().replace(/[^\d\.]/g, ''));
                            }
                            e.val(e.val().replace(/^(0|\.)([0-9]+\.*[0-9]*)/, '$2'));
                        };
                    })(t, e);
                    e.keyup(func);
                    e.change(func);
                }

                // 使用正则表达式
                if (e.attr('regex')) {
                    var func = (function (validator, e) {
                        return function () {
                            var reg = new RegExp();
                            reg.compile(e.attr('regex'));
                            if (!reg.test(e.value)) {
                                validator.setTip(e, false, 'regex', '输入不正确');
                            } else {
                                validator.removeTip(e);
                            }
                        };
                    })(t, eles[i]);
                    e.blur(func);
                    e.keyup(func);
                } else {
                    // 常规校验
                    if (e.attr("isrequired") == "true" || e.attr("required") == "true") {
                        var fn = (function (validator, e) {
                            return function () {
                                if (e.val().replace(/\s/g, '') == '') {
                                    validator.setTip(e, false, 'required', '该项不能为空');
                                } else {
                                    validator.removeTip(e);
                                }
                            };
                        })(t, e);
                        e.blur(fn);
                        e.keyup(fn);
                    }

                    //长度限制
                    if (e.attr("nodeName") == "INPUT" && e.attr('length') != null) {
                        var fn = (function (validator, e) {
                            return function () {
                                var pro_val = e.attr('length');
                                var reg = /\[(\d*),(\d*)\]/ig;
                                var l_s = parseInt(pro_val.replace(reg, '$1')),
                                    l_e = parseInt(pro_val.replace(reg, '$2'));
                                if (e.val().length < l_s) {
                                    validator.setTip(e, false, 'length', l_e ? '长度必须为' + l_s + '-' + l_e + '位' : '长度至少' + (l_s) + '位');
                                } else if (e.val().length > l_e) {
                                    validator.setTip(e, false, 'length', l_s ? '长度必须为' + l_s + '-' + l_e + '位' : '长度超出' + (l_e) + '位');
                                } else if (e.attr('required') == null || e.val().length > 0) {
                                    validator.removeTip(e);
                                }
                            };
                        })(t, e);
                        e.blur(fn);
                        e.keyup(fn);
                    }
                }
            });

        },

        validate: function (id) {
            var eles;
            if (id) {
                //指定了父元素
                eles = jr.$fn("#" + id + " .ui-validate");
            } else {
                //所有元素，未指定父元素
                eles = jr.$fn(".ui-validate");
            }
            eles.each(function (i, e) {
                if (e.attr("required") == "true" || e.attr("isrequired") == "true" ||
                    (e.attr("nodeName") == "INPUT" && e.attr("length")) || e.attr("regex")) {
                    if (e.elem().onblur != null) {
                        e.elem().onblur();
                    }
                }
            });
            return this.result(id);
        }
    }
});

jr.event.add(window, 'load', function () {
    jr.validator.init();
});