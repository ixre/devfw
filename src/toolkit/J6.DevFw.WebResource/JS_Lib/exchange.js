//交换信息

if (window.j6) {
    jr.event.add(window, 'load', function () {
        jr.each(document.getElementsByClassName('ui-exchange'), function (i, e) {
            var v = null;
            var ev = null;
            var attr = null;
            var exattr = 'exchange';

            switch (e.nodeName) {
                case 'IMG':
                    attr = 'src';
                    break;
                default:
                    attr = 'innerHTML';
                    break;
            }
            if (attr == null) return;
            v = e[attr];
            ev = e.getAttribute(exattr);
            if (ev) {
                jr.event.add(e, 'mouseover', (function (_e, _attr, _v) {
                    return function () {
                        _e[_attr] = _v;
                    };
                })(e, attr, ev));

                jr.event.add(e, 'mouseout', (function (_e, _attr, _v) {
                    return function () {
                        _e[_attr] = _v;
                    };
                })(e, attr, v));
            }
        });
    });
}