/*
@license
dhtmlxScheduler v.4.3.25 Professional

This software can be used only as part of dhtmlx.com site.
You are not allowed to use it on any other site

(c) Dinamenta, UAB.


*/
Scheduler.plugin(function (e) {
    ! function () {
        var t = e.dhtmlXTooltip = e.tooltip = {};
        t.config = {
            className: "dhtmlXTooltip tooltip",
            timeout_to_display: 50,
            timeout_to_hide: 50,
            delta_x: 15,
            delta_y: -20
        }, t.tooltip = document.createElement("div"), t.tooltip.className = t.config.className, t.show = function (a, n) {
            if (!e.config.touch || e.config.touch_tooltip) {
                var i = t,
                    r = this.tooltip,
                    d = r.style;
                i.tooltip.className = i.config.className;
                var o = this.position(a),
                    l = a.target || a.srcElement;
                if (!this.isTooltip(l)) {
                    var s = o.x + (i.config.delta_x || 0),
                        _ = o.y - (i.config.delta_y || 0);
                    d.visibility = "hidden", d.removeAttribute ? (d.removeAttribute("right"), d.removeAttribute("bottom")) : (d.removeProperty("right"), d.removeProperty("bottom")), d.left = "0", d.top = "0", this.tooltip.innerHTML = n, document.body.appendChild(this.tooltip);
                    var c = this.tooltip.offsetWidth,
                        u = this.tooltip.offsetHeight;
                    document.body.offsetWidth - s - c < 0 ? (d.removeAttribute ? d.removeAttribute("left") : d.removeProperty("left"), d.right = document.body.offsetWidth - s + 2 * (i.config.delta_x || 0) + "px") : 0 > s ? d.left = o.x + Math.abs(i.config.delta_x || 0) + "px" : d.left = s + "px",
                        document.body.offsetHeight - _ - u < 0 ? (d.removeAttribute ? d.removeAttribute("top") : d.removeProperty("top"), d.bottom = document.body.offsetHeight - _ - 2 * (i.config.delta_y || 0) + "px") : 0 > _ ? d.top = o.y + Math.abs(i.config.delta_y || 0) + "px" : d.top = _ + "px", d.visibility = "visible", this.tooltip.onmouseleave = function (t) {
                            t = t || window.event;
                            for (var a = e.dhtmlXTooltip, n = t.relatedTarget; n != e._obj && n;) n = n.parentNode;
                            n != e._obj && a.delay(a.hide, a, [], a.config.timeout_to_hide)
                        }, e.callEvent("onTooltipDisplayed", [this.tooltip, this.tooltip.event_id]);
                }
            }
        }, t._clearTimeout = function () {
            this.tooltip._timeout_id && window.clearTimeout(this.tooltip._timeout_id)
        }, t.hide = function () {
            if (this.tooltip.parentNode) {
                var t = this.tooltip.event_id;
                this.tooltip.event_id = null, this.tooltip.onmouseleave = null, this.tooltip.parentNode.removeChild(this.tooltip), e.callEvent("onAfterTooltip", [t])
            }
            this._clearTimeout()
        }, t.delay = function (e, t, a, n) {
            this._clearTimeout(), this.tooltip._timeout_id = setTimeout(function () {
                var n = e.apply(t, a);
                return e = t = a = null, n
            }, n || this.config.timeout_to_display);
        }, t.isTooltip = function (e) {
            var t = !1;
            for ("dhtmlXTooltip" == e.className.split(" ")[0]; e && !t;) t = e.className == this.tooltip.className, e = e.parentNode;
            return t
        }, t.position = function (e) {
            if (e = e || window.event, e.pageX || e.pageY) return {
                x: e.pageX,
                y: e.pageY
            };
            var t = window._isIE && "BackCompat" != document.compatMode ? document.documentElement : document.body;
            return {
                x: e.clientX + t.scrollLeft - t.clientLeft,
                y: e.clientY + t.scrollTop - t.clientTop
            }
        }, e.attachEvent("onMouseMove", function (a, n) {
            var i = window.event || n,
                r = i.target || i.srcElement,
                d = t,
                o = d.isTooltip(r),
                l = d.isTooltipTarget && d.isTooltipTarget(r);
            if (a || o || l) {
                var s;
                if (a || d.tooltip.event_id) {
                    var _ = e.getEvent(a) || e.getEvent(d.tooltip.event_id);
                    if (!_) return;
                    if (d.tooltip.event_id = _.id, s = e.templates.tooltip_text(_.start_date, _.end_date, _), !s) return d.hide()
                }
                l && (s = "");
                var c;
                if (_isIE) {
                    c = {
                        pageX: void 0,
                        pageY: void 0,
                        clientX: void 0,
                        clientY: void 0,
                        target: void 0,
                        srcElement: void 0
                    };
                    for (var u in c) c[u] = i[u]
                }
                if (!e.callEvent("onBeforeTooltip", [a]) || !s) return;
                d.delay(d.show, d, [c || i, s])
            } else d.delay(d.hide, d, [], d.config.timeout_to_hide)
        }), e.attachEvent("onBeforeDrag", function () {
            return t.hide(), !0
        }), e.attachEvent("onEventDeleted", function () {
            return t.hide(), !0
        }), e.templates.tooltip_date_format = e.date.date_to_str("%Y-%m-%d %H:%i"), e.templates.tooltip_text = function (t, a, n) {
            return "<b>Service Case #:</b> " + n.text + "<br/><b>Start date:</b> " + e.templates.tooltip_date_format(t) + "<br/><b>End date:</b> " + e.templates.tooltip_date_format(a)
        }
    }()
});