
var accordion = function () {

    var tm = 10;
    var sp = 10;

    function slider(n) {
        this.nm = n;
        this.arr = [];
    }

    slider.prototype.init = function (t, defIdx, showedLayerCls) {
        var a, h, s, l, i;
        var d;
        a = document.getElementById(t);
        this._showedLayerCls = showedLayerCls || '';
        h = a.getElementsByTagName('dt');
        s = a.getElementsByTagName('dd');
        this.l = h.length;
        for (i = 0; i < this.l; i++) {
            d = h[i];
            this.arr[i] = d;
            d.onclick = new Function(this.nm + '.pro(this)');
            if (defIdx == i) {
                d.className = this._showedLayerCls;
            }
        }
        l = s.length;
        for (i = 0; i < l; i++) {
            d = s[i];
            d.mh = d.offsetHeight;
            if (defIdx != i) {
                d.style.height = 0;
                d.style.display = 'none';
            }
        }
    }

    slider.prototype.pro = function (d) {
        for (var i = 0; i < this.l; i++) {
            var h = this.arr[i], s = h.nextSibling;
            s = s.nodeType != 1 ? s.nextSibling : s;
            clearInterval(s.tm);
            if ((h == d && s.style.display == 'none') || (h == d && s.style.display == '')) {
                s.style.display = '';
                su(s, 1);
                h.className =  this._showedLayerCls;
            } else if (s.style.display == '') {
                su(s, -1);
                h.className = '';
            }
        }
    }

    function su(c, f) {
        c.tm = setInterval(function () {
            sl(c, f);
        }, tm);
    }

    function sl(c, f) {
        var h = c.offsetHeight, m = c.mh, d = f == 1 ? m - h : h;
        c.style.height = h + (Math.ceil(d / sp) * f) + 'px';
        c.style.opacity = h / m;
        c.style.filter = 'alpha(opacity=' + h * 100 / m + ')';
        if (f == 1 && h >= m) {
            clearInterval(c.tm);
        } else if (f != 1 && h == 1) {
            c.style.display = 'none';
            clearInterval(c.tm);
        }
    }

    return { slider: slider }

}();

rs.extend({
    accordion: function (id, defaultIndex) {
        window.accordion_1 = new accordion.slider("accordion_1");
        window.accordion_1.init(id, defaultIndex || 0, "open");
        return window.accordion_1;
    }
});